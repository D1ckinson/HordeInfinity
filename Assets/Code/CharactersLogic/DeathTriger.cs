using Assets.Code.CharactersLogic.Movement;
using Assets.Code.Tools;
using Assets.Scripts.Configs;
using Assets.Scripts.Factories;
using UnityEngine;

namespace Assets.Code.CharactersLogic
{
    public class DeathTriger : MonoBehaviour
    {
        private Health _health;
        private LootFactory _lootFactory;
        private LootDropInfo[] _loots;
        private Mover _mover;
        private Rotator _rotator;

        private void OnEnable()
        {
            if (_health.IsNotNull())
            {
                _health.Died += OnDeath;
            }
        }

        private void OnDisable()
        {
            if (_health.IsNotNull())
            {
                _health.Died -= OnDeath;
            }
        }

        public void Initialize(Health health, Mover mover, Rotator rotator, LootFactory lootFactory, LootDropInfo[] loots)
        {
            _health = health.ThrowIfNull();
            _lootFactory = lootFactory.ThrowIfNull();
            _loots = loots.ThrowIfNullOrEmpty();
            _mover = mover.ThrowIfNull();
            _rotator = rotator.ThrowIfNull();

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
            _mover.Stop();
            _rotator.Stop();
        }
    }
}
