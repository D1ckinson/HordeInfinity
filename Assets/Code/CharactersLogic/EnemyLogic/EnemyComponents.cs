using Assets.Code.CharactersLogic.Movement;
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
        public NewMover Mover { get; private set; }
        public NewRotator Rotator { get; private set; }
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

            Mover = new(directionSource, Rigidbody, GetComponent<Animator>(), config.MoveSpeed);
            Rotator = new(directionSource, Rigidbody, config.RotationSpeed);

            Health.Initialize(config.MaxHealth, config.InvincibilityDuration, config.InvincibilityTriggerPercent);
            CollisionDamage.Initialize(config.Damage, config.DamageLayer);
            DeathTriger.Initialize(Health, Mover, Rotator, lootFactory, config.Loot);
            Booster.Initialize(Mover, Health);
            CharacterType = config.Type;
        }
    }
}
