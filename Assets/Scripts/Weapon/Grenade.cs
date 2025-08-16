using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private WeaponDataSO weaponDataSO;
    private Rigidbody _rigidbody;
    private SphereCollider sphereCollider;

    [SerializeField] GameObject buffEffect;
    private bool isExplosion = false;
    [SerializeField] HashSet<Collider> effectedTargets = new HashSet<Collider>();

    public void Init(WeaponDataSO weaponDataSO, Vector3 direction, float force)
    {
        this.weaponDataSO = weaponDataSO;
        _rigidbody = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        sphereCollider.enabled = false;
        sphereCollider.radius = weaponDataSO.range;

        Throw(direction, force);
    }

    void Throw(Vector3 direction, float force)
    {
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isExplosion)
        {
            AudioManager.Instance.PlaySFX("BoomSound");
            isExplosion = true;
            StartCoroutine(GrenadeEffectCoroutine());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        ApplyEffect(other);
    }

    void OnTriggerExit(Collider other)
    {
        RemoveEffect(other);
    }

    IEnumerator GrenadeEffectCoroutine()
    {
        yield return new WaitForSeconds(weaponDataSO.explosionDelay);

        // 수류탄 모델 비활성화 후 이펙트 생성
        _rigidbody.velocity = Vector3.zero;
        transform.GetChild(0).gameObject.SetActive(false);
        GameObject effect = GenerateEffect();
        sphereCollider.enabled = true;

        // 콜라이더 초기 활성화 시 TriggerEnter가 되지않을 경우 대비
        Collider[] targets = Physics.OverlapSphere(transform.position, weaponDataSO.range);
        foreach (var target in targets)
        {
            OnTriggerEnter(target);
        }

        yield return new WaitForSeconds(weaponDataSO.duration);

        foreach (var target in effectedTargets)
        {
            if (target != null)
                RemoveEffect(target);
        }

        Destroy(gameObject);
        Destroy(effect);
    }

    void ApplyEffect(Collider target)
    {
        // 적용 중이지 않은 Target만 적용
        if (effectedTargets.Contains(target)) return;

        switch (weaponDataSO.weaponType)
        {
            case WeaponType.Buff:
                if (target.TryGetComponent(out Turret turret))
                {
                    // 터렛 공격속도 강화 효과
                    turret.DecreaseDelay(weaponDataSO.buffEffect.attackSpeedMultiplier);
                    effectedTargets.Add(target);
                }
                break;

            case WeaponType.Debuff:
                if (target.TryGetComponent(out AI_Base enemy))
                {
                    // 적 이동속도 감소 효과
                    enemy.ApplyBuff(weaponDataSO.debuffEffect.speedMultiplier);
                    effectedTargets.Add(target);
                }
                break;
        }
    }

    private void RemoveEffect(Collider target)
    {
        switch (weaponDataSO.weaponType)
        {
            case WeaponType.Buff:
                if (target.TryGetComponent(out Turret turret))
                {
                    // 터렛 강화 효과 제거
                    turret.RestoreDelay();
                }
                break;

            case WeaponType.Debuff:
                if (target.TryGetComponent(out AI_Base enemy))
                {
                    // 적 디버프 제거
                    enemy.RemoveBuff();
                }
                break;
        }
    }

    private GameObject GenerateEffect()
    {
        GameObject effect = Instantiate(buffEffect, transform.position, Quaternion.identity);

        ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>();

        foreach (var particle in particles)
        {
            var shape = particle.shape;
            shape.radius = weaponDataSO.range;
        }

        return effect;
    }
}
