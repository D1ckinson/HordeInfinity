using System;
using UnityEngine;

namespace Assets.Code.Data.SettingsStructures
{
    [Serializable]
    public struct GameAreaSettings
    {
        [field: SerializeField] public Vector3 Center { get; private set; }
        [field: SerializeField][field: Min(1)] public float Radius { get; private set; }
    }
}
