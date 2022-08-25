using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


// [RequireComponent(typeof(LevelDirector))]
public class RoomSaver : MonoBehaviour
{
    public SerialKeyValuePair<string, GameObject>[] tagsToPrefab;

    public List<Dictionary<string, List<TransformLite>>>
        roomSaves = new List<Dictionary<string, List<TransformLite>>>();

    public List<bool[]> textSaves = new List<bool[]>();
    public static RoomSaver instance;

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


    public void SaveRoom()
    {
        var roomIndex = LevelDirector.Instance.roomId;
        //Hasnt instantiated save structs
        if (roomSaves.Count - 1 < roomIndex)
        {
            roomSaves.Add(new Dictionary<string, List<TransformLite>>());
            foreach (var tagPair in tagsToPrefab)
            {
                roomSaves[roomIndex][tagPair.key] = new List<TransformLite>();
            }

            //TODO: DOes not save random text from barry b benson (this is fine)
            //textSaves.Add(new bool[room.directionText.Length]);

        }
        var currentSave = roomSaves[roomIndex];
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

        //for (int i = 0; i < room.directionText.Length; i++)
        //{
        //    textSaves[roomIndex][i] = room.directionText[i].gameObject.activeInHierarchy;
        //}
    }

    public void LoadRoom(Scene arg0, LoadSceneMode arg1)
    {
        var roomIndex = LevelDirector.Instance.roomId;
        if (roomSaves.Count - 1 < roomIndex)
        {
            return;
        }

        var currentSave = roomSaves[roomIndex];
        foreach (var tagPair in tagsToPrefab)
        {
            foreach (var go in GameObject.FindGameObjectsWithTag(tagPair.key))
            {
                Destroy(go);
            }

            foreach (var trans in currentSave[tagPair.key])
            {
                //TODO offset by collision normal(do collide sphere)
                // var obj = Instantiate(tagPair.val, trans.pos, Quaternion.Euler(trans.rot));
                var obj = Instantiate(tagPair.val, trans.pos, trans.rot);
            }
        }

        //for (int i = 0; i < room.directionText.Length; i++)
        //{
        //    room.directionText[i].gameObject.SetActive(textSaves[roomIndex][i]);
        //}
    }

}