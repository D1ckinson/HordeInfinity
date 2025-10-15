using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class FireballProjectile : MonoBehaviour
    {
        [SerializeField][Min(1f)] private float _speed = 30f;
        [SerializeField][Min(1f)] private float _lifeTime = 3f;
        [SerializeField] private SoundPause _soundPause;

        private readonly Collider[] _colliders = new Collider[30];
        private readonly Timer _timer = new();

        private LayerMask _damageLayer;
        private Pool<ParticleSystem> _explosionEffectPool;
        private Pool<AudioSource> _explosionSoundPool;
        private float _damage;
        private float _explosionRadius;
        private Vector3 _direction;

        private Dictionary<AbilityType, int> _damageDealt;
        private Dictionary<AbilityType, int> _killCount;

        private void OnTriggerEnter(Collider other)
        {
            if (_damageLayer.Contains(other.gameObject.layer))
            {
                Explode();
            }
        }

        private void OnDisable()
        {
            UpdateService.UnregisterUpdate(Move);
            _timer.Stop();
            _timer.Completed -= Explode;
        }

        public void Initialize(LayerMask damageLayer, float damage, float explosionRadius,
            Pool<ParticleSystem> explosionEffectPool, Pool<AudioSource> explosionSoundPool,
            ITimeService timeService, Dictionary<AbilityType, int> damageDealt, Dictionary<AbilityType, int> killCount)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            _explosionEffectPool = explosionEffectPool.ThrowIfNull();
            _explosionSoundPool = explosionSoundPool.ThrowIfNull();
            _soundPause.Initialize(timeService);

            _damageDealt = damageDealt.ThrowIfNull();
            _killCount = killCount.ThrowIfNull();

            SetStats(damage, explosionRadius);
        }

        public void SetStats(float damage, float explosionRadius)
        {
            _damage = damage.ThrowIfNegative();
            _explosionRadius = explosionRadius.ThrowIfNegative();
        }

        public void Fly(Vector3 from, Vector3 direction)
        {
            transform.position = from;
            _direction = direction.ThrowIfNotNormalize();

            UpdateService.RegisterUpdate(Move);

            _timer.Start(_lifeTime);
            _timer.Completed += Explode;
        }

        private void Move()
        {
            transform.position += _speed * Time.deltaTime * _direction;
        }

        private void Explode()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, _explosionRadius, _colliders, _damageLayer);

            for (int i = Constants.Zero; i < count; i++)
            {
                if (_colliders[i].TryGetComponent(out Health health))
                {
                    _damageDealt[AbilityType.Fireball] += (int)_damage;

                    if (health.TakeDamage(_damage))
                    {
                        _killCount[AbilityType.Fireball]++;
                    }
                }
            }

            ParticleSystem effect = _explosionEffectPool.Get();
            effect.transform.position = transform.position;
            effect.Play();

            AudioSource sound = _explosionSoundPool.Get();
            sound.transform.position = transform.position;
            sound.Play();

            this.SetActive(false);
        }
    }
}
