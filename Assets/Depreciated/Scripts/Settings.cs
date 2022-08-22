using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Settings : MonoBehaviour
{
    [Header("World UI")]
    public Image[] images;
    public Color[] initialColours;
    public float appearDelay = 2f;
    public float fadeInDur = 1f;
    bool endFade = false;
    float timer = 0;

    [Header("Audio")]
    public Slider soundSlider;
    public Slider musicSlider;

    private void Awake()
    {
        images = GetComponentsInChildren<Image>();
        initialColours = new Color[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            initialColours[i] = images[i].color;
        }

        soundSlider.onValueChanged.AddListener((float value) => UpdateSoundSlider(value));
        AudioManager.Instance.masterMixer.GetFloat("sfxVolume", out float soundDB);
        soundSlider.value = Mathf.Pow(10.0f, soundDB / 20.0f);
        musicSlider.onValueChanged.AddListener((float value) => UpdateMusicSlider(value));
        AudioManager.Instance.masterMixer.GetFloat("musicVolume", out float musicDB);
        musicSlider.value = Mathf.Pow(10.0f, musicDB / 20.0f);
    }
    private void Start()
    {
        foreach (Image image in images)
            image.color = Color.clear;
    }
    private void Update()
    {
        FadeInUI();
    }
    void FadeInUI()
    {
        if (endFade) return;
        timer += Time.deltaTime;
        if (timer >= appearDelay)
        {
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = Color.Lerp(Color.clear, initialColours[i], (timer - appearDelay) / fadeInDur);
            }
            //foreach (Image image in images)
            //{
            //    Color newColor = image.color;
            //    newColor.a = Mathf.Lerp(0, 1, (timer - appearDelay) / fadeInDur);
            //    image.color = newColor;
            //}
        }
        else if (timer > appearDelay + fadeInDur)
            endFade = false;
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
}
