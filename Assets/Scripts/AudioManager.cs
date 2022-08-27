using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer mixer;

    [Header("Audio")]
    public AudioSource[] musicSources;
    public AudioSource sfxSource;
    public float sfxPitchVariance = 0.1f;

    [Header("Music Tracks")]
    public AudioClip[] lobbyTheme;

    [Header("Global SFX")]
    public AudioClip deathSound;
    public AudioClip buttonPressSound, buttonAdjustSound;

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
    #region Music
    #endregion
    #region Sound
    public void PlaySFX(AudioClip clip, float volume = 1, float pitch = 1)
    {
        sfxSource.pitch = pitch + Random.Range(-sfxPitchVariance, sfxPitchVariance);
        sfxSource.PlayOneShot(clip, volume);
    }
    #endregion
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
}