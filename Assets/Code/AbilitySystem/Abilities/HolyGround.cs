using Assets.Code.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class HolyGround : Ability
    {
        private readonly HolyRune _holyRune;

        public HolyGround(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform hero, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            _holyRune = config.ThrowIfNull().ProjectilePrefab.GetComponentOrThrow<HolyRune>().Instantiate();

            AbilityStats stats = config.GetStats(level);
            _holyRune.Initialize(stats.Damage, stats.Range, config.DamageLayer, hero);
        }

        protected override void Apply()
        {
            _holyRune.DealDamage();
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();
            _holyRune.SetStats(stats.Damage, stats.Range);
        }

        public override void Dispose()
        {
            _holyRune.DestroyGameObject();
        }
    }
}
