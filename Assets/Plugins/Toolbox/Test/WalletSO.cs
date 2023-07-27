using UnityEngine;
using Toolbox.Data;

namespace Toolbox.Loot
{
    /// <summary>Holds the Players Current Currencies (Wallet)</summary>
    [CreateAssetMenu(menuName = "Loot/Currency/Wallet", fileName = "PlayerWallet")]
    public class WalletSO : SO
    {
        [SerializeField] Currency[] wallet;

        private void Awake()
        {
            Load();
        }

        #region Add
        /// <summary>Add 3 Gold to Wallet</summary>
        public void Add(LootSO type, int amount) { Add(new Currency(type, amount)); }

        /// <summary>Add 3 Gold to Wallet</summary>
        public void Add(Currency currency)
        {
            foreach (Currency c in wallet)
            {
                if (c.Type.Equals(currency.Type))
                    c.Add(currency.Amount);
                this.Log("[+]" + currency.Type + ": " + currency.Amount + " = " + c.Amount, debug);
            }
            Save();
        }
        #endregion

        #region Substract
        /// <summary>Remove 3 Gold from Wallet</summary>
        public void Substract(LootSO preset, int amount) { Substract(new Currency(preset, amount)); }

        /// <summary>Remove 3 Gold from Wallet</summary>
        public void Substract(Currency currency)
        {
            foreach (Currency c in wallet)
            {
                if (c.Type.Equals(currency.Type))
                    c.Substract(currency.Amount);
                this.Log("[-]" + currency.Type + ": " + currency.Amount + " = " + c.Amount, debug);
            }
            Save();
        }
        #endregion

        #region Save & Load
        /// <summary>Save wallet to local storage</summary>
        public void Save()
        {
            FileIO.Save(wallet, debug);
        }

        /// <summary>Loads wallet from local storage</summary>
        public void Load()
        {
            wallet = FileIO.Load<Currency[]>(wallet, debug);
        }
        #endregion
    }
}
