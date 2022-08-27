using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Enemy;
using UnityEngine.SceneManagement;

public class SpawnDirector : MonoBehaviour
{
    public Transform[] spawnPos;
    public Transform[] coinPos;
    public float roomRadius = 20f;
    public float noSpawnRadius = 10f;
    public Player playerPrefab;
    public Skeleton skeletonPrefab;
    public Coin coinPrefab;
    //public int currentEnemies;
    public static SpawnDirector instance;

    protected virtual void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        SpawnPlayer();
        SpawnCoins();
        for (int i = 0; i < LevelDirector.CurrentRoom.enemySpawns; i++)
        {
            SpawnEnemy();
        }
    }
    //Instead of using inheritance on room manager, treat spawn director as a different body part and compositionalise
    public Vector3 RandomNavmeshLocation()
    {
        Vector3 randomPos = Random.insideUnitSphere * roomRadius;
        UnityEngine.AI.NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;

        if (UnityEngine.AI.NavMesh.SamplePosition(randomPos, out hit, roomRadius, 1))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }
    public void SpawnPlayer()
    {
        int rand = Random.Range(0, spawnPos.Length);
        Player player = Instantiate(playerPrefab, spawnPos[rand].position, Quaternion.identity, transform.parent).GetComponent<Player>();
        // Sets initial rotation to the transform rotation of spawnPos
        LevelDirector.instance.povController.m_HorizontalAxis.Value = spawnPos[rand].localEulerAngles.y;
        LevelDirector.instance.povController.m_VerticalAxis.Value = 0;
    }
    public void SpawnEnemy()
    {
        // Spawn enemy in random location outside light radius

        if (noSpawnRadius > roomRadius) throw new System.Exception("no spawn radius is bigger than room");
        Vector3 enemySpawn = RandomNavmeshLocation();
        while (Vector3.Distance(Player.Instance.transform.position, enemySpawn) < noSpawnRadius)
        {
            // Exclude this spawn location by looping until a location is picked outside the range
            enemySpawn = RandomNavmeshLocation();
        }

        Quaternion randRot = Quaternion.LookRotation(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized);
        Instantiate(skeletonPrefab, enemySpawn, randRot);
        //currentEnemies++;
        //return enemy;
    }
    public void SpawnCoins()
    {
        if (coinPos == null) return;
        KongrooUtils.ShuffleArray(coinPos);
        for (int i = 0; i < LevelDirector.CurrentRoom.objectiveSpawns; i++)
        {
            Instantiate(coinPrefab, coinPos[i].transform.position, coinPos[i].transform.rotation);
        }
        foreach (var item in coinPos)
        {
            item.gameObject.SetActive(false);
        }
    }
    //protected virtual GameObject SelectEnemyPrefab()
    //{
    //    Color enemyColour = LevelDirector.Instance.CurrentEnemyWeights().Evaluate(Random.value);
    //    foreach (var enemyPrefab in spawnEnemyPrefabs)
    //    {
    //        if (enemyColour == enemyPrefab.GetComponent<Skeleton>().colour)
    //            return enemyPrefab;
    //    }

    //    throw new System.Exception($"Enemy Selection {enemyColour} not valid");
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 0.1f, 0.1f);
        Gizmos.DrawSphere(Vector3.zero, roomRadius);
        Gizmos.DrawSphere(Vector3.zero, noSpawnRadius);
    }
}


