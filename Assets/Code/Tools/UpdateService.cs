using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Code.Tools
{
    public static class UpdateService
    {
        private static readonly HashSet<Action> _registeredUpdateMethods = new();
        private static readonly HashSet<Action> _registeredFixedUpdateMethods = new();

        static UpdateService()
        {
            UpdateRunner updater = new GameObject("UpdateService").AddComponent<UpdateRunner>();
            updater.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(updater.gameObject);
        }

        private static event Action OnUpdate;
        private static event Action OnFixedUpdate;

        public static void RegisterUpdate(Action updateMethod)
        {
            if (_registeredUpdateMethods.Add(updateMethod))
            {
                OnUpdate += updateMethod;
            }
        }

        public static void RegisterFixedUpdate(Action updateMethod)
        {
            if (_registeredFixedUpdateMethods.Add(updateMethod))
            {
                OnFixedUpdate += updateMethod;
            }
        }

        public static void UnregisterUpdate(Action updateMethod)
        {
            if (_registeredUpdateMethods.Remove(updateMethod))
            {
                OnUpdate -= updateMethod;
            }
        }

        public static void UnregisterFixedUpdate(Action updateMethod)
        {
            if (_registeredFixedUpdateMethods.Remove(updateMethod))
            {
                OnFixedUpdate -= updateMethod;
            }
        }

        private class UpdateRunner : MonoBehaviour
        {
            private void Update()
            {
                OnUpdate?.Invoke();
            }

            private void FixedUpdate()
            {
                OnFixedUpdate?.Invoke();
            }
        }
    }
}
