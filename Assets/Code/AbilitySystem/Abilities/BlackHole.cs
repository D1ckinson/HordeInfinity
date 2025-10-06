using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class BlackHole : Ability
    {
        private readonly BlackHoleProjectile _projectile;
        private readonly Transform _blackHolePoint;
        private readonly Pool<ParticleSystem> _effectPool;

        public BlackHole(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform blackHolePoint, int level = 1) : base(config,  abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);
            _blackHolePoint = blackHolePoint.ThrowIfNull();

            _effectPool = new(() => config.Effect.Instantiate(), Constants.One);

            _projectile = config.ProjectilePrefab
                .GetComponentOrThrow<BlackHoleProjectile>()
                .Instantiate(false)
                .Initialize(config.DamageLayer, stats.Damage, stats.Range, stats.PullForce, _effectPool);
        }

        public override void Dispose()
        {
            _projectile.DestroyGameObject();
            _effectPool.DestroyAll();
        }

        protected override void Apply()
        {
            _projectile.Activate(_blackHolePoint.position);
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();
            _projectile.SetStats(stats.Damage, stats.Range, stats.PullForce);
        }
    }
}
