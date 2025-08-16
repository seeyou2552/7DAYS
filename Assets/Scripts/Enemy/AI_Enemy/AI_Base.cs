using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;
using Random = UnityEngine.Random;

public abstract class AI_Base : MonoBehaviour
{
    public SO_EnemyAI enemyData;
    protected Animator animator;
    protected AudioSource audioSource;
    protected NavMeshAgent agent;
    protected AIStateMachine fsm = new();
    public GameObject player;
    private Health health;

    private float shakeDuration = 0.5f;
    private float magnitude = 0.05f;



    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        animator =  GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (TryGetComponent(out agent))
        {
            agent.speed = enemyData.moveSpeed;
        }
    }

    protected virtual void OnEnable()
    {
        health.Init(enemyData.maxHealth);
        health.OnTakeDamage += DamageEffect;

        if(agent != null)
         agent.enabled = false;
    }



    protected virtual void OnDisable()
    {
        health.OnTakeDamage -= DamageEffect;
    }


    protected virtual void Start()
    {
        health.Init(enemyData.maxHealth);
    }

    public virtual void Init()
    {

    }

    protected virtual void Update()
    {
        fsm.Tick();
    }
    public abstract void Attack(GameObject target);

    protected abstract void Setting();


    // 버프 적용 [speed만 있어서 일단 이렇게 적용]
    public void ApplyBuff(float multiplier)
    {
        Debug.Log("DeBuff Apply");
        agent.speed *= multiplier;
    }

    // 버프 해제
    public void RemoveBuff()
    {
        Debug.Log("DeBuff hh");
        agent.speed = enemyData.moveSpeed;
    }

    private void DamageEffect()
    {
        //StartCoroutine(Shake(0.2f, 0.1f));  // 지속시간, 진폭
    }

    private IEnumerator Shake(float v1, float v2)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(offsetX, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
