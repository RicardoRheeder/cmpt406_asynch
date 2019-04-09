using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "soundName", menuName = "Sound")]
public class Sound : ScriptableObject {
    public SoundName soundName;
    public AudioClip audioClip;
    public UnityEngine.Audio.AudioMixerGroup mixerGroup;

    [HideInInspector]
    public AudioSource audioSource;

    [Range (0f, 1f)]
    public float volume = 1f;

    [Range (-3f, 3f)]
    public float pitch = 1f;

    public bool loop = false;
}
