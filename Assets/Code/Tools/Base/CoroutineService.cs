using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Code.Tools.Base
{
    public static class CoroutineService
    {
        private const int Zero = 0;
        private const int One = 1;

        private static readonly Dictionary<object, List<Coroutine>> _trackedCoroutines = new();
        private static readonly CoroutineRunner _runner;

        static CoroutineService()
        {
            _runner = new GameObject("CoroutineService").AddComponent<CoroutineRunner>();
            _runner.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(_runner.gameObject);
        }

        public static Coroutine StartCoroutine(IEnumerator routine, object owner = null)
        {
            Coroutine coroutine = _runner.StartCoroutine(routine);

            if (owner != null)
            {
                TrackCoroutine(owner, coroutine);
            }

            return coroutine;
        }

        public static void StopCoroutine(Coroutine coroutine, object owner = null)
        {
            if (coroutine != null)
            {
                _runner.StopCoroutine(coroutine);

                if (owner != null)
                {
                    UntrackCoroutine(owner, coroutine);
                }
            }
        }

        public static void StopAllCoroutines(object owner)
        {
            if (owner.IsNotNull() && _trackedCoroutines.TryGetValue(owner, out List<Coroutine> coroutines))
            {
                coroutines.ForEach(coroutine => _runner.StopCoroutine(coroutine));
                coroutines.Clear();
                _trackedCoroutines.Remove(owner);
            }
        }

        public static Coroutine DelayedAction(float delay, Action action, object owner = null)
        {
            return StartCoroutine(DelayedActionRoutine(delay, action), owner);
        }

        public static Coroutine RepeatAction(float interval, Action action, int repeatCount = -One, object owner = null)
        {
            return StartCoroutine(RepeatActionRoutine(interval, action, repeatCount), owner);
        }

        private static IEnumerator DelayedActionRoutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);

            action?.Invoke();
        }

        private static IEnumerator RepeatActionRoutine(float interval, Action action, int repeatCount)
        {
            int count = Zero;
            WaitForSeconds wait = new(interval);

            while (repeatCount == -One || count < repeatCount)
            {
                yield return wait;

                action?.Invoke();
                count++;
            }
        }

        private static void TrackCoroutine(object owner, Coroutine coroutine)
        {
            if (_trackedCoroutines.ContainsKey(owner) == false)
            {
                _trackedCoroutines[owner] = new List<Coroutine>();
            }

            _trackedCoroutines[owner].Add(coroutine);
        }

        private static void UntrackCoroutine(object owner, Coroutine coroutine)
        {
            if (_trackedCoroutines.TryGetValue(owner, out List<Coroutine> coroutines))
            {
                coroutines.Remove(coroutine);

                if (coroutines.Count == Zero)
                {
                    _trackedCoroutines.Remove(owner);
                }
            }
        }

        private class CoroutineRunner : MonoBehaviour { }
    }
}