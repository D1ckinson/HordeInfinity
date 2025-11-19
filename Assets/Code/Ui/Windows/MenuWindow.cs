using Assets.Code.Data;
using Assets.Code.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Ui.Windows
{
    public class MenuWindow : BaseWindow
    {
        [SerializeField] private TMP_Text _playText;

        [field: SerializeField] public ToggleButton VolumeButton { get; private set; }
        [field: SerializeField] public Button LeaderboardButton { get; private set; }
        [field: SerializeField] public Button RemoveAdButton { get; private set; }
        [field: SerializeField] public Button ShopButton { get; private set; }
        [field: SerializeField] public Button PlayButton { get; private set; }

        private void Awake()
        {
            LeaderboardButton.Subscribe(Disable);
            ShopButton.Subscribe(Disable);
            PlayButton.Subscribe(Disable);
            _playText.SetText(UIText.Play);

            VolumeButton.Clicked += ToggleSound;
        }

        private void OnDestroy()
        {
            LeaderboardButton.Unsubscribe(Disable);
            ShopButton.Unsubscribe(Disable);
            PlayButton.Unsubscribe(Disable);
            VolumeButton.Clicked -= ToggleSound;
        }

        private void ToggleSound(bool isOn)
        {
            AudioListener.volume = isOn ? Constants.One : Constants.Zero;
        }
    }
}
