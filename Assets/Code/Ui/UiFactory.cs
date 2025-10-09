using Assets.Code.Data;
using Assets.Code.Tools;
using Assets.Code.Ui.Windows;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Assets.Code.Ui
{
    public class UiFactory
    {
        private readonly Dictionary<Type, BaseWindow> _windows = new();
        private readonly Dictionary<Type, Func<BaseWindow>> _createMethods;
        private readonly UiCanvas _canvas;
        private readonly UIConfig _uIConfig;
        private readonly Wallet _wallet;
        private readonly Dictionary<AbilityType, AbilityConfig> _abilityConfigs;
        private readonly Dictionary<AbilityType, int[]> _upgradeCost;
        private readonly Dictionary<AbilityType, int> _abilityUnlockLevel;
        private readonly Dictionary<AbilityType, int> _abilityMaxLevel;
        private readonly HeroLevel _heroLevel;

        private LootCollector _lootCollector;

        public UiFactory(UIConfig uIConfig, Wallet wallet, Dictionary<AbilityType, int[]> upgradeCost,
            Dictionary<AbilityType, AbilityConfig> abilityConfigs, Dictionary<AbilityType, int> abilityUnlockLevel,
            HeroLevel heroLevel)
        {
            _uIConfig = uIConfig.ThrowIfNull();
            _wallet = wallet.ThrowIfNull();
            _upgradeCost = upgradeCost.ThrowIfNullOrEmpty();
            _abilityConfigs = abilityConfigs.ThrowIfNullOrEmpty();
            _abilityUnlockLevel = abilityUnlockLevel.ThrowIfNullOrEmpty();
            _abilityMaxLevel = abilityConfigs.ToDictionary(pair => pair.Key, pair => pair.Value.MaxLevel);
            _canvas = Object.Instantiate(_uIConfig.TestCanvasUiFactory);
            _heroLevel = heroLevel.ThrowIfNull();

            _createMethods = new()
            {
                [typeof(DeathWindow)] = CreateDeathWindow,
                [typeof(FadeWindow)] = CreateFadeWindow,
                [typeof(FPSWindow)] = CreateFPSView,
                [typeof(MenuWindow)] = CreateMenuWindow,
                [typeof(ShopWindow)] = CreateShopWindow,
                [typeof(Joystick)] = CreateJoystick,
                [typeof(LeaderboardWindow)] = CreateLeaderboardWindow,
                [typeof(PauseWindow)] = CreatePauseWindow,
                [typeof(GameWindow)] = CreateGameWindow
            };
        }

        public void AddLootCollector(LootCollector collector)
        {
            _lootCollector = collector.ThrowIfNull();
        }

        public T Create<T>(bool isActive = true) where T : BaseWindow
        {
            Type windowType = typeof(T);

            if (_windows.TryGetValue(windowType, out BaseWindow window) == false)
            {
                window = _createMethods[windowType].Invoke();
                _windows.Add(windowType, window);
            }

            window.SetActive(isActive);

            return (T)window;
        }

        private BaseWindow CreateFadeWindow()
        {
            return _uIConfig.FadeWindow.Instantiate(_canvas.FadeContainer, false);
        }

        private BaseWindow CreateDeathWindow()
        {
            return _uIConfig.DeathWindow.Instantiate(_canvas.DeathWindowPoint, false).Initialize();
        }

        private BaseWindow CreateFPSView()
        {
            return _uIConfig.FPSWindow.Instantiate(_canvas.Container, false);
        }

        private BaseWindow CreateMenuWindow()
        {
            return _uIConfig.MenuWindow.Instantiate(_canvas.Container, false).Initialize(_wallet);
        }

        private BaseWindow CreateShopWindow()
        {
            ShopWindow shopWindow = _uIConfig.ShopWindow.Instantiate(_canvas.Container, false).Initialize(_abilityUnlockLevel, _abilityMaxLevel, _upgradeCost, _wallet);

            foreach (AbilityType abilityType in Constants.GetEnums<AbilityType>())
            {
                ShopOption shopOption = _uIConfig.ShopOption.Instantiate().Initialize(abilityType);
                shopOption.AbilityIcon.sprite = _abilityConfigs[abilityType].Icon;

                shopWindow.AddOption(shopOption);
            }

            return shopWindow;
        }

        private BaseWindow CreateJoystick()
        {
            return _uIConfig.Joystick.Instantiate(_canvas.Container, false);
        }

        private BaseWindow CreateLeaderboardWindow()
        {
            return _uIConfig.Leaderboard.Instantiate(_canvas.Container, false);
        }

        private BaseWindow CreatePauseWindow()
        {
            return _uIConfig.PauseWindow.Instantiate(_canvas.Container, false);
        }

        private BaseWindow CreateGameWindow()
        {
            return _uIConfig.GameWindow.Instantiate(_canvas.Container, false).Initialize(_lootCollector, _heroLevel);
        }
    }
}