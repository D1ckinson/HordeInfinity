using Assets.Code.CharactersLogic.EnemyLogic;
using Assets.Code.Data.Setting_Structures;
using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Factories;
using UnityEngine;

namespace Assets.Code.Spawners
{
    public class EnemySpawner
    {
        private const float GoldEnemyMinSpawnDelay = 120;
        private const float GoldEnemyMaxSpawnDelay = 300;
        private const float DelayDecreaseValue = 0.05f;

        private readonly EnemyFactory _enemyFactory;
        private readonly Timer _timer = new();
        private readonly SpawnTypeByTime[] _spawnTypeByTime;

        private int _spawnTypeIndex = -1;
        private float _delay;

        private float _goldEnemySpawnDelay;

        public EnemySpawner(EnemyFactory enemyFactory, SpawnTypeByTime[] spawnTypeByTime)
        {
            _enemyFactory = enemyFactory.ThrowIfNull();
            _spawnTypeByTime = spawnTypeByTime.ThrowIfNullOrEmpty();
        }

        public void Run()
        {
            SetSpawnType();
            UpdateService.RegisterUpdate(SpawnEnemy);
            _goldEnemySpawnDelay = Random.Range(GoldEnemyMinSpawnDelay, GoldEnemyMaxSpawnDelay);
        }

        public void Pause()
        {
            _timer.Pause();
            _enemyFactory.StopAll();
            UpdateService.UnregisterUpdate(SpawnEnemy);
        }

        public void Continue()
        {
            _timer.Continue();
            _enemyFactory.ContinueAll();
            UpdateService.RegisterUpdate(SpawnEnemy);
        }

        public void Reset()
        {
            UpdateService.UnregisterUpdate(SpawnEnemy);
            _spawnTypeIndex = -Constants.One;
            _timer.Stop();
            _timer.Completed -= SetSpawnType;
            _enemyFactory.DisableAll();
        }

        private void SpawnEnemy()
        {
            if (_enemyFactory.IsSpawnLimitReached)
            {
                return;
            }

            _delay += Time.deltaTime;
            _goldEnemySpawnDelay -= Time.deltaTime;

            if (_delay < _enemyFactory.Delay - DelayDecreaseValue * _spawnTypeIndex)
            {
                return;
            }

            TrySpawnGoldEnemy();
            _enemyFactory.Spawn(_spawnTypeByTime[_spawnTypeIndex].Type);

            _delay = Constants.Zero;
        }

        private void SetSpawnType()
        {
            _timer.Completed -= SetSpawnType;
            int nextIndex = _spawnTypeIndex + Constants.One;

            if (_spawnTypeByTime.Length == nextIndex)
            {
                return;
            }

            int time = _spawnTypeByTime[nextIndex].Time;
            _spawnTypeIndex = nextIndex;
            _timer.Start(time);
            _timer.Completed += SetSpawnType;
        }

        private EnemyComponents TrySpawnGoldEnemy()
        {
            if (_goldEnemySpawnDelay > Constants.Zero)
            {
                return null;
            }

            _goldEnemySpawnDelay = Random.Range(GoldEnemyMinSpawnDelay, GoldEnemyMaxSpawnDelay);

            return _enemyFactory.Spawn(CharacterType.GoldEnemy);
        }
    }
}
