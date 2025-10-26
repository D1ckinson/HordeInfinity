using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class IceStaff : Ability
    {
        private readonly WaitForSeconds _delay = new(0.2f);
        private readonly Pool<IceSpike> _projectilePool;
        private readonly Pool<AudioSource> _hitSoundPool;
        private readonly LayerMask _damageLayer;
        private readonly Transform _heroCenter;
        private readonly AudioSource _throwSound;
        private readonly Collider[] _colliders = new Collider[10];

        private float _damage;
        private float _attackRadius;
        private int _projectilesCount;

        public IceStaff(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenterPoint,
            Dictionary<AbilityType, int> damageDealt, Dictionary<AbilityType, int> killCount, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);
            _heroCenter = heroCenterPoint.ThrowIfNull();

            _damage = stats.Damage;
            _attackRadius = stats.Range;
            _projectilesCount = stats.ProjectilesCount;
            _damageLayer = config.DamageLayer;

            _throwSound = config.ThrowSound.Instantiate(heroCenterPoint);
            _hitSoundPool = new(() => config.HitSound.Instantiate());
            _projectilePool = new(CreateIceSpike);

            IceSpike CreateIceSpike()
            {
                IceSpike iceSpike = config.ProjectilePrefab.Instantiate().GetComponentOrThrow<IceSpike>();
                iceSpike.Initialize(_damageLayer, _damage, _hitSoundPool, damageDealt, killCount);

                return iceSpike;
            }
        }

        public override void Dispose()
        {
            CoroutineService.StopAllCoroutines(this);

            _projectilePool.DestroyAll();
            _hitSoundPool.DestroyAll();
            _throwSound.DestroyGameObject();
        }

        protected override void Apply()
        {
            CoroutineService.StartCoroutine(LaunchSpikes(), this);
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();
            _damage = stats.Damage;
            _attackRadius = stats.Range;
            _projectilesCount = stats.ProjectilesCount;

            _projectilePool.ForEach(spike => spike.SetDamage(_damage));
        }

        private IEnumerator LaunchSpikes()
        {
            for (int i = Constants.Zero; i < _projectilesCount; i++)
            {
                int count = Physics.OverlapSphereNonAlloc(_heroCenter.position, _attackRadius, _colliders, _damageLayer);
                Vector3 direction;

                if (count == Constants.Zero)
                {
                    direction = Utilities.GenerateRandomDirection();
                }
                else
                {
                    Collider collider = _colliders
                        .Where(collider => collider.NotNull())
                        .OrderBy(collider => (collider.transform.position - _heroCenter.position).sqrMagnitude)
                        .First();

                    direction = (collider.transform.position - _heroCenter.position).normalized;
                }

                _projectilePool.Get().Fly(_heroCenter.position, direction);
                _throwSound.PlayRandomPitch();

                yield return _delay;
            }

            yield return null;
        }
    }
}
