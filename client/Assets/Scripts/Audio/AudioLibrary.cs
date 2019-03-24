using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class AudioLibrary {

    public SoundName name;

    public AudioClip audioClip;

    [Range (0f, 1f)]
    public float volume = 1f;

    [Range (-3f, 3f)]
    public float pitch = 1f;

    public bool loop;

    public AudioMixerGroup audioMixerGroup;

    [HideInInspector]
    public AudioSource audioSource;
}

