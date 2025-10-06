using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class SwordStrike : Ability
    {
        private const int MaxStrikeCount = 50;

        private readonly ParticleSystem _swingEffect;
        private readonly LayerMask _damageLayer;
        private readonly Collider[] _colliders;
        private readonly Transform _heroCenter;
        private readonly Animator _animator;

        private float _damage;
        private float _radius;

        public SwordStrike(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter, Animator animator, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            _colliders = new Collider[MaxStrikeCount];
            _heroCenter = heroCenter.ThrowIfNull();
            _swingEffect = config.Effect.Instantiate(_heroCenter);
            _damageLayer = config.DamageLayer;

            AbilityStats stats = config.ThrowIfNull().GetStats(level.ThrowIfZeroOrLess());

            _damage = stats.Damage;
            _radius = stats.Range;
            _swingEffect.SetShapeRadius(_radius);
            _animator = animator;
        }

        public override void Dispose()
        {
            _swingEffect.DestroyGameObject();
        }

        protected sealed override void Apply()
        {
            int count = Physics.OverlapSphereNonAlloc(_heroCenter.position, _radius, _colliders, _damageLayer);

            for (int i = Constants.Zero; i < count; i++)
            {
                Collider collider = _colliders[i];

                if (collider.TryGetComponent(out Health health) == false)
                {
                    continue;
                }

                health.TakeDamage(_damage);
            }

            _swingEffect.Play();
            _animator.SetTrigger(AnimationParameters.IsAttacking);
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            _damage = stats.Damage;
            _radius = stats.Range;
            _swingEffect.SetShapeRadius(_radius);
        }
    }
}
