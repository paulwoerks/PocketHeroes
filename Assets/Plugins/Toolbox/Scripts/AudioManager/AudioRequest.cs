using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
///<summary>Holds sound request to play with Sound Pooler.</summary>
public class AudioRequest
{
    public AudioPresetSO preset;
    [SerializeField] AudioCue cue;

    public void SetParent(Transform parent)
    {
        cue.parent = parent;
    }

    public AudioPresetSO GetPreset()
    {
        return preset;
    }

    public AudioCue GetCue()
    {
        return cue;
    }
}

[System.Serializable]
///<summary>Custom settings for this sound instance</summary>
public class AudioCue
{
    [Title("Custom Settings", "Tweak the audio played individually.")]

    [Tooltip("3D Sounds need a parent, otherwise it's treated as 2d")]
    public Transform parent;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 2f)] public float pitch = 1f;

    public float fadeIn = 0f;
    public float fadeOut = 0f;

    public float delayIn = 0f;
    public float delayOut = 0f;

    public bool randomizeVolume = false;
    public bool randomizePitch = false;
}
