using Assets.Code.Tools.Base;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Code.LootSystem.NewSystem
{
    public class LootSpawner
    {
        private readonly Dictionary<NewLootType, LootPoolsContainer> _lootContainers = new();
        private readonly LootSpawnSettings _settings;

        public LootSpawner(NewLoot[] loots, LootSpawnSettings settings)
        {
            loots.ThrowIfNullOrEmpty();

            foreach (NewLoot loot in loots)
            {
                NewLootType type = loot.Type;

                if (_lootContainers.ContainsKey(type) == false)
                {
                    _lootContainers.Add(type, new());
                }

                _lootContainers[type].AddLoot(loot);
            }

            _settings = settings;
        }

        public void Spawn(NewLootType type, Vector3 position, int value = 1)
        {
            List<NewLoot> loots = _lootContainers[type].Get(value.ThrowIfNegative());

            foreach (var loot in loots)
            {
                loot.transform.SetPositionAndRotation(position, GenerateRotation());
                Vector3 targetPosition = position + GenerateOffset();

                loot.transform
                    .DOJump(targetPosition, _settings.JumpPower, Constants.One, _settings.LootAirTime)
                    .SetEase(Ease.Linear);
            }
        }

        public void DisableAll()
        {
            foreach (LootPoolsContainer container in _lootContainers.Values)
            {
                container.DisableAll();
            }
        }

        private Vector3 GenerateOffset()
        {
            Vector3 offset = new()
            {
                x = Random.Range(-_settings.MaxOffset, _settings.MaxOffset),
                z = Random.Range(-_settings.MaxOffset, _settings.MaxOffset)
            };

            return offset;
        }

        private Quaternion GenerateRotation()
        {
            return Quaternion.Euler(Constants.Zero, Random.Range(Constants.Zero, Constants.FullCircleDegrees), Constants.Zero);
        }
    }
}
