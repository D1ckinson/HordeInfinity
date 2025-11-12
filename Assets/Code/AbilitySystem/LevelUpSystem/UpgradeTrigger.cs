using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Factories;
using Assets.Scripts.Ui;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem
{
    public class UpgradeTrigger
    {
        private const int SuggestedUpgradesCount = 3;
        private const int CoinsCount = 5;

        private readonly HeroLevel _heroLevel;
        private readonly LevelUpWindow _levelUpWindow;
        private readonly ITimeService _timeService;
        private readonly LootFactory _lootFactory;
        private readonly Transform _hero;
        private readonly Upgrader _upgrader;
        private readonly PseudoRandomUpgradeSelector _upgradeSelector;

        public UpgradeTrigger(
            HeroLevel heroLevel,
            LevelUpWindow levelUpWindow,
            ITimeService timeService,
            LootFactory lootFactory,
            Transform hero,
            Upgrader upgrader,
            PseudoRandomUpgradeSelector upgradeSelector)
        {
            _heroLevel = heroLevel.ThrowIfNull();
            _levelUpWindow = levelUpWindow.ThrowIfNull();
            _timeService = timeService.ThrowIfNull();
            _lootFactory = lootFactory.ThrowIfNull();
            _hero = hero.ThrowIfNull();
            _upgrader = upgrader.ThrowIfNull();
            _upgradeSelector = upgradeSelector.ThrowIfNull();
        }

        ~UpgradeTrigger()
        {
            if (_heroLevel.IsNotNull())
            {
                _heroLevel.LevelChanged -= GenerateUpgrades;
            }

            if (_levelUpWindow.IsNotNull())
            {
                _levelUpWindow.UpgradeChosen -= UpgradeAbility;
            }
        }

        public bool IsOffering => _levelUpWindow.IsOn;

        public void Run()
        {
            _heroLevel.LevelChanged += GenerateUpgrades;
            _levelUpWindow.UpgradeChosen += UpgradeAbility;
        }

        public void Stop()
        {
            _levelUpWindow.Hide();
            _upgradeSelector.Reset();

            _heroLevel.LevelChanged -= GenerateUpgrades;
            _levelUpWindow.UpgradeChosen -= UpgradeAbility;
        }

        private void GenerateUpgrades(int level)
        {
            if (IsOffering || level == Constants.One)
            {
                return;
            }

            List<UpgradeOption> upgradeOptions = _upgradeSelector.Generate(SuggestedUpgradesCount);

            if (upgradeOptions.Count == Constants.Zero)
            {
                _lootFactory.Spawn(Loot.LootType.Coin, _hero.position, CoinsCount);
                _heroLevel.DecreaseLevelUpsCount();

                return;
            }

            _timeService.Pause();
            _levelUpWindow.Show(upgradeOptions, level);
        }

        private void UpgradeAbility(Enum abilityType)
        {
            _upgrader.Upgrade(abilityType);
            _heroLevel.DecreaseLevelUpsCount();

            if (_heroLevel.LevelUpsCount > Constants.Zero)
            {
                GenerateUpgrades(_heroLevel.Level - _heroLevel.LevelUpsCount);
            }
            else
            {
                _timeService.Continue();
            }
        }
    }
}
