using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Tools;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class ShurikenProjectile : MonoBehaviour
    {
        [SerializeField][Min(5f)] private float _speed = 40f;
        [SerializeField][Min(1f)] private float _lifeTime = 3f;
        [SerializeField] private SoundPause _soundPause;

        private readonly Collider[] _colliders = new Collider[10];
        private readonly Timer _timer = new();

        private LayerMask _damageLayer;
        private float _damage;
        private float _searchRadius;
        private int _maxBounces;
        private int _currentBounces;
        private Vector3 _moveDirection;
        private Collider _target;
        private Collider _lastTarget;
        private Pool<AudioSource> _hitSound;

        private void OnTriggerEnter(Collider other)
        {
            if (_target.NotNull() && other != _target)
            {
                return;
            }

            if (TryDamage(other))
            {
                _currentBounces++;
                SetTarget();
            }

            if (_currentBounces == _maxBounces)
            {
                this.SetActive(false);
            }
        }

        private void OnEnable()
        {
            _timer.Completed += Disable;
        }

        private void OnDisable()
        {
            UpdateService.UnregisterUpdate(Move);
            UpdateService.UnregisterFixedUpdate(CalculateDirection);
            _timer.Completed -= Disable;
        }

        private bool TryDamage(Collider collider)
        {
            if (_damageLayer.Contains(collider.gameObject.layer) && collider.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
                AudioSource sound = _hitSound.Get();
                sound.transform.position = transform.position;
                sound.PlayRandomPitch();

                _lastTarget = _target;

                return true;
            }

            return false;
        }

        public void Initialize(LayerMask damageLayer, float damage, float searchRadius, int maxBounces, Pool<AudioSource> hitSound, ITimeService timeService)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            _soundPause.Initialize(timeService);
            _hitSound = hitSound.ThrowIfNull();

            SetStats(damage, searchRadius, maxBounces);
        }

        public void SetStats(float damage, float searchRadius, int maxBounces)
        {
            _damage = damage.ThrowIfNegative();
            _searchRadius = searchRadius.ThrowIfNegative();
            _maxBounces = maxBounces.ThrowIfNegative();
        }

        public void Launch(Vector3 from, Vector3 direction)
        {
            this.SetActive(true);
            _target = null;
            _lastTarget = null;
            _moveDirection = direction.ThrowIfNotNormalize();

            transform.position = from;
            _currentBounces = Constants.Zero;

            UpdateService.RegisterUpdate(Move);
            UpdateService.RegisterFixedUpdate(CalculateDirection);
            _timer.Start(_lifeTime);
        }

        private void Disable()
        {
            this.SetActive(false);
        }

        private void CalculateDirection()
        {
            if (_target.IsNull())
            {
                return;
            }

            if (_target.IsActive() == false)
            {
                _target = null;

                return;
            }

            _moveDirection = (_target.transform.position - transform.position).normalized;
        }

        private void Move()
        {
            transform.position += _speed * Time.deltaTime * _moveDirection;
        }

        private void SetTarget()
        {
            _target = null;

            int count = Physics.OverlapSphereNonAlloc(transform.position, _searchRadius, _colliders, _damageLayer);
            float sqrDistance = float.MaxValue;

            for (int i = Constants.Zero; i < count; i++)
            {
                Collider collider = _colliders[i];

                if (collider == _lastTarget)
                {
                    continue;
                }

                float sqrMagnitude = (collider.transform.position - transform.position).sqrMagnitude;

                if (sqrMagnitude < sqrDistance)
                {
                    _target = collider;
                    sqrDistance = sqrMagnitude;
                }
            }

            if (_target.IsNull())
            {
                _moveDirection = Utilities.GenerateRandomDirection(transform.position.y);
            }

            _timer.Start(_lifeTime);
        }
    }
}
