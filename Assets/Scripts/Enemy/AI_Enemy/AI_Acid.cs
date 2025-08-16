using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Acid : AI_Base
{
    private GameObject rangedTarget;
    [SerializeField] private GameObject projectilePrefab;
    private GameObject target;

    protected override void Start()
    {
        base.Start();
        Setting();
    }

    protected override void Setting()
    {
        var idle = new AIIdleState(gameObject);
        var chase = new AIChasingState(gameObject, TestGameManager.Inst.testPlayer.transform);
        var attack = new AIAttackState(gameObject);
        var dead = new AIDeadState(gameObject);

      

        fsm.AddTransition(idle, chase, () =>
        {
            return DroneManager.HasAliveDrones ||
                   Vector3.Distance(transform.position, TestGameManager.Inst.testPlayer.transform.position) < enemyData.chasingRange;
        });


        fsm.AddTransition(chase, attack, () =>
        {
            if (IsWallOrTurretNearby())
            {
                attack.SetTarget(rangedTarget);
                return true;
            }
            return false;
        });


        fsm.AddTransition(attack, chase, () =>
        {
            var t = attack.CurrentTarget;
            return t == null || !t.activeInHierarchy;
        });

        fsm.AddAnyTransition(dead, () => GetComponent<Health>().IsDead);

        fsm.SetInitialState(idle);
    }

    public override void Attack(GameObject target)
    {
        // Debug.Log("acid attack");

        if (target == null)
            this.target = target;

    }

    private bool IsWallOrTurretNearby()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, enemyData.attackRange); 
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Wall") || hit.CompareTag("Turret"))
            {
                rangedTarget = hit.gameObject;
                return true;
            }
        }
        return false;
    }

    public static Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 target, float timeToTarget)
    {
        Vector3 displacement = target - start;
        Vector3 gravity = Physics.gravity;

        Vector3 velocity = displacement / timeToTarget - 0.5f * gravity * timeToTarget;
        return velocity;
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyData.attackRange);
    }

    public void AnimAttack()
    {
       
        if (projectilePrefab == null || rangedTarget == null)
            return;

        Vector3 spawnPos = transform.position + Vector3.up + Vector3.forward;
        Vector3 targetPos = rangedTarget.transform.position;


        Vector3 launchVelocity = CalculateLaunchVelocity(spawnPos, targetPos, 2);

        if (launchVelocity == Vector3.zero)
        {
            Debug.LogWarning("Target out of range or invalid launch");
            return;
        }

        GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        proj.GetComponent<Projectile>().Init(gameObject);
        proj.GetComponent<Rigidbody>().velocity = launchVelocity;
    }
}
