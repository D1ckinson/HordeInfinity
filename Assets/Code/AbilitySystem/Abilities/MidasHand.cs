using Assets.Code.AbilitySystem.Base;
using Assets.Code.AbilitySystem.StatTypes;
using Assets.Code.CharactersLogic.GeneralLogic;
using Assets.Code.Data.Base;
using Assets.Code.LootSystem.Legacy;
using Assets.Code.Tools.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class MidasHand : Ability
    {
        private const int MaxCoins = 100;

        private readonly LayerMask _damageLayer;
        private readonly Collider[] _colliders = new Collider[50];
        private readonly Transform _heroCenter;
        private readonly LootFactory _lootFactory;
        private readonly AudioSource _hitSound;

        public MidasHand(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter,
            LootFactory lootFactory, BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _damageLayer = config.DamageLayer.ThrowIfNull();
            _heroCenter = heroCenter.ThrowIfNull();
            _lootFactory = lootFactory.ThrowIfNull();

            _hitSound = config.HitSound.Instantiate(heroCenter);
        }

        protected override void OnDispose()
        {
            _hitSound.DestroyGameObject();
        }

        protected override void Apply()
        {
            int count = Physics.OverlapSphereNonAlloc(_heroCenter.position, CurrentStats.Get(FloatStatType.Range), _colliders, _damageLayer);
            float distance = float.MaxValue;
            Collider closest = null;

            for (int i = Constants.Zero; i < count; i++)
            {
                Collider collider = _colliders[i];
                float sqrDistance = (collider.transform.position - _heroCenter.position).sqrMagnitude;

                if (sqrDistance < distance)
                {
                    closest = collider;
                    distance = sqrDistance;
                }
            }

            if (closest.IsNotNull() && closest.TryGetComponent(out Health health))
            {
                RecordHitResult(health.TakeDamage(CurrentStats.Get(FloatStatType.Damage)));

                float floatPercent = CurrentStats.Get(FloatStatType.HealthPercent) / Constants.Hundred;
                int coinsCount = (int)(health.MaxValue * floatPercent).Clamp(Constants.One, MaxCoins);

                _lootFactory.Spawn(LootType.LowCoin, closest.transform.position, coinsCount);
                _hitSound.Play();
            }
        }

        protected override void OnStatsUpdate() { }
    }
}
