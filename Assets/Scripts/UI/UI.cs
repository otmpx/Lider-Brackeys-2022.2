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
    private void Update()
    {
        levelCounter.text = $"{LevelDirector.instance.coinsCollected} / {LevelDirector.instance.coinsRequired}";
    }
}