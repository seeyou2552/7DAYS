using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{

    [SerializeField] private float health;
    public float CurrentHealth => health;
    private float maxHealth;

    public float MaxHealth => maxHealth;


    public event Action OnTakeDamage;
    public event Action OnDie;
    public bool IsDead => health == 0;

 

    public void Init(float maxHP)
    {
        maxHealth = Mathf.Max(maxHP, health);
        this.health = maxHP;
    }

    public void TakeDamage(float amount)
    {

        if (health == 0)
            return;

        health = Mathf.Max(health - amount, 0);
        AudioManager.Instance.PlaySFX("EnemyHit");
        OnTakeDamage?.Invoke();

        if (health == 0)
        {
            AudioManager.Instance.PlaySFX("EnemyDie");
            OnDie?.Invoke();

        }
    }

    public float GetHealth()
    {
        return health;
    }
}
