using Assets.Code.AbilitySystem;
using Assets.Code.CharactersLogic.Movement;
using Assets.Code.Tools;
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

        private Vector3 _defaultPosition;

        public Health Health { get; private set; }
        public LootCollector LootCollector { get; private set; }
        public AbilityContainer AbilityContainer { get; } = new();
        public HeroLevel HeroLevel { get; private set; }
        public Animator Animator { get; private set; }
        public NewMover Mover { get; private set; }
        public NewRotator Rotator { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            LootCollector = GetComponent<LootCollector>();
            Animator = GetComponent<Animator>();

            _defaultPosition = transform.position;
        }

        public HeroComponents Initialize(ITellDirection directionSource, CharacterConfig config, HeroLevel heroLevel, Wallet wallet)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            HeroLevel = heroLevel.ThrowIfNull();

            Mover = new(directionSource, rigidbody, Animator, config.MoveSpeed);
            Rotator = new(directionSource, rigidbody, config.RotationSpeed);

            Health.Initialize(config.MaxHealth, config.InvincibilityDuration, config.InvincibilityTriggerPercent);
            LootCollector.Initialize(config.AttractionRadius, config.PullSpeed, wallet, heroLevel);

            return this;
        }

        public void SetDefaultPosition()
        {
            transform.SetPositionAndRotation(_defaultPosition, Quaternion.identity);
        }
    }
}
