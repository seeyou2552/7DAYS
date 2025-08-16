using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class AI_Runner : AI_Base
{
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

        fsm.SetInitialState(idle);

        fsm.AddTransition(idle, chase, () => Vector3.Distance(transform.position, TestGameManager.Inst.testPlayer.transform.position) < enemyData.chasingRange);



        fsm.AddTransition(chase, attack, () =>
        {
            var target = chase.CurrentTarget;
            if (target == null) return false;

            bool isWall = target.CompareTag("Wall");
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
        fsm.AddTransition(attack, idle, () =>
        {
            var t = attack.CurrentTarget;
            return t == null || !t.gameObject.activeInHierarchy;
        });
        // fsm.AddTransition(attack, chase, () => Vector3.Distance(transform.position, chase.CurrentTarget.position) < enemyData.chasingRange);

        fsm.AddAnyTransition(dead, () => GetComponent<Health>().IsDead);

    }

    public override void Attack(GameObject target)
    {
        if (target.TryGetComponent(out IDamageable player))
        {
            player.TakeDamage(enemyData.attackPower);
        }

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(origin, enemyData.wallDetectDistance);
    }
}
