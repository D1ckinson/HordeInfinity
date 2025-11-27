using Assets.Code.Data.Base;
using Assets.Code.Data.Value;
using Assets.Code.LootSystem.Legacy;
using Assets.Code.Tools.Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.CharactersLogic.HeroLogic
{
    [RequireComponent(typeof(SphereCollider))]
    public class LootCollector : MonoBehaviour
    {
        private const float CollectDistance = 1f;
        private const float ExperienceTransferDelay = 0.15f;

        private readonly List<Loot> _loots = new();

        private float _time;
        private float _pullSpeed;
        private SphereCollider _collectArea;
        private IWalletService _wallet;
        private HeroLevel _heroLevel;

        public LootAffecter LootAffecter { get; } = new();
        public IValueContainer AttractionRadius { get; private set; }
        public float CollectedGold { get; private set; }

        public event Action<int> GoldValueChanged;

        private void Awake()
        {
            _collectArea = GetComponent<SphereCollider>();
            _collectArea.isTrigger = true;
        }

        private void OnDestroy()
        {
            if (AttractionRadius.IsNotNull())
            {
                AttractionRadius.ValueChanged -= SetRadius;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Loot loot) == false || _loots.Contains(loot))
            {
                return;
            }

            _loots.Add(loot);
        }

        public void Initialize(
            IValueContainer attractionRadius,
            float pullSpeed,
            IWalletService wallet,
            HeroLevel heroLevel)
        {
            AttractionRadius = attractionRadius.ThrowIfNull();
            _pullSpeed = pullSpeed.ThrowIfZeroOrLess();
            _wallet = wallet.ThrowIfNull();
            _heroLevel = heroLevel.ThrowIfNull();

            _collectArea.radius = AttractionRadius.Value;
            AttractionRadius.ValueChanged += SetRadius;
        }

        public void Run()
        {
            UpdateService.RegisterUpdate(TransferExperience);
            UpdateService.RegisterFixedUpdate(AttractLoot);
        }

        public void Stop()
        {
            UpdateService.UnregisterUpdate(TransferExperience);
            UpdateService.UnregisterFixedUpdate(AttractLoot);

            _loots.ForEach(loot => loot.Rigidbody.velocity = Vector3.zero);
        }

        public void Reset()
        {
            Stop();
            _loots.Clear();
            LootAffecter.Reset();
            AttractionRadius.Reset();
        }

        public void TransferGold()
        {
            if (CollectedGold > Constants.Zero)
            {
                _wallet.Add((int)CollectedGold);

                CollectedGold = Constants.Zero;
                GoldValueChanged?.Invoke((int)CollectedGold);
            }
        }

        private void TransferExperience(float deltaTime)
        {
            if (_heroLevel.IsNull())
            {
                return;
            }

            _time += Time.deltaTime;

            if (_time > ExperienceTransferDelay)
            {
                _heroLevel.Transfer();
                _time = Constants.Zero;
            }
        }

        private void AttractLoot(float deltaTime)
        {
            for (int i = _loots.LastIndex(); i >= Constants.Zero; i--)
            {
                Loot loot = _loots[i];

                Vector3 distance = transform.position - loot.transform.position;
                Vector3 rawDirection = distance / (distance.magnitude + Constants.One);

                loot.Rigidbody.velocity = rawDirection * _pullSpeed;

                if (distance.sqrMagnitude <= CollectDistance)
                {
                    int collectValue = loot.Collect();
                    CollectValue(loot.Type, collectValue);
                    _loots.Remove(loot);
                }
            }
        }

        private void CollectValue(LootType type, int collectValue)
        {
            collectValue.ThrowIfNegative();

            switch (type)
            {
                case LootType.LowExperience:
                case LootType.MediumExperience:
                case LootType.HighExperience:
                    _heroLevel.Add(LootAffecter.Affect(collectValue, type));
                    break;

                case LootType.LowCoin:
                case LootType.MediumCoin:
                case LootType.HighCoin:
                    CollectGold(collectValue);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void SetRadius(ValueContainer container)
        {
            _collectArea.radius = container.Value;
        }

        private void CollectGold(int collectValue)
        {
            CollectedGold += LootAffecter.Affect(collectValue, LootType.LowCoin);
            GoldValueChanged?.Invoke((int)CollectedGold);
        }
    }
}
