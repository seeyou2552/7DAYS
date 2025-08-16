using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIIdleState : AIState
{
    private readonly int IdleHas = Animator.StringToHash("Idle");
    private const float CrossFadeDuration = 0.1f;

    public AIIdleState(GameObject owner) : base(owner)
    {
        owner.GetComponent<Animator>().CrossFadeInFixedTime(IdleHas, CrossFadeDuration);
    }

    public override void Enter()
    {
        Debug.Log("Idle");


        if (owner.TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = true;
        }
    }

    public override void Exit()
    {
     
    }

    public override void Tick()
    {
      
    }

 
}
