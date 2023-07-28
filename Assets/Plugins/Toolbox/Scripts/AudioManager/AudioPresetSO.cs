using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using Toolbox;

#region Info:
// Author:  Paul Woerner
// * Unity Official: 'Chop Chop': https://youtu.be/WLDgtRNK2VE
#endregion

public enum AudioFilePriority { Lowest = 32, Low = 64, Medium = 128, High = 192, Highest = 255 }

///<summary>SO to configure Pre Audio Settings Sound Designer friendly.</summary>
[CreateAssetMenu(fileName = "AudioPreset", menuName = "Audio/Preset SO")]
public class AudioPresetSO : SO
{
    [Required]
    [Tooltip("path of the file")]
    public AudioClip filePath;
    [Required]
    [Tooltip("The group the sound is played in.")]
    public AudioMixerGroup mixerGroup;

    #region General
    [Title("General", "Settings applying to every sound.")]

    [Tooltip("The Type of sound")]
    public bool isLooping = false;
    [Tooltip("Ambiences for example can only be played once per scene")]
    public bool isSingleInstance = false;

    [Tooltip("How important the sound is. Less important sounds will be stopped when there is too much going on.")]
    public AudioFilePriority priority = AudioFilePriority.Medium;

    [Tooltip("Base volume (0 - 1)")] [Range(0f, 1f)] public float volume = 1f;
    [MinMaxSlider(0.0f, 1.0f, true)]
    [Tooltip("Range when playing random volume")]
    public Vector2 volumeRange = new Vector2(0.8f, 1f);

    [Tooltip("Base pitch of the audio")] [Range(0f, 2f)] public float pitch = 1f;
    [MinMaxSlider(0.1f, 2.0f, true)]

    [Tooltip("Range when playing random pitch.")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    [Tooltip("Left/Right ear shift")]
    [Range(-1.0f, 1.0f)] public float panStereo = 0f;

    [Tooltip("Sets the amount of the output signal that gets routed to the reverb zones. The amount is linear in the (0 - 1) range, but allows for a 10 dB amplification in the (1 - 1.1) range which can be useful to achieve the effect of near-field and distant sounds.")]
    [Range(0.0f, 1.1f)] public float reverbZoneMix = 1f;
    #endregion

    #region Spatialization
    [Title("Spatialization", "3D Sound settings. Only relevant for sounds played in 3D space.")]
    [Tooltip("Sets how much the 3D engine has an effect on the audio source.")]
    [Range(0.0f, 1.0f)] public float spatialBlend = 1f;

    [HideIf("@spatialBlend == 0f")]
    [Tooltip("Sets the spread angle to 3D stereo or multichannel sound in speaker space.")]
    [Range(0, 360)] public int spread = 0;

    [HideIf("@spatialBlend == 0f")]
    [Tooltip("Determines how much doppler effect will be applied to this audio source (if is set to 0, then no effect is applied).")]
    [Range(0.0f, 1.0f)] public float dopplerLevel = 0f;

    [HideIf("@spatialBlend == 0f")]
    [Tooltip("[Logarithmic]: The sound is loud when you are close to the audio source, but when you get away from the object it decreases significantly fast.\n\n" +
    "[Linear]: The further away from the audio source you go, the less you can hear it.\n\n" +
    "[Custom]: Don't use it.")]
    public AudioRolloffMode rollOffMode = AudioRolloffMode.Logarithmic;

    [HideIf("@spatialBlend == 0f")]
    [MinMaxSlider(1, 1000, true)]
    [Tooltip("Within the MinDistance, the sound will stay at loudest possible. Outside MinDistance it will begin to attenuate. Increase the MinDistance of a sound to make it ‘louder’ in a 3d world, and decrease it to make it ‘quieter’ in a 3d world. MaxDistance is The distance where the sound stops attenuating at. Beyond this point it will stay at the volume it would be at MaxDistance units from the listener and will not attenuate any more.")]
    public Vector2 distanceRange = new Vector2(1, 1000);
    #endregion

    #region Ignores
    [Title("Ignores", "You can ignore cetain effects applied to channels or listeners.")]
    public bool showIgnores = false;

    [Tooltip("This is to quickly “by-pass” filter effects applied to the audio source. An easy way to turn all effects on/off.")]
    [ShowIf("@showIgnores")] public bool ignoreBypassEffects = false;

    [Tooltip("This is to quickly turn all Listener effects on/off.")]
    [ShowIf("@showIgnores")] public bool ignoreBypassListenerEffects = false;

    [Tooltip("This is to quickly turn all Reverb Zones on/off.")]
    [ShowIf("@showIgnores")] [EnableIf("@reverbZoneMix > 0f")] public bool ignoreBypassReverbZones = false;

    [Tooltip("Ignores the volume on the listener.")]
    [ShowIf("@showIgnores")] public bool ignoreListenerVolume = false;

    [Tooltip("Ignores if the listener is on pause.")]
    [ShowIf("@showIgnores")] public bool ignoreListenerPause = false;
    #endregion

    #region Preview
#if UNITY_EDITOR
    [Title("Preview", "Press 'play' or 'stop' to preview current settings.")]
    [SerializeField] bool previewLoop = false;
    [DisableIf("@pitchRange.x == pitchRange.y")]
    [SerializeField] bool previewRandomPitch = false;
    [DisableIf("@volumeRange.x == volumeRange.y")]
    [SerializeField] bool previewRandomVolume = false;

    AudioSource previewer;
#endif
    #endregion

    // Preview Audio from inspector
    #region Preview Mode
#if UNITY_EDITOR
    #region Unity Functions
    private void OnEnable()
    {
        previewer = EditorUtility.
            CreateGameObjectWithHideFlags("AudioPreview", HideFlags.HideAndDontSave,
            typeof(AudioSource))
            .GetComponent<AudioSource>();
        previewer.spatialBlend = 0f;
    }

    private void OnDisable()
    {
        DestroyImmediate(previewer.gameObject);
    }

    private void OnValidate()
    {
        SetPreviewer();
    }
    #endregion

    #region Preview
    [ButtonGroup("PreviewControls")]
    [GUIColor(.3f, 1f, .3f)]
    [Button("▶")]
    async void Preview()
    {
        previewer.Stop();

        #region Set Source
        previewer.clip = filePath;
        SetPreviewer();

        #endregion

        previewer.Play();

        Vector2 _volumeRange = Vector2.zero;
        float _volumeMod = 1f;

        Vector2 _pitchRange = Vector2.zero;
        float _pitchMod = 1f;

        while (previewer.isPlaying)
        {

            // when volumeRange has changed
            #region Random Volume
            if (previewRandomVolume)
            {
                if (_volumeRange != volumeRange)
                {
                    _volumeMod = Random.Range(volumeRange.x, volumeRange.y);
                    _volumeRange = volumeRange;
                    previewer.volume = volume * _volumeMod;
                }
            }
            else
            {
                previewer.volume = volume;
            }
            #endregion

            // when pitchRange has changed
            #region Random Pitch
            if (previewRandomPitch)
            {
                if (_pitchRange != pitchRange)
                {
                    _pitchMod = Random.Range(pitchRange.x, pitchRange.y);
                    _pitchRange = pitchRange;
                    previewer.pitch = pitch * _pitchMod;
                }
            }
            else
            {
                previewer.pitch = pitch;
            }
            #endregion

            await Task.Delay(1);
        }
    }
    #endregion

    #region Stop
    [ButtonGroup("PreviewControls")]
    [EnableIf("@previewer.isPlaying || previewLoop == true")]
    [GUIColor(1f, .3f, .3f)]
    [Button(ButtonSizes.Large)]
    void Stop()
    {
        previewer.Stop();
    }
    #endregion

    void SetPreviewer()
    {
        if (previewer != null)
        {
            previewer.volume = volume;
            previewer.pitch = pitch;
            previewer.panStereo = panStereo;
            previewer.reverbZoneMix = reverbZoneMix;

            previewer.rolloffMode = rollOffMode;
            previewer.minDistance = distanceRange.x;
            previewer.maxDistance = distanceRange.y;
            previewer.spread = spread;
            previewer.dopplerLevel = dopplerLevel;
            previewer.bypassEffects = ignoreBypassEffects;
            previewer.bypassListenerEffects = ignoreBypassListenerEffects;
            previewer.bypassReverbZones = ignoreBypassReverbZones;
            previewer.ignoreListenerVolume = ignoreListenerVolume;
            previewer.ignoreListenerPause = ignoreListenerPause;
            previewer.loop = previewLoop;
        }
    }

#endif
    #endregion
}
