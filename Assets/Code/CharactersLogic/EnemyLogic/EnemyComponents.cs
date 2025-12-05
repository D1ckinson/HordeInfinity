using Assets.Code.CharactersLogic.GeneralLogic;
using Assets.Code.CharactersLogic.Movement.Interfaces;
using Assets.Code.Core;
using Assets.Code.Data.Base;
using Assets.Code.Data.Value;
using Assets.Code.LootSystem;
using Assets.Code.Tools.Base;
using UnityEngine;

namespace Assets.Code.CharactersLogic.EnemyLogic
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(CollisionDamage))]
    [RequireComponent(typeof(DeathTriger))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(EnemyBooster))]
    public class EnemyComponents : MonoBehaviour
    {
        [field: SerializeField] public Collider Collider { get; private set; }

        public Health Health { get; private set; }
        public CollisionDamage CollisionDamage { get; private set; }
        public DeathTriger DeathTriger { get; private set; }
        public CharacterType CharacterType { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public Mover Mover { get; private set; }
        public Rotator Rotator { get; private set; }
        public EnemyBooster Booster { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            CollisionDamage = GetComponent<CollisionDamage>();
            DeathTriger = GetComponent<DeathTriger>();
            Rigidbody = GetComponent<Rigidbody>();
            Booster = GetComponent<EnemyBooster>();
        }

        public void Initialize(CharacterConfig config, ITellDirection directionSource, LootSpawner lootSpawner)
        {
            config.ThrowIfNull();

            ValueContainer speed = new(config.MoveSpeed);
            Mover = new(directionSource, Rigidbody, GetComponent<Animator>(), speed);
            Rotator = new(directionSource, Rigidbody, config.RotationSpeed);

            Regenerator regenerator = new(Health, config.Regeneration);
            Resist resist = new(config.Resist);

            float triggerValue = config.MaxHealth * Constants.PercentToMultiplier(config.InvincibilityTriggerPercent);
            Invincibility invincibility = new(config.InvincibilityDuration, triggerValue);
            Health.Initialize(config.MaxHealth, invincibility, regenerator, resist);

            CollisionDamage.Initialize(config.Damage, config.DamageLayer);
            DeathTriger.Initialize(Health, Mover, Rotator, lootSpawner, config.Loot);
            Booster.Initialize(Mover, Health);
            CharacterType = config.Type;
        }
    }
}
