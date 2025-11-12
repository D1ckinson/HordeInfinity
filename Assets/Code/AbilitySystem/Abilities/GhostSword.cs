using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using System;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class GhostSword : MonoBehaviour, IProjectile
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private float _speed;
        [SerializeField] private float _lifeTime;
        [SerializeField] private AudioSource _hitSound;

        private float _damage;
        private bool _isPiercing;
        private LayerMask _damageLayer;

        public event Action<HitResult> Hit;

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
                Hit?.Invoke(health.TakeDamage(_damage));
                _hitSound.Play();

                if (_isPiercing == false)
                {
                    Stop();
                }
            }
        }

        public GhostSword Initialize(float damage, bool isPiercing, LayerMask damageLayer)
        {
            SetStats(damage, isPiercing);
            _damageLayer = damageLayer.ThrowIfNull();

            return this;
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
            TimerService.StartTimer(_lifeTime, Stop, this);
        }

        private void Stop()
        {
            UpdateService.UnregisterUpdate(Fly);
            TimerService.StopTimer(this, Stop);

            _collider.enabled = false;
            this.SetActive(false);
        }

        private void Fly(float deltaTime)
        {
            transform.Translate(_speed * deltaTime * Vector3.forward);
        }
    }
}
