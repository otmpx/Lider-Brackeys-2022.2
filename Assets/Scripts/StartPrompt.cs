using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPrompt : MonoBehaviour
{
    public bool showControls = false;
    public Image controls;
    public Text text;
    public float fadeoutDur = 1f;
    bool started = false;
    float startTime;
    private void Start()
    {
        if (!showControls)
            controls.gameObject.SetActive(false);
        text.text = LevelDirector.CurrentRoom.levelPrompt;
    }
    private void Update()
    {
        if (Player.Instance.isShooting && !started)
        {
            startTime = Time.time;
            started = true;
        }
        if (started)
        {
            Color controlsA = controls.color;
            controlsA.a = Mathf.Lerp(1, 0, Mathf.Clamp01((Time.time - startTime) / fadeoutDur));
            controls.color = controlsA;

            Color textA = text.color;
            textA.a = Mathf.Lerp(1, 0, Mathf.Clamp01((Time.time - startTime) / fadeoutDur));
            text.color = textA;

            if (Time.time - startTime > fadeoutDur)
            {
                controls.gameObject.SetActive(false);
                text.gameObject.SetActive(false);
            }
        }
    }
}
