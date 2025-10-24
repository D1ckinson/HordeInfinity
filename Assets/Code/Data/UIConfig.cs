using Assets.Code.Ui;
using Assets.Code.Ui.LevelUp;
using Assets.Code.Ui.Windows;
using UnityEngine;

namespace Assets.Code.Data
{
    [CreateAssetMenu(menuName = "Game/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        [Header("LevelUp Window")]
        public Canvas LevelUpCanvas;
        public LevelUpButton LevelUpButton;

        [Header("Other")]
        public DeathWindow DeathWindow;
        public UiCanvas UiCanvas;
        public BaseWindow FadeWindow;
        public FPSWindow FPSWindow;
        public MenuWindow MenuWindow;
        public ShopOption ShopOption;
        public LeaderboardWindow Leaderboard;
        public PauseWindow PauseWindow;
        public GameWindow GameWindow;
        public ShopWindow ShopWindow;
        public StartAbilityOption StartAbilityOption;
        public Canvas JoystickCanvas;
    }
}
