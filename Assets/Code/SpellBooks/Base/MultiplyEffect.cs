using Assets.Code.AbilitySystem.Base;
using Assets.Code.AbilitySystem.StatTypes;
using Assets.Code.Tools.Base;
using System;
using UnityEngine;

namespace Assets.Code.SpellBooks.Base
{
    [Serializable]
    public class MultiplyEffect : IEffect
    {
        [SerializeField][Min(0)] private float _multiplier;
        [SerializeField] private FloatStatType _statType;

        public MultiplyEffect(
            float multiplier,
            FloatStatType statType,
            int priority)
        {
            _multiplier = multiplier.ThrowIfNegative();
            _statType = statType.ThrowIfNull();
            Priority = priority;
        }

        [field: SerializeField][field: Min(0)] public int Priority { get; private set; }

        public event Action ValueChanged;

        public AbilityStats Apply(AbilityStats stats)
        {
            stats.ThrowIfNull().Multiply(_statType, _multiplier);

            return stats;
        }

        public void ChangeValue(float value)
        {
            _multiplier = value.ThrowIfNegative();
            ValueChanged?.Invoke();
        }
    }
}
