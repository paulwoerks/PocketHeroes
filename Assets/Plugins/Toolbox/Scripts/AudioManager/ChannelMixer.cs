using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using Sirenix.OdinInspector;
using Toolbox.Singleton;

namespace Toolbox{
    public class ChannelMixer : Singleton<ChannelMixer>{
        AudioMixer mixer;
    ChannelInfo[] channels;

    [Required] [SerializeField] AudioMixerGroupConfigSO mixerGroupConfig;

    #region Unity Functions
    private void Start()
    {
        channels = mixerGroupConfig.channels;
        mixer = mixerGroupConfig.mixer;
    }
    #endregion

    public void SetChannelVolume(string channelName, float desiredVolume, float fadeTime)
    {
        if (!mixer) { this.LogError("(Volume): No AudioMixer attached."); return; }

        if (desiredVolume <= 0f) { desiredVolume = 0.001f; }
        if (desiredVolume > 1f) { desiredVolume = 1f; }

        ChannelInfo channel = null;
        foreach (ChannelInfo c in channels)
        {
            if (c.channelName == channelName)
            {
                channel = c;
                break;
            }
        }

        if (channel == null) { this.LogError("Channel " + channelName + " not found"); }

        if (channel.fader != null) { StopCoroutine(channel.fader); } // Stop any coroutine that might be in the middle of fading this channel

        if (fadeTime <= 0f)
        {
            mixer.SetFloat(channel + "_Volume", Mathf.Log10(desiredVolume) * 25);
            this.Log("(Volume): '" + channel + "' = " + (Mathf.Log10(desiredVolume) * 25) + "' db");
        } else
        {
            channel.fader = ExecuteVolumeJob(channel.channelName, desiredVolume, fadeTime);
            StartCoroutine(channel.fader);
        }
    }

        IEnumerator ExecuteVolumeJob(string channel, float desiredVolume, float fadeTime)
    {
        float timer = 0f;
        mixer.GetFloat(channel + "_Volume", out float startVolume);

        this.Log("(Fade " + fadeTime + "s ): '" + channel + "' [" + startVolume + " db] => [" + desiredVolume + "' db]");

        // fading
        while (timer < fadeTime)
        {
            timer += UnityEngine.Time.deltaTime;
            mixer.SetFloat(channel + "_Volume", Mathf.Log10(Mathf.Lerp(startVolume, desiredVolume, timer / fadeTime)) * 20);
            yield return null; // Wait 1 frame
        }

        mixer.SetFloat(channel + "_Volume", Mathf.Log10(desiredVolume) * 20); // Just to make sure
        this.Log("(Fade " + fadeTime + "s ): '" + channel + "' = " + desiredVolume + "' db");
    }
    }
}
