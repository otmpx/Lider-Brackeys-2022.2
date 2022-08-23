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

    public int currentRoomIndex = 0;
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
    private void Update()
    {
    }
    //void OnEnable()
    //{
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //}

    //void OnDisable()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //}

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    //UI.Instance.UpdateSanityBarSize();
    //}


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
}