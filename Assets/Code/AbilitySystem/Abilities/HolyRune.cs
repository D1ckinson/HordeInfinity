using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class HolyRune : MonoBehaviour
    {
        [SerializeField][Range(0.1f, 2f)] private float _scaleChangeDuration = 0.7f;
        [SerializeField] private Follower _follower;
        [SerializeField] private SoundPause _soundPause;

        private readonly List<Health> _health = new();

        private LayerMask _damageLayer;
        private float _damage;

        private void Awake()
        {
            transform.localScale = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject gameObject = other.ThrowIfNull().gameObject;

            if (_damageLayer.Contains(gameObject.layer) && gameObject.TryGetComponent(out Health health))
            {
                _health.Add(health);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            GameObject gameObject = other.ThrowIfNull().gameObject;

            if (_damageLayer.Contains(gameObject.layer) && gameObject.TryGetComponent(out Health health))
            {
                _health.Remove(health);
            }
        }

        public void Initialize(float damage, float radius, LayerMask damageLayer, Transform followTarget, ITimeService timeService)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            SetStats(damage, radius);
            _follower.Follow(followTarget);
            _soundPause.Initialize(timeService);
        }

        public void SetStats(float damage, float radius)
        {
            _damage = damage.ThrowIfNegative();

            CoroutineService.StopAllCoroutines(this);
            CoroutineService.StartCoroutine(SetScale(radius / Constants.Two), this);
        }

        public void DealDamage()
        {
            _health.RemoveAll(health => health.IsActive() == false);
            _health.ForEach(health => health.TakeDamage(_damage));
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
    }
}
