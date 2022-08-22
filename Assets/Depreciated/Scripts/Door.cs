using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public bool isReal = false;
    public Transform spawnPos;
    public Cardinal direction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //if (!isReal)
                //LevelDirector.Instance.ChooseWrongRoom(direction);
            //else
                LevelDirector.Instance.GoToNextRoom();
        }
    }
}
