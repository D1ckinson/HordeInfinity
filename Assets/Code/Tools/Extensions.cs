using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Code.Tools
{
    public static class Extensions
    {
        private const int Zero = 0;
        private const int One = 1;

        public static bool IsNull<T>(this T argument)
        {
            return argument == null;
        }

        public static bool NotNull<T>(this T argument)
        {
            return argument != null;
        }

        public static int ParseOrThrow(this string argument)
        {
            if (int.TryParse(argument, out var result))
            {
                return result;
            }

            throw new ArgumentException();
        }

        public static void SetText(this TMP_Text textWindow, string text)
        {
            textWindow.text = text;
        }

        public static TValue GetValueOrThrow<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value;
            }

            throw new KeyNotFoundException();
        }

        public static bool Compare(this Vector2 v1, Vector2 v2, int accuracy)
        {
            bool x = (int)(v1.x * accuracy) == (int)(v2.x * accuracy);
            bool y = (int)(v1.y * accuracy) == (int)(v2.y * accuracy);

            return x && y;
        }

        public static T GetComponentOrThrow<T>(this Component component)
        {
            if (component.TryGetComponent(out T value))
            {
                return value;
            }

            throw new MissingComponentException();
        }

        public static T GetComponentInChildrenOrThrow<T>(this Component component)
        {
            return component.GetComponentInChildren<T>() ?? throw new MissingComponentException();
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action?.Invoke(item);
            }
        }

        public static void ForEachValues<TKey, TValue>(this Dictionary<TKey, TValue> pairs, Action<TValue> action)
        {
            foreach (TValue item in pairs.Values)
            {
                action?.Invoke(item);
            }
        }

        public static string ToWrapText(this IEnumerable<string> strings)
        {
            return string.Join(Environment.NewLine, strings);
        }

        public static void PlayAsNew(this Animator animator, int hash, int layer = 0)
        {
            animator.Play(hash, layer, Zero);
        }

        public static int LastIndex<T>(this ICollection<T> collection)
        {
            if (collection is IDictionary dictionary)
            {
                return dictionary.Values.Count - One;
            }

            return collection.Count - One;
        }

        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (One << layer)) != Zero;
        }

        public static void SetActive(this Component component, bool isActive)
        {
            component.gameObject.SetActive(isActive);
        }

        public static bool IsActive(this Component component)
        {
            return component.gameObject.activeSelf;
        }

        public static void Subscribe(this Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        public static void SetColor(this Button button, Color color)
        {
            const float HighlightedMultiplier = 1.2f;
            const float PressedMultiplier = 0.8f;
            const float DisabledMultiplier = 0.5f;

            ColorBlock colors = button.colors;

            colors.normalColor = color;
            colors.highlightedColor = color * HighlightedMultiplier;
            colors.pressedColor = color * PressedMultiplier;
            colors.selectedColor = color;
            colors.disabledColor = color * DisabledMultiplier;

            button.colors = colors;
        }

        public static void UnsubscribeAll(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }

        public static void Unsubscribe(this Button button, UnityAction action)
        {
            button.onClick.RemoveListener(action);
        }

        public static int IndexOf<T>(this T[] values, T value)
        {
            return Array.IndexOf(values, value);
        }

        public static string ToMinutesString(this float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);

            return $"{(int)time.TotalMinutes}:{time.Seconds:00}";
        }

        public static T Instantiate<T>(this T component, Transform parent, bool worldPositionStays) where T : Object
        {
            return Object.Instantiate(component, parent, worldPositionStays);
        }

        public static T Instantiate<T>(this T component, Transform parent) where T : Object
        {
            return Object.Instantiate(component, parent);
        }

        public static T Instantiate<T>(this T component, bool isActive = true) where T : Component
        {
            T @object = Object.Instantiate(component);
            @object.SetActive(isActive);

            return @object;
        }

        public static void SetText(this TMP_Text textWindow, int value)
        {
            textWindow.SetText(value.ToString());
        }

        public static int GetStateLayerIndex(this Animator animator, int hash)
        {
            for (int i = Zero; i < animator.layerCount; i++)
            {
                if (animator.HasState(i, hash))
                {
                    return i;
                }
            }

            return -One;
        }

        public static bool IsAnimationFinished(this Animator animator, int hash, int layerIndex = 0)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            bool isCurrentState = stateInfo.shortNameHash == hash;

            return isCurrentState == false && (stateInfo.normalizedTime >= One || stateInfo.loop);
        }

        public static void DestroyGameObject(this Component component)
        {
            Object.Destroy(component.gameObject);
        }

        public static void SetShapeRadius(this ParticleSystem system, float radius)
        {
            ParticleSystem.ShapeModule shapeModule = system.shape;
            shapeModule.radius = radius;
        }

        public static void PlayRandomPitch(this AudioSource audioSource, float minPitch = 0.9f, float maxPitch = 1.1f)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.Play();
        }

        public static void SetPosition(this Component component, Transform transform)
        {
            component.transform.position = transform.position;
        }

        public static void SetPosition(this Component component, Vector3 position)
        {
            component.transform.position = position;
        }
    }
}
