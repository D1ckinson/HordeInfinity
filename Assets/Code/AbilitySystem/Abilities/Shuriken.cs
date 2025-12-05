using Assets.Code.AbilitySystem.Base;
using Assets.Code.AbilitySystem.Interfaces;
using Assets.Code.AbilitySystem.Projectiles;
using Assets.Code.AbilitySystem.StatTypes;
using Assets.Code.Core;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Shuriken : Ability
    {
        private readonly Transform _heroCenter;
        private readonly Pool<ShurikenProjectile> _projectilePool;
        private readonly Pool<AudioSource> _hitSoundPool;

        public Shuriken(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter,
            ITimeService timeService, BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _heroCenter = heroCenter.ThrowIfNull();
            config.ThrowIfNull();
            _hitSoundPool = new(() => config.HitSound.Instantiate());
            _projectilePool = new(CreateShurikenProjectile);

            ShurikenProjectile CreateShurikenProjectile()
            {
                ITargetSelector targetSelector = new BounceTargetSelector(config.SearchRadius, config.DamageLayer);

                ShurikenProjectile projectile = config.ProjectilePrefab
                    .Instantiate(false)
                    .GetComponentOrThrow<ShurikenProjectile>()
                    .Initialize(targetSelector, _hitSoundPool, timeService);

                projectile.SetStats(CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(IntStatType.BouncesQuantity));
                projectile.Hit += RecordHitResult;

                return projectile;
            }
        }

        protected override void OnDispose()
        {
            _projectilePool.ForEach(projectile => projectile.Hit -= RecordHitResult);
            _projectilePool.DestroyAll();

            _hitSoundPool.DestroyAll();
        }

        protected override void Apply()
        {
            for (int i = Constants.Zero; i < CurrentStats.Get(IntStatType.ProjectilesCount); i++)
            {
                _projectilePool.Get().Launch(_heroCenter.position, Utilities.GenerateRandomDirection());
            }
        }

        protected override void OnStatsUpdate()
        {
            _projectilePool.ForEach(projectile => projectile.SetStats(CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(IntStatType.BouncesQuantity)));
        }
    }
}
