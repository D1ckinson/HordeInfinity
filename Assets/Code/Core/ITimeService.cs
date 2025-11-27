using System;

namespace Assets.Code.Core
{
    public interface ITimeService
    {
        public float TimeScale { get; }

        public event Action TimeChanged;
        public event Action<bool> TimeChanging;

        public void Pause();

        public void Continue();
    }
}