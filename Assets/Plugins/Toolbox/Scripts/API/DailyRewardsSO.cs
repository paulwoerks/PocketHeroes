using Toolbox.Events;
using Toolbox.Data;
using UnityEngine;

namespace Toolbox.Time
{
    [CreateAssetMenu(fileName = "DailyRewards", menuName = "Time/Daily Rewards", order = 0)]
    /// Checks for Daily Rewards
    public class DailyRewardsSO : SO
    {
        [Header("Info")]
        [SerializeField] int consecutiveDailySessions;
        [SerializeField] double averageSessionTime;

        [Header("Listening to ...")]
        [SerializeField] VoidChannelSO OnTimeSet;

        [Header("Broadcasting on ...")]
        [SerializeField] IntChannelSO OnConsecutiveDaysChanged;


        private void OnEnable()
        {
            OnTimeSet?.Subscribe(UpdateConsecutiveDailySessions, this);
        }

        private void OnDisable()
        {
            OnTimeSet?.Unsubscribe(UpdateConsecutiveDailySessions, this);
        }


        void UpdateConsecutiveDailySessions()
        {
            consecutiveDailySessions = FileIO.Load(consecutiveDailySessions, "consecutiveDailySessions", debug);

            int daysSinceLastSession = (int)RealTime.AFKTime.TotalDays;

            switch (daysSinceLastSession)
            {
                case < 0:
                    this.Log(daysSinceLastSession + "d since last session. Time traveller?", debug);
                    break;
                case 0:
                    this.Log("Same day as last session", debug);
                    break;
                case 1:
                    // Add a day to the streak of consecutive daily sessions
                    consecutiveDailySessions += 1;
                    OnConsecutiveDaysChanged?.Invoke(consecutiveDailySessions);
                    this.Log("It's the next day!", debug);
                    break;
                case > 1:
                    // Reset Streak, you missed a day
                    consecutiveDailySessions = 0;
                    OnConsecutiveDaysChanged?.Invoke(consecutiveDailySessions);
                    this.Log(daysSinceLastSession + "d since last session. Your streak is broken", debug);
                    break;
            }
            // Check for days Online
            FileIO.Save(consecutiveDailySessions, "consecutiveDailySessions", debug);
        }
    }
}
