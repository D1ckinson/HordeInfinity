using Assets.Scripts;
using System;
using UnityEngine;

namespace Assets.Code.Tools
{
    public class SoundPause : MonoBehaviour
    {
        [field: SerializeField] private AudioSource[] _audioSources;

        private ITimeService _timeService;

        private void OnDestroy()
        {
            if (_timeService.NotNull())
            {
                _timeService.TimeChanged -= ToggleSource;
            }
        }

        public void Initialize(ITimeService timeService)
        {
            _timeService = timeService.ThrowIfNull();
            _timeService.TimeChanged += ToggleSource;
        }

        private void ToggleSource()
        {
            switch (Time.timeScale)
            {
                case Constants.Zero:
                    _audioSources.ForEach(source => source.Pause());
                    break;

                case Constants.One:
                    _audioSources.ForEach(source => source.UnPause());
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
