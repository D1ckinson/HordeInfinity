using Assets.Code.Tools;
using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class HolyGround : Ability
    {
        private readonly HolyRune _holyRune;

        public HolyGround(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform hero,
            ITimeService timeService, BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _holyRune = config.ThrowIfNull()
                .ProjectilePrefab
                .GetComponentOrThrow<HolyRune>()
                .Instantiate()
                .Initialize(CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(FloatStatType.Range), config.DamageLayer, hero, timeService);

            _holyRune.Hit += RecordHitResult;
        }

        protected override void Apply()
        {
            _holyRune.DealDamage();
        }

        protected override void OnDispose()
        {
            _holyRune.Hit -= RecordHitResult;
            _holyRune.DestroyGameObject();
        }

        protected override void OnStatsUpdate()
        {
            _holyRune.SetStats(CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(FloatStatType.Range));
        }
    }
}
