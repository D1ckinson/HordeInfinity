using Assets.Code;
using Assets.Code.AbilitySystem;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data;
using Assets.Code.InputActions;
using Assets.Code.Spawners;
using Assets.Code.Tools;
using Assets.Code.Ui;
using Assets.Code.Ui.Windows;
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

        private StateMachine _stateMachine;

        private void Awake()
        {
            PlayerData playerData = YG2.saves.Load();
            HeroLevel heroLevel = new(_levelSettings.CalculateExperienceForNextLevel);

            ITimeService timeService = new TimeService();
            IInputService inputService = new InputReader(_uIConfig.JoystickCanvas.Instantiate(), timeService);

            GameAreaSettings gameAreaSettings = _levelSettings.GameAreaSettings;
            HeroComponents hero = _levelSettings.HeroConfig.Prefab
                .Instantiate(gameAreaSettings.Center, Quaternion.identity)
                .GetComponentOrThrow<HeroComponents>()
                .Initialize(inputService, _levelSettings.HeroConfig, heroLevel, playerData.Wallet);

            Camera.main.GetComponentOrThrow<Follower>().Follow(hero.transform);

            LootFactory lootFactory = new(_levelSettings.Loots);
            EnemyFactory enemyFactory = new(_levelSettings.EnemyConfigs, lootFactory, hero.transform, _levelSettings.EnemySpawnerSettings,
                _levelSettings.GameAreaSettings, _levelSettings.GoldEnemy);

            EnemySpawner enemySpawner = new(enemyFactory, _levelSettings.SpawnTypeByTimes);

            Dictionary<AbilityType, AbilityConfig> abilities = _levelSettings.AbilityConfigs;
            LevelUpWindow levelUpWindow = new(_uIConfig.LevelUpCanvas, _uIConfig.LevelUpButton);

            AbilityFactory abilityFactory = new(abilities, hero.transform, hero.Center, playerData.AbilityUnlockLevel,
                playerData.DamageDealt, playerData.KillCount, lootFactory, hero.Animator, timeService);

            UpgradeTrigger upgradeTrigger = new(heroLevel, abilities, hero.AbilityContainer, levelUpWindow, abilityFactory,
                timeService, playerData.AbilityUnlockLevel, lootFactory, hero.transform);

            UiFactory uiFactory = new(_uIConfig, _levelSettings.UpgradeCost, _levelSettings.AbilityConfigs, heroLevel, playerData,
                hero.LootCollector);

            uiFactory.Create<FPSWindow>();

            SpellBookSpawner bookSpawner = new(hero.transform, _levelSettings.Books, gameAreaSettings, _levelSettings.BooksSpawnerSettings);

            _stateMachine = new();
            _stateMachine
                .AddState(new MenuState(_stateMachine, uiFactory, _levelSettings.MenuMusic.Instantiate(hero.transform)))
                .AddState(new GameState(_stateMachine, hero, new(enemyFactory, _levelSettings.SpawnTypeByTimes), abilityFactory, uiFactory,
                playerData, inputService, timeService, upgradeTrigger, _levelSettings.BackgroundMusic, bookSpawner));

            _stateMachine.SetState<MenuState>();
        }

        private void Update()
        {
            _stateMachine.Update();
        }
    }
}
