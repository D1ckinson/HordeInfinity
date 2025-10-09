using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class HolyRune : MonoBehaviour
    {
        [SerializeField] private Follower _follower;

        private readonly List<Health> _health = new();

        private LayerMask _damageLayer;
        private float _damage;

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

        public void Initialize(float damage, float radius, LayerMask damageLayer, Transform followTarget)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            SetStats(damage, radius);
            _follower.Follow(followTarget);
        }

        public void SetStats(float damage, float radius)
        {
            _damage = damage.ThrowIfNegative();

            float scale = radius.ThrowIfZeroOrLess() / Constants.Two;
            transform.localScale = new(scale, Constants.One, scale);
        }

        public void DealDamage()
        {
            _health.RemoveAll(health => health.IsActive() == false);
            _health.ForEach(health => health.TakeDamage(_damage));
        }
    }
}
