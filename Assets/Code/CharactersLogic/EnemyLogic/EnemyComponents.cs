using Assets.Code.CharactersLogic.Movement;
using Assets.Code.Data;
using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Configs;
using Assets.Scripts.Factories;
using Assets.Scripts.Movement;
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

        public void Initialize(CharacterConfig config, ITellDirection directionSource, LootFactory lootFactory)
        {
            config.ThrowIfNull();

            ValueContainer speed = new(config.MoveSpeed);
            Mover = new(directionSource, Rigidbody, GetComponent<Animator>(), speed);
            Rotator = new(directionSource, Rigidbody, config.RotationSpeed);

            Regenerator regenerator = new(Health, config.Regeneration);
            ValueContainer resist = new(config.Resist);
            float triggerValue = config.MaxHealth * Constants.PercentToMultiplier(config.InvincibilityTriggerPercent);
            Invincibility invincibility = new(config.InvincibilityDuration, triggerValue);
            Health.Initialize(config.MaxHealth, invincibility, regenerator, resist);

            CollisionDamage.Initialize(config.Damage, config.DamageLayer);
            DeathTriger.Initialize(Health, Mover, Rotator, lootFactory, config.Loot);
            Booster.Initialize(Mover, Health);
            CharacterType = config.Type;
        }
    }
}
