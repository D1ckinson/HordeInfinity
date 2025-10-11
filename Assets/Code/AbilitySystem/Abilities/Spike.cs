using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using DG.Tweening;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Spike : MonoBehaviour
    {
        [SerializeField][Min(0.1f)] private float _appendInterval = 0.3f;
        [SerializeField][Min(0.5f)] private float _revealDuration = 2f;
        [SerializeField][Min(0.5f)] private float _hideDuration = 0.5f;
        [SerializeField][Range(0f, 360f)] private float _rotationSpeed = 180f;
        [SerializeField] private Collider _collider;

        private Vector3 _height;
        private LayerMask _damageLayer;
        private float _damage;
        private Sequence _animationSequence;

        private void Awake()
        {
            _height = new(Constants.Zero, _collider.bounds.size.y, Constants.Zero);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_damageLayer.Contains(other.gameObject.layer) && other.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
            }
        }

        private void OnDestroy()
        {
            _animationSequence?.Kill();
            _animationSequence = null;
        }

        public void Initialize(LayerMask damageLayer, float damage)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            SetDamage(damage);
        }

        public void SetDamage(float damage)
        {
            _damage = damage.ThrowIfNegative();
        }

        public void Strike(Vector3 position)
        {
            Vector3 startPosition = position - _height;
            transform.position = startPosition;

            _animationSequence = DOTween.Sequence()
                .Append(transform.DOMove(position, _revealDuration)
                .SetEase(Ease.OutBack))
                .Join(transform.DORotate(new(Constants.Zero, _rotationSpeed, Constants.Zero), _revealDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutCubic))
                .AppendInterval(_appendInterval)
                .Append(transform.DOMove(startPosition, _hideDuration)
                .SetEase(Ease.InBack))
                .Join(transform.DORotate(new(Constants.Zero, -_rotationSpeed, Constants.Zero), _hideDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InCubic))
                .OnComplete(() =>
                    {
                        this.SetActive(false);
                        _animationSequence = null;
                    })
                .Play();
        }
    }
}
