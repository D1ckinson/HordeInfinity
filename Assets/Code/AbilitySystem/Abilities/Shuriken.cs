using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Shuriken : Ability
    {
        private readonly Transform _heroCenter;
        private readonly Pool<ShurikenProjectile> _projectilePool;
        private readonly Pool<AudioSource> _hitSoundPool;

        private float _damage;
        private float _range;
        private float _projectilesCount;
        private int _bouncesQuantity;

        public Shuriken(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter,
            ITimeService timeService, Dictionary<AbilityType, float> damageDealt, Dictionary<AbilityType, int> killCount, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);
            _heroCenter = heroCenter.ThrowIfNull();

            _damage = stats.Damage;
            _range = stats.Range;
            _projectilesCount = stats.ProjectilesCount;
            _bouncesQuantity = stats.BouncesQuantity;

            _hitSoundPool = new(() => config.HitSound.Instantiate());
            _projectilePool = new(CreateShurikenProjectile);

            ShurikenProjectile CreateShurikenProjectile()
            {
                ShurikenProjectile projectile = config.ProjectilePrefab.Instantiate(false).GetComponentOrThrow<ShurikenProjectile>();
                projectile.Initialize(config.DamageLayer, _damage, _range, _bouncesQuantity, _hitSoundPool, timeService, damageDealt, killCount);

                return projectile;
            }
        }

        public override void Dispose()
        {
            _projectilePool.DestroyAll();
            _hitSoundPool.DestroyAll();
        }

        protected override void Apply()
        {
            for (int i = Constants.Zero; i < _projectilesCount; i++)
            {
                _projectilePool.Get().Launch(_heroCenter.position, Utilities.GenerateRandomDirection());
            }
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();

            _damage = stats.Damage;
            _range = stats.Range;
            _projectilesCount = stats.ProjectilesCount;
            _bouncesQuantity = stats.BouncesQuantity;

            _projectilePool.ForEach(projectile => projectile.SetStats(_damage, _range, _bouncesQuantity));
        }
    }
}
