using Assets.Code;
using Assets.Code.AbilitySystem;
using Assets.Code.CharactersLogic;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.InputActions;
using Assets.Code.Spawners;
using Assets.Code.Tools;
using Assets.Code.Ui;
using Assets.Code.Ui.Windows;
using System.Collections;
using UnityEngine;
using YG;

namespace Assets.Scripts.State_Machine
{
    public class GameState : State
    {
        private const float ToggleMusicDuration = 5f;

        private readonly HeroComponents _hero;
        private readonly AbilityFactory _abilityFactory;
        private readonly EnemySpawner _enemySpawner;
        private readonly UiFactory _uiFactory;
        private readonly string _resurrectRewardId = "resurrect";
        private readonly PlayerData _playerData;
        private readonly IInputService _inputService;
        private readonly ITimeService _timeService;
        private readonly UpgradeTrigger _upgradeTrigger;
        private readonly AudioSource _backgroundMusic;
        private readonly float _musicVolume;
        private readonly SpellBookSpawner _bookSpawner;

        public GameState(
            StateMachine stateMachine,
            HeroComponents heroComponents,
            EnemySpawner enemySpawner,
            AbilityFactory abilityFactory,
            UiFactory uiFactory,
            PlayerData playerData,
            IInputService inputService,
            ITimeService timeService,
            UpgradeTrigger upgradeTrigger,
            AudioSource backgroundMusic,
            SpellBookSpawner bookSpawner) : base(stateMachine)
        {
            _hero = heroComponents.ThrowIfNull();
            _enemySpawner = enemySpawner.ThrowIfNull();
            _abilityFactory = abilityFactory.ThrowIfNull();
            _uiFactory = uiFactory.ThrowIfNull();
            _playerData = playerData.ThrowIfNull();
            _inputService = inputService.ThrowIfNull();
            _timeService = timeService.ThrowIfNull();
            _upgradeTrigger = upgradeTrigger.ThrowIfNull();
            _bookSpawner = bookSpawner.ThrowIfNull();
            _backgroundMusic = backgroundMusic.ThrowIfNull().Instantiate(_hero.transform);
            _musicVolume = _backgroundMusic.volume;
        }

        ~GameState()
        {
            CoroutineService.StopAllCoroutines(this);
        }

        public override void Enter()
        {
            PauseWindow pauseWindow = _uiFactory.Create<PauseWindow>(false);
            pauseWindow.ExitButton.Subscribe(OnExit);
            pauseWindow.ContinueButton.Subscribe(Continue);

            DeathWindow deathWindow = _uiFactory.Create<DeathWindow>(false);
            deathWindow.BackToMenuButton.Subscribe(OnExit);
            deathWindow.ContinueForAddButton.Subscribe(ShowAdd);

            GameWindow gameWindow = _uiFactory.Create<GameWindow>();
            gameWindow.PauseButton.Subscribe(Pause);

            _hero.AbilityContainer.Add(_abilityFactory.Create(_playerData.StartAbility));
            _hero.AbilityContainer.Run();
            _hero.Health.Died += ShowDeathWindow;
            _hero.LootCollector.Run();
            _hero.Mover.Run();
            _hero.Rotator.Run();
            _enemySpawner.Run();
            _bookSpawner.Run();
            _upgradeTrigger.Run();

            _inputService.BackPressed += Pause;

            CoroutineService.StopAllCoroutines(this);
            CoroutineService.StartCoroutine(TurnOnMusic(), this);
            TimerService.StartTimer(this);
        }

        public override void Update()
        {

        }

        public override void Exit()
        {
            DeathWindow deathWindow = _uiFactory.Create<DeathWindow>(false);
            deathWindow.BackToMenuButton.Unsubscribe(OnExit);
            deathWindow.ContinueForAddButton.Unsubscribe(ShowAdd);
            deathWindow.ContinueForAddButton.interactable = true;

            PauseWindow pauseWindow = _uiFactory.Create<PauseWindow>(false);
            pauseWindow.ExitButton.Unsubscribe(OnExit);
            pauseWindow.ContinueButton.Unsubscribe(_timeService.Continue);

            GameWindow gameWindow = _uiFactory.Create<GameWindow>(false);
            gameWindow.PauseButton.Subscribe(Pause);

            _hero.Health.Died -= ShowDeathWindow;

            _hero.AbilityContainer.RemoveAll();
            _hero.BuffContainer.Reset();
            _hero.LootCollector.TransferGold();
            _hero.Health.ResetValues();
            _hero.Mover.Stop();
            _hero.Rotator.Stop();
            _hero.BuffView.Clear();
            _hero.SetDefaultPosition();
            _upgradeTrigger.Stop();

            float time = TimerService.GetElapsedTime(this);

            if (time > _playerData.ScoreRecord)
            {
                _playerData.ScoreRecord = time;
                YG2.SetLBTimeConvert(Constants.LeaderboardName, _playerData.ScoreRecord);
            }

            _inputService.BackPressed -= Pause;
            TimerService.StopTimer(this);
            _enemySpawner.Reset();
            _bookSpawner.Stop();

            YG2.saves.Save(_playerData);

            if (_playerData.IsAdOn)
            {
                YG2.InterstitialAdvShow();
            }
        }

        private void OnExit()
        {
            _hero.HeroLevel.Reset();
            _hero.LootCollector.Reset();

            _timeService.Continue();

            CoroutineService.StopAllCoroutines(this);
            CoroutineService.StartCoroutine(TurnOffMusic(), this);

            _uiFactory.Create<FadeWindow>().Show(SetState<MenuState>);
        }

        private void Pause()
        {
            if (_uiFactory.IsActive<DeathWindow>())
            {
                return;
            }

            _timeService.Pause();
            _uiFactory.Create<PauseWindow>();
        }

        private void Continue()
        {
            if (_upgradeTrigger.IsOffering == false)
            {
                _timeService.Continue();
            }
        }

        private void ShowDeathWindow(Health health)
        {
            _hero.LootCollector.Stop();
            _hero.Mover.Stop();
            _hero.Rotator.Stop();
            _hero.AbilityContainer.Stop();

            _enemySpawner.Pause();
            _bookSpawner.Pause();
            TimerService.PauseTimer(this);

            DeathWindow deathWindow = _uiFactory.Create<DeathWindow>();
            deathWindow.CoinsQuantity.SetText(_hero.LootCollector.CollectedGold.ToString(StringFormat.WholeNumber));
            deathWindow.MinutesQuantity.SetText(TimerService.GetElapsedTime(this).ToMinutesString());
        }

        private void ShowAdd()
        {
            YG2.RewardedAdvShow(_resurrectRewardId, Resurrect);
        }

        private void Resurrect()
        {
            _hero.LootCollector.Run();
            _hero.Mover.Run();
            _hero.Rotator.Run();
            _hero.AbilityContainer.Run();
            _hero.Health.ResetValues();

            _enemySpawner.Continue();
            _bookSpawner.Continue();
            TimerService.ResumeTimer(this);

            _uiFactory.Create<DeathWindow>(false).ContinueForAddButton.interactable = false;
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
