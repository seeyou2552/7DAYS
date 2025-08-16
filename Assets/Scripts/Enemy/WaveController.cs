using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MonsterSpawnInfo
{
    public GameObject prefab;
    public int count;
}

[CreateAssetMenu(menuName = "Wave/WaveData")]
public class WaveData : ScriptableObject
{
    public List<MonsterSpawnInfo> spawnList;
    public float waveWaitTime;
}


public class WaveController : MonoBehaviour
{
    public List<WaveData> waves; // 웨이브 순서대로 등록
    public Transform[] spawnPoints; // 랜덤 스폰 지점

    private int currentWave = 0;

    public void StartNextWave()
    {
        if (currentWave >= waves.Count)
        {
            return;
        }

        var wave = waves[currentWave];
        StartCoroutine(SpawnWave(wave));
        currentWave++;
    }

    //시간남으면 Factory 패턴이랑 결합
    private IEnumerator SpawnWave(WaveData wave)
    {
        foreach (var info in wave.spawnList)
        {
            for (int i = 0; i < info.count; i++)
            {  var type = info.prefab.GetComponent<AI_Base>().enemyData.type;
                GameObject monster = ObjectPoolManager.Inst.Get(type);

                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                monster.transform.position = spawnPoint.position;


                monster.GetComponent<AI_Base>()?.Init();

                yield return new WaitForSeconds(wave.waveWaitTime);
            }
        }
    }
}
