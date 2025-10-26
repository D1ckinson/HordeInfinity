using Assets.Code.Data.Setting_Structures;
using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Factories;
using System;
using UnityEngine;

namespace Assets.Code.Spawners
{
    public class NewEnemySpawner
    {
        private readonly EnemyFactory _enemyFactory;
        private readonly SpawnTypeByTime[] _spawnTypeByTime;

        private int _spawnTypeIndex = -1;
        private float _delay;

        private const float GoldEnemyMinSpawnDelay = 10;
        private const float GoldEnemyMaxSpawnDelay = 10;
        private float _goldEnemySpawnDelay;

        public NewEnemySpawner(EnemyFactory enemyFactory, SpawnTypeByTime[] spawnTypeByTime, int spawnTypeIndex, float delay)
        {
            _enemyFactory = enemyFactory;
            _spawnTypeByTime = spawnTypeByTime;
            _spawnTypeIndex = spawnTypeIndex;
            _delay = delay;
        }
    }

    [Serializable]
    public struct EnemySpawnSettings
    {
        [field: SerializeField] public CharacterType CharacterType { get; private set; }
        [field: SerializeField] public float SpawnAddTime { get; private set; }
        [field: SerializeField] public float SpawnRemoveTime { get; private set; }
        [field: SerializeField] public int SpawnWeight { get; private set; }
    }
}
