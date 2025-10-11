using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Bombard : Ability
    {
        private const float MaxThrowDistance = 15f;

        private readonly WaitForSeconds _delay = new(0.1f);
        private readonly Pool<Bomb> _bombPool;
        private readonly Pool<ParticleSystem> _visualEffectPool;
        private readonly Pool<AudioSource> _hitSoundPool;
        private readonly Transform _launchPoint;
        private readonly AudioSource _throwSound;

        private float _damage;
        private float _explosionRadius;
        private float _projectilesCount;

        public Bombard(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform launchPoint, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);
            _launchPoint = launchPoint.ThrowIfNull();

            _damage = stats.Damage;
            _projectilesCount = stats.ProjectilesCount;
            _explosionRadius = stats.Range;

            _visualEffectPool = new(() => config.Effect.Instantiate());
            _hitSoundPool = new(() => config.HitSound.Instantiate());
            _bombPool = new(CreateBomb);
            _throwSound = config.ThrowSound.Instantiate();

            Bomb CreateBomb()
            {
                Bomb bomb = config.ProjectilePrefab.GetComponentOrThrow<Bomb>().Instantiate();
                bomb.Initialize(_damage, _explosionRadius, config.DamageLayer, _visualEffectPool, _hitSoundPool);

                return bomb;
            }
        }

        protected override void Apply()
        {
            CoroutineService.StartCoroutine(LaunchBombs(), this);
        }

        private IEnumerator LaunchBombs()
        {
            for (int i = Constants.Zero; i < _projectilesCount; i++)
            {
                Bomb bomb = _bombPool.Get();
                bomb.Fly(_launchPoint.position, GenerateRandomPoint());
                _throwSound.PlayRandomPitch();

                yield return _delay;
            }
        }

        public override void Dispose()
        {
            CoroutineService.StopAllCoroutines(this);
            _throwSound.DestroyGameObject();

            _bombPool.DestroyAll();
            _hitSoundPool.DestroyAll();
            _visualEffectPool.DestroyAll();
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();

            _damage = stats.Damage;
            _projectilesCount = stats.ProjectilesCount;
            _explosionRadius = stats.Range;

            _bombPool.ForEach(bomb => bomb.SetStats(_damage, _explosionRadius));
        }

        private Vector3 GenerateRandomPoint()
        {
            Vector3 distance = Utilities.GenerateRandomDirection() * Random.Range(Constants.One, MaxThrowDistance);
            Vector3 point = _launchPoint.position + distance;

            return point;
        }
    }
}
