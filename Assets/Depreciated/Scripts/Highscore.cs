using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Highscore : MonoBehaviour
{
    TextMeshPro bestTime;
    private void Awake()
    {
        bestTime = GetComponent<TextMeshPro>();
    }
    private void Start()
    {
        if (FixedRoomSaver.instance.bestSpeedrunTime != 0)
        {
            float timer = FixedRoomSaver.instance.bestSpeedrunTime;
            string minutes = Mathf.Floor(timer / 60).ToString("00");
            string seconds = Mathf.Floor(timer % 60).ToString("00");
            string miliseconds = (timer % 1).ToString("000");
            bestTime.text = "Your best time\n" + minutes + ":" + seconds + ":" + miliseconds;
        }
        else
            bestTime.text = "";
    }
}
