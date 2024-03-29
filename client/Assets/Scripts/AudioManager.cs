﻿using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : MonoBehaviour {

    private Sound[] sounds;
    public AudioMixer masterMixer;

    void Awake() {
        // Instaniate the object. If one already exists, destroy it.
        DontDestroyOnLoad(this);

        sounds = Resources.LoadAll<Sound>("Audio/");

        foreach (Sound sound in sounds) {
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
        Play(SoundName.Theme);
    }

    // Looks for sound used in the audioLibrary by name and plays that sound.
    public void Play(SoundName soundName) {
        Sound s = Array.Find(sounds, sound => sound.soundName == soundName);
        s.audioSource.Play();
    }

    // Plays a sound when there is a collection of sounds to choose from
    public void Play(UnitType unit, SoundType type, bool isVoice = false) {
        if (isVoice) {
            PlayVoice(unit, type);
        }
        else {
            Play(unit, type);
        }
    }

    // Plays a sound based on the unit type and the sound type
    private void Play(UnitType unit, SoundType type) {
        string soundName = unit.ToString() + "_" + type.ToString();
        Sound s = Array.Find(sounds, sound => sound.soundName.ToString() == soundName);
        if(s != null) {
            s.audioSource.Play();
        }
        else {
            Debug.Log("Missing Sound: " + soundName);
        }
    }

    private bool isPlayingVoice = false;

    // Plays a random voice line based on the unit type and the sound type
    private void PlayVoice(UnitType unit, SoundType type, bool generateRandom = true) {
        int randomNumber = generateRandom ? UnityEngine.Random.Range(0, SoundMetadata.VoiceCountsDictionary[type][unit]) : 0;
        string soundName = unit.ToString() + "_" + type.ToString() + "_" + randomNumber;
        Sound s = Array.Find(sounds, sound => sound.soundName.ToString() == soundName);
        if (!isPlayingVoice) {
            if (s != null) {
                isPlayingVoice = true;
                StartCoroutine(DonePlayingVoice(s.audioClip.length));
                s.audioSource.Play();
            }
            else {
                if (generateRandom) {
                    Debug.Log("Generated sound " + soundName + " which didn't exist, using default voiceline");
                    PlayVoice(unit, type, false);
                }
                else {
                    Debug.Log("Missing Sound: " + soundName);
                }
            }
        }
    }

    private IEnumerator DonePlayingVoice(float time) {
        yield return new WaitForSeconds(time);
        isPlayingVoice = false;
    }

    // Goes through the list of sounds in the AudioLibrary and mutes them
    public void Mute() {
        foreach (Sound sound in sounds) {
            sound.audioSource.mute = !sound.audioSource.mute;
        }
    }

    // Looks for sound used in the audioLibrary by name and mutes it.
    public void Mute(SoundName soundName){
        Sound s = Array.Find(sounds, sound => sound.soundName == soundName);
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