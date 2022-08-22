using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    public List<Door> doors = new List<Door>();

    //public int currentRoom;
    public Cardinal correctDoor;
    public Cardinal lastDoor;

    public Vector3 spawn;

    [Tooltip("if 1 spawns 1 extra enemy last stage")]
    public float scaledEnemySpawns = 3f;

    public int staticText = 3;
    //public float scaledRandomText = 1;
    //public float scaledDirectionText = 0.5f;

    // public int maxCorrectText = 1;
    public float maxTextRot = 50f;
    [Range(-1, 1)] public float baseTextSpawn = 0.1f;
    public GameObject textPrefab;

    public TextMeshPro[] directionText;
    public int totalEnemies = 3;

    public static RoomManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        var currentRoomIndex = LevelDirector.Instance.currentRoomIndex;

        //correctDoor = LevelDirector.Instance.correctDoors[currentRoomIndex];
        //GetDoor(correctDoor).isReal = true;
        //ComputeLastDoor();

        //int randomSpawns = Mathf.CeilToInt(currentRoomIndex * scaledRandomText);

        //for (int i = 0; i < randomSpawns; i++)
        //{
        //    string text = LevelDirector.Instance.groundTextOptions[
        //        Random.Range(0, LevelDirector.Instance.groundTextOptions.Count)];
        //    SpawnText(text);
        //}

         //int dirSpawns = Mathf.CeilToInt((LevelDirector.Instance.totalRooms - currentRoomIndex) * scaledDirectionText) + staticRandomText;
        int dirSpawns = staticText;

        for (int i = 0; i < dirSpawns; i++)
        {
            string text = correctDoor.ToString();
            SpawnText(text);
        }

        for (int i = 0; i < directionText.Length; i++)
        {
            directionText[i].gameObject.SetActive(false);
        }
        // for (int i = 0; i < directionText.Length; i++)
        // {
        //     directionText[i].text = correctDoor.ToString();
        //     print(correctDoor.ToString());
        //     if ((1 - LevelDirector.Instance.Scaling()) + baseTextSpawn > Random.value)
        //     {
        //         directionText[i].gameObject.SetActive(true);
        //     }
        //     else
        //     {
        //         directionText[i].gameObject.SetActive(false);
        //     }
        // }

        SpawnEnemies();
        //RoomSaver.instance.LoadRoom(this);
        
    }

    private void SpawnEnemies()
    {
        if (LevelDirector.Instance.currentRoomIndex == 0) return;
        totalEnemies += Mathf.CeilToInt(LevelDirector.Instance.Scaling() * scaledEnemySpawns);

        for (int i = 0; i < totalEnemies; i++)
        {
            SpawnDirector.instance.SpawnEnemy();
        }

    }

    //private void OnEnable()
    //{
    //    if (LevelDirector.Instance.currentRoomIndex == 0) return;
    //    SpawnDirector.instance.allEnemiesDead += OpenDoor;
    //}

    //private void OnDisable()
    //{
    //    if (LevelDirector.Instance.currentRoomIndex == 0) return;
    //    SpawnDirector.instance.allEnemiesDead -= OpenDoor;
    //}

    //private void OpenDoor()
    //{

    //}

    private void ComputeLastDoor()
    {
        //if (LevelDirector.Instance.currentRoomIndex == 0) return;
        ////else if (LevelDirector.Instance.currentRoomIndex == 1)
        ////{
        ////    this.lastDoor = LevelDirector.Instance.wrongDoor;
        ////}
        ////else
        ////    this.lastDoor = LevelDirector.Instance.correctDoors[LevelDirector.Instance.currentRoomIndex - 1];
        //else
        //{
        //    lastDoor = LevelDirector.Instance.lastDoor;
        //}

        Door lastDoorConnection = GetDoor(LevelDirector.InvertCardinal(lastDoor));
        Player.Instance.transform.position = lastDoorConnection.spawnPos.position;
        Player.Instance.transform.rotation = lastDoorConnection.spawnPos.rotation;
    }

    private Door GetDoor(Cardinal input)
    {
        return doors.Find((d) => d.direction == input);
    }

    public void SpawnText(string input)
    {
        Quaternion textRot = Quaternion.Euler(90, Random.Range(-maxTextRot, maxTextRot), 0);
        TextMeshPro text = Instantiate(textPrefab, SpawnDirector.instance.RandomNavmeshLocation(), textRot).GetComponent<TextMeshPro>();
        text.text = input;
    }






    //Tendency to return point near heavy mesh areas (more indices there)
    // public Vector3 RandomNavmeshLocation()
    // {
    //     NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
    //     // Pick the first indice of a random triangle in the nav mesh
    //     int t = Random.Range(0, navMeshData.indices.Length - 3);
    //
    //     // Select a random point on it
    //     return Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]],
    //         navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
    //     
    // }

    // private void Update()
    // {
    //     SpawnEnemy();
    // }

   
}