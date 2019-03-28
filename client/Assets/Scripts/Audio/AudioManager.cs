using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioLibrary[] sounds;
    public AudioMixer masterMixer;

    void Awake() {
        // Creates an a modifible element in the inspector of the audioManager for each audio clip.
        foreach (AudioLibrary sound in sounds) {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
            sound.audioSource.outputAudioMixerGroup = sound.audioMixerGroup;
        }
        DontDestroyOnLoad(this);
    }


    // Starts the menu theme music
    void Start() {
        Play(SoundName.Theme);
    }

    // Looks for sound used in the audioLibrary by name and plays that sound.
    public void Play(SoundName soundName) {
        AudioLibrary s = Array.Find(sounds, sound => sound.name == soundName);
        s.audioSource.Play();
    }

    // Looks for sound used in the audioLibrary by name and mutes it.
    public void Mute(SoundName soundName) {
        AudioLibrary s = Array.Find(sounds, sound => sound.name == soundName);
        s.audioSource.mute = !s.audioSource.mute;
    }

    public void setVolume(float masterVolume) {
        masterMixer.SetFloat("masterVolume", masterVolume);
    }

    public void Play(UnitType unit, SoundType type) {
        string soundName = unit.ToString() + "_" + type.ToString();
        AudioLibrary s = Array.Find(sounds, sound => sound.name.ToString() == soundName);
        s.audioSource.Play();
    }
}