using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Spine : AI_Base
{
    [SerializeField] private GameObject projectilePrefab;
    private float startY = 9f;
    private GameObject targetObj;

    protected override void Start()
    {
        base.Start();
        Setting();
    }

    public override void Init()
    {
        Vector3 pos = transform.position;
        pos.y = startY;
        transform.position = pos;
    }

    protected override void Setting()
    {
        var idle = new AIIdleState(gameObject);
        var chase = new AIFlyingChaseState(gameObject, TestGameManager.Inst.testPlayer.transform);
        var attack = new AIAttackState(gameObject);
        var dead = new AIDeadState(gameObject);

        fsm.AddTransition(idle, chase, () => Vector3.Distance(transform.position,
            TestGameManager.Inst.testPlayer.transform.position) < enemyData.chasingRange);
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


        fsm.AddTransition(attack, idle, () =>
        {
            var t = attack.CurrentTarget;
            return t == null || !t.activeInHierarchy;
        });

        fsm.AddAnyTransition(dead, () => GetComponent<Health>().IsDead);

        fsm.SetInitialState(idle);

    }

    public override void Attack(GameObject target)
    {
        if (targetObj == null || targetObj != target)
        {
            targetObj = target;
        }
            
    }



    public void AnimAttack()
    {
        if (projectilePrefab == null || targetObj == null)
            return;

        Vector3 spawnPos = transform.position + transform.forward * 2;
        Vector3 targetPos = targetObj.transform.position;

        Vector3 direction = (targetPos - spawnPos).normalized;
        float distance = Vector3.Distance(spawnPos, targetPos);
        float duration = 2f;
        float speed = distance / duration;

        GameObject proj = GameObject.Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));
        proj.GetComponent<Projectile>().Init(gameObject);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.velocity = direction * speed;
    }
}
