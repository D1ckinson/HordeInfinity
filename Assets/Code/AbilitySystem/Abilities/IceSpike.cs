using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class IceSpike : MonoBehaviour, IProjectile
    {
        [SerializeField][Min(0.5f)] private float _lifeTime = 3f;
        [SerializeField][Min(1f)] private float _speed = 30f;

        private Pool<AudioSource> _hitSoundPool;
        private LayerMask _damageLayer;
        private float _damage;

        public event Action<HitResult> Hit;

        private void OnTriggerEnter(Collider other)
        {
            if (_damageLayer.Contains(other.gameObject.layer) && other.TryGetComponent(out Health health))
            {
                Hit?.Invoke(health.TakeDamage(_damage));
                _hitSoundPool.Get(transform).PlayRandomPitch();

                this.SetActive(false);
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        public IceSpike Initialize(LayerMask damageLayer, float damage, Pool<AudioSource> hitSoundPool)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            _hitSoundPool = hitSoundPool.ThrowIfNull();

            SetDamage(damage);

            return this;
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
