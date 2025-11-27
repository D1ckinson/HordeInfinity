using Assets.Code.Tools.Base;
using Assets.Code.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Animation
{
    public sealed class EntityAnimator<T> : IAnimator where T : Enum
    {
        private const float TransitionDuration = 0.1f;

        private readonly Animator _animator;
        private readonly Dictionary<Enum, AnimationData> _animationData;
        private readonly Dictionary<int, AnimationData> _currentAnimation = new();

        public EntityAnimator(Animator animator)
        {
            _animator = animator.ThrowIfNull();
            _animationData = CreateAnimationData();

            for (int i = 0; i < _animator.layerCount; i++)
            {
                _currentAnimation[i] = AnimationData.Empty;
            }
        }

        public void Play(Enum animation)
        {
            AnimationData animationInfo = _animationData[animation.ThrowIfNull()];
            int layerIndex = animationInfo.LayerIndex;

            AnimationData currentAnimationInfo = _currentAnimation[layerIndex];

            if (animationInfo.Hash == _animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash)
            {
                return;
            }

            if (animationInfo.Priority >= currentAnimationInfo.Priority || _animator.IsAnimationFinished(animationInfo.Hash, layerIndex))
            {
                _animator.CrossFade(animationInfo.Hash, TransitionDuration, layerIndex);
                _currentAnimation[layerIndex] = animationInfo;
            }
        }

        private Dictionary<Enum, AnimationData> CreateAnimationData()
        {
            IEnumerable<T> animationsNames = Constants.GetEnums<T>();

            Dictionary<Enum, AnimationData> animationHashes = new(animationsNames.Count());

            foreach (T name in animationsNames)
            {
                int hash = Animator.StringToHash(name.ToString());
                int layerIndex = _animator.GetStateLayerIndex(hash).ThrowIfNegative(new MissingAnimationException());

                AnimationData animationInfo = AnimationData.Create(name, layerIndex);
                animationHashes.Add(name, animationInfo);
            }

            return animationHashes;
        }
    }
}
