using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Tools;
using System;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class FireballProjectile : MonoBehaviour, IProjectile
    {
        [SerializeField][Min(1f)] private float _speed = 30f;
        [SerializeField][Min(1f)] private float _lifeTime = 3f;
        [SerializeField] private SoundPause _soundPause;

        private readonly Collider[] _colliders = new Collider[30];

        private LayerMask _damageLayer;
        private Pool<ParticleSystem> _explosionEffectPool;
        private Pool<AudioSource> _explosionSoundPool;
        private float _damage;
        private float _explosionRadius;
        private Vector3 _direction;

        public event Action<HitResult> Hit;

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
            TimerService.StopTimer(this, Explode);
        }

        public FireballProjectile Initialize(LayerMask damageLayer, float damage, float explosionRadius,
            Pool<ParticleSystem> explosionEffectPool, Pool<AudioSource> explosionSoundPool, ITimeService timeService)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            _explosionEffectPool = explosionEffectPool.ThrowIfNull();
            _explosionSoundPool = explosionSoundPool.ThrowIfNull();
            _soundPause.Initialize(timeService);

            SetStats(damage, explosionRadius);

            return this;
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
            TimerService.StartTimer(_lifeTime, Explode, this);
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
                    Hit?.Invoke(health.TakeDamage(_damage));
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
