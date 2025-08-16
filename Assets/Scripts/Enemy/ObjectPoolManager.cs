using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Inst;
    private Transform aIPool;

    [System.Serializable]
    public class Pool
    {
        [HideInInspector] public EENEMYTYPE type;
        public GameObject prefab;
        public int initialSize;
    }

    public List<Pool> pools;
    private Dictionary<EENEMYTYPE, Queue<GameObject>> poolDict;

    private void Awake()
    {
        Inst = this;
        poolDict = new Dictionary<EENEMYTYPE, Queue<GameObject>>();

        //aIPool = new GameObject("aIPool").transform;
        //aIPool.transform.position = new Vector3(1000, 0, 1000);

        foreach (var pool in pools)
        {
            var type = pool.prefab.GetComponent<AI_Base>().enemyData.type;
            pool.type = type;

            var queue = new Queue<GameObject>();
            for (int i = 0; i < pool.initialSize; i++)
            {
                var obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            poolDict.Add(type, queue);
        }
    }

    public GameObject Get(EENEMYTYPE type)
    {
        if (poolDict[type].Count == 0)
        {
            var prefab = pools.Find(p => p.type == type).prefab;
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            poolDict[type].Enqueue(obj);
        }

        var go = poolDict[type].Dequeue();
       // go.transform.SetParent(aIPool, true); 
        go.SetActive(true);
        return go;
    }

    public void Return(EENEMYTYPE type, GameObject obj)
    {
        obj.SetActive(false);
       // obj.transform.SetParent(aIPool); 
        poolDict[type].Enqueue(obj);
    }

    public Transform GetPoolRoot()
    {
        return aIPool;
    }
}
