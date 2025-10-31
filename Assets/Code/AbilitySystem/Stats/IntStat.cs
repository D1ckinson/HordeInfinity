using System;
using UnityEngine;

namespace Assets.Code
{
    [Serializable]
    public struct IntStat
    {
        [Min(0)] public int Value;
        public IntStatType Type;
    }
}
