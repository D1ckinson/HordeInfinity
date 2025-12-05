using Assets.Code.AbilitySystem.Interfaces;
using Assets.Code.CharactersLogic.GeneralLogic;
using Assets.Code.Core;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Projectiles
{
    public class HolyRune : MonoBehaviour, IProjectile
    {
        [SerializeField][Range(0.1f, 2f)] private float _scaleChangeDuration = 0.7f;
        [SerializeField] private Follower _follower;
        [SerializeField] private SoundPause _soundPause;
        [SerializeField][Min(1.1f)] private float _scaleFactor = 1.5f;

        private readonly List<Health> _health = new();

        private LayerMask _damageLayer;
        private float _damage;

        public event Action<HitResult> Hit;

        private void Awake()
        {
            transform.localScale = Vector3.zero;
        }

        private void OnDisable()
        {
            _health.ForEach(health => health.Died -= Remove);
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject gameObject = other.ThrowIfNull().gameObject;

            if (_damageLayer.Contains(gameObject.layer) && gameObject.TryGetComponent(out Health health))
            {
                _health.Add(health);
                health.Died += Remove;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            GameObject gameObject = other.ThrowIfNull().gameObject;

            if (_damageLayer.Contains(gameObject.layer) && gameObject.TryGetComponent(out Health health))
            {
                _health.Remove(health);
                health.Died -= Remove;
            }
        }

        public HolyRune Initialize(float damage, float radius, LayerMask damageLayer, Transform followTarget, ITimeService timeService)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            SetStats(damage, radius);
            _follower.Follow(followTarget);
            _soundPause.Initialize(timeService);

            return this;
        }

        public void SetStats(float damage, float radius)
        {
            _damage = damage.ThrowIfNegative();

            CoroutineService.StopAllCoroutines(this);
            CoroutineService.StartCoroutine(SetScale(radius * _scaleFactor), this);
        }

        public void DealDamage()
        {
            for (int i = _health.LastIndex(); i >= Constants.Zero; i--)
            {
                Hit?.Invoke(_health[i].TakeDamage(_damage));
            }
        }

        private IEnumerator SetScale(float targetScale)
        {
            float elapsed = Constants.Zero;
            float startScale = transform.localScale.x;

            while (elapsed < _scaleChangeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / _scaleChangeDuration;

                float scale = Mathf.Lerp(startScale, targetScale, progress);
                transform.localScale = new(scale, scale, scale);

                yield return null;
            }

            transform.localScale = new(targetScale, targetScale, targetScale);
        }

        private void Remove(Health health)
        {
            health.Died -= Remove;
            _health.Remove(health);
        }
    }
}
