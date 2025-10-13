using Assets.Code.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class CharacterMovement : MonoBehaviour
    {
        private readonly Dictionary<Type, Coroutine> _slowTimers;

        private ITellDirection _directionSource;
        private Mover _mover;
        private Rotator _rotator;
        private Vector3 _direction;
        private Rigidbody _rigidbody;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnDisable()
        {
            if (_rigidbody != null)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }

        private void OnDestroy()
        {
            if (_directionSource != null)
            {
                _directionSource.DirectionChanged -= SetDirection;
            }
        }

        public void Initialize(float moveSpeed, float rotationSpeed, ITellDirection directionSource)
        {
            _directionSource = directionSource.ThrowIfNull();
            _rigidbody = GetComponent<Rigidbody>();

            _mover = new(_rigidbody, moveSpeed);
            _rotator = new(_rigidbody, rotationSpeed);
        }

        private void Move()
        {
            if (_direction == Vector3.zero)
            {
                return;
            }

            _mover.Move(_direction);
            _rotator.Rotate(_direction);
        }

        private void SetDirection(Vector3 vector)
        {
            _direction = vector;

            _animator.SetBool(AnimationParameters.IsMoving, _direction != Vector3.zero);
        }

        public void AddSpeed(float value)
        {
            _mover.AddSpeed(value);
        }

        public void ResetSpeed()
        {
            _mover.ResetSpeed();
        }

        public void AddSlow(SlowEffect slow)
        {
            if (_slowTimers.TryGetValue(slow.ThrowIfDefault().Source, out Coroutine slowTimer))
            {
                StopCoroutine(slowTimer);
                slowTimer = StartCoroutine(StartSlowTimer(slow));

                return;
            }

            slowTimer = StartCoroutine(StartSlowTimer(slow));
            _slowTimers.Add(slow.Source, slowTimer);

            _mover.AddMultiplier(slow.Multiplier);
        }

        private IEnumerator StartSlowTimer(SlowEffect slow)
        {
            float duration = slow.Duration;

            while (duration > Constants.Zero)
            {
                duration -= Time.deltaTime;

                yield return null;
            }

            _slowTimers.Remove(slow.Source);
            _mover.RemoveMultiplier(slow.Multiplier);
        }

        public void SetAdditionalSpeed(int value)
        {
            _mover.AddSpeed(value.ThrowIfNegative());
        }

        public void Run()
        {
            _directionSource.DirectionChanged += SetDirection;
            _directionSource.Enable();

            UpdateService.RegisterFixedUpdate(Move);
        }

        public void Stop()
        {
            _directionSource.Disable();
            _directionSource.DirectionChanged -= SetDirection;

            UpdateService.UnregisterFixedUpdate(Move);
        }
    }
}
