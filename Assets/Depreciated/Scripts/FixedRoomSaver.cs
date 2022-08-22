using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FixedRoomSaver : MonoBehaviour
{
    public SerialKeyValuePair<string, GameObject>[] tagsToPrefab;

    private Dictionary<int, Dictionary<string, List<TransformLite>>>
        roomSaves = new Dictionary<int, Dictionary<string, List<TransformLite>>>();

    public static FixedRoomSaver instance;
    public float bestSpeedrunTime = 0f;

    //Awake runs regardless if component is disabled or not
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LoadRoom;
        // SceneManager.sceneUnloaded += SaveRoom;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadRoom;
        // SceneManager.sceneUnloaded -= SaveRoom;
    }


    public void SaveRoom(Scene unload)
    {
        //Hasnt instantiated save structs
        int buildIndex = unload.buildIndex;
        if (!roomSaves.ContainsKey(buildIndex))
        {
            roomSaves.Add(buildIndex, new Dictionary<string, List<TransformLite>>());
            foreach (var tagPair in tagsToPrefab)
            {
                roomSaves[buildIndex][tagPair.key] = new List<TransformLite>();
            }
        }

        var currentSave = roomSaves[buildIndex];
        foreach (var tagPair in tagsToPrefab)
        {
            currentSave[tagPair.key].Clear();
            foreach (var go in GameObject.FindGameObjectsWithTag(tagPair.key))
            {
                TransformLite newSave;
                newSave.pos = go.transform.position;
                newSave.rot = go.transform.rotation;
                currentSave[tagPair.key].Add(newSave);
            }
        }
    }

    public void LoadRoom(Scene arg0, LoadSceneMode arg1)
    {
        int buildIndex = arg0.buildIndex;
        if (!roomSaves.ContainsKey(buildIndex))
        {
            return;
        }

        var currentSave = roomSaves[buildIndex];
        foreach (var tagPair in tagsToPrefab)
        {
            foreach (var go in GameObject.FindGameObjectsWithTag(tagPair.key))
            {
                Destroy(go);
            }

            foreach (var trans in currentSave[tagPair.key])
            {
                var obj = Instantiate(tagPair.val, trans.pos, trans.rot);
            }
        }
    }
}