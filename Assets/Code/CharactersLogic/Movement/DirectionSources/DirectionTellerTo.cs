using Assets.Code.CharactersLogic.Movement.Interfaces;
using Assets.Code.Tools.Base;
using System;
using TMPro;
using UnityEngine;

namespace Assets.Code.CharactersLogic.Movement.DirectionSources
{
    public class DirectionTellerTo : ITargetDirectionTeller
    {
        private const float CalculateDelay = 0.15f;

        private readonly Transform _owner;
        private Transform _target;
        private Vector3 _moveDirection;
        private float _time;

        public DirectionTellerTo(Transform owner)
        {
            _owner = owner.ThrowIfNull();
        }

        public event Action<Vector3> DirectionChanged;

        public void SetTarget(Transform target)
        {
            _target = target.ThrowIfNull();
        }

        public void Enable()
        {
            UpdateService.RegisterUpdate(CalculateDirection);
        }

        public void Disable()
        {
            UpdateService.UnregisterUpdate(CalculateDirection);

            _moveDirection = Vector3.zero;
            _time = CalculateDelay;
            DirectionChanged?.Invoke(_moveDirection);
        }

        private void CalculateDirection(float deltaTime)
        {
            if (_owner.IsActive() == false || _target.IsNull() || _target.IsActive() == false)
            {
                Disable();

                return;
            }

            _time -= deltaTime;

            if (_time > Constants.Zero)
            {
                return;
            }

            _time = CalculateDelay;
            Vector3 moveDirection = (_target.position - _owner.position).normalized;

            if (_moveDirection.Compare(moveDirection, Constants.CompareAccuracy))
            {
                return;
            }

            _moveDirection = moveDirection;
            DirectionChanged?.Invoke(_moveDirection);
        }
    }
}
