using Assets.Code.AbilitySystem;
using Assets.Code.BuffSystem;
using Assets.Code.CharactersLogic.Movement;
using Assets.Code.Data;
using Assets.Code.Tools;
using Assets.Code.Ui.Buff_View;
using Assets.Scripts;
using Assets.Scripts.Configs;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Code.CharactersLogic.HeroLogic
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(LootCollector))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class HeroComponents : MonoBehaviour
    {
        [field: SerializeField] public Transform Center { get; private set; }
        [field: SerializeField] public BuffView BuffView { get; private set; }

        private Vector3 _defaultPosition;

        public AbilityContainer AbilityContainer { get; } = new();
        public BuffContainer BuffContainer { get; } = new();
        public Health Health { get; private set; }
        public LootCollector LootCollector { get; private set; }
        public HeroLevel HeroLevel { get; private set; }
        public Animator Animator { get; private set; }
        public Mover Mover { get; private set; }
        public Rotator Rotator { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            LootCollector = GetComponent<LootCollector>();
            Animator = GetComponent<Animator>();

            _defaultPosition = transform.position;
        }

        public HeroComponents Initialize(
            ITellDirection directionSource,
            CharacterConfig config,
            HeroLevel heroLevel,
            IWalletService wallet)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            HeroLevel = heroLevel.ThrowIfNull();

            ValueContainer speed = new(config.MoveSpeed);
            Mover = new(directionSource, rigidbody, Animator, speed);
            Rotator = new(directionSource, rigidbody, config.RotationSpeed);

            Regenerator regenerator = new(Health, config.Regeneration);
            Resist resist = new(config.Resist);
            float triggerValue = config.MaxHealth * Constants.PercentToMultiplier(config.InvincibilityTriggerPercent);
            Invincibility invincibility = new(config.InvincibilityDuration, triggerValue);
            Health.Initialize(config.MaxHealth, invincibility, regenerator, resist);

            IValueContainer attractionRadius = new ValueContainer(config.AttractionRadius, Constants.One);
            LootCollector.Initialize(attractionRadius, config.PullSpeed, wallet, heroLevel);

            return this;
        }

        public void SetDefaultPosition()
        {
            transform.SetPositionAndRotation(_defaultPosition, Quaternion.identity);
        }
    }
}
