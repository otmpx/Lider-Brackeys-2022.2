using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPrompt : MonoBehaviour
{
    public Image controls;
    public Text text;
    public float fadeoutDur = 1f;
    bool started = false;
    float startTime;
    private void Update()
    {
        if (Player.Instance.isShooting && !started)
        {
            startTime = Time.time;
            started = true;
        }
        if (started)
        {
            if (controls != null)
            {
                Color controlsA = controls.color;
                controlsA.a = Mathf.Lerp(1, 0, Mathf.Clamp01((Time.time - startTime) / fadeoutDur));
                controls.color = controlsA;
            }

            if (text != null)
            {
                Color textA = text.color;
                textA.a = Mathf.Lerp(1, 0, Mathf.Clamp01((Time.time - startTime) / fadeoutDur));
                text.color = textA;
            }

            if (Time.time - startTime > fadeoutDur)
            {
                if (controls != null)
                    controls.gameObject.SetActive(false);
                if (text != null)
                    text.gameObject.SetActive(false);
            }
        }
    }
}
