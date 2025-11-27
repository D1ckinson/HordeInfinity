using Assets.Code.Tools.Base;
using System;
using UnityEngine;

namespace Assets.Code.Animation
{
    public readonly struct AnimationData
    {
        public readonly int Hash;
        public readonly int Priority;
        public readonly int LayerIndex;

        public static AnimationData Empty => new(Constants.Zero, -Constants.Zero, Constants.Zero);

        private AnimationData(int hash, int priority, int layerIndex)
        {
            Hash = hash;
            Priority = priority.ThrowIfNegative();
            LayerIndex = layerIndex.ThrowIfNegative();
        }

        public static AnimationData Create(Enum name, int layerIndex)
        {
            int hash = Animator.StringToHash(name.ToString());
            int priority = Convert.ToInt32(name);

            return new(hash, priority, layerIndex);
        }
    }
}