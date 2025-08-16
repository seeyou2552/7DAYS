using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Stealth : AI_Base
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

        fsm.AddTransition(idle, chase, () => Vector3.Distance(transform.position, TestGameManager.Inst.testPlayer.transform.position) < enemyData.chasingRange);
        fsm.AddTransition(chase, idle, () => Vector3.Distance(transform.position, TestGameManager.Inst.testPlayer.transform.position) > enemyData.chasingRange);
        fsm.AddTransition(chase, attack, () => Vector3.Distance(transform.position, TestGameManager.Inst.testPlayer.transform.position) < enemyData.attackRange);
        fsm.AddTransition(attack, chase, () => Vector3.Distance(transform.position, TestGameManager.Inst.testPlayer.transform.position) > enemyData.attackRange);
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
