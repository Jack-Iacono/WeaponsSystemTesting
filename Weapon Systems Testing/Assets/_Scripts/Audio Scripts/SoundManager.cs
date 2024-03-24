using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static AudioSource audioSrc;

    public AudioClip shootClip;

    public static SoundData shoot { get; private set; } = new SoundData();

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        shoot.SetSoundData(shootClip, 0.2f, 1f);
    }

    public static void PlaySound(SoundData sound)
    {
        PlaySound(sound, audioSrc);
    }
    public static void PlaySound(SoundData sound, AudioSource source)
    {
        source.pitch = sound.pitch;
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
