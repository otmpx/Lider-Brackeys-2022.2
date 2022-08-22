using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Cardinal
{
    North,
    South,
    East,
    West
}

public class LevelDirector : MonoBehaviour
{
    [Tooltip("Including Intro room")] public int totalRooms;

    public Cardinal[] correctDoors;
    public Cardinal lastDoor;
    public List<int> roomOrder = new List<int>();
    public static LevelDirector Instance;

    public int currentRoomIndex = 0;
    public bool[] roomsExplored;
    public static int maxSanity = 100;
    public static int playerSanity = 0;

    public float globalTimer = 0f;
    public bool isSpeedRunTimerActivated = false;

    public Gradient[] enemySpawnWeights;

    public int minRandRoomsIndex = 1;
    public int maxRandRoomindex = 6;

    public float Scaling()
    {
        return (float) (currentRoomIndex-1) / (totalRooms-1);
    }

    public Gradient CurrentEnemyWeights()
    {
        return enemySpawnWeights[currentRoomIndex];
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            correctDoors = new Cardinal[totalRooms];
            correctDoors[0] = Cardinal.North;

            roomsExplored = new bool[totalRooms];
            playerSanity = maxSanity;

            for (int i = 1; i < totalRooms; i++)
            {
                bool isOpposite;
                Cardinal generated;
                do
                {
                    generated = (Cardinal)Random.Range(0, 4);
                    isOpposite = false;
                    switch (generated)
                    {
                        case Cardinal.North:
                            if (correctDoors[i - 1] == Cardinal.South)
                                isOpposite = true;
                            break;
                        case Cardinal.South:
                            if (correctDoors[i - 1] == Cardinal.North)
                                isOpposite = true;
                            break;
                        case Cardinal.East:
                            if (correctDoors[i - 1] == Cardinal.West)
                                isOpposite = true;
                            break;
                        case Cardinal.West:
                            if (correctDoors[i - 1] == Cardinal.East)
                                isOpposite = true;
                            break;
                    }
                } while (isOpposite);

                correctDoors[i] = generated;
                roomsExplored[i] = false;
                AudioManager.Instance.PlayMusic(AudioManager.Instance.themeLoop);
            }

            roomOrder = KongrooUtils.ShuffleArray(Enumerable.Range(1, totalRooms - 1).ToArray()).Prepend(0).ToList();

            // ShuffleRooms();
            //TODO: disable all graves in settings first
            // ShuffleGraves();
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        playerSanity = Mathf.Clamp(playerSanity, 0, maxSanity);
        if (isSpeedRunTimerActivated)
            globalTimer += Time.deltaTime;
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

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    private void ReloadLevel()
    {
        // RoomSaver.instance.SaveRoom();
        FixedRoomSaver.instance.SaveRoom(SceneManager.GetActiveScene());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToFirstRoom(Cardinal chosenDoor)
    {
        AudioManager.Instance.soundSource.PlayOneShot(AudioManager.Instance.wrongdoorSound);
        //RoomSaver.instance.SaveRoom(RoomManager.instance);
        lastDoor = chosenDoor;
        currentRoomIndex = 1;
        LoadScene(roomOrder[1]);
    }

    public void GoToNextRoom()
    {
        AudioManager.Instance.soundSource.PlayOneShot(AudioManager.Instance.correctdoorSound);
        //RoomSaver.instance.SaveRoom(RoomManager.instance);
        currentRoomIndex++;
        if (currentRoomIndex == roomOrder.Count)
        {
            // Win
            return;
        }
        if (currentRoomIndex != 0)
        {
            roomsExplored[currentRoomIndex] = true;
        }
        LoadScene(roomOrder[currentRoomIndex]+1); // +1 for intro scene
    }

    public void RestartGame()
    {
        Destroy(gameObject);
        LoadScene(roomOrder[0]+1);
    }

    public void UpdateSanity(int value)
    {
        playerSanity += value;
        UI.Instance.sanityValue = playerSanity;
        //if (value < 0)
        //    Player.Instance.takedamageSound.Play(Player.Instance);
    }

    public static Cardinal InvertCardinal(Cardinal input)
    {
        switch (input)
        {
            case Cardinal.North:
                return Cardinal.South;
            case Cardinal.South:
                return Cardinal.North;
            case Cardinal.East:
                return Cardinal.West;
            case Cardinal.West:
                return Cardinal.East;
            default:
                throw new System.Exception("Thats not a direction u fuck");
        }
    }
}