using Assets.Code.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code
{
    [CreateAssetMenu(menuName = "Game/AbilityConfig")]
    public class AbilityConfig : ScriptableObject
    {
        [field: Header("Base Settings")]
        [field: SerializeField] public Transform ProjectilePrefab { get; private set; }
        [field: SerializeField] public ParticleSystem Effect { get; private set; }
        [field: SerializeField] public LayerMask DamageLayer { get; private set; }
        [field: SerializeField] public AbilityType Type { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: Header("Sound")]
        [field: SerializeField] public AudioSource ThrowSound { get; private set; }
        [field: SerializeField] public AudioSource AppearingSound { get; private set; }
        [field: SerializeField] public AudioSource HitSound { get; private set; }
        [field: SerializeField] public AudioSource ProjectileSound { get; private set; }

        [Header("Upgrades")]
        [SerializeField] private List<AbilityStats> _abilityStats;

        public int[] UpgradesCost => _abilityStats.Select(stat => stat.Cost).ToArray();
        public int MaxLevel => _abilityStats.Count;

        public AbilityStats GetStats(int level)
        {
            level.ThrowIfZeroOrLess().ThrowIfMoreThan(MaxLevel);

            return _abilityStats[level - Constants.One];
        }
    }
}
