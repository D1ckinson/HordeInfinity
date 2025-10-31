using Assets.Scripts;
using System;
using UnityEngine;

namespace Assets.Code.Data.Setting_Structures
{
    [Serializable]
    public struct SpawnTypeByTime
    {
        [field: SerializeField] public int Time { get; private set; }
        [field: SerializeField] public CharacterType Type { get; private set; }
    }
}