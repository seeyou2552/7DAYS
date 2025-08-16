using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Collapser : AI_Base
{
    public CollapserPhaseController phaseController;
    private float AreaAttackRange = 10f;
    [SerializeField] private GameObject attackRangeEffect;
    protected override void Awake()
    {
        base.Awake();
        phaseController = GetComponent<CollapserPhaseController>();
    }
    protected override void Start()
    {
        base.Start();
        Setting();
    }

    protected override void Setting()
    {
        var idle = new AIIdleState(gameObject);
        var chase = new AIChasingState(gameObject, TestGameManager.Inst.testPlayer.transform);  //신호탑으로 교체 예정
        var attack = new AIAttackState(gameObject);
        var dead = new AIDeadState(gameObject);

        fsm.AddTransition(idle, chase, () => Vector3.Distance(transform.position, TestGameManager.Inst.testPlayer.transform.position) < enemyData.chasingRange);

        fsm.AddTransition(chase, attack, () =>
        {
            var target = chase.CurrentTarget;
            if (target == null) return false;

            float dist = Vector3.Distance(transform.position, target.position);

            bool readyToAttack = dist < enemyData.attackRange;

            if (readyToAttack)
            {
                attack.SetTarget(chase.CurrentTarget.gameObject);
            }

            return dist < enemyData.attackRange;
        });

        fsm.AddTransition(attack, chase, () =>
        {
            var t = attack.CurrentTarget;
            return t == null
                || !t.activeInHierarchy
                || Vector3.Distance(transform.position, t.transform.position) > enemyData.attackRange;
        });

        fsm.AddAnyTransition(dead, () => GetComponent<Health>().IsDead && fsm.CurrentState != dead);

        fsm.SetInitialState(idle);
    }


    public override void Attack(GameObject target)
    {
    
        if (phaseController.CurrentPhase == EBOSSPHASE.PHASE1)  //일반공격
        {
            SingleAttack(target);
        }
        else   //광역공격
        {
            AreaAttack(target);
        }
    }

    void SingleAttack(GameObject target)
    {
  
        Debug.Log("Single Attack");
        if (target.TryGetComponent(out IDamageable player))
        {
            player.TakeDamage(enemyData.attackPower);
        }
    }

    void AreaAttack(GameObject target)
    {
        Debug.Log("Area Attack");
        Collider[] hits = Physics.OverlapSphere(transform.position, AreaAttackRange);
    
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            if (hit.TryGetComponent<IDamageable>(out var player))
            {
                player.TakeDamage(enemyData.attackPower);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AreaAttackRange);
    }

    public void AnimEnable()
    {
        attackRangeEffect.SetActive(true);
    }

    public void AnimDisable()
    {
        attackRangeEffect.SetActive(false);
    }

}
