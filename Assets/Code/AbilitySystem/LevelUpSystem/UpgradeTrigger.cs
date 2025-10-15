using Assets.Scripts;
using Assets.Scripts.Ui;
using Assets.Code.Tools;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Assets.Code.Data;
using Assets.Scripts.Factories;
using UnityEngine;

namespace Assets.Code.AbilitySystem
{
    public class UpgradeTrigger
    {
        private const int SuggestedUpgradesCount = 3;
        private const int CoinsCount = 5;

        private readonly HeroLevel _heroExperience;
        private readonly Dictionary<AbilityType, AbilityConfig> _abilityConfigs;
        private readonly AbilityContainer _abilityContainer;
        private readonly LevelUpWindow _levelUpWindow;
        private readonly AbilityFactory _abilityFactory;
        private readonly ITimeService _timeService;
        private readonly Dictionary<AbilityType, int> _abilityUnlockLevel;
        private readonly LootFactory _lootFactory;
        private readonly Transform _hero;

        public UpgradeTrigger
            (HeroLevel heroExperience, Dictionary<AbilityType, AbilityConfig> abilityConfigs,
            AbilityContainer abilityContainer, LevelUpWindow levelUpWindow, AbilityFactory abilityFactory,
            ITimeService timeService, Dictionary<AbilityType, int> abilityUnlockLevel, LootFactory lootFactory, Transform hero)
        {
            _heroExperience = heroExperience.ThrowIfNull();
            _abilityContainer = abilityContainer.ThrowIfNull();
            _abilityConfigs = abilityConfigs.ThrowIfNullOrEmpty();
            _levelUpWindow = levelUpWindow.ThrowIfNull();
            _abilityFactory = abilityFactory.ThrowIfNull();
            _timeService = timeService.ThrowIfNull();
            _abilityUnlockLevel = abilityUnlockLevel.ThrowIfNullOrEmpty();
            _lootFactory = lootFactory.ThrowIfNull();
            _hero = hero.ThrowIfNull();
        }

        ~UpgradeTrigger()
        {
            if (_heroExperience.NotNull())
            {
                _heroExperience.LevelChanged -= GenerateUpgrades;
            }

            if (_levelUpWindow.NotNull())
            {
                _levelUpWindow.UpgradeChosen -= UpgradeAbility;
            }
        }

        public bool IsOffering => _levelUpWindow.IsOn;

        public void Run()
        {
            _heroExperience.LevelChanged += GenerateUpgrades;
            _levelUpWindow.UpgradeChosen += UpgradeAbility;
        }

        public void Stop()
        {
            _levelUpWindow.Hide();

            _heroExperience.LevelChanged -= GenerateUpgrades;
            _levelUpWindow.UpgradeChosen -= UpgradeAbility;
        }

        private void GenerateUpgrades(int level)
        {
            if (IsOffering || level == Constants.One)
            {
                return;
            }

            List<AbilityType> possibleUpgrades = Constants.GetEnums<AbilityType>()
                .Except(_abilityContainer.MaxedAbilities)
                .Except(_abilityUnlockLevel.Where(pair => pair.Value == Constants.Zero).Select(pair => pair.Key))
                .ToList();

            List<UpgradeOption> upgradeOptions = new();

            for (int i = Constants.Zero; i < SuggestedUpgradesCount; i++)
            {
                if (possibleUpgrades.Count == Constants.Zero)
                {
                    break;
                }

                int index = Random.Range(Constants.Zero, possibleUpgrades.LastIndex());
                AbilityType abilityType = possibleUpgrades[index];
                possibleUpgrades.RemoveAt(index);

                int abilityLevel = _abilityContainer.GetAbilityLevel(abilityType);

                AbilityConfig abilityConfig = _abilityConfigs[abilityType];
                AbilityStats nextStats = abilityConfig.GetStats(abilityLevel + Constants.One);
                List<string> statsDescription;

                if (abilityLevel > Constants.Zero)
                {
                    statsDescription = (nextStats - abilityConfig.GetStats(abilityLevel)).GetStatsDescription();
                }
                else
                {
                    statsDescription = nextStats.GetStatsDescription();
                }

                upgradeOptions.Add(new(abilityType, abilityLevel, statsDescription, abilityConfig.Icon, UIText.AbilityName[abilityType]));
            }

            if (upgradeOptions.Count == Constants.Zero)
            {
                _lootFactory.Spawn(Loot.LootType.Coin, _hero.position, CoinsCount);
                _heroExperience.DecreaseLevelUpsCount();

                return;
            }

            _timeService.Pause();
            _levelUpWindow.Show(upgradeOptions, level);
        }

        private void UpgradeAbility(AbilityType abilityType)
        {
            abilityType.ThrowIfNull();

            switch (_abilityContainer.HasAbility(abilityType))
            {
                case true:
                    _abilityContainer.Upgrade(abilityType);
                    break;

                case false:
                    _abilityContainer.Add(_abilityFactory.Create(abilityType));
                    break;
            }

            _heroExperience.DecreaseLevelUpsCount();

            if (_heroExperience.LevelUpsCount > Constants.Zero)
            {
                GenerateUpgrades(_heroExperience.Level - _heroExperience.LevelUpsCount);
            }
            else
            {
                _timeService.Continue();
            }
        }
    }
}
