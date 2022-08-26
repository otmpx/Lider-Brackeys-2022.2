using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Button musicDown, musicUp, soundDown, soundUp, sensDown, sensUp;
    public Text musicDisp, soundDisp, sensDisp;
    private void Awake()
    {
        musicDown.onClick.AddListener(() => SetMusicVol(-0.1f));
        musicDown.onClick.AddListener(() => AudioManager.instance.PlayUISound());
        musicUp.onClick.AddListener(() => SetMusicVol(0.1f));
        musicUp.onClick.AddListener(() => AudioManager.instance.PlayUISound());
        soundDown.onClick.AddListener(() => SetSoundVol(-0.1f));
        soundDown.onClick.AddListener(() => AudioManager.instance.PlayUISound());
        soundUp.onClick.AddListener(() => SetSoundVol(0.1f));
        soundUp.onClick.AddListener(() => AudioManager.instance.PlayUISound());
        sensDown.onClick.AddListener(() => SetSens(-0.5f));
        sensDown.onClick.AddListener(() => AudioManager.instance.PlayUISound());
        sensUp.onClick.AddListener(() => SetSens(0.5f));
        sensUp.onClick.AddListener(() => AudioManager.instance.PlayUISound());
    }
    private void OnEnable()
    {
        musicDisp.text = Mathf.FloorToInt(Settings.musicVol * 10).ToString();
        soundDisp.text = Mathf.FloorToInt(Settings.soundVol * 10).ToString();
        sensDisp.text = Mathf.FloorToInt(Settings.aimSensitivity).ToString();
    }
    public void SetMusicVol(float value)
    {
        float vol = Mathf.Clamp01(Settings.musicVol + value);
        Settings.instance.SetMusicVol(vol);
        musicDisp.text = Mathf.FloorToInt(Settings.musicVol * 10).ToString();
    }
    public void SetSoundVol(float value)
    {
        float vol = Mathf.Clamp01(Settings.soundVol + value);
        Settings.instance.SetSoundVol(vol);
        soundDisp.text = Mathf.FloorToInt(Settings.soundVol * 10).ToString();
    }
    public void SetSens(float value)
    {
        Settings.aimSensitivity = Mathf.Clamp(Settings.aimSensitivity + value, 0.5f, 5.5f);
        sensDisp.text = (Mathf.FloorToInt(Settings.aimSensitivity * 2) - 1).ToString();
    }
}