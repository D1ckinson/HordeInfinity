using Assets.Code.Core;
using Assets.Code.Data.Base;
using Assets.Code.LootSystem;
using Assets.Code.Tools.Base;
using Assets.Code.Ui.LevelUp;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.LevelUpSystem
{
    public class UpgradeTrigger
    {
        private const int SuggestedUpgradesCount = 3;

        private readonly HeroLevel _heroLevel;
        private readonly LevelUpWindow _levelUpWindow;
        private readonly ITimeService _timeService;
        private readonly LootSpawner _lootSpawner;
        private readonly Transform _hero;
        private readonly Upgrader _upgrader;
        private readonly PseudoRandomUpgradeSelector _upgradeSelector;

        public UpgradeTrigger(
            HeroLevel heroLevel,
            LevelUpWindow levelUpWindow,
            ITimeService timeService,
            LootSpawner lootSpawner,
            Transform hero,
            Upgrader upgrader,
            PseudoRandomUpgradeSelector upgradeSelector)
        {
            _heroLevel = heroLevel.ThrowIfNull();
            _levelUpWindow = levelUpWindow.ThrowIfNull();
            _timeService = timeService.ThrowIfNull();
            _lootSpawner = lootSpawner.ThrowIfNull();
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
                _lootSpawner.Spawn(LootType.Coin, _hero.position, level);
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
