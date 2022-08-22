using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound", menuName = "ScriptableObjects/SoundCard")]
public class SoundCard : ScriptableObject
{
    public AudioClip[] sounds;
    [Range(0, 2)] public float minPitch = 0.8f;
    [Range(0, 2)] public float maxPitch = 1.2f;
    [Range(0, 1)] public float volume = 1f;

    public void Play(ISoundAble actor)
    {
        actor.sound.volume = volume;
        actor.sound.clip = sounds[Random.Range(0, sounds.Length)];
        actor.sound.pitch = Random.Range(minPitch, maxPitch) * actor.pitchMultiplier;
        actor.sound.Play();
    }

    public void PlayAfterFinish(ISoundAble actor)
    {
        if (actor.sound.isPlaying) return;
        Play(actor);
    }

    public void PlaySecondary(AudioSource source)
    {
        source.volume = volume;
        source.clip = sounds[Random.Range(0, sounds.Length)];
        source.pitch = Random.Range(minPitch, maxPitch);
        source.PlayOneShot(source.clip);
    }
    //
    // //TODO: Everything should maybe have state or audio stuff handled with inheritance
    public void SourcePlay(AudioSource sound)
    {
        sound.volume = volume;
        sound.clip = sounds[Random.Range(0, sounds.Length)];
        sound.pitch = Random.Range(minPitch, maxPitch);
        sound.Play();
    }
}

public interface ISoundAble
{
    public AudioSource sound { get; set; }
    public float pitchMultiplier { get; }
}