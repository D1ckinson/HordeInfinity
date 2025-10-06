using Assets.Code.AbilitySystem;
using Assets.Code.Animation;
using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Code.CharactersLogic.HeroLogic
{
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(LootCollector))]
    [RequireComponent(typeof(AbilityContainer))]
    [RequireComponent(typeof(Animator))]
    public class HeroComponents : MonoBehaviour
    {
        [field: SerializeField] public Transform Center { get; private set; }

        public CharacterMovement CharacterMovement { get; private set; }
        public Health Health { get; private set; }
        public LootCollector LootCollector { get; private set; }
        public AbilityContainer AbilityContainer { get; private set; }
        public HeroLevel HeroLevel { get; private set; }
        public Animator Animator { get; private set; }

        private Vector3 _defaultPosition;

        private void Awake()
        {
            CharacterMovement = GetComponent<CharacterMovement>();
            Health = GetComponent<Health>();
            LootCollector = GetComponent<LootCollector>();
            AbilityContainer = GetComponent<AbilityContainer>();
            Animator = GetComponent<Animator>();
        }

        public void Initialize(HeroLevel heroLevel, Vector3 defaultPosition)
        {
            HeroLevel = heroLevel.ThrowIfNull();
            _defaultPosition = defaultPosition;
        }

        public void SetDefaultPosition()
        {
            transform.SetPositionAndRotation(_defaultPosition, Quaternion.identity);
        }
    }
}
