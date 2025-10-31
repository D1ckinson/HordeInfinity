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

        public Shuriken(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter,
            ITimeService timeService, BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _heroCenter = heroCenter.ThrowIfNull();
            config.ThrowIfNull();
            _hitSoundPool = new(() => config.HitSound.Instantiate());
            _projectilePool = new(CreateShurikenProjectile);

            ShurikenProjectile CreateShurikenProjectile()
            {
                ShurikenProjectile projectile = config.ProjectilePrefab
                    .Instantiate(false)
                    .GetComponentOrThrow<ShurikenProjectile>()
                    .Initialize(config.DamageLayer, CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(FloatStatType.Range),
                    CurrentStats.Get(IntStatType.BouncesQuantity),
                    _hitSoundPool, timeService);

                projectile.Hit += RecordHitResult;

                return projectile;
            }
        }

        public override void Dispose()
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
            _projectilePool.ForEach(projectile => projectile.SetStats(CurrentStats.Get(FloatStatType.Damage),
                CurrentStats.Get(FloatStatType.Range), CurrentStats.Get(IntStatType.BouncesQuantity)));
        }
    }
}
