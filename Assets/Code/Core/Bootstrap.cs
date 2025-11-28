using Assets.Code.AbilitySystem.Base;
using Assets.Code.BuffSystem.Base;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.CharactersLogic.Movement.DirectionSources;
using Assets.Code.Data.Base;
using Assets.Code.Data.SettingsStructures;
using Assets.Code.EnemySpawnLogic;
using Assets.Code.InputActions;
using Assets.Code.LevelUpSystem;
using Assets.Code.LootSystem;
using Assets.Code.SpellBooks.Base;
using Assets.Code.StateMachineLogic.Base;
using Assets.Code.StateMachineLogic.States;
using Assets.Code.Tools.Base;
using Assets.Code.Ui.Base;
using Assets.Code.Ui.LevelUp;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace Assets.Code.Core
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private UIConfig _uIConfig;

        private void Awake()
        {
            PlayerData playerData = YG2.saves.Load();
            HeroLevel heroLevel = new(_gameConfig.CalculateExperienceForNextLevel);

            ITimeService timeService = new TimeService();
            IInputService inputService = new InputReader(_uIConfig.JoystickCanvas.Instantiate(), timeService);
            IWalletService walletService = new WalletService(playerData.Wallet);

            GameAreaSettings gameAreaSettings = _gameConfig.GameAreaSettings;
            HeroComponents hero = _gameConfig.HeroConfig.Prefab
                .Instantiate(gameAreaSettings.Center, Quaternion.identity)
                .GetComponentOrThrow<HeroComponents>()
                .Initialize(inputService, _gameConfig.HeroConfig, heroLevel, walletService);

            Camera.main.GetComponentOrThrow<Follower>().Follow(hero.transform);

            LootSpawner lootSpawner = new(_gameConfig.Loots,_gameConfig.LootSpawnSettings);
            EnemyFactory enemyFactory = new(_gameConfig.EnemyConfigs, lootSpawner, hero.transform, _gameConfig.EnemySpawnerSettings,
                _gameConfig.GameAreaSettings, _gameConfig.GoldEnemy);


            Dictionary<AbilityType, AbilityConfig> abilityConfigs = _gameConfig.AbilityConfigs;
            LevelUpWindow levelUpWindow = new(_uIConfig.LevelUpCanvas, _uIConfig.LevelUpButton);

            AbilityFactory abilityFactory = new(abilityConfigs, hero.transform, hero.Center, playerData.AbilityUnlockLevel,
                playerData.BattleMetrics, lootSpawner, hero.Animator, timeService);

            BuffFactory buffFactory = new(_gameConfig.BuffConfigs, hero);
            AbilityUpgradeGenerator abilityGenerator = new(hero.AbilityContainer, playerData.AbilityUnlockLevel, abilityConfigs);
            BuffUpgradeGenerator buffGenerator = new(hero.BuffContainer, _gameConfig.BuffConfigs);

            Upgrader upgrader = new(hero.AbilityContainer, abilityFactory, hero.BuffContainer, buffFactory);
            PseudoRandomUpgradeSelector upgradeSelector = new(abilityGenerator, buffGenerator);

            UpgradeTrigger upgradeTrigger = new(heroLevel, levelUpWindow,
                timeService, lootSpawner, hero.transform, upgrader, upgradeSelector);

            UiFactory uiFactory = new(_uIConfig, _gameConfig.UpgradeCost, _gameConfig.AbilityConfigs, heroLevel, playerData,
                hero.LootCollector, walletService);

            AdRewarder adRewarder = new(hero, uiFactory);

            SpellBookSpawner bookSpawner = new(hero.transform, _gameConfig.Books, gameAreaSettings, _gameConfig.BooksSpawnerSettings);
            EnemySpawner enemySpawner = new(enemyFactory, _gameConfig.SpawnTypeByTimes);

            StateMachine stateMachine = new();

            stateMachine
                .AddState(new MenuState(stateMachine, uiFactory, _gameConfig.MenuMusic.Instantiate(hero.transform)))
                .AddState(new GameState(stateMachine, hero, enemySpawner, abilityFactory, uiFactory,
                playerData, inputService, timeService, upgradeTrigger, _gameConfig.BackgroundMusic, bookSpawner));

            stateMachine.SetState<MenuState>();
        }
    }
}
