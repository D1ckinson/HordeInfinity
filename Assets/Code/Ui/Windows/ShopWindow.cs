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
    public class ShopWindow : BaseWindow
    {
        [SerializeField] private TMP_Text _coinsQuantity;
        [SerializeField] private Transform _layoutGroup;

        [field: SerializeField] public Button ExitButton { get; private set; }

        private readonly Dictionary<AbilityType, ShopOption> _options = new();
        private readonly Color _upgradeMaxed = Color.yellow;
        private readonly Color _upgradeAvailable = Color.green;
        private readonly Color _upgradeUnavailable = Color.red;
        private Dictionary<AbilityType, int> _abilityUnlockLevel;
        private Dictionary<AbilityType, int> _abilityMaxLevel;
        private Dictionary<AbilityType, int[]> _upgradeCost;
        private Wallet _wallet;

        private void Awake()
        {
            ExitButton.Subscribe(Disable);
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

            if (_wallet.NotNull())
            {
                _wallet.ValueChanged -= UpdateAllOptions;
            }

            ExitButton.Unsubscribe(Disable);
        }

        public ShopWindow Initialize(Dictionary<AbilityType, int> abilityUnlockLevel, Dictionary<AbilityType, int> abilityMaxLevel, Dictionary<AbilityType, int[]> upgradeCost, Wallet wallet)
        {
            _abilityUnlockLevel = abilityUnlockLevel.ThrowIfNullOrEmpty();
            _abilityMaxLevel = abilityMaxLevel.ThrowIfNullOrEmpty();
            _upgradeCost = upgradeCost.ThrowIfNullOrEmpty();
            _wallet = wallet.ThrowIfNull();
            _coinsQuantity.SetText((int)_wallet.CoinsQuantity);
            _wallet.ValueChanged += UpdateAllOptions;

            return this;
        }

        public void AddOption(ShopOption option)
        {
            option.ThrowIfNull().transform.SetParent(_layoutGroup, false);
            AbilityType abilityType = option.AbilityType;

            option.LevelNumber.SetText(_abilityUnlockLevel[abilityType]);
            UpdateOption(option, _wallet.CoinsQuantity);

            option.UpgradeButton.Subscribe(() => IncreaseUnlockLevel(abilityType));
            _options.Add(option.AbilityType, option);
        }

        private void UpdateOption(ShopOption option, float coinsQuantity)
        {
            AbilityType abilityType = option.AbilityType;
            int unlockLevel = _abilityUnlockLevel[abilityType];
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
            int unlockLevel = _abilityUnlockLevel[abilityType];
            _wallet.Spend(_upgradeCost[abilityType][unlockLevel]);
            _abilityUnlockLevel[abilityType] = ++unlockLevel;
            _options[abilityType].LevelNumber.SetText(unlockLevel);

            UpdateAllOptions(_wallet.CoinsQuantity);
            YG2.SaveProgress();
            YG2.saves.Save();
        }

        private void UpdateAllOptions(float coinsQuantity)
        {
            _coinsQuantity.SetText((int)coinsQuantity);
            _options.ForEachValues(option => UpdateOption(option, coinsQuantity));
        }
    }
}
