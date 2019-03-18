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

    public AudioMixer audioMixer;

    [HideInInspector]
    public AudioSource audioSource;
}

public enum SoundName
{
    Theme,
    ButtonPress,
    ButtonError,
    ButtonQuit,

    TrooperMove,
    TrooperAttack,
    TrooperDeath,

    ReconMove,
    ReconAttack,
    ReconDeath,

    SteamerMove,
    SteamerAttack,
    SteamerDeath,

    PewpewMove,
    PewpewAttack,
    PewpewDeath,

    CompensatorMove,
    CompensatorAttack,
    CompensatorDeath,

    TungstenMove,
    TungstenAttack,
    TungstenDeath,

    SandmanMove,
    SandmanAttack,
    SandmanDeath,

    FoundationMove,
    FoundationAttack,
    FoundationDeath,

    PowersurgeMove,
    PowersurgeAttack,
    PowersurgeDeath,

    MidasMove,
    MidasAttack,
    MidasDeath,

    ClaymoreMove,
    ClaymoreAttack,
    ClaymoreDeath,

    AlbarnMove,
    AlbarnAttack,
    AlbarnDeath,

    AdrenMove,
    AdrenAttack,
    AdrenDeath
}