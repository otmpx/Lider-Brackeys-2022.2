using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class SpawnDirector : MonoBehaviour
{
    public float roomRadius = 20f;
    public float noSpawnRadius = 10f;
    public Skeleton skeletonPrefab;
    public int currentEnemies;
    public static SpawnDirector instance;

    protected virtual void Awake()
    {
        instance = this;
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

    public Skeleton SpawnEnemy()
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
        Skeleton enemy = Instantiate(skeletonPrefab, enemySpawn, randRot).GetComponent<Skeleton>();
        currentEnemies++;
        return enemy;
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


