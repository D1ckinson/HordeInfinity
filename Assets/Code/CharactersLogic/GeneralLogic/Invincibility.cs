using Assets.Code.Tools.Base;
using System;

namespace Assets.Code.CharactersLogic.GeneralLogic
{
    public class Invincibility
    {
        private readonly float _duration;
        private readonly float _triggerValue;
        private readonly Action _setFalse;

        public Invincibility(float duration, float triggerValue)
        {
            _duration = duration.ThrowIfNegative();
            _triggerValue = triggerValue.ThrowIfNegative();
            _setFalse = () => IsOn = false;
        }

        ~Invincibility()
        {
            TimerService.StopAllTimersForOwner(this);
        }

        public bool IsOn { get; private set; }

        public void HandleDamage(float damage)
        {
            if (damage < _triggerValue)
            {
                return;
            }

            IsOn = true;
            TimerService.StartTimer(_duration, _setFalse, this);
        }
    }
}
