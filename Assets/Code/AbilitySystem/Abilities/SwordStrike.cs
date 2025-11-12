using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts;
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

        public SwordStrike(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter,
            Animator animator, BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _heroCenter = heroCenter.ThrowIfNull();
            _swingEffect = config.ThrowIfNull().Effect.Instantiate(_heroCenter);
            _damageLayer = config.DamageLayer;

            _swingEffect.SetShapeRadius(CurrentStats.Get(FloatStatType.Range));

            _animator = animator.ThrowIfNull();
            _audioSource = config.HitSound.Instantiate();
        }

        protected override void OnDispose()
        {
            _swingEffect.DestroyGameObject();
            _audioSource.DestroyGameObject();
        }

        protected sealed override void Apply()
        {
            int count = Physics.OverlapSphereNonAlloc(_heroCenter.position, CurrentStats.Get(FloatStatType.Range), _colliders, _damageLayer);

            for (int i = Constants.Zero; i < count; i++)
            {
                Collider collider = _colliders[i];

                if (collider.TryGetComponent(out Health health))
                {
                    RecordHitResult(health.TakeDamage(CurrentStats.Get(FloatStatType.Damage)));
                }
            }

            _swingEffect.Play();
            _animator.SetTrigger(AnimationParameters.IsAttacking);
            _audioSource.SetActive(true);
            _audioSource.transform.position = _heroCenter.position;
            _audioSource.PlayRandomPitch();
        }

        protected override void OnStatsUpdate()
        {
            _swingEffect.SetShapeRadius(CurrentStats.Get(FloatStatType.Range));
        }
    }
}
