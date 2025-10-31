using System;
using UnityEngine;

namespace Assets.Code
{
    [Serializable]
    public struct FloatStat
    {
        [Min(0)] public float Value;
        public FloatStatType Type;
    }
}
