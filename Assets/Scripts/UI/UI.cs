using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public Text levelCounter;
    public RectTransform crosshair;
    public float triggerSize;
    public float restingSize;
    public float speed;
    float currentSize;
    [HideInInspector] public float alpha;
    Image[] crosshairLines;
    private void Awake()
    {
        Instance = this;
        crosshairLines = GetComponentsInChildren<Image>();
    }
    private void Update()
    {
        levelCounter.text = $"{LevelDirector.instance.coinsCollected} / {LevelDirector.instance.coinsRequired}";

        Color fade = Color.white;
        fade.a = alpha;
        for (int i = 0; i < crosshairLines.Length; i++)
            crosshairLines[i].color = fade;
        
        if (currentSize > restingSize + 2f)
            currentSize = Mathf.Lerp(currentSize, restingSize, Time.deltaTime * speed);
        else
            currentSize = restingSize;

        crosshair.sizeDelta = new Vector2(currentSize, currentSize);
    }
    public void TriggerCrosshair()
    {
        currentSize = triggerSize;
    }
}