using Assets.Code.Loot;
using Assets.Code.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
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
        private float _attractionRadius;
        private Wallet _wallet;
        private HeroLevel _heroLevel;

        public float CollectedGold { get; private set; }

        public event Action<int> GoldValueChanged;

        private void Awake()
        {
            _collectArea = GetComponent<SphereCollider>();
            _collectArea.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Loot loot) == false || _loots.Contains(loot))
            {
                return;
            }

            _loots.Add(loot);
        }

        public void Initialize(float attractionRadius, float pullSpeed, Wallet wallet, HeroLevel heroLevel)
        {
            _pullSpeed = pullSpeed.ThrowIfZeroOrLess();
            _attractionRadius = attractionRadius.ThrowIfNegative();
            _wallet = wallet.ThrowIfNull();
            _heroLevel = heroLevel.ThrowIfNull();

            _collectArea.radius = _attractionRadius;
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

        public void AddAttractionRadius(int value)
        {
            _collectArea.radius = _attractionRadius + value.ThrowIfNegative();
        }

        private void TransferExperience()
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

        private void AttractLoot()
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
            switch (type)
            {
                case LootType.LowExperience:
                    _heroLevel.Add(collectValue);
                    break;

                case LootType.MediumExperience:
                    _heroLevel.Add(collectValue);
                    break;

                case LootType.HighExperience:
                    _heroLevel.Add(collectValue);
                    break;

                case LootType.Coin:
                    CollectGold(collectValue);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void CollectGold(int collectValue)
        {
            CollectedGold += collectValue.ThrowIfNegative();
            GoldValueChanged?.Invoke((int)CollectedGold);
        }
    }
}
