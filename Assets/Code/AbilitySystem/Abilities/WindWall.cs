using Assets.Code.CharactersLogic.EnemyLogic;
using Assets.Code.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class WindWall : MonoBehaviour
    {
        [SerializeField][Min(1f)] private float _speed = 10f;
        [SerializeField][Min(1f)] private float _lifeTime = 2f;
        [SerializeField][Min(1f)] private float _pushForce = 5f;
        [SerializeField] private AudioSource _sound;

        private LayerMask _damageLayer;
        private float _damage;

        private Dictionary<AbilityType, int> _damageDealt;
        private Dictionary<AbilityType, int> _killCount;

        private void OnTriggerEnter(Collider other)
        {
            if (_damageLayer.Contains(other.gameObject.layer) && other.TryGetComponent(out EnemyComponents enemy))
            {
                _damageDealt[AbilityType.WindFlow] += (int)_damage;

                if (enemy.Health.TakeDamage(_damage))
                {
                    _killCount[AbilityType.WindFlow]++;
                }

                enemy.Rigidbody.AddForce(transform.forward * _pushForce, ForceMode.Impulse);
            }
        }

        private void OnDisable()
        {
            TimerService.StopTimer(this, Disable);
            UpdateService.UnregisterUpdate(Move);
        }

        public void Initialize(LayerMask damageLayer, float damage, Dictionary<AbilityType, int> damageDealt, Dictionary<AbilityType, int> killCount)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            SetDamage(damage);

            _damageDealt = damageDealt.ThrowIfNull();
            _killCount = killCount.ThrowIfNull();
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
