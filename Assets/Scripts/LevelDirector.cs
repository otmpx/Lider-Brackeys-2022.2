using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
[Serializable]
public class RoomSettings
{
    public string sceneName;
    public int enemySpawns;
    public int objectiveSpawns;
}
public class LevelDirector : MonoBehaviour
{
    public static LevelDirector Instance;
    public RoomSettings[] allRooms;
    public CinemachineVirtualCamera vCam;
    public int test;
    public static int currentRoomIndex = 0;
    public int coinsCollected;
    public int coinsRequired = 3;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        coinsCollected = 0;
        currentRoomIndex = SceneManager.GetActiveScene().buildIndex;
        test = currentRoomIndex;
    }


    public void ReloadLevel()
    {
        // RoomSaver.instance.SaveRoom();
        FixedRoomSaver.instance.SaveRoom(SceneManager.GetActiveScene());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(allRooms[0].sceneName);
    }
    public void PlayJumpscare()
    {

    }
    public void RegisterCoin()
    {
        coinsCollected++;
        if (coinsCollected >= coinsRequired)
        {
            SceneManager.LoadScene(allRooms[currentRoomIndex].sceneName);
        }
    }
}