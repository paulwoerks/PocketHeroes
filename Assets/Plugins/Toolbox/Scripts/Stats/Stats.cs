using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox.Units
{
    [CreateAssetMenu(menuName = "Units/Stats", fileName = "UnitStats")]
    public class Stats : SerializedSO
    {
        public Dictionary<Stat, float> stats = new();

        public float GetStat(Stat stat) =>
            stats.TryGetValue(stat, out float value) ? value : 0;

        public void Add(Stat stat, float amount)
        {
            if (!stats.ContainsKey(stat))
                return;

            stats[stat] += amount;
        }

        public void Reduce(Stat stat, float amount)
        {
            if (!stats.ContainsKey(stat))
                return;

            stats[stat] -= amount;
        }
    }
}