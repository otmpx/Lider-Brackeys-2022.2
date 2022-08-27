using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum MusicTheme { menu, forest, whispers, echo, buildup, suspense, ringing }
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer mixer;

    [Header("Audio")]
    public AudioSource[] musicSources;
    public AudioSource sfxSource;
    public float sfxPitchVariance = 0.1f;

    [Header("Music Tracks")]
    public MusicTheme currentTheme;
    public AudioClip menuTheme, forestTheme, whispersTheme, echoTheme, buildupTheme, suspenseTheme, ringingTheme;
    public float fadeDuration = 3f;

    [Header("Global SFX")]
    public AudioClip buttonPressSound, buttonAdjustSound;
    public int toggle;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetMusicVol(Settings.musicVol);
            SetSfxVol(Settings.soundVol);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (LevelDirector.instance == null)
            SwitchTheme(MusicTheme.menu);
        else
            SwitchTheme(LevelDirector.CurrentRoom.levelTheme);
    }
    #region Music
    void SwitchTheme(MusicTheme newTheme)
    {
        if (currentTheme == newTheme) return;
        currentTheme = newTheme;
        StopAllCoroutines();

        switch (currentTheme)
        {
            case MusicTheme.menu:
                musicSources[toggle].clip = menuTheme;
                break;
            case MusicTheme.forest:
                musicSources[toggle].clip = forestTheme;
                break;
            case MusicTheme.whispers:
                musicSources[toggle].clip = whispersTheme;
                break;
            case MusicTheme.echo:
                musicSources[toggle].clip = echoTheme;
                break;
            case MusicTheme.buildup:
                musicSources[toggle].clip = buildupTheme;
                break;
            case MusicTheme.suspense:
                musicSources[toggle].clip = suspenseTheme;
                break;
            case MusicTheme.ringing:
                musicSources[toggle].clip = ringingTheme;
                break;
        }
        SetMusicGain();
        musicSources[toggle].Play();

        if (toggle == 0)
        {
            StartCoroutine(FadeMixerGroup.StartFade(mixer, "music1", fadeDuration, 1));
            StartCoroutine(FadeMixerGroup.StartFade(mixer, "music2", fadeDuration, 0));
        }
        else
        {
            StartCoroutine(FadeMixerGroup.StartFade(mixer, "music2", fadeDuration, 1));
            StartCoroutine(FadeMixerGroup.StartFade(mixer, "music1", fadeDuration, 0));
        }
        toggle = 1 - toggle;
    }
    void SetMusicGain()
    {
        if (LevelDirector.instance == null)
            musicSources[toggle].volume = 1;
        else
        {
            musicSources[toggle].volume = LevelDirector.CurrentRoom.musicVol;
        }
    }
    #endregion
    #region Sound
    public void PlaySFX(AudioClip clip, float volume = 1, float pitch = 1)
    {
        sfxSource.pitch = pitch + Random.Range(-sfxPitchVariance, sfxPitchVariance);
        sfxSource.PlayOneShot(clip, volume);
    }
    #endregion
    #region Init
    public void SetMusicVol(float value)
    {
        float linear2dB = value == 0 ? -80f : 20f * Mathf.Log10(value);
        mixer.SetFloat("musicVol", linear2dB);
    }
    public void SetSfxVol(float value)
    {
        float linear2dB = value == 0 ? -80f : 20f * Mathf.Log10(value);
        mixer.SetFloat("sfxVol", linear2dB);
    }
    #endregion
}