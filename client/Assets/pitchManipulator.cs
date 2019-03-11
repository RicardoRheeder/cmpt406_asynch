//Attach this script to a GameObject.
//Attach an AudioSource to your GameObject (Click Add Component and go to Audio>Audio Source). Choose an audio clip in the AudioClip field.
//This script sets the pitch of the audio at the start, and then gradually turns it down to 0 as time passes.
using UnityEngine.Audio;
using UnityEngine;

//Make sure there is an Audio Source component on the GameObject
[RequireComponent(typeof(AudioSource))]

public class pitchManipulator : MonoBehaviour
{
    [Range (-3f,3f)]
    public int startingPitch;
    public float timeToDecrease;
    public AudioSource audioSource;

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        //audioSource = GetComponent<AudioSource>();

        //Initialize the pitch
        audioSource.pitch = startingPitch;
    }

    void Update()
    {
        //While the pitch is over 0, decrease it as time passes.
        if (audioSource.pitch < 1)
        {
            audioSource.pitch -= Time.deltaTime * startingPitch * timeToDecrease;
        }
        else
        {
            audioSource.pitch = 1;
        }
    }
}