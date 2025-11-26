using Assets.Code;
using Assets.Code.AbilitySystem;
using Assets.Code.BuffSystem;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data;
using Assets.Code.InputActions;
using Assets.Code.Spawners;
using Assets.Code.Tools;
using Assets.Code.Ui;
using Assets.Scripts.Configs;
using Assets.Scripts.Factories;
using Assets.Scripts.Movement;
using Assets.Scripts.State_Machine;
using Assets.Scripts.Tools;
using Assets.Scripts.Ui;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace Assets.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private LevelSettings _levelSettings;
        [SerializeField] private UIConfig _uIConfig;

        private void Awake()
        {
            PlayerData playerData = YG2.saves.Load();
            HeroLevel heroLevel = new(_levelSettings.CalculateExperienceForNextLevel);

            ITimeService timeService = new TimeService();
            IInputService inputService = new InputReader(_uIConfig.JoystickCanvas.Instantiate(), timeService);
            IWalletService walletService = new WalletService(playerData.Wallet);

            GameAreaSettings gameAreaSettings = _levelSettings.GameAreaSettings;
            HeroComponents hero = _levelSettings.HeroConfig.Prefab
                .Instantiate(gameAreaSettings.Center, Quaternion.identity)
                .GetComponentOrThrow<HeroComponents>()
                .Initialize(inputService, _levelSettings.HeroConfig, heroLevel, walletService);

            Camera.main.GetComponentOrThrow<Follower>().Follow(hero.transform);

            LootFactory lootFactory = new(_levelSettings.Loots);
            EnemyFactory enemyFactory = new(_levelSettings.EnemyConfigs, lootFactory, hero.transform, _levelSettings.EnemySpawnerSettings,
                _levelSettings.GameAreaSettings, _levelSettings.GoldEnemy);


            Dictionary<AbilityType, AbilityConfig> abilityConfigs = _levelSettings.AbilityConfigs;
            LevelUpWindow levelUpWindow = new(_uIConfig.LevelUpCanvas, _uIConfig.LevelUpButton);

            AbilityFactory abilityFactory = new(abilityConfigs, hero.transform, hero.Center, playerData.AbilityUnlockLevel,
                playerData.BattleMetrics, lootFactory, hero.Animator, timeService);

            BuffFactory buffFactory = new(_levelSettings.BuffConfigs, hero);
            AbilityUpgradeGenerator abilityGenerator = new(hero.AbilityContainer, playerData.AbilityUnlockLevel, abilityConfigs);
            BuffUpgradeGenerator buffGenerator = new(hero.BuffContainer, _levelSettings.BuffConfigs);

            Upgrader upgrader = new(hero.AbilityContainer, abilityFactory, hero.BuffContainer, buffFactory);
            PseudoRandomUpgradeSelector upgradeSelector = new(abilityGenerator, buffGenerator);

            UpgradeTrigger upgradeTrigger = new(heroLevel, levelUpWindow,
                timeService, lootFactory, hero.transform, upgrader, upgradeSelector);

            UiFactory uiFactory = new(_uIConfig, _levelSettings.UpgradeCost, _levelSettings.AbilityConfigs, heroLevel, playerData,
                hero.LootCollector, walletService);

            AdRewarder adRewarder = new(hero, uiFactory);

            SpellBookSpawner bookSpawner = new(hero.transform, _levelSettings.Books, gameAreaSettings, _levelSettings.BooksSpawnerSettings);
            EnemySpawner enemySpawner = new(enemyFactory, _levelSettings.SpawnTypeByTimes);

            StateMachine stateMachine = new();

            stateMachine
                .AddState(new MenuState(stateMachine, uiFactory, _levelSettings.MenuMusic.Instantiate(hero.transform)))
                .AddState(new GameState(stateMachine, hero, enemySpawner, abilityFactory, uiFactory,
                playerData, inputService, timeService, upgradeTrigger, _levelSettings.BackgroundMusic, bookSpawner));

            stateMachine.SetState<MenuState>();
        }
    }
}
