using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioLibrary[] sounds;
    public static AudioManager instance = null;

    void Awake() {
        // Creates an a modifible element in the inspector of the audioManager for each audio clip.
        foreach (AudioLibrary sound in sounds) {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
        }

        // Instaniate the object. If one already exists, destroy it.
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Starts the menu theme music
    void Start() {
        Play("Theme");
    }

    // Looks for sound used in the audioLibrary by name and plays that sound.
    public void Play(string name) {
        AudioLibrary s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " was not found. Check for any misspellings");
            return;
        }
        s.audioSource.Play();
    }

    public void Mute(string name) {
        AudioLibrary s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " was not found. Check for any misspellings");
        }
        s.audioSource.mute = !s.audioSource.mute;
    }

    public void MuteTheme() {
        Mute("Theme");
    }
}
