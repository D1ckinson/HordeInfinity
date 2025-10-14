using System;

namespace Assets.Scripts
{
    public interface ITimeService
    {
        public event Action TimeChanged;
        public event Action<bool> TimeChanging;

        public void Pause();

        public void Continue();
    }
}