using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Tools
{
    public class Timer
    {
        private const int Zero = 0;
        private const int One = 1;

        private readonly List<ScheduledCallback> _scheduledCallbacks = new();

        private float _remainingTime;

        public float Duration { get; private set; }

        public event Action Completed;

        public void Start(float remainingTime = float.MaxValue)
        {
            if (remainingTime <= Zero)
            {
                throw new ArgumentOutOfRangeException();
            }

            _remainingTime = remainingTime;
            Duration = Zero;

            _scheduledCallbacks.Clear();
            UpdateService.RegisterUpdate(UpdateTime);
        }

        public void WaitFor(float delay, Action callback)
        {
            if (delay <= Zero || callback == null)
            {
                throw new ArgumentException();
            }

            _scheduledCallbacks.Add(new(delay + Duration, callback));
        }

        public void Stop()
        {
            UpdateService.UnregisterUpdate(UpdateTime);
            _remainingTime = Zero;
            Duration = Zero;
        }

        public void Pause()
        {
            UpdateService.UnregisterUpdate(UpdateTime);
        }

        public void Continue()
        {
            UpdateService.RegisterUpdate(UpdateTime);
        }

        private void UpdateTime()
        {
            Duration += Time.deltaTime;

            for (int i = _scheduledCallbacks.Count - One; i >= Zero; i--)
            {
                ScheduledCallback scheduled = _scheduledCallbacks[i];

                if (Duration >= scheduled.TriggerTime)
                {
                    scheduled.Callback.Invoke();
                    _scheduledCallbacks.RemoveAt(i);
                }
            }

            if (Duration >= _remainingTime)
            {
                UpdateService.UnregisterUpdate(UpdateTime);
                Completed?.Invoke();
            }
        }

        private readonly struct ScheduledCallback
        {
            public readonly Action Callback;
            public readonly float TriggerTime;

            public ScheduledCallback(float triggerTime, Action callback)
            {
                TriggerTime = triggerTime;
                Callback = callback;
            }
        }
    }
}
