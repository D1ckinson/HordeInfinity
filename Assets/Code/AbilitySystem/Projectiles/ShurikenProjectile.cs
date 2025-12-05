using Assets.Code.AbilitySystem.Interfaces;
using Assets.Code.CharactersLogic.GeneralLogic;
using Assets.Code.Core;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Projectiles
{
    public class ShurikenProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField][Min(5f)] private float _speed = 40f;
        [SerializeField][Min(1f)] private float _lifeTime = 3f;
        [SerializeField] private SoundPause _soundPause;

        private float _damage;
        private int _maxBounces;
        private int _currentBounces;
        private Vector3 _moveDirection;
        private Pool<AudioSource> _hitSound;
        private ITargetSelector _targetSelector;

        public event Action<HitResult> Hit;

        private void OnTriggerEnter(Collider other)
        {
            if (_targetSelector.Target.IsNotNull() && other != _targetSelector.Target)
            {
                return;
            }

            if (TryDamage(other))
            {
                _currentBounces++;

                if (_currentBounces == _maxBounces)
                {
                    this.SetActive(false);
                }

                _targetSelector.FindTarget(transform.position);
                _moveDirection = Utilities.GenerateRandomDirection();
                TimerService.StartTimer(_lifeTime, Disable, this);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other == _targetSelector.Target)
            {
                _targetSelector.ResetTarget();
                _moveDirection = Utilities.GenerateRandomDirection();
            }
        }

        private void OnDisable()
        {
            UpdateService.UnregisterUpdate(Move);
            UpdateService.UnregisterFixedUpdate(CalculateDirection);
            TimerService.StopTimer(this, Disable);
        }

        public ShurikenProjectile Initialize(ITargetSelector targetSelector, Pool<AudioSource> hitSound, ITimeService timeService)
        {
            _targetSelector = targetSelector.ThrowIfNull();
            _soundPause.Initialize(timeService);
            _hitSound = hitSound.ThrowIfNull();

            return this;
        }

        public void SetStats(float damage, int maxBounces)
        {
            _damage = damage.ThrowIfNegative();
            _maxBounces = maxBounces.ThrowIfNegative();
        }

        public void Launch(Vector3 from, Vector3 direction)
        {
            this.SetActive(true);
            _targetSelector.ResetTarget();

            _moveDirection = direction.ThrowIfNotNormalize();

            transform.position = from;
            _currentBounces = Constants.Zero;

            UpdateService.RegisterUpdate(Move);
            UpdateService.RegisterFixedUpdate(CalculateDirection);
            TimerService.StartTimer(_lifeTime, Disable, this);
        }

        private bool TryDamage(Collider collider)
        {
            if (_targetSelector.DamageLayer.Contains(collider.gameObject.layer) && collider.TryGetComponent(out Health health))
            {
                ApplyDamage(health);

                return true;
            }

            return false;
        }

        private void ApplyDamage(Health health)
        {
            Hit?.Invoke(health.TakeDamage(_damage));
            _hitSound.Get(transform).PlayRandomPitch();
        }

        private void Disable()
        {
            this.SetActive(false);
        }

        private void CalculateDirection(float fixedDeltaTime)
        {
            if (_targetSelector.Target.IsNull())
            {
                return;
            }

            if (_targetSelector.Target.IsActive() == false)
            {
                _targetSelector.ResetTarget();
                _moveDirection = Utilities.GenerateRandomDirection();

                return;
            }

            Vector3 direction = (_targetSelector.Target.transform.position - transform.position).normalized;
            direction.y = Constants.Zero;

            _moveDirection = direction;
        }

        public Vector3 Direction => _moveDirection;

        private void Move(float deltaTime)
        {
            transform.position += _speed * deltaTime * _moveDirection;
        }
    }
}
