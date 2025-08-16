using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAttackState : AIState
{
    private GameObject target;
    private NavMeshAgent agent;
    private float attackCooldown;
    private float lastAttackTime = Mathf.NegativeInfinity; // 마지막 공격 시간
    public GameObject CurrentTarget => target;
    private readonly int AttackHas = Animator.StringToHash("Attack");
    private readonly int Attack2Has = Animator.StringToHash("Attack2");
    EBOSSPHASE lastPhase = EBOSSPHASE.PHASE1;

    private const float CrossFadeDuration = 0.1f;
    public AIAttackState(GameObject owner) : base(owner)
    {
    }

    public override void Enter()
    {
        if(CurrentTarget != null)
        {
            owner.transform.LookAt(CurrentTarget.transform);
        }

    
         owner.GetComponent<Animator>().CrossFadeInFixedTime(AttackHas, CrossFadeDuration);
       


        Debug.Log("Attack");

        if (owner.TryGetComponent(out NavMeshAgent agent))
        {
            this.agent = agent;
            agent.isStopped = true;
        }



        SO_EnemyAI data = owner.GetComponent<AI_Base>().enemyData;
        attackCooldown = data.attackCoolTime;

  
        lastAttackTime = Time.time - attackCooldown;
    }
    public override void Tick()
    {
        if (target == null || !target.activeInHierarchy)  return;

        AI_Collapser collapser = owner.GetComponent<AI_Base>() as AI_Collapser;
        if (collapser != null)
        {
            var currentPhase = collapser.phaseController.CurrentPhase;
            if (currentPhase != lastPhase)
            {
                lastPhase = currentPhase;
                PlayAttackAnimation(currentPhase);
            }
        }


        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            owner.GetComponent<AI_Base>().Attack(target); // Base 클래스 공격 호출
        }
    }

    public override void Exit()
    {
        if(agent != null)
            agent.isStopped = false;

        target = null;
        lastPhase = EBOSSPHASE.PHASE1;
    }
    public void SetTarget(GameObject t)
    {
        target = t;
    }
    private void PlayAttackAnimation(EBOSSPHASE phase)
    {
        int hash = (phase == EBOSSPHASE.PHASE2) ? Attack2Has : AttackHas;
        Animator animator = owner.GetComponent<Animator>();
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != hash)
        {
            animator.CrossFadeInFixedTime(hash, CrossFadeDuration);
        }
    }


}
