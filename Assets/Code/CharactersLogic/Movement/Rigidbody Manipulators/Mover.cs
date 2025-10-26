using Assets.Code.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    public class Mover
    {
        private readonly List<float> _multipliers = new();
        private readonly Rigidbody _rigidbody;
        private readonly float _defaultSpeed;

        private float _speed;

        public Mover(Rigidbody rigidbody, float speed)
        {
            _rigidbody = rigidbody.ThrowIfNull();
            _defaultSpeed = speed.ThrowIfNegative();
            _speed = _defaultSpeed;

        }

        public void Move(Vector3 direction)
        {
            direction.ThrowIfNotNormalize();
            direction.y = Constants.Zero;

            Vector3 position = _rigidbody.position + direction * (_speed * Time.fixedDeltaTime);
            _rigidbody.MovePosition(position);
        }

        public void AddSpeed(float value)
        {
            _speed = _defaultSpeed + value.ThrowIfNegative();
        }

        public void ResetSpeed()
        {
            _speed = _defaultSpeed;
        }

        public void AddMultiplier(float multiplier)
        {
            _multipliers.Add(multiplier.ThrowIfZeroOrLess().ThrowIfMoreThan(Constants.One));
        }

        public void RemoveMultiplier(float multiplier)
        {
            if (_multipliers.Remove(multiplier) == false)
            {
                throw new ArgumentException();
            }
        }

        private void CalculateSpeed()
        {
            float resultMultiplier = Constants.One;
            _multipliers.ForEach(multiplier => resultMultiplier *= multiplier);

            _speed = _defaultSpeed * resultMultiplier;
        }
    }
}
