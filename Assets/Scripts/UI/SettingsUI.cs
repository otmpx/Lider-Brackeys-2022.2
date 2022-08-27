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
        musicDown.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonAdjustSound));
        musicUp.onClick.AddListener(() => SetMusicVol(0.1f));
        musicUp.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonAdjustSound));
        soundDown.onClick.AddListener(() => SetSoundVol(-0.1f));
        soundDown.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonAdjustSound));
        soundUp.onClick.AddListener(() => SetSoundVol(0.1f));
        soundUp.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonAdjustSound));
        sensDown.onClick.AddListener(() => SetSens(-0.5f));
        sensDown.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonAdjustSound));
        sensUp.onClick.AddListener(() => SetSens(0.5f));
        sensUp.onClick.AddListener(() => AudioManager.instance.PlaySFX(AudioManager.instance.buttonAdjustSound));
    }
    public void LoadSettings()
    {
        musicDisp.text = Mathf.FloorToInt(Settings.musicVol * 10).ToString();
        soundDisp.text = Mathf.FloorToInt(Settings.soundVol * 10).ToString();
        sensDisp.text = (Mathf.FloorToInt(Settings.aimSensitivity * 2) - 1).ToString();
    }
    public void SetMusicVol(float value)
    {
        Settings.musicVol = Mathf.Clamp01(Settings.musicVol + value);
        AudioManager.instance.SetMusicVol(Settings.musicVol);
        musicDisp.text = Mathf.FloorToInt(Settings.musicVol * 10).ToString();
    }
    public void SetSoundVol(float value)
    {
        Settings.soundVol = Mathf.Clamp01(Settings.soundVol + value);
        AudioManager.instance.SetSfxVol(Settings.soundVol);
        soundDisp.text = Mathf.FloorToInt(Settings.soundVol * 10).ToString();
    }
    public void SetSens(float value)
    {
        Settings.aimSensitivity = Mathf.Clamp(Settings.aimSensitivity + value, 0.5f, 5.5f);
        sensDisp.text = (Mathf.FloorToInt(Settings.aimSensitivity * 2) - 1).ToString();
    }
}