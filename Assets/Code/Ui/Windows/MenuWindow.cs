using Assets.Code.Data;
using Assets.Code.Tools;
using Assets.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Windows
{
    public class MenuWindow : BaseWindow
    {
        [SerializeField] private TMP_Text _personalBestText;
        [SerializeField] private TMP_Text _minutesText;
        [SerializeField] private TMP_Text _playText;
        [SerializeField] private TMP_Text _minutesQuantity;
        [SerializeField] private TMP_Text _coinsQuantity;

        [field: SerializeField] public ToggleButton VolumeButton { get; private set; }
        [field: SerializeField] public Button LeaderboardButton { get; private set; }
        [field: SerializeField] public Button RemoveAdButton { get; private set; }
        [field: SerializeField] public Button ShopButton { get; private set; }
        [field: SerializeField] public Button PlayButton { get; private set; }

        private Wallet _wallet;

        private void Awake()
        {
            LeaderboardButton.Subscribe(Disable);
            ShopButton.Subscribe(Disable);
            PlayButton.Subscribe(Disable);

            VolumeButton.Clicked += ToggleSound;
        }

        private void OnDestroy()
        {
            LeaderboardButton.Unsubscribe(Disable);
            ShopButton.Unsubscribe(Disable);
            PlayButton.Unsubscribe(Disable);
            VolumeButton.Clicked -= ToggleSound;

            if (_wallet.IsNotNull())
            {
                _wallet.ValueChanged -= UpdateCoinsQuantity;
            }
        }

        public MenuWindow Initialize(Wallet wallet)
        {
            _personalBestText.SetText(UIText.PersonalBest);
            _minutesText.SetText(UIText.Minutes);
            _playText.SetText(UIText.Play);
            _wallet = wallet.ThrowIfNull();

            UpdateCoinsQuantity(_wallet.CoinsQuantity);
            _wallet.ValueChanged += UpdateCoinsQuantity;

            return this;
        }

        private void UpdateCoinsQuantity(float coinsQuantity)
        {
            _coinsQuantity.SetText((int)coinsQuantity);
        }

        private void ToggleSound(bool isOn)
        {
            AudioListener.volume = isOn ? Constants.One : Constants.Zero;
        }
    }
}
