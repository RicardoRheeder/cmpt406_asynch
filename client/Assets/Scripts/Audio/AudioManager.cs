using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {

    public AudioLibrary[] sounds;
    public AudioMixer masterMixer;


    void Awake() {
        // Instaniate the object. If one already exists, destroy it.
        DontDestroyOnLoad(this);
    }


    private void InitalizeAudio() {
        // Creates an a modifible element in the inspector of the audioManager for each audio clip.
        foreach (AudioLibrary sound in sounds) {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;
            sound.audioSource.outputAudioMixerGroup = sound.mixerGroup;
        }
    }

    // Starts the menu theme music
    void Start() {
        InitalizeAudio();
        Play(SoundName.Theme);
    }

    // Looks for sound used in the audioLibrary by name and plays that sound.
    public void Play(SoundName soundName) {
        AudioLibrary s = Array.Find(sounds, sound => sound.name == soundName);
        s.audioSource.Play();
    }

    // Plays a sound based on the unit type and the sound type
    public void Play(UnitType unit, SoundType type) {
        string soundName = unit.ToString() + "_" + type.ToString();
        AudioLibrary s = Array.Find(sounds, sound => sound.name.ToString() == soundName);
        if(s != null) {
            s.audioSource.Play();
        }
        else {
            Debug.Log("Missing Sound: " + soundName);
        }
    }

    // Goes through the list of sounds in the AudioLibrary and mutes them
    public void Mute() {
        foreach (AudioLibrary sound in sounds) {
            sound.audioSource.mute = !sound.audioSource.mute;
        }
    }

    // Looks for sound used in the audioLibrary by name and mutes it.
    public void Mute(SoundName soundName){
        AudioLibrary s = Array.Find(sounds, sound => sound.name == soundName);
        s.audioSource.mute = !s.audioSource.mute;
    }

    // Mutes all sound in the game
    public void toggleMute() {
        FindObjectOfType<AudioManager>().Mute();
        //muteToggle.isOn = !muteToggle.isOn;
    }

    // Sets the volume. (Currently it's based on the in game UI slider)
    public void setMasterVolume(float masterVolume) {
        masterMixer.SetFloat("masterVolume", Mathf.Log10(masterVolume) * 20);
    }

    // Sets the volume of the music.
    public void setMusicVolume(float musicVol) {
        masterMixer.SetFloat("musicVol", Mathf.Log10(musicVol) * 20);
    }

    // Sets the volume of sound effects.
    public void setSFXVolume(float sfxVol) {
        masterMixer.SetFloat("sfxVol", Mathf.Log10(sfxVol) * 20);
    }
}