using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Windows
{
    public class GameWindow : BaseWindow
    {
        [SerializeField][Range(0.1f, 1f)] private float _fillingDuration = 0.3f;
        [SerializeField] private TMP_Text _coinsCount;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _levelValue;
        [SerializeField] private Image _barFilling;

        private LootCollector _lootCollector;
        private HeroLevel _heroLevel;

        [field: SerializeField] public Button PauseButton { get; private set; }
        [field: SerializeField] public AdBonusButton AdBonusButton { get; private set; }

        private void OnDestroy()
        {
            if (_lootCollector.IsNotNull())
            {
                _lootCollector.GoldValueChanged -= OnGoldChanged;
            }

            if (_heroLevel.IsNotNull())
            {
                _heroLevel.ValueChanged -= OnExperienceChanged;
                _heroLevel.LevelChanged -= OnLevelChanged;
            }
        }

        public GameWindow Initialize(LootCollector lootCollector, HeroLevel heroLevel)
        {
            _lootCollector = lootCollector.ThrowIfNull();
            _heroLevel = heroLevel.ThrowIfNull();

            OnExperienceChanged();
            OnLevelChanged(_heroLevel.Level);
            OnGoldChanged((int)_lootCollector.CollectedGold);

            _lootCollector.GoldValueChanged += OnGoldChanged;
            _heroLevel.ValueChanged += OnExperienceChanged;
            _heroLevel.LevelChanged += OnLevelChanged;

            _levelText.SetText(UIText.LevelCut);

            return this;
        }

        private void OnLevelChanged(int level)
        {
            _levelValue.SetText(level);
        }

        private void OnExperienceChanged()
        {
            float targetPercent = _heroLevel.Value / _heroLevel.LevelUpValue;
            _barFilling.DOFillAmount(targetPercent, _fillingDuration);
        }

        private void OnGoldChanged(int value)
        {
            _coinsCount.SetText(value);
        }
    }
}
