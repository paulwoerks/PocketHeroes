using UnityEngine;
using UnityEngine.UI;

// BY  : Paul Woerner, info@paulwoerner.com

namespace Toolbox.Loot
{
    /// <summary>Base of all Loot - Items, Equipment, Skills, even Currencies</summary>
    [CreateAssetMenu(menuName = "Loot/Loot", fileName = "Loot")]
    public class LootSO : SO
    {
        [SerializeField] Image icon;

        public string Name => name;
        public Image Icon => icon;

        /// <summary>Claim Loot</summary>
        public virtual void Claim() { }
    }
}