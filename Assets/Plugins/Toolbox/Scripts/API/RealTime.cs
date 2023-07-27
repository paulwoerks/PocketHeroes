using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using SimpleJSON;
using Sirenix.OdinInspector;
using Toolbox.API;
using Toolbox.Data;
using Toolbox.Events;
using Toolbox.Singleton;

// Author: Paul Woerner
namespace Toolbox.Time
{
    /// <summary>Provides the Real Time over the Internet if possible with just 1 API Call on Start</summary>
    public class RealTime : Singleton<RealTime>
    {
        public static DateTime Now => DateTime.UtcNow.AddMinutes(Instance.offsetInMinutes);
        public static DateTime LocalTime => DateTime.Now.AddMinutes(Instance.offsetInMinutes);
        public static TimeSpan AFKTime => (Instance.currentSessionStarted - Instance.lastSessionEnded);

        DateTime currentSessionStarted;
        DateTime lastSessionEnded;

        [SerializeField] bool isRealTime;

        readonly string API_URL = "http://worldtimeapi.org/api/timezone/Europe/London";

        double offsetInMinutes;

        [Header("Broadcasting on ...")]
        [SerializeField] BoolChannelSO OnTimeSet;

        void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                isRealTime = false;

                lastSessionEnded = FileIO.Load(lastSessionEnded, "sessionEnd", debug);

                offsetInMinutes = 0;
                currentSessionStarted = Now;

                RequestUTCServerTime();
            }
            else
            {
                lastSessionEnded = Now;
                FileIO.Save(lastSessionEnded, "sessionEnd", debug);
#if UNITY_EDITOR
                ended = lastSessionEnded.ToString();
#endif
            }
        }
        [Button("Fetch Data")]
        void RequestUTCServerTime() => APIReader.Read(API_URL, SetServerTime, ServerTimeFailed);

        /// <summary>When the Server Time is Recieved, Apply it to the Real Time</summary>
        void SetServerTime(JSONNode root)
        {
            DateTime serverTime = ConvertAPIDateTimeFormat(root["datetime"]);
            currentSessionStarted = serverTime;

            double offset = (Now - currentSessionStarted).TotalMinutes;
            offsetInMinutes = currentSessionStarted < Now ? -offset : offset;

            isRealTime = true;
            OnTimeSet?.Invoke(isRealTime);
        }

        void ServerTimeFailed(string result, string error)
        {
            offsetInMinutes = 0;

            isRealTime = false;
            OnTimeSet?.Invoke(isRealTime);

            this.LogError($"{result}: {error}");
        }

        DateTime ConvertAPIDateTimeFormat(string dateTime)
        {
            string date = Regex.Match(dateTime, @"^\d{4}-\d{2}-\d{2}").Value; //match 0000-00-00
            string time = Regex.Match(dateTime, @"\d{2}:\d{2}:\d{2}").Value; //match 00:00:00

            return DateTime.Parse(string.Format("{0} {1}", date, time));
        }

        #region Editor
#if UNITY_EDITOR
        [SerializeField] string now;
        [SerializeField] string localTime;
        [SerializeField] string afkTime;
        [SerializeField] string started;
        [SerializeField] string ended;

        private void OnEnable() => StartCoroutine(DisplayTime());

        private void OnDisable() => StopAllCoroutines();

        IEnumerator DisplayTime()
        {
            while (true)
            {
                now = Now.ToString();
                localTime = LocalTime.ToString();
                afkTime = $"{((int)AFKTime.TotalMinutes).ToString()}m";
                started = currentSessionStarted.ToString();
                ended = lastSessionEnded.ToString();
                yield return new WaitForSecondsRealtime(1);
            }
        }
#endif
        #endregion
    }
}