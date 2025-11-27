using System;
using UnityEngine;

namespace Assets.Code.LootSystem.NewSystem
{
    [Serializable]
    public struct LootSpawnSettings
    {
        [field: SerializeField][field: Range(0f, 5f)] public float MaxOffset { get; private set; }
        [field: SerializeField][field: Range(0.5f, 1f)] public float LootAirTime { get; private set; }
        [field: SerializeField][field: Range(0.5f, 2f)] public float JumpPower { get; private set; }
    }
}
