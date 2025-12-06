using Assets.Code.CharactersLogic.Movement.Interfaces;
using Assets.Code.Data.Value;
using Assets.Code.Tools.Base;
using UnityEngine;

namespace Assets.Code.CharactersLogic.GeneralLogic
{
    public class Mover
    {
        private readonly ITellDirection _directionSource;
        private readonly Rigidbody _rigidbody;
        private readonly Animator _animator;

        private Vector3 _direction;

        public Mover(
            ITellDirection directionSource,
            Rigidbody rigidbody,
            Animator animator,
            ValueContainer speed)
        {
            _directionSource = directionSource.ThrowIfNull();
            _rigidbody = rigidbody.ThrowIfNull();
            _animator = animator.ThrowIfNull();
            Speed = speed.ThrowIfNull();
        }

        ~Mover()
        {
            if (_directionSource.IsNotNull())
            {
                _directionSource.DirectionChanged -= SetDirection;
            }

            UpdateService.UnregisterFixedUpdate(Move);
        }

        public ValueContainer Speed { get; }

        public void Run()
        {
            _directionSource.Enable();
            _directionSource.DirectionChanged += SetDirection;

            UpdateService.RegisterFixedUpdate(Move);
        }

        public void Stop()
        {
            _directionSource.Disable();
            _directionSource.DirectionChanged -= SetDirection;

            UpdateService.UnregisterFixedUpdate(Move);
        }

        private void Move(float fixedDeltaTime)
        {
            Vector3 position = _rigidbody.position + _direction * (Speed.Value * fixedDeltaTime);
            _rigidbody.MovePosition(position);
        }

        private void SetDirection(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                _direction = Vector3.zero;
            }
            else
            {
                direction.y = Constants.Zero;
                _direction = direction;
            }

            _animator.SetBool(AnimationParameters.IsMoving, _direction != Vector3.zero);
        }
    }
}
