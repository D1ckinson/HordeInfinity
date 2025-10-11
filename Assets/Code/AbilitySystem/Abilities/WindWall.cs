using Assets.Code.CharactersLogic.EnemyLogic;
using Assets.Code.Tools;
using System;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class WindWall : MonoBehaviour
    {
        [SerializeField][Min(1f)] private float _speed = 10f;
        [SerializeField][Min(1f)] private float _lifeTime = 2f;
        [SerializeField][Min(1f)] private float _pushForce = 5f;
        [SerializeField] private AudioSource _sound;

        private readonly Timer _timer = new();

        private LayerMask _damageLayer;
        private float _damage;

        private void OnTriggerEnter(Collider other)
        {
            if (_damageLayer.Contains(other.gameObject.layer) && other.TryGetComponent(out EnemyComponents enemy))
            {
                enemy.Health.TakeDamage(_damage);
                enemy.Rigidbody.AddForce(transform.forward * _pushForce, ForceMode.Impulse);
            }
        }

        private void OnDisable()
        {
            _timer.Completed -= Disable;
            UpdateService.UnregisterUpdate(Move);
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

        public void Launch()
        {
            _timer.Start(_lifeTime);
            _timer.Completed += Disable;
            _sound.PlayRandomPitch();

            UpdateService.RegisterUpdate(Move);
        }

        private void Move()
        {
            transform.position += _speed * Time.deltaTime * transform.forward;
        }

        private void Disable()
        {
            this.SetActive(false);
        }
    }
}
