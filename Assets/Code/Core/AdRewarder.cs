using Assets.Code.CharactersLogic;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Tools;
using Assets.Code.Ui;
using Assets.Code.Ui.Windows;
using YG;

namespace Assets.Scripts.State_Machine
{
    public class AdRewarder
    {
        private const string HealRewardId = "heal";
        private const float HealthTrigerPercent = 0.5f;
        private const float ShowDelay = 60f;
        private const float AdditionalAttractionRadius = 100f;

        private readonly HeroComponents _hero;
        private readonly UiFactory _uiFactory;

        private float _delay;

        public AdRewarder(HeroComponents hero, UiFactory uiFactory)
        {
            _hero = hero.ThrowIfNull();
            _uiFactory = uiFactory.ThrowIfNull();

            _hero.Health.ValueChanged += HandleHealthValue;
            _hero.Health.Died += Hide;
            UpdateService.RegisterUpdate(DecreaseDelay);

            _uiFactory.Create<AdBonusButton>(false).Button.Subscribe(ShowAd);
        }

        ~AdRewarder()
        {
            _hero.Health.ValueChanged -= HandleHealthValue;
            _hero.Health.Died -= Hide;
            _uiFactory.Create<AdBonusButton>(false).Button.Unsubscribe(ShowAd);

            UpdateService.UnregisterUpdate(DecreaseDelay);
            TimerService.StopAllTimersForOwner(this);
        }

        private void HandleHealthValue(float value)
        {
            if (_delay > Constants.Zero)
            {
                return;
            }

            if (value <= _hero.Health.MaxValue * HealthTrigerPercent)
            {
                _uiFactory.Create<AdBonusButton>();
                _delay = ShowDelay;
            }
        }

        private void DecreaseDelay(float deltaTime)
        {
            _delay -= deltaTime.ThrowIfNegative();
        }

        private void ShowAd()
        {
            YG2.RewardedAdvShow(HealRewardId, Reward);
        }

        private void Reward()
        {
            _hero.Health.Heal(_hero.Health.MaxValue);
            _hero.LootCollector.AttractionRadius.Increase(AdditionalAttractionRadius);
            TimerService.StartTimer(Constants.One, () => _hero.LootCollector.AttractionRadius.Decrease(AdditionalAttractionRadius), this);
        }

        private void Hide(Health health)
        {
            _uiFactory.Create<AdBonusButton>(false);
            TimerService.StopAllTimersForOwner(this);
        }
    }
}
