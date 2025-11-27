using Assets.Code.CharactersLogic.GeneralLogic;
using Assets.Code.Tools.Base;
using UnityEngine;

namespace Assets.Code.CharactersLogic.EnemyLogic
{
    [RequireComponent(typeof(Collider))]
    public class CollisionDamage : MonoBehaviour
    {
        private float _damage;
        private LayerMask _damageLayer;

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            HandleCollision(collision);
        }

        public void Initialize(float damage, LayerMask damageLayer)
        {
            _damage = damage.ThrowIfNegative();
            _damageLayer = damageLayer.ThrowIfNull();
        }

        public void SetDamage(float damage)
        {
            _damage = damage.ThrowIfNegative();
        }

        private void HandleCollision(Collision collision)
        {
            GameObject gameObject = collision.ThrowIfNull().gameObject;

            if (_damageLayer.Contains(gameObject.layer) && gameObject.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
            }
        }
    }
}
