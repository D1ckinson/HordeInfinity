using Assets.Code.Tools;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class GhostSwords : Ability
    {
        private readonly WaitForSeconds _actionWait = new(0.05f);
        private readonly Transform _heroCenter;
        private readonly Pool<GhostSword> _pool;
        private readonly List<GhostSword> _spawnedSwords = new();

        private float _damage;
        private int _projectilesCount;

        public GhostSwords(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter, int level = 1) : base(config, abilityUnlockLevel, level)
        {
            AbilityStats stats = config.ThrowIfNull().GetStats(level.ThrowIfZeroOrLess());
            _damage = stats.Damage.ThrowIfNegative();
            _projectilesCount = stats.ProjectilesCount.ThrowIfNegative();
            _heroCenter = heroCenter.ThrowIfNull();

            GhostSword CreateSword()
            {
                GhostSword sword = config.ProjectilePrefab.GetComponentOrThrow<GhostSword>().Instantiate();
                sword.Initialize(_damage, stats.IsPiercing, config.DamageLayer);

                return sword;
            }

            _pool = new(CreateSword);
        }

        protected override void Apply()
        {
            CoroutineService.StartCoroutine(SpawnSwords(), this);
        }

        protected override void UpdateStats(AbilityStats stats)
        {
            stats.ThrowIfNull();
            _damage = stats.Damage;
            _projectilesCount = stats.ProjectilesCount;
            _pool.ForEach(sword => sword.SetStats(_damage, stats.IsPiercing));
        }

        private IEnumerator SpawnSwords()
        {
            for (int i = Constants.Zero; i < _projectilesCount; i++)
            {
                GhostSword sword = _pool.Get(false);

                float angle = i * (Constants.FullCircleDegrees / _projectilesCount);

                Vector3 localPosition = CalculateSwordLocalPosition(angle);
                sword.transform.SetParent(_heroCenter);
                sword.transform.localPosition = localPosition;

                sword.transform.localRotation = Quaternion.LookRotation(localPosition.normalized);

                sword.SetActive(true);
                _spawnedSwords.Add(sword);

                yield return _actionWait;
            }

            CoroutineService.StartCoroutine(LaunchSwords(), this);
        }

        private Vector3 CalculateSwordLocalPosition(float angle)
        {
            float radianAngle = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(radianAngle) * Constants.One;
            float z = Mathf.Sin(radianAngle) * Constants.One;

            return new Vector3(x, Constants.Zero, z);
        }

        private IEnumerator LaunchSwords()
        {
            for (int i = _spawnedSwords.LastIndex(); i >= Constants.Zero; i--)
            {
                _spawnedSwords[i].Launch();
                _spawnedSwords.RemoveAt(i);

                yield return _actionWait;
            }
        }

        public override void Dispose()
        {
            CoroutineService.StopAllCoroutines(this);
            _pool.DestroyAll();
        }
    }
}
