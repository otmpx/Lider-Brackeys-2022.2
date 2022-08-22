using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

public class InteractableSound : MonoBehaviour, ISoundAble
{
    public SoundCard interactSound;
    [field: HideInNormalInspector]
    public AudioSource sound { get; set; }
    [field: SerializeField]
    public float pitchMultiplier { get; set; }
    [field: SerializeField]
    public float spatialBlend { get; set; }
    public AudioMixerGroup mixer;


    void Awake()
    {
        sound = gameObject.AddComponent<AudioSource>();
        sound.outputAudioMixerGroup = mixer;
        sound.spatialBlend = spatialBlend;
    }

    private void OnCollisionEnter(Collision other)
    {
        interactSound.Play(this);
    }
}