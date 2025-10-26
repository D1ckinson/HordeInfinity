using Assets.Code.Tools.Exceptions;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Code.Tools
{
    public static class ThrowHelper
    {
        public static Vector3 ThrowIfNotNormalize(this Vector3 vector)
        {
            if (Mathf.Approximately(vector.sqrMagnitude, Constants.One) == false)
            {
                throw new VectorNotNormalizedException();
            }

            return vector;
        }

        public static void ThrowIfNotNormalize(this Vector2 vector)
        {
            if (Mathf.Approximately(vector.sqrMagnitude, Constants.One) == false)
            {
                throw new VectorNotNormalizedException();
            }
        }

        public static T ThrowIfNullOrEmpty<T>(this T collection) where T : ICollection
        {
            collection.ThrowIfNull();

            if (collection.Count == Constants.Zero)
            {
                throw new EmptyCollectionException();
            }

            foreach (object item in collection)
            {
                item.ThrowIfNull();
            }

            return collection;
        }

        public static T ThrowIfDefault<T>(this T argument) where T : struct
        {
            if (argument.Equals(default(T)))
            {
                throw new ArgumentException();
            }

            return argument;
        }

        public static void ThrowIfFalse(this bool isValid, Exception exception = null)
        {
            if (isValid == false)
            {
                throw exception ?? new MissingComponentException();
            }
        }

        public static void ThrowIfTrue(this bool isValid, Exception exception = null)
        {
            if (isValid)
            {
                throw exception ?? new NotImplementedException();
            }
        }

        public static T ThrowIfNull<T>(this T argument, string message = null)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(message);
            }

            return argument;
        }

        public static string ThrowIfNullOrEmpty(this string argument)
        {
            string.IsNullOrEmpty(argument).ThrowIfTrue(new ArgumentException());

            return argument;
        }

        public static int ThrowIfLessThan(this int value, int range)
        {
            if (value < range)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value;
        }

        public static int ThrowIfZeroOrLess(this int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value;
        }

        public static float ThrowIfMoreThan(this float value, float range)
        {
            if (value > range)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value;
        }

        public static float ThrowIfLessThan(this float value, float range)
        {
            if (value < range)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value;
        }

        public static int ThrowIfMoreThan(this int value, int range)
        {
            if (value > range)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value;
        }

        public static float ThrowIfZeroOrLess(this float value)
        {
            if (value < 0 || Mathf.Approximately(value, 0))
            {
                throw new ArgumentOutOfRangeException();
            }

            return value;
        }

        public static float ThrowIfNegative(this float value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value;
        }

        public static int ThrowIfNegative(this int value, Exception exception = null)
        {
            if (value < 0)
            {
                throw exception ?? new ArgumentOutOfRangeException();
            }

            return value;
        }

        public static void ThrowIfAnimationMissing(this Animator animator, int hash, int layerIndex = Constants.Zero)
        {
            if (animator.HasState(layerIndex, hash) == false)
            {
                throw new MissingAnimationException();
            }
        }
    }
}
