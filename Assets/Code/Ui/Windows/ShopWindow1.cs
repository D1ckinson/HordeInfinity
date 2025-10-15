using Assets.Code.Data;
using Assets.Code.Tools;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Code.Ui.Windows
{
    public class ShopWindow1 : BaseWindow
    {
        [SerializeField] private TMP_Text _coinsQuantity;


        [SerializeField] private Transform _upgradeScrollView;
        [SerializeField] private Transform _startAbilityScrollView;
        [SerializeField] private Transform _upgradeGroup;
        [SerializeField] private Transform _startAbilityGroup;
        [SerializeField] private TMP_Text _tip;

        [SerializeField] private Button _enableUpgradeButton;
        [SerializeField] private Button _enableStartAbilityButton;

        [field: SerializeField] public Button ExitButton { get; private set; }

        private readonly Dictionary<AbilityType, ShopOption> _options = new();
        private readonly Dictionary<AbilityType, StartAbilityOption> _startOptions = new();

        private readonly Color _upgradeMaxed = Color.yellow;
        private readonly Color _upgradeAvailable = Color.green;
        private readonly Color _upgradeUnavailable = Color.red;

        private Dictionary<AbilityType, int> _abilityMaxLevel;
        private Dictionary<AbilityType, int[]> _upgradeCost;
        private PlayerData _playerData;

        private void Awake()
        {
            ExitButton.Subscribe(Disable);

            _enableUpgradeButton.Subscribe(EnableShopList);
            _enableStartAbilityButton.Subscribe(EnableStartAbilityList);
        }

        private void OnEnable()
        {
            UpdateAllStartOptions();
        }

        private void OnDestroy()
        {
            foreach (ShopOption shopOption in _options.Values)
            {
                if (shopOption.NotNull() && shopOption.UpgradeButton.NotNull())
                {
                    shopOption.UpgradeButton.UnsubscribeAll();
                }
            }

            foreach (StartAbilityOption shopOption in _startOptions.Values)
            {
                if (shopOption.NotNull() && shopOption.ChoseButton.NotNull())
                {
                    shopOption.ChoseButton.UnsubscribeAll();
                }
            }

            if (_playerData.NotNull() && _playerData.Wallet.NotNull())
            {
                _playerData.Wallet.ValueChanged -= UpdateAllOptions;
            }

            ExitButton.Unsubscribe(Disable);
        }

        public ShopWindow1 Initialize(Dictionary<AbilityType, int> abilityMaxLevel,
            Dictionary<AbilityType, int[]> upgradeCost, PlayerData playerData)
        {
            _abilityMaxLevel = abilityMaxLevel.ThrowIfNullOrEmpty();
            _upgradeCost = upgradeCost.ThrowIfNullOrEmpty();
            _playerData = playerData.ThrowIfNull();

            _coinsQuantity.SetText((int)_playerData.Wallet.CoinsQuantity);
            _playerData.Wallet.ValueChanged += UpdateAllOptions;

            _tip.SetText(UIText.Tip);

            return this;
        }

        public void AddOption(ShopOption option)
        {
            option.ThrowIfNull().transform.SetParent(_upgradeGroup, false);
            AbilityType abilityType = option.AbilityType;

            option.LevelNumber.SetText(_playerData.AbilityUnlockLevel[abilityType]);
            UpdateOption(option, _playerData.Wallet.CoinsQuantity);

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
        }

        private void EnableShopList()
        {
            _startAbilityScrollView.SetActive(false);
            _upgradeScrollView.SetActive(true);

            _enableStartAbilityButton.interactable = true;
            _enableUpgradeButton.interactable = false;
        }

        private void SetStatus(StartAbilityOption option)
        {
            bool isLocked = _playerData.DamageDealt[option.AbilityType] < _playerData.UnlockDamage[option.AbilityType];

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

        private void UpdateOption(ShopOption option, float coinsQuantity)
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
            _playerData.Wallet.Spend(_upgradeCost[abilityType][unlockLevel]);
            _playerData.AbilityUnlockLevel[abilityType] = ++unlockLevel;
            _options[abilityType].LevelNumber.SetText(unlockLevel);

            UpdateAllOptions(_playerData.Wallet.CoinsQuantity);
            YG2.SaveProgress();
            YG2.saves.Save();
        }

        private void UpdateAllOptions(float coinsQuantity)
        {
            _coinsQuantity.SetText((int)coinsQuantity);
            _options.ForEachValues(option => UpdateOption(option, coinsQuantity));
        }

        private void UpdateAllStartOptions()
        {
            _startOptions.ForEachValues(option => UpdateStartOption(option));
        }

        private void UpdateStartOption(StartAbilityOption option)
        {
            int dealtDamage = _playerData.DamageDealt[option.AbilityType];
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
