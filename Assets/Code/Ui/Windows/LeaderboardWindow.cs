using Assets.Code.Data;
using Assets.Code.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Assets.Code.Ui.Windows
{
    public class LeaderboardWindow : BaseWindow
    {
        [SerializeField] private TMP_Text _leadersText;

        [field: SerializeField] public Button ExitButton { get; private set; }
        [field: SerializeField] public LeaderboardYG Leaderboard { get; private set; }

        private void Awake()
        {
            _leadersText.SetText(UIText.Leaders);
            ExitButton.Subscribe(Disable);
        }

        private void OnDestroy()
        {
            ExitButton.Unsubscribe(Disable);
        }
    }
}