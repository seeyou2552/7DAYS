using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Steel : AI_Base
{
    // Start is called before the first frame update
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
            if (DroneManager.HasAliveDrones)
                return true;

            return Vector3.Distance(transform.position, TestGameManager.Inst.testPlayer.transform.position) < enemyData.chasingRange;
        });

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
            return t == null || !t.activeInHierarchy;
        });

        fsm.AddAnyTransition(dead, () => GetComponent<Health>().IsDead);

        fsm.SetInitialState(idle);
    }
    public override void Attack(GameObject target)
    {
        if (target.TryGetComponent(out IDamageable player))
        {
            player.TakeDamage(enemyData.attackPower);
        }

    }
}
