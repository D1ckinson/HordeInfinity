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
            _enemyFactory.StopAll();

            TimerService.PauseTimer(this, SetSpawnType);
            UpdateService.UnregisterUpdate(SpawnEnemy);
        }

        public void Continue()
        {
            _enemyFactory.ContinueAll();

            TimerService.ResumeTimer(this, SetSpawnType);
            UpdateService.RegisterUpdate(SpawnEnemy);
        }

        public void Reset()
        {
            UpdateService.UnregisterUpdate(SpawnEnemy);
            TimerService.StopTimer(this, SetSpawnType);

            _spawnTypeIndex = -Constants.One;
            _enemyFactory.DisableAll();
        }

        private void SpawnEnemy(float deltaTime)
        {
            if (_enemyFactory.IsSpawnLimitReached)
            {
                return;
            }

            _delay += deltaTime;
            _goldEnemySpawnDelay -= deltaTime;

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
            int nextIndex = _spawnTypeIndex + Constants.One;

            if (_spawnTypeByTime.Length == nextIndex)
            {
                TimerService.StopTimer(this, SetSpawnType);

                return;
            }

            int time = _spawnTypeByTime[nextIndex].Time;
            _spawnTypeIndex = nextIndex;

            TimerService.StartTimer(time, SetSpawnType, this, true);
        }

        private void TrySpawnGoldEnemy()
        {
            if (_goldEnemySpawnDelay > Constants.Zero)
            {
                return;
            }

            _goldEnemySpawnDelay = Random.Range(GoldEnemyMinSpawnDelay, GoldEnemyMaxSpawnDelay);
            _enemyFactory.Spawn(CharacterType.GoldEnemy);
        }
    }
}
