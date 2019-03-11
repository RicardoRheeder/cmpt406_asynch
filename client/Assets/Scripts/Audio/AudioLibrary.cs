using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class AudioLibrary {
     
    public string name;
    
    public AudioClip audioClip;

    [Range (0f, 1f)]
    public float volume = 1;

    [Range (-3f, 3f)]
    public float pitch = 1;

    public bool loop;

    [HideInInspector]
    public AudioSource audioSource;
}
