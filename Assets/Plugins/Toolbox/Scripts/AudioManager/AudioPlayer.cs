using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
using Toolbox;

/// <summary> Resource optimized Audio Player to Pool Audio Tracks efficiently</summary>
public class AudioPlayer : MonoBehaviour
{
#if UNITY_EDITOR
    public bool showNotes = false;

    [ShowIf("@showNotes")]
    [Title("Notes", bold: false)]
    [HideLabel]
    [MultiLineProperty(2)]
    public string notes;
#endif

    enum PoolBehaviour { Ignore, Expand, Prioritize }
    enum JobAction { Play, Loop, Stop }

    #region Classes
    class AudioTrack
    {
        public IEnumerator coroutine;
        public GameObject go;
        public AudioSource source;

        public AudioTrack(GameObject go, Transform parent)
        {
            this.go = go;
            this.source = go.AddComponent<AudioSource>();
            go.transform.SetParent(parent);
            this.go.SetActive(false);
        }
    }

    class AudioJob
    {
        public AudioTrack track;
        public JobAction action;

        // Audio Settings
        public float spatialBlend;
        public float fadeTime;

        public float volume = 1;
        public float pitch = 1;

        public Transform parent;

        public AudioPresetSO preset;

        public AudioJob(JobAction action, AudioPresetSO preset, AudioCue cue, AudioTrack track, Transform poolerTransform)
        {
            this.preset = preset;
            this.track = track;

            this.action = action;

            switch (action) 
            {
                case JobAction.Play:
                    this.fadeTime = cue.fadeIn;
                    AudioSource source = track.source;
                    source.volume = (preset.volume + cue.volume) / 2f;
                    source.clip = preset.filePath;
                    source.outputAudioMixerGroup = preset.mixerGroup;
                    source.priority = (int)preset.priority;

                    if (cue.parent == null) { source.spatialBlend = 0f; parent = poolerTransform; }
                    else {
                        source.spatialBlend = preset.spatialBlend; parent = cue.parent; 
                    }

                    track.go.transform.SetParent(parent, false);
                    track.go.transform.localPosition = Vector3.zero;

                    if (cue.randomizeVolume) { volume = preset.volume * Random.Range(preset.volumeRange.x, preset.volumeRange.y); }
                    else { volume = preset.volume; }

                    if (cue.randomizePitch) { source.pitch = preset.pitch * Random.Range(preset.pitchRange.x, preset.pitchRange.y); }
                    else { source.pitch = preset.pitch; }

                    source.panStereo = preset.panStereo;
                    source.reverbZoneMix = preset.reverbZoneMix;

                    source.rolloffMode = preset.rollOffMode;
                    source.minDistance = preset.distanceRange.x;
                    source.maxDistance = preset.distanceRange.y;
                    source.spread = preset.spread;
                    source.dopplerLevel = preset.dopplerLevel;

                    source.bypassEffects = !preset.ignoreBypassEffects;
                    source.bypassListenerEffects = !preset.ignoreBypassListenerEffects;
                    source.bypassReverbZones = !preset.ignoreBypassReverbZones;
                    source.ignoreListenerVolume = preset.ignoreListenerVolume;
                    source.ignoreListenerPause = preset.ignoreListenerPause;

                    if (preset.isLooping) { this.action = JobAction.Loop; }
                    break;
                case JobAction.Stop:
                    this.fadeTime = cue.fadeOut;
                    break;
            }

            if (this.fadeTime < 0f) { this.fadeTime = 0f; }
        }

    }

    class AudioSubscription
    {
        public int subscriberCount;
        public AudioPresetSO preset;
        public GameObject track; 

        public AudioSubscription(GameObject track)
        {
            this.track = track;
        }

        public GameObject GetTrack()
        {
            return track;
        }

        public int AddSubscriber()
        {
            subscriberCount += 1;
            return subscriberCount;
        }

        public int RemoveSubscriber()
        {
            subscriberCount -= 1;
            return subscriberCount;
        }
    }
    #endregion


    [SerializeField] bool debug = false;

    [SerializeField] PoolBehaviour behaviour;
    [SerializeField] int trackSize = 32;
    List<AudioTrack> tracks = new List<AudioTrack>();
    Dictionary<Transform, List<AudioJob>> activeJobs = new Dictionary<Transform, List<AudioJob>>(); // Parent, jobs on parent
    Dictionary<AudioPresetSO, AudioSubscription> subscriptions = new Dictionary<AudioPresetSO, AudioSubscription>(); // Sounds that can only be played in a single instance

    [SerializeField] GameObject[] subscriptionTracks;

    public static AudioPlayer instance;

    #region Unity Functions
    private void Awake()
    {
        instance = this;
        Configure();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    #endregion

    #region Public Functions
    public static void Subscribe(AudioRequest request)
    {
        if (instance == null) { instance.LogWarning("Instance was null"); return; }
        if (request.preset == null) { return; }
        instance.Sub(request);

    }

    public static void Unsubscribe(AudioRequest request)
    {
        if (instance == null) { instance.LogWarning("Instance was null"); return; }
        if (request.preset == null) { return; }
        instance.Unsub(request);
    }

    public static void Play(AudioRequest request)
    {
        if (instance == null) { instance.LogWarning("Instance was null"); return; }
        if (request.preset == null) { return; }
        instance.RequestPlay(request);
    }

    public static void Stop(AudioRequest request)
    {
        if (instance == null) { instance.LogWarning("Instance was null"); return; }
        if (request.preset == null) { return; }
        instance.RequestStop(request);
    }
    #endregion

    #region Private Functions
    void Sub(AudioRequest request)
    {
        AudioPresetSO preset = request.GetPreset();

        // Create Subscription
        if (!subscriptions.ContainsKey(preset))
        {
            GameObject track = null;

            // Get Empty SubscriptionTrack
            foreach (GameObject t in subscriptionTracks)
            {
                if (!t.activeSelf)
                {
                    track = t;
                }
            }

            if (track == null) { this.LogError("SubscriptionTracks are full. Shouldnt happen."); return; }

            AudioSubscription subber = new AudioSubscription(track);
            subscriptions.Add(preset, subber);

            AudioSource source = track.GetComponent<AudioSource>();
            track.SetActive(true);
            track.name = "[Sub " + preset.name + "]";
            source.clip = preset.filePath;
            source.Play();
        }
        int subscribers = subscriptions[preset].AddSubscriber();
    }

    void Unsub(AudioRequest request)
    {
        AudioPresetSO preset = request.GetPreset();

        if (!subscriptions.ContainsKey(preset)) { this.LogWarning("No Subscription active on " + preset.name); return; }

        AudioSubscription subber = subscriptions[preset];
        int subscribers = subber.RemoveSubscriber();
        if (subscribers > 0)
        {
            return;
        }

        // Kill Instance
        GameObject track = subber.GetTrack();
        AudioSource source = track.GetComponent<AudioSource>();

        source.Stop();
        track.SetActive(false);

        subscriptions.Remove(preset);
    }

    void Configure()
    {
        // Configure Tracks
        for (int c = 0; c < trackSize; c++)
        {
            AudioTrack track = new AudioTrack(new GameObject("[Track " + c + "]"), transform);
            tracks.Add(track);
        }
        this.Log("Configured " + trackSize + " tracks", debug);
    }

    ///<summary> Finds Active job and stops it.</summary>
    async void RequestStop(AudioRequest request)
    {
        AudioPresetSO preset = request.GetPreset();
        AudioCue cue = request.GetCue();
        if (cue.delayOut > 0f) { await Task.Delay((int)(cue.delayOut * 1000)); } // Delay

        if (this == null) { return; }

        Transform parent = cue.parent;
        if (parent == null) { parent = transform; }
        if (!activeJobs.ContainsKey(parent)) { this.LogWarning("Stop failed: " + parent.name + " - "+ preset.name + " doesn't exist."); return; }

        foreach (AudioJob job in activeJobs[parent])
        {
            if (job.preset == preset)
            {
                this.Log("Found parent", debug);
                // Prevent errors
                await Task.Delay(1);
                if (this == null) { return; }
                if (job.track.go == null) { this.LogWarning("No track GO found."); return; }
                // Fade Out
                if (cue.fadeOut > 0f)
                {
                    this.Log("Fade Out", debug);
                    job.fadeTime = cue.fadeOut;
                    StopCoroutine(job.track.coroutine);

                    AudioJob fadeOutJob = new AudioJob(JobAction.Stop, preset, cue, job.track, transform);
                    fadeOutJob.track.coroutine = ExecuteJob(job);
                    StartCoroutine(fadeOutJob.track.coroutine);
                    return;
                }

                // Reset Track
                AudioTrack track = job.track;
                track.go.transform.SetParent(transform);
                track.go.SetActive(false);

                // Stop Running Job
                StopCoroutine(track.coroutine);
                if (activeJobs[parent].Count <= 1)
                {
                    activeJobs.Remove(parent);
                } else
                {
                    activeJobs[parent].Remove(job);
                }
                this.Log("Stopped " + preset.name + " on " + parent.name + " " + track.go.name, debug);
                return;
            }
        }
        this.LogWarning("Stopping failed: " + preset.name + " on " + parent.name + " wasn't playing");
    }

    ///<summary>Chooses Track and sets up Audio Job</summary>
    async void RequestPlay(AudioRequest request)
    {
        AudioPresetSO preset = request.GetPreset();
        AudioCue cue = request.GetCue();
        if (cue.delayIn > 0f) { await Task.Delay((int)(cue.delayIn * 1000)); } // Delay
        if (this == null) { return; }

        RemoveConflictingJobs(preset, cue);

        AudioTrack track = GetInactiveTrack();

        // no empty track found
        if (track == null)
        {
            switch (behaviour)
            {
                // Ignore the sound request completely
                case PoolBehaviour.Ignore:
                    return;
                // Expand tracks
                case PoolBehaviour.Expand:
                    trackSize += 1;
                    track = new AudioTrack(new GameObject("[Track " + trackSize + "]"), transform);
                    tracks.Add(track);
                    break;
                // Replace lowest priority
                case PoolBehaviour.Prioritize:
                    track = GetLowestPriority((int)preset.priority);

                    if (track == null) { return; } // no lower priority found

                    track.source.Stop();
                    track.go.SetActive(false);
                    StopCoroutine(track.coroutine);

                    // Clear from jobs list
                    foreach (AudioJob j in activeJobs[track.go.transform.parent])
                    {
                        if (j.track == track)
                        {
                            activeJobs[track.go.transform.parent].Remove(j);
                            break;
                        }
                    }

                    // Reset Parent
                    track.go.transform.SetParent(transform);
                    break;
            }

            if (track == null) { this.LogWarning("No track found"); return; }
        }

        this.Log("Play: " + track.go.name + " - " + preset.name, debug);
        AudioJob job = new AudioJob(JobAction.Play, preset, cue, track, transform);
        track.coroutine = ExecuteJob(job);
        StartCoroutine(track.coroutine);
        activeJobs[job.parent].Add(job);
    }

    ///<summary>Executes AudioJob</summary>
    IEnumerator ExecuteJob(AudioJob job)
    {
        AudioTrack track = job.track;
        AudioSource source = track.source;

        source.loop = false;
        track.go.SetActive(true);

        switch (job.action)
        {
            case JobAction.Play:
                if (job.fadeTime > 0f) { source.volume = 0f; }
                source.Play();
                break;
            case JobAction.Loop:
                if (job.fadeTime > 0f) { source.volume = 0f; }
                source.Play();
                source.loop = true;
                break;
        }

        // Fade
        if (job.fadeTime > 0f)
        {
            this.Log("(Fade - " + job.fadeTime + "s ): Start", debug);
            // When Start or restart, initialValue is 0f Otherwise its 1f
            float initialVolume = job.action != JobAction.Stop ? 0f : 1f;
            // If initialValue is 0, set it 1, otherwise 0
            float targetVolume = initialVolume == 0 ? 1 : 0;
            float timer = 0f;

            while (timer <= job.fadeTime)
            {
                track.source.volume = Mathf.Lerp(initialVolume, targetVolume, timer / job.fadeTime);
                timer += Time.deltaTime;
                yield return null;
            }
            this.Log("(Fade - " + job.fadeTime + "s ): Done", debug);
        }

        // Loop
        switch (job.action)
        {
            case JobAction.Play:
                yield return new WaitForSeconds((source.clip.length / source.pitch) - job.fadeTime);
                break;
            case JobAction.Loop:
                while (true)
                {
                    // Just wait forever
                    yield return new WaitForSeconds(source.clip.length / source.pitch);
                }
        }
        // Stop
        source.Stop();

        // Reset GO
        track.go.transform.SetParent(transform, false);
        track.go.SetActive(false);

        // When only job on parent, remove from dictionary
        if (activeJobs[job.parent].Count <= 1)
        {
            activeJobs.Remove(job.parent);
        }
        // When there are more jobs on parent, just remove this
        else
        {
            activeJobs[job.parent].Remove(job);
        }
    }

    ///<summary>Search for first inactive Track and return it.</summary>
    AudioTrack GetInactiveTrack()
    {
        foreach (AudioTrack track in tracks)
        {
            if (track.source == null) { continue; }
            if (!track.source.isPlaying)
            {
                return track;
            }
        }
        return null;
    }

    ///<summary>Search for track with the lowest priority and return it.</summary>
    AudioTrack GetLowestPriority(int lowestPriority)
    {
        foreach (AudioTrack track in tracks)
        {
            if (track.source.priority <= lowestPriority)
            {
                lowestPriority = track.source.priority;
                return track;
            }
        }
        return null;
    }

    ///<summary>Remove Jobs on the same parent</summary>
    void RemoveConflictingJobs(AudioPresetSO preset, AudioCue cue)
    {
        // Remove Conflicting Jobs
        Transform parent = cue.parent;
        if (parent == null) { parent = transform;
        }

        // check for same parent
        if (!activeJobs.ContainsKey(parent)) { activeJobs.Add(parent, new List<AudioJob>()); return; }

        // Check if same audioTag is already active on parent
        for (int i = 0; i < activeJobs[parent].Count; i++)
        {
            // If found, remove it
            if (activeJobs[parent][i].preset == preset)
            {
                // Reset the track
                AudioTrack track = activeJobs[parent][i].track;
                track.go.transform.SetParent(transform);
                track.go.SetActive(false);
                // Remove Running Job
                StopCoroutine(track.coroutine);

                // Remove from list
                activeJobs[parent].RemoveAt(i);
                this.Log("Removed Duplicate: '" + preset + "' on '" + parent.name + "' (" + i + ")", debug);
                return;
            }
        }
    }
    #endregion

#if UNITY_EDITOR
    [Button("List Active Jobs")]
    void ListActiveJobs()
    {
        string msg = "Active Jobs:\n";
        foreach (KeyValuePair<Transform, List<AudioJob>> parent in activeJobs)
        {
            msg += parent.Key.name +":\n";
            foreach (AudioJob job in parent.Value)
            {
                msg += job.track.go.name + " - '" + job.preset.name + "'\n";
            }
            msg += "\n";
        }
        this.Log(msg, debug);
    }
#endif
}
