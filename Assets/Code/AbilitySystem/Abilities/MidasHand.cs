using Assets.Code.CharactersLogic;
using Assets.Code.Loot;
using Assets.Code.Tools;
using Assets.Scripts.Factories;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class MidasHand : Ability
    {
        private readonly LayerMask _damageLayer;
        private readonly Collider[] _colliders = new Collider[50];
        private readonly Transform _heroCenter;
        private readonly LootFactory _lootFactory;
        private readonly AudioSource _hitSound;

        private float _damage;
        private float _range;
        private int _healthPercent;

        public MidasHand(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter, LootFactory lootFactory, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            _damageLayer = config.DamageLayer.ThrowIfNull();
            _heroCenter = heroCenter.ThrowIfNull();
            _lootFactory = lootFactory.ThrowIfNull();

            AbilityStats stats = config.ThrowIfNull().GetStats(level);

            _damage = stats.Damage.ThrowIfNegative();
            _range = stats.Range.ThrowIfNegative();
            _healthPercent = stats.HealthPercent.ThrowIfNegative();
            _hitSound = config.HitSound.Instantiate(heroCenter);
        }

        public override void Dispose()
        {
            _hitSound.DestroyGameObject();
        }

        protected override void Apply()
        {
            int count = Physics.OverlapSphereNonAlloc(_heroCenter.position, _range, _colliders, _damageLayer);
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

            if (closest.NotNull() && closest.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
                float floatPercent = (float)_healthPercent / Constants.Hundred;
                int coinsCount = (int)(health.MaxValue * floatPercent);

                if (coinsCount <= Constants.Zero)
                {
                    coinsCount = Constants.One;
                }

                _lootFactory.Spawn(LootType.Coin, closest.transform.position, coinsCount);
                _hitSound.Play();
            }
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();
            _damage = stats.Damage;
            _range = stats.Range;
            _healthPercent = stats.HealthPercent;
        }
    }
}
