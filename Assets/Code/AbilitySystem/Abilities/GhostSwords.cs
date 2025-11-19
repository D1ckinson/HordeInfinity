using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Abilities
{
    public class GhostSwords : Ability
    {
        private readonly WaitForSeconds _actionDelay = new(0.05f);
        private readonly Transform _heroCenter;
        private readonly Pool<GhostSword> _pool;
        private readonly List<GhostSword> _spawnedSwords = new();
        private readonly AudioSource _throwSound;
        private readonly AudioSource _appearingSound;

        public GhostSwords(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, Transform heroCenter,
            BattleMetrics battleMetrics, int level = 1) : base(config, abilityUnlockLevel, battleMetrics, level)
        {
            _heroCenter = heroCenter.ThrowIfNull();

            _throwSound = config.ThrowSound.Instantiate(heroCenter.transform);
            _appearingSound = config.AppearingSound.Instantiate(heroCenter.transform);

            GhostSword CreateSword()
            {
                GhostSword sword = config.ProjectilePrefab
                    .GetComponentOrThrow<GhostSword>()
                    .Instantiate()
                    .Initialize(CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(BoolStatType.IsPiercing), config.DamageLayer);

                sword.Hit += RecordHitResult;

                return sword;
            }

            _pool = new(CreateSword);
        }

        protected override void Apply()
        {
            CoroutineService.StartCoroutine(SpawnSwords(), this);
        }

        private IEnumerator SpawnSwords()
        {
            for (int i = Constants.Zero; i < CurrentStats.Get(IntStatType.ProjectilesCount); i++)
            {
                GhostSword sword = _pool.Get(false);

                float angle = i * (Constants.FullCircleDegrees / CurrentStats.Get(IntStatType.ProjectilesCount));

                Vector3 localPosition = CalculateSwordLocalPosition(angle);
                sword.transform.SetParent(_heroCenter);
                sword.transform.localPosition = localPosition;

                sword.transform.localRotation = Quaternion.LookRotation(localPosition.normalized);

                sword.SetActive(true);
                _spawnedSwords.Add(sword);
                _appearingSound.PlayRandomPitch();

                yield return _actionDelay;
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
            for (int i = Constants.Zero; i < _spawnedSwords.Count; i++)
            {
                _spawnedSwords[i].Launch();

                yield return _actionDelay;
            }

            _spawnedSwords.Clear();
        }

        protected override void OnDispose()
        {
            CoroutineService.StopAllCoroutines(this);

            _pool.ForEach(sword => sword.Hit -= RecordHitResult);
            _pool.DestroyAll();

            _appearingSound.DestroyGameObject();
            _throwSound.DestroyGameObject();
        }

        protected override void OnStatsUpdate()
        {
            _pool.ForEach(sword => sword.SetStats(CurrentStats.Get(FloatStatType.Damage), CurrentStats.Get(BoolStatType.IsPiercing)));
        }
    }
}
