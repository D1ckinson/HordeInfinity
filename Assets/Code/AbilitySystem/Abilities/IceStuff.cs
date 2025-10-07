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
        private readonly Transform _heroCenter;

        private float _damage;
        private float _attackRadius;
        private int _projectilesCount;

        public IceStuff(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenterPoint, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);
            _heroCenter = heroCenterPoint.ThrowIfNull();

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

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();
            _damage = stats.Damage;
            _projectilesCount = stats.ProjectilesCount;

            _pool.ForEach(spike => spike.SetDamage(_damage));
        }

        private IEnumerator LaunchSpikes()
        {
            Collider[] colliders = new Collider[MaxTargets];

            for (int i = Constants.Zero; i < _projectilesCount; i++)
            {
                Physics.OverlapSphereNonAlloc(_heroCenter.position, _attackRadius, colliders, _damageLayer);

                Collider collider = colliders
                    .Where(collider => collider.NotNull())
                    .OrderBy(collider => (collider.transform.position - _heroCenter.position).sqrMagnitude)
                    .First();

                Vector3 direction;

                if (collider.IsNull())
                {
                    direction = Utilities.GenerateRandomDirection(_heroCenter.transform.position.y);
                }
                else
                {
                    direction = (collider.transform.position - _heroCenter.position).normalized;
                }

                _pool.Get().Fly(_heroCenter.position, direction);

                yield return _delay;
            }
        }
    }
}
