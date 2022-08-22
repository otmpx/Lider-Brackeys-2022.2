using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PoolType
{
    NOPOOL,
    CoomBlood,
    FriendlyProjectile,
    EnemyProjectile,
    Projectile,
    BoneWall,
    HomingProjectile
}

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public PoolType tag;
        public GameObject prefab;
        [Tooltip("Initial objects spawned")] public int initSize;
    }

    public List<Pool> pools;
    private Dictionary<PoolType, Queue<GameObject>> poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();
    public static ObjectPooler instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.initSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.GetComponent<IPoolable>().reusable = true;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    private GameObject Spawn(PoolType tag, Vector3 pos, Quaternion rot)
    {
        if (tag == PoolType.NOPOOL)
            throw new System.Exception("Object type not asked to pool");

        if (poolDictionary[tag].Count == 0)
        {
            Debug.Log($"Spare instance of {tag.ToString()} instantiated");
            var instance = Instantiate(pools.Find((p) => p.tag == tag).prefab, pos, rot);
            instance.GetComponent<IPoolable>().reusable = false;
            return instance;
        }

        var dequeued = poolDictionary[tag].Dequeue();
        dequeued.SetActive(true);
        dequeued.transform.position = pos;
        dequeued.transform.rotation = rot;

        return dequeued;
    }
    public static GameObject Instantiate(PoolType tag, Vector3 pos, Quaternion rot)
    {
        return instance.Spawn(tag, pos, rot);
    }

    public void Despawn(PoolType tag, GameObject obj)
    {
        if (tag == PoolType.NOPOOL)
        {
            Destroy(obj);
        }

        poolDictionary[tag].Enqueue(obj);
        obj.SetActive(false);
    }
}

public interface IPoolable
{
    public bool reusable { get; set; }
    public void ReturnToPool();
}