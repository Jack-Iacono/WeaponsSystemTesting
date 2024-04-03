using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    private static AudioSource audioSrc;

    public SoundData shoot = new SoundData();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public static void PlaySound(SoundData sound)
    {
        PlaySound(sound, audioSrc);
    }
    public static void PlaySound(SoundData sound, AudioSource source)
    {
        source.pitch = Random.Range(sound.pitch - 0.01f, sound.pitch + 0.01f);
        source.PlayOneShot(sound.clip, sound.volume);
    }

    public static void PlayRandomSound(List<SoundData> sounds)
    {
        PlayRandomSound(sounds, audioSrc);
    }
    public static void PlayRandomSound(List<SoundData> sounds, AudioSource source)
    {
        int rand = Random.Range(0, sounds.Count);

        PlaySound(sounds[rand], source);
    }
}
