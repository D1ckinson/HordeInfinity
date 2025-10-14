using Assets.Code.CharactersLogic.EnemyLogic;
using Assets.Code.CharactersLogic.Movement.Direction_Sources;
using Assets.Code.Tools;
using Assets.Scripts.Configs;
using Assets.Scripts.Movement;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Factories
{
    public class EnemyFactory
    {
        private readonly Dictionary<CharacterType, CharacterConfig> _enemiesConfigs;
        private readonly LootFactory _lootFactory;
        private readonly Transform _hero;
        private readonly EnemySpawnerSettings _spawnerSettings;
        private readonly GameAreaSettings _gameAreaSettings;
        private readonly Dictionary<CharacterType, Pool<EnemyComponents>> _pools;
        private readonly CharacterConfig _goldEnemy;

        public EnemyFactory(Dictionary<CharacterType, CharacterConfig> enemiesConfigs, LootFactory lootFactory,
            Transform hero, EnemySpawnerSettings spawnerSettings, GameAreaSettings gameAreaSettings, CharacterConfig goldEnemy)
        {
            _enemiesConfigs = enemiesConfigs.ThrowIfNullOrEmpty();
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
            enemy.CharacterMovement.Run();

            return enemy;
        }

        public void DisableAll()
        {
            _pools.ForEachValues(pool => pool.DisableAll());
            _lootFactory.DisableAll();
        }

        public void StopAll()
        {
            IEnumerable<EnemyComponents> enemyComponents = _pools.Values.SelectMany(pool => pool.GetAllActive());
            enemyComponents.ForEach(enemyComponent => enemyComponent.CharacterMovement.Stop());
        }

        public void ContinueAll()
        {
            IEnumerable<EnemyComponents> enemyComponents = _pools.Values.SelectMany(pool => pool.GetAllActive());
            enemyComponents.ForEach(enemyComponent => enemyComponent.CharacterMovement.Run());
        }

        private EnemyComponents Create(CharacterConfig config)
        {
            EnemyComponents enemy = Object.Instantiate(config.Prefab).GetComponentOrThrow<EnemyComponents>();
            enemy.Initialize(new DirectionTellerTo(enemy.transform));

            enemy.CharacterMovement.Initialize(config.MoveSpeed, config.RotationSpeed, enemy.DirectionTeller);
            enemy.Health.Initialize(config.MaxHealth, config.InvincibilityDuration, config.InvincibilityTriggerPercent);
            enemy.CollisionDamage.Initialize(config.Damage, config.DamageLayer);
            enemy.DeathTriger.Initialize(enemy.Health, _lootFactory, config.Loot, enemy.CharacterMovement);
            enemy.DirectionTeller.SetTarget(_hero);
            enemy.SetType(config.Type);

            return enemy;
        }

        private EnemyComponents CreateGoldEnemy()
        {
            EnemyComponents enemy = Object.Instantiate(_goldEnemy.Prefab).GetComponentOrThrow<EnemyComponents>();
            enemy.Initialize(new DirectionTellerFrom(enemy.transform));

            enemy.CharacterMovement.Initialize(_goldEnemy.MoveSpeed, _goldEnemy.RotationSpeed, enemy.DirectionTeller);
            enemy.Health.Initialize(_goldEnemy.MaxHealth, _goldEnemy.InvincibilityDuration, _goldEnemy.InvincibilityTriggerPercent);
            enemy.CollisionDamage.Initialize(_goldEnemy.Damage, _goldEnemy.DamageLayer);
            enemy.DeathTriger.Initialize(enemy.Health, _lootFactory, _goldEnemy.Loot, enemy.CharacterMovement);
            enemy.DirectionTeller.SetTarget(_hero);
            enemy.SetType(_goldEnemy.Type);

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
