using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Linq;
public class Settings : MonoBehaviour
{
    [Range(0.1f, 10f)]
    public float aimSensitivity = 1f;
    public static Settings instance;
    public Slider soundSlider;
    public Slider musicSlider;
    [HideInInspector] public CinemachinePOV povController;

    private void Awake()
    {
        instance = this;
        povController = LevelDirector.Instance.vCam.GetComponentPipeline().First(cb => cb is CinemachinePOV) as CinemachinePOV;
        //headBob = LevelDirector.Instance.vCam.GetComponentPipeline().First(cb => cb is CinemachineBasicMultiChannelPerlin) as CinemachineBasicMultiChannelPerlin;
        soundSlider.onValueChanged.AddListener((float value) => UpdateSoundSlider(value));
        AudioManager.Instance.masterMixer.GetFloat("sfxVolume", out float soundDB);
        soundSlider.value = Mathf.Pow(10.0f, soundDB / 20.0f);
        musicSlider.onValueChanged.AddListener((float value) => UpdateMusicSlider(value));
        AudioManager.Instance.masterMixer.GetFloat("musicVolume", out float musicDB);
        musicSlider.value = Mathf.Pow(10.0f, musicDB / 20.0f);

        povController.m_HorizontalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
        povController.m_VerticalAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;

        povController.m_HorizontalAxis.m_MaxSpeed = 1f;
        povController.m_VerticalAxis.m_MaxSpeed = 1f;
    }
    private void Update()
    {
        SetSensitivity(aimSensitivity);
    }
    public void UpdateSoundSlider(float value)
    {
        float linear2dB = value == 0 ? -80f : 20f * Mathf.Log10(value);
        AudioManager.Instance.masterMixer.SetFloat("sfxVolume", linear2dB);
    }
    public void UpdateMusicSlider(float value)
    {
        float linear2dB = value == 0 ? -80f : 20f * Mathf.Log10(value);
        AudioManager.Instance.masterMixer.SetFloat("musicVolume", linear2dB);
    }
    public void SetSensitivity(float aimSensitivity)
    {
        povController.m_HorizontalAxis.m_MaxSpeed = aimSensitivity;
        povController.m_VerticalAxis.m_MaxSpeed = aimSensitivity;
    }
}
