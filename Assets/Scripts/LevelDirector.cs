using Cinemachine;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class LidarExtensions
{
    public static Color ToColor(this PointType type) => type switch
    {
        PointType.Static => Color.white,
        PointType.Dynamic => Color.blue,
        PointType.Enemy => Color.red,
        PointType.Objective => Color.yellow,
        _ => throw new NotImplementedException()
    };
}


[Serializable]
public class RoomSettings
{
    public string sceneName;
    public string levelPrompt;
    public int enemySpawns;
    public int objectiveSpawns;
    public MusicTheme levelTheme;
    [Range(0, 1)]
    public float musicVol;
}
public class LevelDirector : MonoBehaviour
{
    public static LevelDirector instance;
    public RoomSettings[] allRooms;
    public Camera cam;
    public CinemachineVirtualCamera vCam;
    //public static int currentRoomIndex = 0;
    public int coinsCollected;
    public int coinsRequired = 3;
    public int roomId;
    public static RoomSettings CurrentRoom => instance.allRooms[instance.roomId];
    [HideInInspector] public CinemachinePOV povController;
    public PauseUI pauseScreen;
    [HideInInspector] public bool paused = false;

    public AudioClip objectiveSound;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            povController = instance.vCam.GetComponentPipeline().First(cb => cb is CinemachinePOV) as CinemachinePOV;
            povController.m_HorizontalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            povController.m_VerticalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            SetSensitivity();
        }
        else
            Destroy(gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseUnpause();
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        coinsCollected = 0;
        //currentRoomIndex = SceneManager.GetActiveScene().buildIndex;
    }


    public void ReloadLevel()
    {
        // RoomSaver.instance.SaveRoom();
        //FixedRoomSaver.instance.SaveRoom(SceneManager.GetActiveScene());
        if (paused)
            PauseUnpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartGame()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(allRooms[0].sceneName);
    }
    public void RegisterCoin()
    {
        coinsCollected++;
        AudioManager.instance.PlaySFX(objectiveSound);
        if (coinsCollected >= coinsRequired)
            AdvanceLevel();
    }
    public void AdvanceLevel()
    {
        roomId++;
        if (roomId > allRooms.Length)
        {
            SceneManager.LoadScene("End");
            Destroy(gameObject);
        }
        else
            SceneManager.LoadScene(CurrentRoom.sceneName);
    }
    public void SetSensitivity()
    {
        instance.povController.m_HorizontalAxis.m_MaxSpeed = Settings.aimSensitivity;
        instance.povController.m_VerticalAxis.m_MaxSpeed = Settings.aimSensitivity;
    }
    public void PauseUnpause()
    {
        paused = !paused;
        if (paused)
        {
            Time.timeScale = 0;
            pauseScreen.gameObject.SetActive(true);
            pauseScreen.LoadPause();
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1;
            pauseScreen.gameObject.SetActive(false);
            SetSensitivity();
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void QuitToMenu()
    {
        SceneManager.LoadScene(1);
        Destroy(gameObject);
    }
}