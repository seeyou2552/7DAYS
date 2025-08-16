using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDeadState : AIState
{
    private readonly int DeadHas = Animator.StringToHash("Dead");
    private const float CrossFadeDuration = 0.1f;
    public AIDeadState(GameObject owner) : base(owner)
    {
    }

    public override void Enter()
    {
        owner.GetComponent<Animator>().CrossFadeInFixedTime(DeadHas, CrossFadeDuration);
        owner.GetComponent<Collider>().enabled = false;
        if (owner.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            if(navMeshAgent.isOnNavMesh)
                navMeshAgent.isStopped = true;
        }
    }
    public override void Tick()
    {
        Animator animator = owner.GetComponent<Animator>();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


        if (stateInfo.shortNameHash == DeadHas && stateInfo.normalizedTime >= 1f)
        {
            Exit();
        }
    }

    public override void Exit()
    {
        // 풀로 돌아가게
        if (owner.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            navMeshAgent.isStopped = false;
        }
        owner.GetComponent<Collider>().enabled = true;

        EENEMYTYPE type = owner.GetComponent<AI_Base>().enemyData.type;
        ObjectPoolManager.Inst.Return(type, owner);


    }

 


}
