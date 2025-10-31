using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Fireball : Ability
    {
        private const float SearchRadius = 100f;

        private readonly Transform _heroCenter;
        private readonly Pool<FireballProjectile> _projectilePool;
        private readonly Pool<ParticleSystem> _explosionEffectPool;
        private readonly Pool<AudioSource> _explosionSoundPool;
        private readonly AudioSource _throwSound;
        private readonly Collider[] _colliders = new Collider[20];
        private readonly LayerMask _damageLayer;

        public Fireball(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter,
            ITimeService timeService, BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _heroCenter = heroCenter.ThrowIfNull();
            _damageLayer = config.ThrowIfNull().DamageLayer;

            _throwSound = config.ThrowSound.Instantiate(heroCenter.transform);
            _explosionEffectPool = new(() => config.Effect.Instantiate(), Constants.One);
            _explosionSoundPool = new(() => config.HitSound.Instantiate(), Constants.One);
            _projectilePool = new(CreateProjectile, Constants.One);

            FireballProjectile CreateProjectile()
            {
                FireballProjectile projectile = config
                    .ProjectilePrefab
                    .Instantiate()
                    .GetComponentOrThrow<FireballProjectile>()
                    .Initialize(_damageLayer, CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(FloatStatType.Range),
                    _explosionEffectPool, _explosionSoundPool, timeService);

                projectile.Hit += RecordHitResult;

                return projectile;
            }
        }

        public override void Dispose()
        {
            _projectilePool.ForEach(projectile => projectile.Hit -= RecordHitResult);
            _projectilePool.DestroyAll();

            _explosionEffectPool.DestroyAll();
            _explosionSoundPool.DestroyAll();
            _throwSound.DestroyGameObject();
        }

        protected override void Apply()
        {
            int count = Physics.OverlapSphereNonAlloc(_heroCenter.position, SearchRadius, _colliders, _damageLayer);

            Collider closest = null;
            float distance = float.MaxValue;

            for (int i = Constants.Zero; i < count; i++)
            {
                Collider collider = _colliders[i];
                float sqrMagnitude = (collider.transform.position - _heroCenter.transform.position).sqrMagnitude;

                if (sqrMagnitude < distance)
                {
                    closest = collider;
                    distance = sqrMagnitude;
                }
            }

            Vector3 direction;

            if (closest.IsNull())
            {
                direction = Utilities.GenerateRandomDirection();
            }
            else
            {
                direction = closest.transform.position - _heroCenter.transform.position;
                direction.y = _heroCenter.position.y;
                direction.Normalize();
            }

            _projectilePool.Get().Fly(_heroCenter.position, direction);
            _throwSound.Play();
        }

        protected override void OnStatsUpdate()
        {
            _projectilePool.ForEach(projectile => projectile.SetStats(CurrentStats.Get(FloatStatType.Damage),
                CurrentStats.Get(FloatStatType.Range)));
        }
    }
}
