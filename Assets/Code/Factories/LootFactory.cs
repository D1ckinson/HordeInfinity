using Assets.Code.Loot;
using Assets.Code.Tools;
using Assets.Scripts.Tools;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Factories
{
    public class LootFactory
    {
        private const float SpawnOffset = 2;
        private const float LootAirTime = 0.5f;
        private const float JumpPower = 1f;

        private readonly Dictionary<LootType, Pool<Loot>> _pools;

        public LootFactory(Loot[] loots)
        {
            loots.ThrowIfNullOrEmpty();

            _pools = new();
            loots.ForEach(loot => _pools.Add(loot.Type, new(() => Create(loot))));
        }

        public void Spawn(LootType type, Vector3 position, int count = 1)
        {
            count.ThrowIfZeroOrLess();

            for (int i = Constants.Zero; i < count; i++)
            {
                Loot loot = _pools[type].Get();
                loot.transform.SetPositionAndRotation(position, GenerateRotation());

                Vector3 targetPosition = position + GenerateOffset();
                loot.transform.DOJump(targetPosition, JumpPower, Constants.One, LootAirTime).SetEase(Ease.Linear);
            }
        }

        public void DisableAll()
        {
            _pools.ForEachValues(pool => pool.DisableAll());
        }

        private Loot Create(Loot loot)
        {
            return Object.Instantiate(loot);
        }

        private Vector3 GenerateOffset()
        {
            Vector3 offset = new()
            {
                x = Random.Range(-SpawnOffset, SpawnOffset),
                z = Random.Range(-SpawnOffset, SpawnOffset)
            };

            return offset;
        }

        private Quaternion GenerateRotation()
        {
            return Quaternion.Euler(Constants.Zero, Random.Range(Constants.Zero, Constants.FullCircleDegrees), Constants.Zero);
        }
    }
}
