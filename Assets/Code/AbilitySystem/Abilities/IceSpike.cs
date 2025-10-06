using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class IceSpike : MonoBehaviour
    {
        [SerializeField][Min(0.5f)] private float _lifeTime = 3f;
        [SerializeField][Min(1f)] private float _speed = 30f;

        private readonly Timer _timer = new();

        private LayerMask _damageLayer;
        private float _damage;

        private void OnTriggerEnter(Collider other)
        {
            if (_damageLayer.Contains(other.gameObject.layer) && other.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
                this.SetActive(false);
            }
        }

        private void OnDisable()
        {
            _timer.Stop();
            _timer.Completed -= Stop;

            UpdateService.UnregisterUpdate(MoveForward);
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

        public void Fly(Vector3 from, Vector3 direction)
        {
            direction.ThrowIfNotNormalize();

            transform.position = from;
            transform.rotation = Quaternion.LookRotation(new(direction.x, Constants.Zero, direction.z));

            UpdateService.RegisterUpdate(MoveForward);

            _timer.Start(_lifeTime);
            _timer.Completed += Stop;
        }

        private void Stop()
        {
            UpdateService.UnregisterUpdate(MoveForward);
            _timer.Completed -= Stop;
        }

        private void MoveForward()
        {
            transform.position += _speed * Time.deltaTime * transform.forward;
        }
    }
}
