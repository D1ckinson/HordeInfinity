using Assets.Code.AbilitySystem.Base;
using Assets.Code.AbilitySystem.Projectiles;
using Assets.Code.AbilitySystem.StatTypes;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class WindFlow : Ability
    {
        private readonly Pool<WindWall> _pool;
        private readonly Transform _hero;

        public WindFlow(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform hero,
            BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _hero = hero.ThrowIfNull();
            _pool = new(CreateWindWall);

            WindWall CreateWindWall()
            {
                WindWall wall = config.ProjectilePrefab
                    .Instantiate()
                    .GetComponentOrThrow<WindWall>()
                    .Initialize(config.DamageLayer, CurrentStats.Get(FloatStatType.Damage));

                wall.Hit += RecordHitResult;

                return wall;
            }
        }

        protected override void OnDispose()
        {
            _pool.ForEach(wall => wall.Hit -= RecordHitResult);
            _pool.DestroyAll();
        }

        protected override void Apply()
        {
            float angleStep = Constants.FullCircleDegrees / CurrentStats.Get(IntStatType.ProjectilesCount);

            for (int i = Constants.Zero; i < CurrentStats.Get(IntStatType.ProjectilesCount); i++)
            {
                WindWall wall = _pool.Get();

                float angle = i * angleStep;
                Vector3 direction = Quaternion.Euler(Constants.Zero, angle, Constants.Zero) * Vector3.forward;

                wall.transform.position = _hero.position;
                wall.transform.rotation = Quaternion.LookRotation(direction);

                wall.Launch();
            }
        }

        protected override void OnStatsUpdate()
        {
            _pool.ForEach(wall => wall.SetDamage(CurrentStats.Get(FloatStatType.Damage)));
        }
    }
}
