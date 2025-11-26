using Assets.Code.Tools;
using Assets.Code.Ui;
using Assets.Code.Ui.Windows;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.State_Machine
{
    public class MenuState : State
    {
        private const float ToggleMusicDuration = 1f;

        private readonly UiFactory _uiFactory;
        private readonly AudioSource _backgroundMusic;
        private readonly float _musicVolume;

        public MenuState(StateMachine stateMachine, UiFactory uiFactory, AudioSource music) : base(stateMachine)
        {
            _uiFactory = uiFactory.ThrowIfNull();
            _backgroundMusic = music.ThrowIfNull();

            _backgroundMusic = music.ThrowIfNull();
            _musicVolume = _backgroundMusic.volume;
        }

        ~MenuState()
        {
            CoroutineService.StopAllCoroutines(this);
        }

        public override void Enter()
        {
            _uiFactory.Create<FadeWindow>().Hide();

            CoroutineService.StopAllCoroutines(this);
            CoroutineService.StartCoroutine(TurnOnMusic(), this);

            _uiFactory.Create<ShopWindow>(false).ExitButton.Subscribe(ShowMenu);
            _uiFactory.Create<LeaderboardWindow>(false).ExitButton.Subscribe(ShowMenu);

            MenuWindow menuWindow = _uiFactory.Create<MenuWindow>();
            menuWindow.ShopButton.Subscribe(ShowShop);
            menuWindow.PlayButton.Subscribe(SetState<GameState>);
            menuWindow.LeaderboardButton.Subscribe(ShowLeaderboard);
        }

        private void ShowMenu()
        {
            _uiFactory.Create<MenuWindow>();
        }

        private void ShowShop()
        {
            _uiFactory.Create<ShopWindow>();
        }

        private void ShowLeaderboard()
        {
            LeaderboardWindow leaderboard = _uiFactory.Create<LeaderboardWindow>();
            leaderboard.Leaderboard.UpdateLB();
        }

        public override void Exit()
        {
            _uiFactory.Create<ShopWindow>(false).ExitButton.Unsubscribe(ShowMenu);
            _uiFactory.Create<LeaderboardWindow>(false).ExitButton.Unsubscribe(ShowMenu);

            MenuWindow menuWindow = _uiFactory.Create<MenuWindow>(false);
            menuWindow.ShopButton.Unsubscribe(ShowShop);
            menuWindow.PlayButton.Unsubscribe(SetState<GameState>);
            menuWindow.LeaderboardButton.Unsubscribe(ShowLeaderboard);

            CoroutineService.StopAllCoroutines(this);
            CoroutineService.StartCoroutine(TurnOffMusic(), this);
        }

        private IEnumerator TurnOnMusic()
        {
            float elapsed = Constants.Zero;
            _backgroundMusic.volume = Constants.Zero;
            _backgroundMusic.Play();

            while (elapsed < ToggleMusicDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float progress = elapsed / ToggleMusicDuration;
                _backgroundMusic.volume = Mathf.Lerp(Constants.Zero, _musicVolume, progress);

                yield return null;
            }

            _backgroundMusic.volume = _musicVolume;
        }

        private IEnumerator TurnOffMusic()
        {
            float elapsed = Constants.Zero;
            float musicVolume = _backgroundMusic.volume;

            while (elapsed < ToggleMusicDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float progress = elapsed / ToggleMusicDuration;
                _backgroundMusic.volume = Mathf.Lerp(musicVolume, Constants.Zero, progress);

                yield return null;
            }

            _backgroundMusic.volume = Constants.Zero;
            _backgroundMusic.Stop();
        }
    }
}
