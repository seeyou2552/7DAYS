using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public void SpawnResource(GameObject obj, float time) // 스폰 코루틴 추가
    {
        StartCoroutine(Spawn(obj, time));
    }

    IEnumerator Spawn(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(true);
    }
}
