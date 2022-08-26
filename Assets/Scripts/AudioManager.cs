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
    public AudioClip[] levelTheme;
    public enum MusicTheme { lobby, level }
    public MusicTheme currentTheme;
    public float fadeDuration = 5f;


    [Header("Global SFX")]
    public AudioClip deathSound;
    public AudioClip buttonPressSound;

    readonly AudioClip[] musicQueue = new AudioClip[2];
    double nextStartTime;
    int toggle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        switch (currentTheme)
        {
            case MusicTheme.lobby:
                EnqueueMusic(lobbyTheme);
                break;
            case MusicTheme.level:
                EnqueueMusic(levelTheme);
                break;
        }
        DequeueMusic();
    }
    #region Music
    void EnqueueMusic(AudioClip[] theme)
    {
        if (musicQueue[1] != null) return;
        if (musicQueue[0] == null)
            musicQueue[0] = theme[Random.Range(0, theme.Length)];
        AudioClip nonRepeated;
        do nonRepeated = theme[Random.Range(0, theme.Length)];
        while (nonRepeated == musicQueue[0]);
        musicQueue[1] = nonRepeated;
    }
    void DequeueMusic()
    {
        if (AudioSettings.dspTime > nextStartTime - 1 - fadeDuration)
        {
            // Set next track in musicQueue to be played
            musicSources[toggle].clip = musicQueue[0];
            musicSources[toggle].PlayScheduled(nextStartTime - fadeDuration);

            // Iterate dspTime and toggle between the 2 music sources
            double clipDuration = (double)musicQueue[0].samples / musicQueue[0].frequency;
            nextStartTime += clipDuration;

            // Fade in/out mixer volume and switch active music sources
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

            // Dequeue musicQueue so that EnqueueMusic will select a new random track
            musicQueue[0] = musicQueue[1];
            musicQueue[1] = null;
        }
    }
    public void ChangeMusic()
    {
        nextStartTime = AudioSettings.dspTime;
        StopAllCoroutines();
    }
    void SwitchTheme(MusicTheme newTheme)
    {
        if (currentTheme == newTheme) return;
        currentTheme = newTheme;
        for (int i = 0; i < musicQueue.Length; i++)
            musicQueue[i] = null;
        nextStartTime = AudioSettings.dspTime;
        StopAllCoroutines();
    }
    #endregion
    #region Sound
    public void PlaySFX(AudioClip clip, float volume = 1, float pitch = 1)
    {
        sfxSource.pitch = pitch + Random.Range(-sfxPitchVariance, sfxPitchVariance);
        sfxSource.PlayOneShot(clip, volume);
    }
    #endregion
    public void PlayUISound()
    {
        PlaySFX(buttonPressSound);
    }
}