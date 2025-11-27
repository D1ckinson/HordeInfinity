using Assets.Code.AbilitySystem.StatTypes;
using System;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Stats
{
    [Serializable]
    public struct IntStat
    {
        [Min(0)] public int Value;
        public IntStatType Type;
    }
}
