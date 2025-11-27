using Assets.Code.AbilitySystem.StatTypes;
using System;

namespace Assets.Code.AbilitySystem.Stats
{
    [Serializable]
    public struct BoolStat
    {
        public bool IsOn;
        public BoolStatType Type;
    }
}
