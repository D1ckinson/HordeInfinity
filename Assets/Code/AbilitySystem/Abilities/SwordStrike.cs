using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class SwordStrike : Ability
    {
        private readonly ParticleSystem _swingEffect;
        private readonly LayerMask _damageLayer;
        private readonly Collider[] _colliders = new Collider[50];
        private readonly Transform _heroCenter;
        private readonly Animator _animator;
        private readonly AudioSource _audioSource;

        private readonly Dictionary<AbilityType, float> _damageDealt;
        private readonly Dictionary<AbilityType, int> _killCount;

        private float _damage;
        private float _radius;

        public SwordStrike(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter,
            Animator animator, Dictionary<AbilityType, float> damageDealt, Dictionary<AbilityType, int> killCount, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            _heroCenter = heroCenter.ThrowIfNull();
            _swingEffect = config.Effect.Instantiate(_heroCenter);
            _damageLayer = config.DamageLayer;

            AbilityStats stats = config.ThrowIfNull().GetStats(level.ThrowIfZeroOrLess());

            _damage = stats.Damage;
            _radius = stats.Range;
            _swingEffect.SetShapeRadius(_radius);

            _animator = animator.ThrowIfNull();
            _audioSource = config.HitSound.Instantiate();

            _damageDealt = damageDealt.ThrowIfNullOrEmpty();
            _killCount = killCount.ThrowIfNullOrEmpty();
        }

        public override void Dispose()
        {
            _swingEffect.DestroyGameObject();
            _audioSource.DestroyGameObject();
        }

        protected sealed override void Apply()
        {
            int count = Physics.OverlapSphereNonAlloc(_heroCenter.position, _radius, _colliders, _damageLayer);

            for (int i = Constants.Zero; i < count; i++)
            {
                Collider collider = _colliders[i];

                if (collider.TryGetComponent(out Health health))
                {
                    _damageDealt[AbilityType.SwordStrike] += _damage;

                    if (health.TakeDamage(_damage))
                    {
                        _killCount[AbilityType.SwordStrike]++;
                    }
                }
            }

            _swingEffect.Play();
            _animator.SetTrigger(AnimationParameters.IsAttacking);
            _audioSource.SetActive(true);
            _audioSource.transform.position = _heroCenter.position;
            _audioSource.PlayRandomPitch();
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            _damage = stats.Damage;
            _radius = stats.Range;
            _swingEffect.SetShapeRadius(_radius);
        }
    }
}
