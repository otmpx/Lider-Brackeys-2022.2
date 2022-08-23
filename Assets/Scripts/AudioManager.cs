using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Audio")] public AudioSource soundSource;
    public AudioSource musicSource;
    public AudioSource secondarySource;
    public AudioMixer masterMixer;

    [Header("Music Tracks")] public AudioClip themeLoop;
    public AudioClip bossLoop1;
    public AudioClip bossLoop2;
    public AudioClip bossLoop3;
    public AudioClip endSong;

    [Header("Global SFX")] public AudioClip wrongdoorSound;
    public AudioClip correctdoorSound;
    public AudioClip usegraveSound;
    public AudioClip deathSound;
    public AudioClip emotionalDamage;


    public SoundCard[] ambientSounds;
    public SoundCard testCard;

    private float randomNoiseTimer;
    private float randomNoiseLerp;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    public void PlayMusic(AudioClip track)
    {
        if (musicSource.clip != track && track != null)
        {
            musicSource.clip = track;
            musicSource.Play();
        }
    }

    public void PlaySound(AudioClip sound)
    {
        if (sound == null) return;
        
        if (soundSource.clip != sound)
        {
            soundSource.clip = sound;
            soundSource.Play();
        }
        else if (!soundSource.isPlaying)
        {
            soundSource.Play();
        }
    }

    public void PlayRandomSound()
    {
    }
}