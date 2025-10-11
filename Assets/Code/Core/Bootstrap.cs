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
using System.Linq;
using UnityEngine;
using YG;

namespace Assets.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private LevelSettings _levelSettings;
        [SerializeField] private UIConfig _uIConfig;

        private StateMachine _stateMachine;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_levelSettings.IsNull())
            {
                return;
            }

            Gizmos.color = Color.red;
            GameAreaSettings gameAreaSettings = _levelSettings.GameAreaSettings;

            Gizmos.DrawSphere(gameAreaSettings.Center, 1);
            CustomGizmos.DrawCircle(gameAreaSettings.Center, gameAreaSettings.Radius, Color.red);
        }
#endif

        private void Awake()
        {
            PlayerData playerData = YG2.saves.Load();
            playerData.Wallet.Add(1000);///////////////////////////////
            ITimeService timeService = new TimeService();
            HeroLevel heroLevel = new(_levelSettings.CalculateExperienceForNextLevel);
            UiFactory uiFactory = new(_uIConfig, playerData.Wallet, _levelSettings.UpgradeCost, _levelSettings.AbilityConfigs, playerData.AbilityUnlockLevel, heroLevel);

            IInputService inputService = new InputReader(uiFactory.Create<Joystick>(), timeService);

            GameAreaSettings gameAreaSettings = _levelSettings.GameAreaSettings;

            HeroComponents heroComponents = new HeroFactory(_levelSettings.HeroConfig, playerData.Wallet, heroLevel, inputService).Create(gameAreaSettings.Center);
            heroComponents.Initialize(heroLevel, gameAreaSettings.Center);
            uiFactory.AddLootCollector(heroComponents.LootCollector);

            Dictionary<AbilityType, AbilityConfig> abilities = _levelSettings.AbilityConfigs;

            LootFactory lootFactory = new(_levelSettings.Loots);
            AbilityFactory abilityFactory = new(abilities, heroComponents.transform, heroComponents.Center, playerData.AbilityUnlockLevel, lootFactory, heroComponents.Animator, timeService);
            EnemyFactory enemyFactory = new(_levelSettings.EnemyConfigs, lootFactory, heroComponents.transform, _levelSettings.EnemySpawnerSettings, gameAreaSettings, _levelSettings.GoldEnemy);

            LevelUpWindow levelUpWindow = new(_uIConfig.LevelUpCanvas, _uIConfig.LevelUpButton);
            UpgradeTrigger upgradeTrigger = new(heroLevel, abilities, heroComponents.AbilityContainer, levelUpWindow, abilityFactory, timeService);

            EnemySpawner enemySpawner = new(enemyFactory, _levelSettings.SpawnTypeByTimes);

            uiFactory.Create<FPSWindow>();
            AudioSource menuMusic = _levelSettings.MenuMusic.Instantiate(heroComponents.transform);

            _stateMachine = new();
            _stateMachine
                .AddState(new MenuState(_stateMachine, uiFactory, menuMusic))
                .AddState(new GameState(_stateMachine, heroComponents, enemySpawner, abilityFactory, uiFactory, playerData, inputService, timeService, upgradeTrigger, _levelSettings.BackgroundMusic));

            _stateMachine.SetState<MenuState>();
        }

        private void Update()
        {
            _stateMachine.Update();
        }
    }
}
