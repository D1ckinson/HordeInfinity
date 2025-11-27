using Assets.Code.AbilitySystem.Base;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Code.Ui.Windows
{
    public class ShopWindow : BaseWindow
    {
        [SerializeField] private TMP_Text _coinsQuantity;
        [SerializeField] private Transform _upgradeScrollView;
        [SerializeField] private Transform _startAbilityScrollView;
        [SerializeField] private Transform _upgradeGroup;
        [SerializeField] private Transform _startAbilityGroup;
        [SerializeField] private TMP_Text _tipText;
        [SerializeField] private Transform _tip;

        [SerializeField] private Button _enableUpgradeButton;
        [SerializeField] private Button _enableStartAbilityButton;

        [field: SerializeField] public Button ExitButton { get; private set; }

        private readonly Dictionary<AbilityType, ShopOption> _options = new();
        private readonly Dictionary<AbilityType, StartAbilityOption> _startOptions = new();

        private readonly Color _upgradeMaxed = Color.yellow;
        private readonly Color _upgradeAvailable = ColorUtility.TryParseHtmlString("#00B400", out Color color) ? color : Color.green;
        private readonly Color _upgradeUnavailable = Color.black;

        private Dictionary<AbilityType, int> _abilityMaxLevel;
        private Dictionary<AbilityType, int[]> _upgradeCost;
        private PlayerData _playerData;
        private IWalletService _walletService;

        private void Awake()
        {
            ExitButton.Subscribe(Disable);

            _enableUpgradeButton.Subscribe(EnableShopList);
            _enableStartAbilityButton.Subscribe(EnableStartAbilityList);
        }

        private void OnDestroy()
        {
            foreach (ShopOption shopOption in _options.Values)
            {
                if (shopOption.IsNotNull() && shopOption.UpgradeButton.IsNotNull())
                {
                    shopOption.UpgradeButton.UnsubscribeAll();
                }
            }

            foreach (StartAbilityOption shopOption in _startOptions.Values)
            {
                if (shopOption.IsNotNull() && shopOption.ChoseButton.IsNotNull())
                {
                    shopOption.ChoseButton.UnsubscribeAll();
                }
            }

            if (_walletService.IsNotNull())
            {
                _walletService.ValueChanged -= UpdateAllOptions;
            }

            ExitButton.Unsubscribe(Disable);
        }

        public ShopWindow Initialize(
            Dictionary<AbilityType, int> abilityMaxLevel,
            Dictionary<AbilityType, int[]> upgradeCost,
            PlayerData playerData,
            IWalletService walletService)
        {
            _abilityMaxLevel = abilityMaxLevel.ThrowIfNullOrEmpty();
            _upgradeCost = upgradeCost.ThrowIfNullOrEmpty();
            _playerData = playerData.ThrowIfNull();
            _walletService = walletService.ThrowIfNull();

            _coinsQuantity.SetText(_walletService.CoinsQuantity);
            _walletService.ValueChanged += UpdateAllOptions;

            _tipText.SetText(UIText.Tip);

            return this;
        }

        public void AddOption(ShopOption option)
        {
            option.ThrowIfNull().transform.SetParent(_upgradeGroup, false);
            AbilityType abilityType = option.AbilityType;

            option.LevelNumber.SetText(_playerData.AbilityUnlockLevel[abilityType]);
            UpdateOption(option, _walletService.CoinsQuantity);

            option.UpgradeButton.Subscribe(() => IncreaseUnlockLevel(abilityType));
            _options.Add(option.AbilityType, option);
        }

        public void AddStartAbilityOption(StartAbilityOption option)
        {
            option.ThrowIfNull().transform.SetParent(_startAbilityGroup, false);

            option.GoalText.SetText(_playerData.UnlockDamage[option.AbilityType]);
            UpdateStartOption(option);

            option.ChoseButton.Subscribe(() => SetStartAbility(option));

            _startOptions.Add(option.AbilityType, option);
        }

        protected override void OnEnableMethod()
        {
            UpdateAllStartOptions();
        }

        private void SetStartAbility(StartAbilityOption option)
        {
            _startOptions[_playerData.StartAbility].ChoseButton.SetColor(_upgradeAvailable);
            _playerData.StartAbility = option.AbilityType;

            _startOptions.ForEachValues(option => option.CheckMarkIcon.SetActive(false));
            option.CheckMarkIcon.SetActive(true);
            option.ChoseButton.SetColor(_upgradeMaxed);
        }

        private void EnableStartAbilityList()
        {
            _startAbilityScrollView.SetActive(true);
            _upgradeScrollView.SetActive(false);

            _enableStartAbilityButton.interactable = false;
            _enableUpgradeButton.interactable = true;
            _tip.SetActive(true);
        }

        private void EnableShopList()
        {
            _startAbilityScrollView.SetActive(false);
            _upgradeScrollView.SetActive(true);

            _enableStartAbilityButton.interactable = true;
            _enableUpgradeButton.interactable = false;
            _tip.SetActive(false);
        }

        private void SetStatus(StartAbilityOption option)
        {
            bool isLocked = _playerData.BattleMetrics.DamageDealt[option.AbilityType] < _playerData.UnlockDamage[option.AbilityType];

            if (_playerData.StartAbility == option.AbilityType)
            {
                option.ChoseButton.SetColor(_upgradeMaxed);
                option.LockIcon.SetActive(false);
                option.CheckMarkIcon.SetActive(true);
            }
            else if (isLocked)
            {
                option.ChoseButton.SetColor(_upgradeUnavailable);
                option.LockIcon.SetActive(true);
                option.CheckMarkIcon.SetActive(false);
                option.ChoseButton.interactable = false;
            }
            else
            {
                option.ChoseButton.SetColor(_upgradeAvailable);
                option.LockIcon.SetActive(false);
                option.CheckMarkIcon.SetActive(false);
                option.ChoseButton.interactable = true;
            }
        }

        private void UpdateOption(ShopOption option, int coinsQuantity)
        {
            AbilityType abilityType = option.AbilityType;
            int unlockLevel = _playerData.AbilityUnlockLevel[abilityType];
            int maxLevel = _abilityMaxLevel[abilityType];

            if (unlockLevel == maxLevel)
            {
                option.OfferDescription.SetActive(false);
                option.LevelMaxText.SetActive(true);
                option.UpgradeButton.interactable = false;
                option.UpgradeButton.SetColor(_upgradeMaxed);
            }
            else if (unlockLevel < maxLevel)
            {
                int[] cost = _upgradeCost[abilityType];
                int upgradeCost = cost[unlockLevel];
                option.Cost.SetText(upgradeCost);

                if (coinsQuantity >= upgradeCost)
                {
                    option.UpgradeButton.SetColor(_upgradeAvailable);
                    option.UpgradeButton.interactable = true;
                }
                else
                {
                    option.UpgradeButton.SetColor(_upgradeUnavailable);
                    option.UpgradeButton.interactable = false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void IncreaseUnlockLevel(AbilityType abilityType)
        {
            int unlockLevel = _playerData.AbilityUnlockLevel[abilityType];
            _walletService.Spend(_upgradeCost[abilityType][unlockLevel]);
            _playerData.AbilityUnlockLevel[abilityType] = ++unlockLevel;
            _options[abilityType].LevelNumber.SetText(unlockLevel);

            UpdateAllOptions(_walletService.CoinsQuantity);
            YG2.saves.Save(_playerData);
        }

        private void UpdateAllOptions(int coinsQuantity)
        {
            _coinsQuantity.SetText(coinsQuantity);
            _options.ForEachValues(option => UpdateOption(option, coinsQuantity));
        }

        private void UpdateAllStartOptions()
        {
            _startOptions.ForEachValues(option => UpdateStartOption(option));
        }

        private void UpdateStartOption(StartAbilityOption option)
        {
            int dealtDamage = _playerData.BattleMetrics.DamageDealt[option.AbilityType];
            int unlockDamage = _playerData.UnlockDamage[option.AbilityType];

            if (dealtDamage >= unlockDamage)
            {
                option.ProgressText.SetText(unlockDamage);
            }
            else
            {
                option.ProgressText.SetText(dealtDamage);
            }

            option.BarFilling.fillAmount = (float)dealtDamage / unlockDamage;

            SetStatus(option);
        }
    }
}
