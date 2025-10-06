using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Shuriken : Ability
    {
        private readonly Transform _heroCenter;
        private readonly Pool<ShurikenProjectile> _pool;

        private float _damage;
        private float _range;
        private float _projectilesCount;
        private int _bouncesQuantity;

        public Shuriken(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);
            _heroCenter = heroCenter.ThrowIfNull();

            _damage = stats.Damage;
            _range = stats.Range;
            _projectilesCount = stats.ProjectilesCount;
            _bouncesQuantity = stats.BouncesQuantity;

            _pool = new(CreateShurikenProjectile);

            ShurikenProjectile CreateShurikenProjectile()
            {
                ShurikenProjectile projectile = config.ProjectilePrefab.Instantiate(false).GetComponentOrThrow<ShurikenProjectile>();
                projectile.Initialize(config.DamageLayer, _damage, _range, _bouncesQuantity);

                return projectile;
            }
        }

        public override void Dispose()
        {
            _pool.DestroyAll();
        }

        protected override void Apply()
        {
            for (int i = Constants.Zero; i < _projectilesCount; i++)
            {
                _pool.Get().Launch(_heroCenter.position, Utilities.GenerateRandomDirection());
            }
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();

            _damage = stats.Damage;
            _range = stats.Range;
            _projectilesCount = stats.ProjectilesCount;
            _bouncesQuantity = stats.BouncesQuantity;

            _pool.ForEach(projectile => projectile.SetStats(_damage, _range, _bouncesQuantity));
        }
    }
}
