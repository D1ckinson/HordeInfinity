using Assets.Code.AbilitySystem.Base;
using Assets.Code.AbilitySystem.Projectiles;
using Assets.Code.AbilitySystem.StatTypes;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
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

        public IceStaff(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenterPoint,
            BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _heroCenter = heroCenterPoint.ThrowIfNull();
            _damageLayer = config.ThrowIfNull().DamageLayer;

            _throwSound = config.ThrowSound.Instantiate(heroCenterPoint);
            _hitSoundPool = new(() => config.HitSound.Instantiate());
            _projectilePool = new(CreateIceSpike);

            IceSpike CreateIceSpike()
            {
                IceSpike spike = config.ProjectilePrefab
                    .Instantiate()
                    .GetComponentOrThrow<IceSpike>()
                    .Initialize(_damageLayer, CurrentStats.Get(FloatStatType.Damage), _hitSoundPool);

                spike.Hit += RecordHitResult;

                return spike;
            }
        }

        protected override void OnDispose()
        {
            CoroutineService.StopAllCoroutines(this);

            _projectilePool.ForEach(spike => spike.Hit -= RecordHitResult);
            _projectilePool.DestroyAll();

            _hitSoundPool.DestroyAll();
            _throwSound.DestroyGameObject();
        }

        protected override void Apply()
        {
            CoroutineService.StartCoroutine(LaunchSpikes(), this);
        }

        protected override void OnStatsUpdate()
        {
            _projectilePool.ForEach(spike => spike.SetDamage(CurrentStats.Get(FloatStatType.Damage)));
        }

        private IEnumerator LaunchSpikes()
        {
            for (int i = Constants.Zero; i < CurrentStats.Get(IntStatType.ProjectilesCount); i++)
            {
                int count = Physics.OverlapSphereNonAlloc(_heroCenter.position, CurrentStats.Get(FloatStatType.Range), _colliders, _damageLayer);
                Vector3 direction;

                if (count == Constants.Zero)
                {
                    direction = Utilities.GenerateRandomDirection();
                }
                else
                {
                    Collider collider = _colliders
                        .Where(collider => collider.IsNotNull())
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
