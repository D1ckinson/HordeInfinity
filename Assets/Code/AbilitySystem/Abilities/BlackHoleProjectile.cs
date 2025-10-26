using Assets.Code.CharactersLogic;
using Assets.Code.CharactersLogic.EnemyLogic;
using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class BlackHoleProjectile : MonoBehaviour
    {
        [SerializeField][Min(0.1f)] private float _baseDamage = 1f;
        [SerializeField][Min(0.1f)] private float _radiusForEnemy = 0.2f;
        [SerializeField][Min(0.1f)] private float _lifeTime = 5f;
        [SerializeField][Min(1.1f)] private float _scaleFactor = 1.6f;
        [SerializeField] private SphereCollider _effectZone;

        private readonly Dictionary<Health, EnemyComponents> _enemies = new();

        private LayerMask _damageLayer;
        private Pool<ParticleSystem> _effectPool;
        private AudioSource _sound;
        private float _damage;
        private float _pullForce;
        private float _radius;

        private Dictionary<AbilityType, int> _damageDealt;
        private Dictionary<AbilityType, int> _killCount;

        private void OnEnable()
        {
            SetShape();
            TimerService.StartTimer(_lifeTime, ()=>this.SetActive(false));
        }

        private void OnDisable()
        {
            _enemies.Clear();
            SetShape();
        }

        private void OnDestroy()
        {
            _sound.DestroyGameObject();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_damageLayer.Contains(other.gameObject.layer) && other.TryGetComponent(out EnemyComponents enemy))
            {
                _enemies.Add(enemy.Health, enemy);
                SetShape();

                enemy.Health.Died += Remove;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out EnemyComponents enemy))
            {
                _enemies.Remove(enemy.Health);
                SetShape();

                enemy.Health.Died -= Remove;
            }
        }

        private void FixedUpdate()
        {
            for (int i = _enemies.LastIndex(); i >= Constants.Zero; i--)
            {
                EnemyComponents enemy = _enemies.Values.ElementAt(i);

                float damage = _baseDamage + _damage * _enemies.Count;
                _damageDealt[AbilityType.BlackHole] += (int)damage;

                if (enemy.Health.TakeDamage(damage))
                {
                    _killCount[AbilityType.BlackHole]++;
                }

                Vector3 directionToCenter = (transform.position - enemy.transform.position).normalized;
                float pullForce = _pullForce * _enemies.Count;

                enemy.Rigidbody.AddForce(directionToCenter * pullForce);
            }
        }

        public BlackHoleProjectile Initialize(LayerMask damageLayer, float damage, float radius, float pullForce,
            Pool<ParticleSystem> effectPool, AudioSource sound, Dictionary<AbilityType, int> damageDealt, Dictionary<AbilityType, int> killCount)
        {
            _damageLayer = damageLayer.ThrowIfNull();
            _effectPool = effectPool.ThrowIfNull();
            _sound = sound.ThrowIfNull();
            _damageDealt = damageDealt.ThrowIfNull();
            _killCount = killCount.ThrowIfNull();

            SetStats(damage, radius, pullForce);

            return this;
        }

        public void SetStats(float damage, float radius, float pullForce)
        {
            _damage = damage.ThrowIfNegative();
            _pullForce = pullForce.ThrowIfNegative();
            _radius = radius.ThrowIfNegative() * _scaleFactor;

            SetShape();
        }

        public void Activate(Vector3 position)
        {
            _sound.transform.position = position;
            _sound.Play();

            transform.position = position;
            this.SetActive(true);

            ParticleSystem effect = _effectPool.Get();
            effect.transform.position = position;
            effect.Play();
        }

        private void SetShape()
        {
            float radius = _radius + _radiusForEnemy * _enemies.Count;

            _effectZone.radius = radius;
            _effectPool?.ForEach(effect => effect.SetShapeRadius(radius));
        }

        private void Remove(Health health)
        {
            health.Died -= Remove;
            _enemies.Remove(health);
        }
    }
}
