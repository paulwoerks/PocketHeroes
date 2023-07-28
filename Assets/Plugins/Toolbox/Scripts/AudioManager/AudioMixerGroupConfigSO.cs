using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Toolbox;

#region Classes
[System.Serializable]
public class ChannelInfo
{
    public string channelName; // Name of the group
    public AudioMixerGroup channel; // Wraps an AudiomixerGroup.
    public IEnumerator fader; // coroutine for track fading

    public ChannelInfo(string channelName, AudioMixerGroup channel)
    {
        this.channelName = channelName;
        this.channel = channel;
    }
}
#endregion

///<summary>Pushes Mutations to GraphQL API Server and gets response.</summary>
[CreateAssetMenu(fileName = "AudioMixerGroupConfig", menuName = "Audio/Audio Mixer Group Config SO")]
public class AudioMixerGroupConfigSO : SO
{
    public AudioMixer mixer;
    public ChannelInfo[] channels;

#if UNITY_EDITOR
    [Button("Configure (once in Editor).")]
    void Configure()
    {
        if (!mixer) { this.LogError("(Volume): No AudioMixer attached."); return; }

        AudioMixerGroup[] groups = mixer.FindMatchingGroups(string.Empty); // Get all AudioMixer Groups
        // Add all mixer groups to the mixerGroupInfo
        List<ChannelInfo> _channels = new List<ChannelInfo>();
        foreach (AudioMixerGroup group in groups)
        {
            ChannelInfo groupInfo = new ChannelInfo(group.name, group);
            _channels.Add(groupInfo);
        }
        channels = _channels.ToArray();

        this.Log(channels.Length + " Channels set");
    }
#endif

    
}
