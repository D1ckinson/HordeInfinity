using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class IceStuff : Ability
    {
        private const int MaxTargets = 10;

        private readonly WaitForSeconds _delay = new(0.2f);
        private readonly Pool<IceSpike> _pool;
        private readonly LayerMask _damageLayer;

        private float _damage;
        private float _attackRadius;
        private int _projectilesCount;

        public IceStuff(AbilityConfig config, Transform transform, Dictionary<AbilityType, int> abilityUnlockLevel, int level = 1) : base(config, transform, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);

            _damage = stats.Damage;
            _attackRadius = stats.Range;
            _projectilesCount = stats.ProjectilesCount;
            _damageLayer = config.DamageLayer;

            _pool = new(CreateIceSpike);

            IceSpike CreateIceSpike()
            {
                IceSpike iceSpike = config.ProjectilePrefab.Instantiate().GetComponentOrThrow<IceSpike>();
                iceSpike.Initialize(_damageLayer, _damage);

                return iceSpike;
            }
        }

        public override void Dispose()
        {
            CoroutineService.StopAllCoroutines(this);
            _pool.DestroyAll();
        }

        protected override void Apply()
        {
            CoroutineService.StartCoroutine(LaunchSpikes(), this);
        }

        protected override void UpdateStats(float damage, float range, int projectilesCount, bool isPiercing, int healthPercent, float pullForce)
        {
            _damage = damage.ThrowIfNegative();
            _projectilesCount = projectilesCount.ThrowIfNegative();

            _pool.ForEach(spike => spike.SetDamage(_damage));
        }

        private IEnumerator LaunchSpikes()
        {
            Collider[] colliders = new Collider[MaxTargets];

            for (int i = Constants.Zero; i < _projectilesCount; i++)
            {
                Physics.OverlapSphereNonAlloc(Position, _attackRadius, colliders, _damageLayer);

                Vector3 targetPosition = colliders
                    .Where(collider => collider.NotNull())
                    .OrderBy(collider => (collider.transform.position - Position).sqrMagnitude)
                    .First().transform.position;

                _pool.Get().Fly(Position, (targetPosition - Position).normalized);

                yield return _delay;
            }
        }
    }
}
