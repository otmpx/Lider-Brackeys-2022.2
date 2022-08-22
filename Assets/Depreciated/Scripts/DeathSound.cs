using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSound : MonoBehaviour
{
    AudioSource sound;
    private void Awake()
    {
        sound = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!sound.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
