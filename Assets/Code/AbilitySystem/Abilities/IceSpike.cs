using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class IceSpike : MonoBehaviour
    {
        [SerializeField][Min(0.5f)] private float _lifeTime = 3f;
        [SerializeField][Min(1f)] private float _speed = 30f;

        private Pool<AudioSource> _hitSoundPool;
        private LayerMask _damageLayer;
        private float _damage;

        private Dictionary<AbilityType, int> _damageDealt;
        private Dictionary<AbilityType, int> _killCount;

        private void OnTriggerEnter(Collider other)
        {
            if (_damageLayer.Contains(other.gameObject.layer) && other.TryGetComponent(out Health health))
            {
                _damageDealt[AbilityType.IceStaff] += (int)_damage;

                if (health.TakeDamage(_damage))
                {
                    _killCount[AbilityType.IceStaff]++;
                }

                _hitSoundPool.Get(transform).PlayRandomPitch();

                this.SetActive(false);
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Initialize(LayerMask damageLayer, float damage, Pool<AudioSource> hitSoundPool, Dictionary<AbilityType, int> damageDealt, Dictionary<AbilityType, int> killCount)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            _hitSoundPool = hitSoundPool.ThrowIfNull();

            _damageDealt = damageDealt.ThrowIfNull();
            _killCount = killCount.ThrowIfNull();

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
            TimerService.StartTimer(_lifeTime, Stop, this);
        }

        private void Stop()
        {
            UpdateService.UnregisterUpdate(MoveForward);
            TimerService.StopTimer(this, Stop);
            this.SetActive(false);
        }

        private void MoveForward()
        {
            transform.position += _speed * Time.deltaTime * transform.forward;
        }
    }
}
