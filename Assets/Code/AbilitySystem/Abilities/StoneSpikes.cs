using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class StoneSpikes : Ability
    {
        private const float MinRadius = 1f;

        private readonly Pool<Spike> _pool;

        private float _damage;
        private float _maxRadius;
        private int _projectilesCount;

        public StoneSpikes(AbilityConfig config, Transform transform, Dictionary<AbilityType, int> abilityUnlockLevel, int level = 1) : base(config, transform, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);

            _damage = stats.Damage;
            _projectilesCount = stats.ProjectilesCount;
            _maxRadius = stats.Range;

            _pool = new(CreateSpike);

            Spike CreateSpike()
            {
                Spike spike = config.ProjectilePrefab.Instantiate().GetComponentOrThrow<Spike>();
                spike.Initialize(config.DamageLayer, _damage);

                return spike;
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
                Vector3 position = Position + Utilities.GenerateRandomDirection() * Random.Range(MinRadius, _maxRadius);

                _pool.Get().Strike(position);
            }
        }

        protected override void UpdateStats(float damage, float range, int projectilesCount, bool isPiercing, int healthPercent, float pullForce)
        {
            _damage = damage.ThrowIfNegative();
            _maxRadius = range.ThrowIfNegative();
            _projectilesCount = projectilesCount.ThrowIfNegative();
        }
    }
}
