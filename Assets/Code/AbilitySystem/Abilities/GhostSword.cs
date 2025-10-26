using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class GhostSword : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private float _speed;
        [SerializeField] private float _lifeTime;
        [SerializeField] private AudioSource _hitSound;

        private float _damage;
        private bool _isPiercing;
        private LayerMask _damageLayer;

        private Dictionary<AbilityType, int> _damageDealt;
        private Dictionary<AbilityType, int> _killCount;

        private void Awake()
        {
            _collider.isTrigger = true;
            _collider.enabled = false;
        }

        private void OnDestroy()
        {
            Stop();
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject gameObject = other.ThrowIfNull().gameObject;

            if (_damageLayer.Contains(gameObject.layer) && gameObject.TryGetComponent(out Health health))
            {
                _damageDealt[AbilityType.GhostSwords] += (int)_damage;

                if (health.TakeDamage(_damage))
                {
                    _killCount[AbilityType.GhostSwords]++;
                }

                _hitSound.Play();

                if (_isPiercing == false)
                {
                    Stop();
                }
            }
        }

        public void Initialize(float damage, bool isPiercing, LayerMask damageLayer, Dictionary<AbilityType, int> damageDealt, Dictionary<AbilityType, int> killCount)
        {
            SetStats(damage, isPiercing);
            _damageLayer = damageLayer.ThrowIfNull();

            _damageDealt = damageDealt.ThrowIfNull();
            _killCount = killCount.ThrowIfNull();
        }

        public void SetStats(float damage, bool isPiercing)
        {
            _damage = damage.ThrowIfNegative();
            _isPiercing = isPiercing;
        }

        public void Launch()
        {
            _collider.enabled = true;
            transform.SetParent(null);

            UpdateService.RegisterUpdate(Fly);
            TimerService.StartTimer(_lifeTime, Stop);
        }

        private void Stop()
        {
            UpdateService.UnregisterUpdate(Fly);
            TimerService.StopTimer(_lifeTime, Stop);

            _collider.enabled = false;
            this.SetActive(false);
        }

        private void Fly()
        {
            transform.Translate(_speed * Time.deltaTime * Vector3.forward);
        }
    }
}
