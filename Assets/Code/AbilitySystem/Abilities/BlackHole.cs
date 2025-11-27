using Assets.Code.AbilitySystem.Base;
using Assets.Code.AbilitySystem.Projectiles;
using Assets.Code.AbilitySystem.StatTypes;
using Assets.Code.Core;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class BlackHole : Ability
    {
        private readonly BlackHoleProjectile _projectile;
        private readonly Transform _heroCenter;
        private readonly Pool<ParticleSystem> _effectPool;

        public BlackHole(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter,
            ITimeService timeService, BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _heroCenter = heroCenter.ThrowIfNull();
            _effectPool = new(() => config.Effect.Instantiate(), Constants.One);

            AudioSource sound = config.ProjectileSound.Instantiate();
            sound.GetComponentOrThrow<SoundPause>().Initialize(timeService);

            _projectile = config.ProjectilePrefab
                .GetComponentOrThrow<BlackHoleProjectile>()
                .Instantiate(false)
                .Initialize(config.DamageLayer, CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(FloatStatType.Range),
                CurrentStats.Get(FloatStatType.PullForce), _effectPool, sound);

            _projectile.Hit += RecordHitResult;
        }

        protected override void OnDispose()
        {
            _projectile.Hit -= RecordHitResult;
            _projectile.DestroyGameObject();
            _effectPool.DestroyAll();
        }

        protected override void Apply()
        {
            _projectile.Activate(_heroCenter.position);
        }

        protected override void OnStatsUpdate()
        {
            _projectile.SetStats(CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(FloatStatType.Range),
                CurrentStats.Get(FloatStatType.PullForce));
        }
    }
}
