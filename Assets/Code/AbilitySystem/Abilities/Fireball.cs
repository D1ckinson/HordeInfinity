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

        private float _damage;
        private float _explosionRadius;

        public Fireball(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter, ITimeService timeService, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);
            _heroCenter = heroCenter.ThrowIfNull();

            _damage = stats.Damage;
            _explosionRadius = stats.Range;
            _damageLayer = config.DamageLayer;

            _throwSound = config.ThrowSound.Instantiate(heroCenter.transform);
            _explosionEffectPool = new(() => config.Effect.Instantiate(), Constants.One);
            _explosionSoundPool = new(() => config.HitSound.Instantiate(), Constants.One);
            _projectilePool = new(CreateProjectile, Constants.One);

            FireballProjectile CreateProjectile()
            {
                FireballProjectile projectile = config.ProjectilePrefab.Instantiate().GetComponentOrThrow<FireballProjectile>();
                projectile.Initialize(_damageLayer, _damage, _explosionRadius, _explosionEffectPool, _explosionSoundPool, timeService);

                return projectile;
            }
        }

        public override void Dispose()
        {
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

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();

            _damage = stats.Damage;
            _explosionRadius = stats.Range;

            _projectilePool.ForEach(projectile => projectile.SetStats(_damage, _explosionRadius));
        }
    }
}
