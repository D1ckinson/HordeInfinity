using Assets.Code.CharactersLogic.EnemyLogic;
using Assets.Code.CharactersLogic.Movement.DirectionSources;
using Assets.Code.Core;
using Assets.Code.Data.Base;
using Assets.Code.Data.SettingsStructures;
using Assets.Code.LootSystem.Legacy;
using Assets.Code.Tools.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.EnemySpawnLogic
{
    public class EnemyFactory
    {
        private readonly LootFactory _lootFactory;
        private readonly Transform _hero;
        private readonly EnemySpawnerSettings _spawnerSettings;
        private readonly GameAreaSettings _gameAreaSettings;
        private readonly Dictionary<CharacterType, Pool<EnemyComponents>> _pools;
        private readonly CharacterConfig _goldEnemy;

        public EnemyFactory(
            Dictionary<CharacterType, CharacterConfig> enemiesConfigs,
            LootFactory lootFactory,
            Transform hero, 
            EnemySpawnerSettings spawnerSettings, 
            GameAreaSettings gameAreaSettings, 
            CharacterConfig goldEnemy)
        {
            _lootFactory = lootFactory.ThrowIfNull();
            _hero = hero.ThrowIfNull();
            _goldEnemy = goldEnemy.ThrowIfNull();

            _spawnerSettings = spawnerSettings.ThrowIfDefault();
            _gameAreaSettings = gameAreaSettings.ThrowIfDefault();

            _pools = new();

            foreach (KeyValuePair<CharacterType, CharacterConfig> pair in enemiesConfigs)
            {
                _pools.Add(pair.Key, new(() => Create(pair.Value)));
            }

            _pools.Add(CharacterType.GoldEnemy, new(CreateGoldEnemy));
        }

        public bool IsSpawnLimitReached => _pools.Values.Sum(pool => pool.ReleaseCount) == _spawnerSettings.SpawnLimit;
        public float Delay => _spawnerSettings.Delay;

        public EnemyComponents Spawn(CharacterType characterType)
        {
            EnemyComponents enemy = _pools[characterType.ThrowIfNull()].Get();

            SetTransform(enemy.transform);
            enemy.Mover.Run();
            enemy.Rotator.Run();

            return enemy;
        }

        public void DisableAll()
        {
            _pools.ForEachValues(pool => pool.DisableAll());
            _pools.ForEachValues(pool => pool.ForEach(enemy => enemy.Booster.ResetHealthBoost()));
            _lootFactory.DisableAll();
        }

        public void StopAll()
        {
            IEnumerable<EnemyComponents> enemyComponents = _pools.Values.SelectMany(pool => pool.GetAllActive());
            enemyComponents.ForEach(enemyComponent => enemyComponent.Mover.Stop());
        }

        public void ContinueAll()
        {
            IEnumerable<EnemyComponents> enemyComponents = _pools.Values.SelectMany(pool => pool.GetAllActive());
            enemyComponents.ForEach(enemyComponent => enemyComponent.Mover.Run());
        }

        private EnemyComponents Create(CharacterConfig config)
        {
            EnemyComponents enemy = config.Prefab.Instantiate().GetComponentOrThrow<EnemyComponents>();
            DirectionTellerTo directionSource = new(enemy.transform);

            directionSource.SetTarget(_hero);
            enemy.Initialize(config, directionSource, _lootFactory);

            return enemy;
        }

        private EnemyComponents CreateGoldEnemy()
        {
            EnemyComponents enemy = _goldEnemy.Prefab.Instantiate().GetComponentOrThrow<EnemyComponents>();
            DirectionTellerFrom directionSource = new(enemy.transform);

            directionSource.SetTarget(_hero);
            enemy.Initialize(_goldEnemy, directionSource, _lootFactory);

            return enemy;
        }

        private void SetTransform(Transform enemy)
        {
            Vector3 position = GenerateRandomPoint();
            Vector3 direction = _hero.position - position;

            direction.y = Constants.Zero;
            Quaternion rotation = Quaternion.LookRotation(direction);

            enemy.transform.SetPositionAndRotation(position, rotation);
        }

        private Vector3 GenerateRandomPoint()
        {
            Vector3 distance = Utilities.GenerateRandomDirection() * _spawnerSettings.Radius;
            Vector3 point = _hero.position + distance;

            return IsPositionInGameArea(point) ? point : _hero.position - distance;
        }

        private bool IsPositionInGameArea(Vector3 position)
        {
            float distance = Vector3.Distance(_gameAreaSettings.Center, position);

            return distance <= _gameAreaSettings.Radius;
        }
    }
}
