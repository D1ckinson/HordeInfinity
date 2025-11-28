using Assets.Code.LootSystem;
using System;
using UnityEngine;

namespace Assets.Code.Data.Base
{
    [Serializable]
    public struct LootDropInfo
    {
        [field: SerializeField] public LootType Type { get; private set; }
        [field: SerializeField][field: Min(1)] public int Value { get; private set; }
        [field: SerializeField][field: Range(1f, 100f)] public float DropChance { get; private set; }
    }
}
