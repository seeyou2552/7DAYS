using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIOverWallState : AIState
{
    private Transform target;
    private float climbSpeed = 10f;
    private float targetClimbY;      // 목표 Y 위치
    private bool climbFinished = false;
    private Rigidbody rb;
    private NavMeshAgent agent;
    public bool IsClimbComplete => climbFinished;

    public AIOverWallState(GameObject owner, Transform target) : base(owner)
    {
        this.target = target;
        rb = owner.GetComponent<Rigidbody>();
        agent = owner.GetComponent<NavMeshAgent>();
    }

    public override void Enter()
    {
        Debug.Log(" Enter OverWall");

        rb.useGravity = false;
   
        if (agent != null)
        {
            agent.enabled = false; 
        }

        // 벽 꼭대기 높이 계산
        Collider wallCollider = target.GetComponent<Collider>();
        if (wallCollider != null)
        {
            float wallTopY = target.position.y + wallCollider.bounds.extents.y;
            targetClimbY = wallTopY + 0.5f; // 여유 추가
        }
        else
        {
            Debug.LogWarning("Target has no collider! Fallback height used.");
            targetClimbY = target.position.y + 2f;
        }

        climbFinished = false;
    }

    public override void Tick()
    {
        if (climbFinished)
            return;

        float delta = climbSpeed * Time.deltaTime;

        Vector3 forward = (target.position - owner.transform.position);
        forward.y = 0f;
        Vector3 climbDir = (Vector3.up * 1f + forward.normalized * 0.4f).normalized;

        rb.MovePosition(owner.transform.position + climbDir * delta);

        if (owner.transform.position.y >= targetClimbY)
        {
            climbFinished = true;

        }
    }

    public override void Exit()
    {
        Debug.Log(" Exit OverWall");
        if (agent != null)
        {
            agent.enabled = true; // 
            rb.useGravity = true;
        }
    }
}
