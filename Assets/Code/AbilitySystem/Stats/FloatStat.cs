using Assets.Code.AbilitySystem.StatTypes;
using System;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Stats
{
    [Serializable]
    public struct FloatStat
    {
        [Min(0)] public float Value;
        public FloatStatType Type;
    }
}
