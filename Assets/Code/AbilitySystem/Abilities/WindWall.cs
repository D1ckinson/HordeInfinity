using Assets.Code.CharactersLogic;
using Assets.Code.CharactersLogic.EnemyLogic;
using Assets.Code.Tools;
using System;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class WindWall : MonoBehaviour, IProjectile
    {
        [SerializeField][Min(1f)] private float _speed = 10f;
        [SerializeField][Min(1f)] private float _lifeTime = 2f;
        [SerializeField][Min(1f)] private float _pushForce = 5f;
        [SerializeField] private AudioSource _sound;

        private LayerMask _damageLayer;
        private float _damage;

        public event Action<HitResult> Hit;

        private void OnTriggerEnter(Collider other)
        {
            if (_damageLayer.Contains(other.gameObject.layer) && other.TryGetComponent(out EnemyComponents enemy))
            {
                Hit?.Invoke(enemy.Health.TakeDamage(_damage));
                enemy.Rigidbody.AddForce(transform.forward * _pushForce, ForceMode.Impulse);
            }
        }

        private void OnDisable()
        {
            TimerService.StopTimer(this, Disable);
            UpdateService.UnregisterUpdate(Move);
        }

        public WindWall Initialize(LayerMask damageLayer, float damage)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            SetDamage(damage);

            return this;
        }

        public void SetDamage(float damage)
        {
            _damage = damage.ThrowIfNegative();
        }

        public void Launch()
        {
            _sound.PlayRandomPitch();

            TimerService.StartTimer(_lifeTime, Disable, this);
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
