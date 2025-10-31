using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts.Tools;
using DG.Tweening;
using System;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Bomb : MonoBehaviour, IProjectile
    {
        private const float MidPointFactor = 0.5f;

        [SerializeField][Min(1)] private float _arcHeight = 5f;
        [SerializeField][Min(0.01f)] private float _airTime = 1f;
        [SerializeField][Min(0.01f)] private float _rotationSpeed = 5f;

        private readonly Collider[] _colliders = new Collider[20];

        private float _damage;
        private float _explosionRadius;
        private Pool<ParticleSystem> _visualEffectPool;
        private Pool<AudioSource> _soundEffectPool;
        private LayerMask _damageLayer;
        private Tween _currentTween;

        public event Action<HitResult> Hit;

        private void Update()
        {
            transform.Rotate(Constants.Zero, _rotationSpeed * Time.deltaTime, Constants.Zero);
        }

        private void OnDestroy()
        {
            _currentTween?.Kill();
        }

        public Bomb Initialize(float damage, float explosionRadius, LayerMask damageLayer,
            Pool<ParticleSystem> visualEffectPool, Pool<AudioSource> soundEffectPool)
        {
            SetStats(damage, explosionRadius);

            _visualEffectPool = visualEffectPool.ThrowIfNull();
            _soundEffectPool = soundEffectPool.ThrowIfNull();
            _damageLayer = damageLayer.ThrowIfNull();

            return this;
        }

        public void SetStats(float damage, float explosionRadius)
        {
            _damage = damage.ThrowIfNegative();
            _explosionRadius = explosionRadius.ThrowIfNegative();
        }

        public void Fly(Vector3 from, Vector3 to)
        {
            _currentTween?.Kill();

            Vector3 controlPoint = (from + to) * MidPointFactor + Vector3.up * _arcHeight;
            transform.position = from;

            _currentTween = transform.DOPath(new Vector3[] { from, controlPoint, to }, _airTime, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .OnComplete(Explode);
        }

        private void Explode()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, _explosionRadius, _colliders, _damageLayer);

            for (int i = Constants.Zero; i < count; i++)
            {
                if (_colliders[i].TryGetComponent(out Health health))
                {
                    Hit?.Invoke(health.TakeDamage(_damage));
                }
            }

            AudioSource audio = _soundEffectPool.Get();
            audio.transform.position = transform.position;
            audio.PlayRandomPitch();

            ParticleSystem effect = _visualEffectPool.Get();
            effect.transform.position = transform.position;
            effect.Play();

            this.SetActive(false);
        }
    }
}
