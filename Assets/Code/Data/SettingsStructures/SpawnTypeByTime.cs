using Assets.Code.Core;
using System;
using UnityEngine;

namespace Assets.Code.Data.SettingsStructures
{
    [Serializable]
    public struct SpawnTypeByTime
    {
        [field: SerializeField] public int Time { get; private set; }
        [field: SerializeField] public CharacterType Type { get; private set; }
    }
}