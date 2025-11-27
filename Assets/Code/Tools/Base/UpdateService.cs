using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Code.Tools.Base
{
    public static class UpdateService
    {
        private static readonly HashSet<Action<float>> _registeredUpdateMethods = new();
        private static readonly HashSet<Action<float>> _registeredFixedUpdateMethods = new();

        static UpdateService()
        {
            UpdateRunner updater = new GameObject("UpdateService").AddComponent<UpdateRunner>();
            updater.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(updater.gameObject);
        }

        private static event Action<float> OnUpdate;
        private static event Action<float> OnFixedUpdate;

        public static void RegisterUpdate(Action<float> updateMethod)
        {
            if (_registeredUpdateMethods.Add(updateMethod))
            {
                OnUpdate += updateMethod;
            }
        }

        public static void RegisterFixedUpdate(Action<float> updateMethod)
        {
            if (_registeredFixedUpdateMethods.Add(updateMethod))
            {
                OnFixedUpdate += updateMethod;
            }
        }

        public static void UnregisterUpdate(Action<float> updateMethod)
        {
            if (_registeredUpdateMethods.Remove(updateMethod))
            {
                OnUpdate -= updateMethod;
            }
        }

        public static void UnregisterFixedUpdate(Action<float> updateMethod)
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
                OnUpdate?.Invoke(Time.deltaTime);
            }

            private void FixedUpdate()
            {
                OnFixedUpdate?.Invoke(Time.fixedDeltaTime);
            }
        }
    }
}
