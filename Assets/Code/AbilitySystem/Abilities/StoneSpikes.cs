using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class StoneSpikes : Ability
    {
        private readonly Pool<Spike> _pool;
        private readonly Transform _hero;
        private readonly AudioSource _sound;

        public StoneSpikes(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform hero,
            BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _hero = hero.ThrowIfNull();
            _sound = config.ThrowIfNull().AppearingSound.Instantiate(_hero);
            _pool = new(CreateSpike);

            Spike CreateSpike()
            {
                Spike spike = config.ProjectilePrefab
                    .Instantiate()
                    .GetComponentOrThrow<Spike>()
                    .Initialize(config.DamageLayer, CurrentStats.Get(FloatStatType.Damage));

                spike.Hit += RecordHitResult;

                return spike;
            }
        }

        public override void Dispose()
        {
            _pool.ForEach(spike => spike.Hit -= RecordHitResult);
            _pool.DestroyAll();
        }

        protected override void Apply()
        {
            for (int i = Constants.Zero; i < CurrentStats.Get(IntStatType.ProjectilesCount); i++)
            {
                Vector3 position = _hero.position + Utilities.GenerateRandomDirection() * Random.Range(Constants.One, CurrentStats.Get(FloatStatType.Range));

                _pool.Get().Strike(position);
            }

            _sound.PlayRandomPitch();
        }

        protected override void OnStatsUpdate()
        {
            _pool.ForEach(spike => spike.SetDamage(CurrentStats.Get(FloatStatType.Damage)));
        }
    }
}
