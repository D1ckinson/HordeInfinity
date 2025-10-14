using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class WindFlow : Ability
    {
        private readonly Pool<WindWall> _pool;
        private readonly Transform _hero;

        private float _damage;
        private int _projectilesCount;

        public WindFlow(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform hero,
            Dictionary<AbilityType, float> damageDealt, Dictionary<AbilityType, int> killCount, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);
            _hero = hero.ThrowIfNull();

            _damage = stats.Damage;
            _projectilesCount = stats.ProjectilesCount;

            _pool = new(CreateWindWall);

            WindWall CreateWindWall()
            {
                WindWall wall = config.ProjectilePrefab.Instantiate().GetComponentOrThrow<WindWall>();
                wall.Initialize(config.DamageLayer, _damage, damageDealt, killCount);

                return wall;
            }
        }

        public override void Dispose()
        {
            _pool.DestroyAll();
        }

        protected override void Apply()
        {
            float angleStep = Constants.FullCircleDegrees / _projectilesCount;

            for (int i = Constants.Zero; i < _projectilesCount; i++)
            {
                WindWall wall = _pool.Get();

                float angle = i * angleStep;
                Vector3 direction = Quaternion.Euler(Constants.Zero, angle, Constants.Zero) * Vector3.forward;

                wall.transform.position = _hero.position;
                wall.transform.rotation = Quaternion.LookRotation(direction);

                wall.Launch();
            }
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();
            _damage = stats.Damage;
            _projectilesCount = stats.ProjectilesCount;

            _pool.ForEach(wall => wall.SetDamage(_damage));
        }
    }
}
