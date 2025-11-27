using Assets.Code.LootSystem.Legacy;
using System;
using UnityEngine;

namespace Assets.Code.Data.Base
{
    [Serializable]
    public class LootDropInfo
    {
        [field: SerializeField] public LootType Type { get; private set; }
        [field: SerializeField][field: Min(1)] public int Count { get; private set; } = 1;
        [field: SerializeField][field: Min(1f)] public float DropChance { get; private set; } = 100;
    }
}
