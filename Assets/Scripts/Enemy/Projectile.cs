using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject attacker { get; private set; }
    [SerializeField] private float autoDestroyTime = 5f;

    public void Init(GameObject attacker)
    {
        this.attacker = attacker;

        Invoke(nameof(DestroyObject), autoDestroyTime);
    }

 

    protected virtual void OnTriggerEnter(Collider other)
    {
        // 충돌하면 타워 대미지 

        // 자신한테 맞지 않게
        if (other.gameObject == attacker) return;

        
        if (other.TryGetComponent(out IDamageable target))
        {
            // 중복 DoT 방지 (선택)
            if (other.GetComponent<DamageDotTime>() == null)
            {
                var dot = other.gameObject.AddComponent<DamageDotTime>();
                dot.Apply();

            }

            else
            {
                target.TakeDamage(attacker.GetComponent<AI_Base>().enemyData.attackPower);
            }
            
            Destroy(gameObject); // 투사체 제거
        }

    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
