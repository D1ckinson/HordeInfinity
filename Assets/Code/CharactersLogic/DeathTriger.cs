using Assets.Code.Tools;
using Assets.Scripts.Configs;
using Assets.Scripts.Factories;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Code.CharactersLogic
{
    public class DeathTriger : MonoBehaviour
    {
        private Health _health;
        private LootFactory _lootFactory;
        private LootDropInfo[] _loots;
        private CharacterMovement _characterMovement;

        private void OnEnable()
        {
            if (_health.NotNull())
            {
                _health.Died += OnDeath;
            }
        }

        private void OnDisable()
        {
            if (_health.NotNull())
            {
                _health.Died -= OnDeath;
            }
        }

        public void Initialize(Health health, LootFactory lootFactory, LootDropInfo[] loots, CharacterMovement characterMovement)
        {
            _health = health.ThrowIfNull();
            _lootFactory = lootFactory.ThrowIfNull();
            _loots = loots.ThrowIfNullOrEmpty();
            _characterMovement = characterMovement.ThrowIfNull();

            _health.Died += OnDeath;
        }

        private void OnDeath(Health health)
        {
            foreach (LootDropInfo lootConfig in _loots)
            {
                if (Random.Range(Constants.Zero, Constants.Hundred) > lootConfig.DropChance)
                {
                    continue;
                }

                _lootFactory.Spawn(lootConfig.Type, transform.position, lootConfig.Count);
            }

            this.SetActive(false);
            _characterMovement.Stop();
        }
    }
}
