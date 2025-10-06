using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class Bombard : Ability
    {
        private const float MaxThrowDistance = 10f;

        private readonly WaitForSeconds _delay = new(0.1f);
        private readonly Pool<Bomb> _bombPool;
        private readonly Pool<ParticleSystem> _effectPool;

        private float _damage;
        private float _explosionRadius;
        private float _projectilesCount;

        public Bombard(AbilityConfig config, Transform transform, Dictionary<AbilityType, int> abilityUnlockLevel, int level = 1) : base(config, transform, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level);

            _damage = stats.Damage;
            _projectilesCount = stats.ProjectilesCount;
            _explosionRadius = stats.Range;

            _effectPool = new(() => config.Effect.Instantiate());
            _bombPool = new(CreateBomb);

            Bomb CreateBomb()
            {
                Bomb bomb = config.ProjectilePrefab.GetComponentOrThrow<Bomb>().Instantiate();
                bomb.Initialize(_damage, _explosionRadius, config.DamageLayer, _effectPool);

                return bomb;
            }
        }

        protected override void Apply()
        {
            CoroutineService.StartCoroutine(LaunchBombs(), this);
        }

        private IEnumerator LaunchBombs()
        {
            for (int i = Constants.Zero; i < _projectilesCount; i++)
            {
                Bomb bomb = _bombPool.Get();
                bomb.Fly(Position, GenerateRandomPoint());

                yield return _delay;
            }
        }

        public override void Dispose()
        {
            CoroutineService.StopAllCoroutines(this);
            _bombPool.DestroyAll();
        }

        protected override void UpdateStats(float damage, float range, int projectilesCount, bool isPiercing, int healthPercent, float pullForce)
        {
            _damage = damage.ThrowIfNegative();
            _projectilesCount = projectilesCount.ThrowIfNegative();
            _explosionRadius = range.ThrowIfNegative();

            _bombPool.ForEach(bomb => bomb.SetStats(_damage, _explosionRadius));
        }

        private Vector3 GenerateRandomPoint()
        {
            Vector3 distance = Utilities.GenerateRandomDirection() * Random.Range(Constants.One, MaxThrowDistance);
            Vector3 point = Position + distance;

            return point;
        }
    }
}
