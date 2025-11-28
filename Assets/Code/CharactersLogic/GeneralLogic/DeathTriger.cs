using Assets.Code.Data.Base;
using Assets.Code.LootSystem;
using Assets.Code.Tools.Base;
using UnityEngine;

namespace Assets.Code.CharactersLogic.GeneralLogic
{
    public class DeathTriger : MonoBehaviour
    {
        private Health _health;
        private LootSpawner _lootSpawner;
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

        public void Initialize(
            Health health,
            Mover mover,
            Rotator rotator,
            LootSpawner lootSpawner,
            LootDropInfo[] loots)
        {
            _health = health.ThrowIfNull();
            _lootSpawner = lootSpawner.ThrowIfNull();
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

                _lootSpawner.Spawn(lootConfig.Type, transform.position, lootConfig.Value);
            }

            this.SetActive(false);
            _mover.Stop();
            _rotator.Stop();
        }
    }
}
