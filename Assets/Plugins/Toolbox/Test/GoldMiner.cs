using System.Collections;
using UnityEngine;
using Toolbox.Data;
using Toolbox.Time;
using Toolbox.Events;
using TMPro;

namespace Toolbox.Loot
{
    public class GoldMiner : MonoBehaviour
    {
        [SerializeField] int gold = 0;
        [SerializeField] float earnSpeed = 1f;
        [SerializeField] int earnAmount = 1;

        [SerializeField] TextMeshProUGUI displayText;

        [Header("Listening to...")]
        [SerializeField] VoidChannelSO OnTimeSet;

        Coroutine MiningCoroutine;

        #region System Functions
        private void OnEnable()
        {
            OnTimeSet?.Subscribe(GetAFKMining, gameObject);
        }
        private void OnDisable()
        {
            OnTimeSet?.Unsubscribe(GetAFKMining, gameObject);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                gold = FileIO.Load(gold, "gold");
                MiningCoroutine = StartCoroutine(Mine());
            }
            else
            {
                StopCoroutine(MiningCoroutine);
                FileIO.Save(gold, "gold");
            }
        }
        #endregion

        void GetAFKMining()
        {
            int offlineEarnings = (int)(RealTime.AFKTime).TotalSeconds * earnAmount; // 1 Coin per Second
            Add(offlineEarnings);
        }

        IEnumerator Mine()
        {
            while (true)
            {
                yield return new WaitForSeconds(earnSpeed);
                Add(earnAmount);
            }
        }

        void Add(int coinsToAdd)
        {
            gold += coinsToAdd;
            displayText.text = gold.ToString();
        }
    }
}
