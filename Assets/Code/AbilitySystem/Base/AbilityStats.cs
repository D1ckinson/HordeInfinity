using Assets.Code.AbilitySystem.StatTypes;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Base
{
    [Serializable]
    public class AbilityStats
    {
        private const char Minus = '-';
        private const char Plus = '+';

        [field: SerializeField] public int Cost { get; private set; }

        [SerializeField] private SerializedDictionary<FloatStatType, float> _floatStats;
        [SerializeField] private SerializedDictionary<BoolStatType, bool> _boolStats;
        [SerializeField] private SerializedDictionary<IntStatType, int> _intStats;

        public AbilityStats() { }

        public AbilityStats(AbilityStats stats)
        {
            stats.ThrowIfNull();

            _floatStats = new(stats._floatStats);
            _boolStats = new(stats._boolStats);
            _intStats = new(stats._intStats);

            Cost = stats.Cost;
        }

        public float Get(FloatStatType type)
        {
            return _floatStats[type];
        }

        public bool Get(BoolStatType type)
        {
            return _boolStats[type];
        }

        public int Get(IntStatType type)
        {
            return _intStats[type];
        }

        public List<string> GetStatsDifference(AbilityStats stats)
        {
            List<string> description = new();

            IEnumerable<FloatStatType> floatStatTypes = Constants.GetEnums<FloatStatType>();

            for (int i = Constants.Zero; i < floatStatTypes.Count(); i++)
            {
                FloatStatType statType = floatStatTypes.ElementAt(i);

                if (_floatStats.TryGetValue(statType, out float currentValue) && stats._floatStats.TryGetValue(statType, out float nextValue))
                {
                    float resultValue = nextValue - currentValue;
                    AddIfNotZero(description, UIText.FloatStatName[statType], resultValue);
                }
            }

            IEnumerable<IntStatType> intStatTypes = Constants.GetEnums<IntStatType>();

            for (int i = Constants.Zero; i < intStatTypes.Count(); i++)
            {
                IntStatType statType = intStatTypes.ElementAt(i);

                if (_intStats.TryGetValue(statType, out int currentValue) && stats._intStats.TryGetValue(statType, out int nextValue))
                {
                    int resultValue = nextValue - currentValue;
                    AddIfNotZero(description, UIText.IntStatName[statType], resultValue);
                }
            }

            IEnumerable<BoolStatType> boolStatTypes = Constants.GetEnums<BoolStatType>();

            for (int i = Constants.Zero; i < boolStatTypes.Count(); i++)
            {
                BoolStatType statType = boolStatTypes.ElementAt(i);

                if (_boolStats.TryGetValue(statType, out bool currentValue) && stats._boolStats.TryGetValue(statType, out bool nextValue))
                {
                    if (currentValue == nextValue)
                    {
                        continue;
                    }

                    char sign = nextValue ? Plus : Minus;

                    description.Add($"{sign} {UIText.BoolStatName[statType]}");
                }
            }

            return description;
        }

        public List<string> GetStatsDescription()
        {
            List<string> description = new();

            description.AddRange(_floatStats.Select(pair => $"{UIText.FloatStatName[pair.Key]} {pair.Value}"));
            description.AddRange(_intStats.Select(pair => $"{UIText.IntStatName[pair.Key]} {pair.Value}"));
            description.AddRange(_boolStats.Where(pair => pair.Value).Select(pair => UIText.BoolStatName[pair.Key]));

            return description;
        }

        private void AddIfNotZero(List<string> list, string statName, float value)
        {
            if (value == Constants.Zero)
            {
                return;
            }

            string formattedValue = value % Constants.One == Constants.Zero
                ? value.ToString("+0;-0")
                : value.ToString("+0.0;-0.0");

            list.Add($"{statName} {formattedValue}");
        }

        public void Multiply(FloatStatType type, float multiplier)
        {
            if (_floatStats.TryGetValue(type, out float stat))
            {
                stat *= multiplier;
                _floatStats[type] = stat;
            }
        }
    }
}
