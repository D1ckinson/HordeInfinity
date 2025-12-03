using Assets.Code.CharactersLogic.GeneralLogic;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Tools.Base;
using Assets.Code.Ui.Windows;

namespace Assets.Code.Core
{
    public class AdEnabler
    {
        private const float HealthTrigerPercent = 0.5f;
        private const float ShowDelay = 60f;

        private readonly HeroComponents _hero;
        private readonly AdBonusButton _adBonusButton;

        private float _delay;

        public AdEnabler(HeroComponents hero, AdBonusButton adBonusButton)
        {
            _hero = hero.ThrowIfNull();
            _adBonusButton = adBonusButton.ThrowIfNull();

            _hero.Health.ValueChanged += HandleHealthValue;
            _hero.Health.Died += DisableButton;

            UpdateService.RegisterUpdate(DecreaseDelay);
        }

        ~AdEnabler()
        {
            _hero.Health.ValueChanged -= HandleHealthValue;
            _hero.Health.Died -= DisableButton;
            UpdateService.UnregisterUpdate(DecreaseDelay);
        }

        public void ResetDelay()
        {
            _delay = Constants.Zero;
        }

        private void HandleHealthValue(float value)
        {
            if (_delay > Constants.Zero)
            {
                return;
            }

            if (value <= _hero.Health.MaxValue * HealthTrigerPercent)
            {
                _delay = ShowDelay;
                _adBonusButton.SetActive(true);
            }
        }

        private void DecreaseDelay(float deltaTime)
        {
            _delay -= deltaTime.ThrowIfNegative();
        }

        private void DisableButton(Health health)
        {
            _adBonusButton.SetActive(false);
        }
    }
}
