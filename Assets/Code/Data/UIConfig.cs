using Assets.Code.Ui;
using Assets.Code.Ui.Buttons;
using Assets.Code.Ui.LevelUp;
using Assets.Code.Ui.Windows;
using UnityEngine;

namespace Assets.Code.Data
{
    [CreateAssetMenu(menuName = "Game/UIConfig")]
    public class UIConfig : ScriptableObject
    {
        [Header("Menu")]
        public Canvas MenuCanvas;
        public TextButton MenuButton;

        [Header("LevelUp Window")]
        public Canvas LevelUpCanvas;
        public LevelUpButton LevelUpButton;

        [Header("Shop")]
        public Canvas ShopCanvas;

        [Header("Other")]
        public DeathWindow DeathWindow;
        public UiCanvas UiCanvas;
        public BaseWindow FadeWindow;
        public FPSWindow FPSWindow;
        public MenuWindow MenuWindow;
        public ShopWindow ShopWindow;
        public ShopOption ShopOption;
        public Joystick Joystick;
        public LeaderboardWindow Leaderboard;
        public PauseWindow PauseWindow;
        public GameWindow GameWindow;
        public ShopWindow1 ShopWindow1;
        public StartAbilityOption StartAbilityOption;
    }
}
