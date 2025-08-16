using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDotTime : MonoBehaviour
{
    public float damagePerTick = 2f;
    public float tickInterval = 1f;
    public float duration = 5f;

    private GameObject target;      //Test용

    public void Apply()
    {
        StartCoroutine(DoTickDamage());
    }

    private IEnumerator DoTickDamage()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (target == null)
                yield break;

            GetComponent<IDamageable>().TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;
        }

        Destroy(this); // 끝나면 자기 자신 제거
    }
}
