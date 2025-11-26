using System;
using System.Collections.Generic;
using UnityEngine;

public static class TimerService
{
    private const int Zero = 0;
    private const int One = 1;

    private static readonly Dictionary<string, TimerData> _activeTimers = new();
    private static readonly Dictionary<object, TimerData> _ownerTimers = new();

    static TimerService()
    {
        GameObject timerGameObject = new("TimerSystem");
        timerGameObject.AddComponent<TimerComponent>();
        UnityEngine.Object.DontDestroyOnLoad(timerGameObject);
    }

    public static void StartTimer(float duration, Action onComplete, object owner = null, bool isLooping = false)
    {
        if (onComplete == null)
        {
            return;
        }

        string key = GetTimerKey(owner, onComplete);

        if (_activeTimers.ContainsKey(key))
        {
            _activeTimers[key].Duration = duration;
            _activeTimers[key].Elapsed = Zero;
            _activeTimers[key].IsLooping = isLooping;
            _activeTimers[key].IsPaused = false;

            return;
        }

        TimerData timerData = new()
        {
            Duration = duration,
            Elapsed = Zero,
            OnComplete = onComplete,
            IsLooping = isLooping,
            Owner = owner
        };

        _activeTimers.Add(key, timerData);
    }

    public static void StartTimer(object owner)
    {
        if (owner == null)
        {
            return;
        }

        if (_ownerTimers.ContainsKey(owner))
        {
            _ownerTimers[owner].Elapsed = Zero;
            _ownerTimers[owner].IsPaused = false;

            return;
        }

        TimerData timerData = new()
        {
            Duration = float.MaxValue,
            Elapsed = Zero,
            OnComplete = null,
            IsLooping = true,
            Owner = owner
        };

        _ownerTimers.Add(owner, timerData);
    }

    public static void StartTimerWithUpdate(float duration, Action onComplete, Action<float> onUpdate, object owner = null, bool isLooping = false)
    {
        if (onComplete == null || onUpdate == null)
        {
            return;
        }

        string key = GetTimerKey(owner, onComplete);

        if (_activeTimers.ContainsKey(key))
        {
            _activeTimers[key].Duration = duration;
            _activeTimers[key].Elapsed = Zero;
            _activeTimers[key].IsLooping = isLooping;
            _activeTimers[key].IsPaused = false;
            _activeTimers[key].OnUpdate = onUpdate;
            return;
        }

        TimerData timerData = new()
        {
            Duration = duration,
            Elapsed = Zero,
            OnComplete = onComplete,
            OnUpdate = onUpdate,
            IsLooping = isLooping,
            Owner = owner
        };

        _activeTimers.Add(key, timerData);
    }

    public static float GetElapsedTime(object owner, Action onComplete)
    {
        string key = GetTimerKey(owner, onComplete);

        return _activeTimers.ContainsKey(key) ? _activeTimers[key].Elapsed : -One;
    }

    public static float GetElapsedTime(object owner)
    {
        if (owner == null)
        {
            return -One;
        }

        return _ownerTimers.ContainsKey(owner) ? _ownerTimers[owner].Elapsed : -One;
    }

    public static float GetRemainingTime(object owner, Action onComplete)
    {
        string key = GetTimerKey(owner, onComplete);

        if (_activeTimers.ContainsKey(key) == false)
        {
            return -One;
        }

        TimerData timer = _activeTimers[key];

        return Mathf.Max(Zero, timer.Duration - timer.Elapsed);
    }

    public static bool IsTimerExists(object owner, Action onComplete)
    {
        string key = GetTimerKey(owner, onComplete);

        return _activeTimers.ContainsKey(key);
    }

    public static bool IsTimerExists(object owner)
    {
        if (owner == null)
        {
            return false;
        }

        return _ownerTimers.ContainsKey(owner);
    }

    public static void PauseTimer(object owner, Action onComplete)
    {
        string key = GetTimerKey(owner, onComplete);

        if (_activeTimers.ContainsKey(key))
        {
            _activeTimers[key].IsPaused = true;
        }
    }

    public static void PauseTimer(object owner)
    {
        if (owner == null)
        {
            return;
        }

        if (_ownerTimers.ContainsKey(owner))
        {
            _ownerTimers[owner].IsPaused = true;
        }
    }

    public static void ResumeTimer(object owner, Action onComplete)
    {
        string key = GetTimerKey(owner, onComplete);

        if (_activeTimers.ContainsKey(key))
        {
            _activeTimers[key].IsPaused = false;
        }
    }

    public static void ResumeTimer(object owner)
    {
        if (owner == null)
        {
            return;
        }

        if (_ownerTimers.ContainsKey(owner))
        {
            _ownerTimers[owner].IsPaused = false;
        }
    }

    public static void StopTimer(object owner, Action onComplete)
    {
        string key = GetTimerKey(owner, onComplete);

        if (_activeTimers.ContainsKey(key))
        {
            _activeTimers.Remove(key);
        }
    }

    public static void StopTimer(object owner)
    {
        if (owner == null)
        {
            return;
        }

        if (_ownerTimers.ContainsKey(owner))
        {
            _ownerTimers.Remove(owner);
        }
    }

    public static void StopAllTimersForOwner(object owner)
    {
        List<string> keysToRemove = new();

        foreach (KeyValuePair<string, TimerData> pair in _activeTimers)
        {
            if (pair.Value.Owner == owner)
            {
                keysToRemove.Add(pair.Key);
            }
        }

        foreach (string key in keysToRemove)
        {
            _activeTimers.Remove(key);
        }

        if (_ownerTimers.ContainsKey(owner))
        {
            _ownerTimers.Remove(owner);
        }
    }

    public static void PauseAllTimers()
    {
        foreach (TimerData timer in _activeTimers.Values)
        {
            timer.IsPaused = true;
        }

        foreach (TimerData timer in _ownerTimers.Values)
        {
            timer.IsPaused = true;
        }
    }

    public static void ResumeAllTimers()
    {
        foreach (TimerData timer in _activeTimers.Values)
        {
            timer.IsPaused = false;
        }

        foreach (TimerData timer in _ownerTimers.Values)
        {
            timer.IsPaused = false;
        }
    }

    public static void StopAllTimers()
    {
        _activeTimers.Clear();
        _ownerTimers.Clear();
    }

    public static void SetTimerDuration(object owner, Action onComplete, float newDuration, bool resetElapsed = false)
    {
        string key = GetTimerKey(owner, onComplete);

        if (_activeTimers.ContainsKey(key))
        {
            _activeTimers[key].Duration = newDuration;

            if (resetElapsed)
            {
                _activeTimers[key].Elapsed = Zero;
            }
        }
    }

    private static string GetTimerKey(object owner, Action onComplete)
    {
        return $"{owner?.GetHashCode() ?? Zero}_{onComplete.GetHashCode()}";
    }

    private class TimerData
    {
        public float Duration;
        public float Elapsed;
        public Action OnComplete;
        public Action<float> OnUpdate;
        public bool IsLooping;
        public bool IsPaused;
        public object Owner;
    }

    private class TimerComponent : MonoBehaviour
    {
        private readonly List<TimerData> _timersToRemove = new();

        private void Update()
        {
            UpdateTimers(Time.deltaTime);
        }

        private void UpdateTimers(float deltaTime)
        {
            _timersToRemove.Clear();

            List<string> keys = new(_activeTimers.Keys);

            for (int i = keys.Count - One; i >= Zero; i--)
            {
                string key = keys[i];

                if (_activeTimers.TryGetValue(key, out TimerData timer) == false)
                {
                    continue;
                }

                if (timer.IsPaused)
                {
                    continue;
                }

                timer.Elapsed += deltaTime;

                if (timer.OnUpdate != null)
                {
                    float progress = Mathf.Clamp01(timer.Elapsed / timer.Duration);
                    timer.OnUpdate.Invoke(progress);
                }


                if (timer.Elapsed >= timer.Duration)
                {
                    timer.OnComplete?.Invoke();

                    if (timer.IsLooping)
                    {
                        timer.Elapsed = Zero;
                    }
                    else
                    {
                        _timersToRemove.Add(timer);
                    }
                }
            }

            List<object> ownerKeys = new(_ownerTimers.Keys);

            for (int i = ownerKeys.Count - One; i >= Zero; i--)
            {
                object key = ownerKeys[i];

                if (_ownerTimers.TryGetValue(key, out TimerData timer) == false)
                {
                    continue;
                }

                if (timer.IsPaused)
                {
                    continue;
                }

                timer.Elapsed += deltaTime;
            }

            foreach (TimerData timer in _timersToRemove)
            {
                _activeTimers.Remove(GetTimerKey(timer.Owner, timer.OnComplete));
            }
        }
    }
}