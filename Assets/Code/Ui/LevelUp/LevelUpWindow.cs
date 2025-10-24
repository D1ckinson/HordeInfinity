using Assets.Code;
using Assets.Code.AbilitySystem;
using Assets.Code.Data;
using Assets.Code.Tools;
using Assets.Code.Ui.LevelUp;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class LevelUpWindow
    {
        private readonly LevelUpButton[] _buttons;
        private readonly Canvas _canvas;

        public LevelUpWindow(Canvas canvas, LevelUpButton buttonPrefab)
        {
            _canvas = canvas.ThrowIfNull().Instantiate();
            Transform layoutGroup = _canvas.GetComponentInChildrenOrThrow<LayoutGroup>().transform;

            _buttons = new LevelUpButton[]
            {
                buttonPrefab.Instantiate(layoutGroup,false),
                buttonPrefab.Instantiate(layoutGroup,false),
                buttonPrefab.Instantiate(layoutGroup,false)
            };

            _buttons.ForEach(button => button.LevelText.SetText(UIText.LevelCut));
            _buttons.ForEach(button => button.SetActive(false));
            _canvas.SetActive(false);
        }

        public event Action<AbilityType> UpgradeChosen;

        public bool IsOn => _canvas.IsActive();

        public void Show(List<UpgradeOption> upgradeOptions, int level = 1)
        {
            upgradeOptions.ThrowIfNullOrEmpty();

            for (int i = Constants.Zero; i < upgradeOptions.Count; i++)
            {
                LevelUpButton button = _buttons[i];
                UpgradeOption upgradeOption = upgradeOptions[i];
                button.SetDescription(upgradeOption.Name, upgradeOption.Icon, upgradeOption.Stats);

                button.LevelNumber.SetText(upgradeOption.NextLevel);

                button.Subscribe(() => Callback(upgradeOption.Type));
                button.SetActive(true);
            }

            _canvas.SetActive(true);
        }

        public void Hide()
        {
            foreach (LevelUpButton button in _buttons)
            {
                button.UnsubscribeAll();
                button.SetActive(false);
            }

            _canvas.SetActive(false);
        }

        private void Callback(AbilityType abilityType)
        {
            Hide();
            UpgradeChosen?.Invoke(abilityType);
        }
    }
}
