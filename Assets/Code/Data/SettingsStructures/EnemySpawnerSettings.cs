using System;
using UnityEngine;

namespace Assets.Code.Data.SettingsStructures
{
    [Serializable]
    public struct EnemySpawnerSettings
    {
        [field: SerializeField][field: Min(1)] public float Radius { get; private set; }
        [field: SerializeField][field: Min(0.1f)] public float Delay { get; private set; }
        [field: SerializeField][field: Min(1)] public int SpawnLimit { get; private set; }
    }
}
