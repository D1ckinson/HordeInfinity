using Assets.Code.AbilitySystem.Base;
using Assets.Code.BuffSystem.Base;
using Assets.Code.Core;
using Assets.Code.Data.SettingsStructures;
using Assets.Code.LootSystem;
using Assets.Code.SpellBooks.Base;
using Assets.Code.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Data.Base
{
    [CreateAssetMenu(menuName = "Game/GameConfig")]
    public partial class GameConfig : ScriptableObject
    {
        [field: Header("Characters Configs")]
        [field: SerializeField] private CharacterConfig[] _enemiesConfigs;
        [field: SerializeField] public CharacterConfig HeroConfig { get; private set; }
        [field: SerializeField] public CharacterConfig GoldEnemy { get; private set; }

        [field: Header("Loot Settings")]
        [field: SerializeField] public Loot[] Loots { get; private set; }
        [field: SerializeField] public LootSpawnSettings LootSpawnSettings { get; private set; }

        [field: Header("Enemy Spawn Settings")]
        [field: SerializeField] public EnemySpawnerSettings EnemySpawnerSettings { get; private set; }
        [field: SerializeField] public SpawnTypeByTime[] SpawnTypeByTimes { get; private set; }

        [field: Header("Book Spawn Settings")]
        [field: SerializeField] public BooksSpawnerSettings BooksSpawnerSettings { get; private set; }
        [field: SerializeField] public SpellBook[] Books { get; private set; }

        [field: Header("Game Area Settings")]
        [field: SerializeField] public GameAreaSettings GameAreaSettings { get; private set; }

        [field: Header("Abilities")]
        [field: SerializeField] private AbilityConfig[] _abilitiesConfigs;
        [field: SerializeField] private BuffConfig[] _buffConfigs;

        [field: Header("Music")]
        [field: SerializeField] public AudioSource BackgroundMusic { get; private set; }
        [field: SerializeField] public AudioSource MenuMusic { get; private set; }

        [Header("Level Formula Settings")]
        [SerializeField][Min(1)] private int _fixedExperience = 100;
        [SerializeField][Min(1)] private int _experienceCoefficient = 50;
        [SerializeField][Min(1)] private float _degree = 1.3f;

        public int CalculateExperienceForNextLevel(int current)
        {
            current.ThrowIfZeroOrLess();

            return (int)(_fixedExperience * current + _experienceCoefficient * MathF.Pow(current, _degree));
        }

        public Dictionary<AbilityType, int[]> UpgradeCost => _abilitiesConfigs.ToDictionary(config => config.Type, config => config.UpgradesCost);
        public Dictionary<AbilityType, AbilityConfig> AbilityConfigs => _abilitiesConfigs.ToDictionary(config => config.Type);
        public Dictionary<CharacterType, CharacterConfig> EnemyConfigs => _enemiesConfigs.ToDictionary(config => config.Type);
        public Dictionary<BuffType, BuffConfig> BuffConfigs => _buffConfigs.ToDictionary(config => config.Type);
    }
}
