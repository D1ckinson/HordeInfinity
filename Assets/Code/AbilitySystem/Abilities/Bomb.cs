using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts.Tools;
using DG.Tweening;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Bomb : MonoBehaviour
    {
        private const float MidPointFactor = 0.5f;

        [SerializeField][Min(1)] private float _arcHeight = 5f;
        [SerializeField][Min(0.01f)] private float _airTime = 1f;
        [SerializeField][Min(0.01f)] private float _rotationSpeed = 5f;

        private readonly Collider[] _colliders = new Collider[20];

        private float _damage;
        private float _explosionRadius;
        private Pool<ParticleSystem> _effectPool;
        private LayerMask _damageLayer;
        private Tween _currentTween;

        private void Update()
        {
            transform.Rotate(Constants.Zero, _rotationSpeed*Time.deltaTime, Constants.Zero);
        }

        private void OnDestroy()
        {
            _currentTween?.Kill();
        }

        public void Initialize(float damage, float explosionRadius, LayerMask damageLayer, Pool<ParticleSystem> effectPool)
        {
            SetStats(damage, explosionRadius);

            _effectPool = effectPool.ThrowIfNull();
            _damageLayer = damageLayer.ThrowIfNull();
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
                    health.TakeDamage(_damage);
                }
            }

            ParticleSystem effect = _effectPool.Get();
            effect.transform.position = transform.position;
            effect.Play();

            this.SetActive(false);
        }
    }
}
