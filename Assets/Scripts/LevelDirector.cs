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
    [TextArea] public string levelPrompt;
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
    Vignette vignette;
    public CinemachineVirtualCamera vCam;
    //public static int currentRoomIndex = 0;
    public int coinsCollected;
    public int coinsRequired = 3;
    public int roomId;
    public static RoomSettings CurrentRoom => instance.allRooms[instance.roomId];
    [HideInInspector] public CinemachinePOV povController;
    [HideInInspector] public CinemachineBasicMultiChannelPerlin headBob;

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
            headBob = instance.vCam.GetComponentPipeline().First(hb => hb is CinemachineBasicMultiChannelPerlin) as CinemachineBasicMultiChannelPerlin;
            povController.m_HorizontalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            povController.m_VerticalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            SetSensitivity();
            vignette = cam.GetComponent<Vignette>();
        }
        else
            Destroy(gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseUnpause();
        if (Input.GetKeyDown(KeyCode.F1))
            UI.Instance.gameObject.SetActive(!UI.Instance.isActiveAndEnabled);
        if (Input.GetKeyDown(KeyCode.F2))
            vignette.enabled = !vignette.enabled;
        if (Input.GetKeyDown(KeyCode.F3))
            AdvanceLevel();
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
    }


    public void ReloadLevel()
    {
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