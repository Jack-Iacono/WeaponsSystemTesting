using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundData
{
    [Range(0,3)]
    public float volume;
    [Range(0.5f, 1.5f)]
    public float pitch;
    public AudioClip clip;

    public SoundData()
    {
        pitch = 1;
        volume = 1;
    }
    public SoundData(AudioClip clip, float volume, float pitch)
    {
        this.clip = clip;
        this.pitch = pitch;
        this.volume = volume;
    }
    public void SetSoundData(AudioClip clip, float volume, float pitch)
    {
        this.clip = clip;
        this.pitch = pitch;
        this.volume = volume;
    }
}
