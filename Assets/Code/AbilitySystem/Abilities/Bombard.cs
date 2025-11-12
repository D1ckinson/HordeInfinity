using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Bombard : Ability
    {
        private readonly WaitForSeconds _delay = new(0.1f);
        private readonly Pool<Bomb> _bombPool;
        private readonly Pool<ParticleSystem> _visualEffectPool;
        private readonly Pool<AudioSource> _hitSoundPool;
        private readonly Transform _launchPoint;
        private readonly AudioSource _throwSound;

        public Bombard(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform launchPoint,
            BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _launchPoint = launchPoint.ThrowIfNull();

            _visualEffectPool = new(() => config.Effect.Instantiate());
            _hitSoundPool = new(() => config.HitSound.Instantiate());
            _bombPool = new(CreateBomb);
            _throwSound = config.ThrowSound.Instantiate();

            Bomb CreateBomb()
            {
                Bomb bomb = config.ProjectilePrefab
                    .GetComponentOrThrow<Bomb>()
                    .Instantiate()
                    .Initialize(CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(FloatStatType.Range), config.DamageLayer, _visualEffectPool, _hitSoundPool);

                bomb.Hit += RecordHitResult;

                return bomb;
            }
        }

        protected override void Apply()
        {
            CoroutineService.StartCoroutine(LaunchBombs(), this);
        }

        private IEnumerator LaunchBombs()
        {
            for (int i = Constants.Zero; i < CurrentStats.Get(IntStatType.ProjectilesCount); i++)
            {
                Bomb bomb = _bombPool.Get();
                bomb.Fly(_launchPoint.position, GenerateRandomPoint());
                _throwSound.PlayRandomPitch();

                yield return _delay;
            }
        }

        protected override void OnDispose()
        {
            CoroutineService.StopAllCoroutines(this);

            _bombPool.ForEach(bomb => bomb.Hit -= RecordHitResult);
            _bombPool.DestroyAll();

            _throwSound.DestroyGameObject();
            _hitSoundPool.DestroyAll();
            _visualEffectPool.DestroyAll();
        }

        private Vector3 GenerateRandomPoint()
        {
            Vector3 distance = Utilities.GenerateRandomDirection() * Random.Range(Constants.One, CurrentStats.Get(FloatStatType.ThrowDistance));
            Vector3 point = _launchPoint.position + distance;

            return point;
        }

        protected override void OnStatsUpdate()
        {
            _bombPool.ForEach(bomb => bomb.SetStats(CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(FloatStatType.Range)));
        }
    }
}
