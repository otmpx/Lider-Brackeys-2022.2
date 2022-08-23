using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public Text levelCounter;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        levelCounter.text = $"{LevelDirector.Instance.currentRoomIndex} / {LevelDirector.Instance.allRooms.Length}";
    }
}