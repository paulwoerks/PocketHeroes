using UnityEngine;

// BY  : Paul Woerner, info@paulwoerner.com

namespace Toolbox.Loot
{
    [System.Serializable]
    public class Currency
    {
        [SerializeField] LootSO type;
        [SerializeField] int amount;

        public LootSO Type => type;
        public int Amount => amount;

        /// <summary>Set up a currency</summary>
        /// <param name="type">Currency Type</param>
        /// <param name="amount">Amount stored</param>
        public Currency (LootSO type, int amount = 0)
        {
            this.type = type;
            this.amount = amount;
        }

        public void Add(int amount) { this.amount += amount; }

        public void Substract(int amount) { this.amount -= amount; }
    }

}