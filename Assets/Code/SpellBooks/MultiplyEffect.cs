using Assets.Code.Tools;
using System;
using UnityEngine;

namespace Assets.Code.SpellBooks
{
    [Serializable]
    public class MultiplyEffect : IEffect
    {
        [SerializeField][Min(0)] private float _multiplier;
        [SerializeField] private FloatStatType _statType;

        [field: SerializeField][field: Min(0)] public int Priority { get; private set; }

        public AbilityStats Apply(AbilityStats stats)
        {
            stats.ThrowIfNull().Multiply(_statType, _multiplier);

            return stats;
        }
    }
}
