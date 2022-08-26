using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Linq;
public class Settings : MonoBehaviour
{
    public static float musicVol;
    public static float soundVol;
    public static float aimSensitivity;
    public static Settings instance;

    private void Awake()
    {
        //headBob = LevelDirector.Instance.vCam.GetComponentPipeline().First(cb => cb is CinemachineBasicMultiChannelPerlin) as CinemachineBasicMultiChannelPerlin;

        //soundSlider.onValueChanged.AddListener((float value) => UpdateSoundSlider(value));
        //AudioManager.Instance.masterMixer.GetFloat("sfxVolume", out float soundDB);
        //soundSlider.value = Mathf.Pow(10.0f, soundDB / 20.0f);
        //musicSlider.onValueChanged.AddListener((float value) => UpdateMusicSlider(value));
        //AudioManager.Instance.masterMixer.GetFloat("musicVolume", out float musicDB);
        //musicSlider.value = Mathf.Pow(10.0f, musicDB / 20.0f);
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
    public void SetSoundVol(float value)
    {
        float linear2dB = value == 0 ? -80f : 20f * Mathf.Log10(value);
        AudioManager.instance.mixer.SetFloat("sfxVolume", linear2dB);
    }
    public void SetMusicVol(float value)
    {
        float linear2dB = value == 0 ? -80f : 20f * Mathf.Log10(value);
        AudioManager.instance.mixer.SetFloat("musicVolume", linear2dB);
    }
    public void SetSensitivity()
    {
        LevelDirector.instance.povController.m_HorizontalAxis.m_MaxSpeed = aimSensitivity;
        LevelDirector.instance.povController.m_VerticalAxis.m_MaxSpeed = aimSensitivity;
    }
}
