using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox.Time;
using Toolbox.Events;

namespace Toolbox.Loot
{
    [System.Serializable]
    // Should be SO?
    public class Mine
    {
        public LootSO currency;
        public double stored = 0;

        public float generatePerMinute = 60;

        /// <summary>Generate Earnings for minutesPassed</summary>
        public void Generate(double minutesPassed)
        {
            stored += generatePerMinute * minutesPassed;
            // Should do something after stuff is generated, like putting it into player wallet
        }
    }
    public class AutoMiner : MonoBehaviour
    {
        [SerializeField] List<Mine> mines = new();

        [Header("References")]
        [SerializeField] WalletSO playerWallet;

        [Header("Listening to...")]
        [SerializeField] VoidChannelSO OnTimeSet;

        Coroutine MiningCoroutine;

        private void OnEnable()
        {
            OnTimeSet?.Subscribe(ReceiveOfflineMinings, gameObject);
        }
        private void OnDisable()
        {
            OnTimeSet?.Unsubscribe(ReceiveOfflineMinings, gameObject);
        }


        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                // Load Mines
                MiningCoroutine = StartCoroutine(Mine());
            }
            else
            {
                StopCoroutine(MiningCoroutine);
                // Save Mines
            }
        }

        void ReceiveOfflineMinings()
        {
            double minutesPassed = (RealTime.AFKTime).TotalMinutes;
            foreach (Mine mine in mines)
            {
                double amountMined = minutesPassed * mine.generatePerMinute;
                playerWallet.Add(mine.currency, (int)amountMined);
                // Rest falls flat now, should be stored;
            }
        }

        IEnumerator Mine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);

                foreach (Mine mine in mines)
                    mine.Generate(1 / 60);
            }
        }
    }
}
