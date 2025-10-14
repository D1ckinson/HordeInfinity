using Assets.Code.Tools;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class TimeService : ITimeService
    {
        private const float SlowdownDuration = 0.5f;
        private const float SpeedupDuration = 1f;

        private readonly float _originalFixedDeltaTime;

        public TimeService()
        {
            _originalFixedDeltaTime = Time.fixedDeltaTime;
        }

        public event Action TimeChanged;
        public event Action<bool> TimeChanging;

        public void Pause()
        {
            TimeChanging?.Invoke(false);
            CoroutineService.StopAllCoroutines(this);
            CoroutineService.StartCoroutine(SlowdownCoroutine(), this);
        }

        public void Continue()
        {
            TimeChanging?.Invoke(true);
            CoroutineService.StopAllCoroutines(this);
            CoroutineService.StartCoroutine(SpeedupCoroutine(), this);
        }

        private IEnumerator SlowdownCoroutine()
        {
            float elapsed = Constants.Zero;
            float startScale = Time.timeScale;

            while (elapsed < SlowdownDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / SlowdownDuration;

                Time.timeScale = Mathf.Lerp(startScale, Constants.Zero, progress * progress);
                Time.fixedDeltaTime = _originalFixedDeltaTime * Time.timeScale;

                yield return null;
            }

            Time.timeScale = Constants.Zero;
            Time.fixedDeltaTime = Constants.Zero;
            TimeChanged?.Invoke();
        }

        private IEnumerator SpeedupCoroutine()
        {
            float elapsed = Constants.Zero;
            float startScale = Time.timeScale;

            while (elapsed < SpeedupDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / SpeedupDuration;

                Time.timeScale = Mathf.Lerp(startScale, Constants.One, progress * progress);
                Time.fixedDeltaTime = _originalFixedDeltaTime * Time.timeScale;

                yield return null;
            }

            Time.timeScale = Constants.One;
            Time.fixedDeltaTime = _originalFixedDeltaTime;
            TimeChanged?.Invoke();
        }
    }
}
