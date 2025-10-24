using Assets.Code.Tools;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Code.CharactersLogic.Movement
{
    internal class NewMovement { }

    public class NewMover
    {
        private readonly ITellDirection _directionSource;
        private readonly Rigidbody _rigidbody;

        private readonly float _defaultSpeed;
        private readonly float _minSpeed;
        private readonly float _maxSpeed;
        private readonly Animator _animator;

        private Vector3 _direction;

        public NewMover(ITellDirection directionSource, Rigidbody rigidbody, Animator animator, float defaultSpeed, float minSpeed = 0, float maxSpeed = float.MaxValue)
        {
            _directionSource = directionSource.ThrowIfNull();
            _rigidbody = rigidbody.ThrowIfNull();
            _animator = animator.ThrowIfNull();

            _defaultSpeed = defaultSpeed.ThrowIfNegative();
            _minSpeed = minSpeed.ThrowIfNegative();
            _maxSpeed = maxSpeed.ThrowIfNegative();

            Speed = _defaultSpeed;
        }

        ~NewMover()
        {
            if (_directionSource.NotNull())
            {
                _directionSource.DirectionChanged -= SetDirection;
            }

            UpdateService.UnregisterFixedUpdate(Move);
        }

        public float Speed { get; private set; }

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

        public void AddSpeed(float speed)
        {
            float resultSpeed = Speed += speed.ThrowIfNegative();

            Speed = resultSpeed > _maxSpeed ? _maxSpeed : resultSpeed;
        }

        public void RemoveSpeed(float speed)
        {
            float resultSpeed = Speed -= speed.ThrowIfNegative();

            Speed = resultSpeed > _minSpeed ? resultSpeed : _minSpeed;
        }

        public void ResetSpeed()
        {
            Speed = _defaultSpeed;
        }

        private void Move()
        {
            Vector3 position = _rigidbody.position + _direction * (Speed * Time.fixedDeltaTime);
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
                direction.ThrowIfNotNormalize();
                direction.y = Constants.Zero;
                _direction = direction;
            }

            _animator.SetBool(AnimationParameters.IsMoving, _direction != Vector3.zero);
        }
    }

    public class NewRotator
    {
        private readonly ITellDirection _directionSource;
        private readonly Rigidbody _rigidbody;
        private readonly float _speed;

        private Vector3 _direction;

        public NewRotator(ITellDirection directionSource, Rigidbody rigidbody, float speed)
        {
            _directionSource = directionSource.ThrowIfNull();
            _rigidbody = rigidbody.ThrowIfNull();
            _speed = speed.ThrowIfNegative();
        }

        ~NewRotator()
        {
            if (_directionSource.NotNull())
            {
                _directionSource.DirectionChanged -= SetDirection;
            }

            UpdateService.UnregisterFixedUpdate(Rotate);
        }

        public void Run()
        {
            _directionSource.DirectionChanged += SetDirection;
            UpdateService.RegisterFixedUpdate(Rotate);
        }

        public void Stop()
        {
            _directionSource.DirectionChanged -= SetDirection;
            UpdateService.UnregisterFixedUpdate(Rotate);
        }

        private void SetDirection(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                _direction = Vector3.zero;
            }
            else
            {
                direction.ThrowIfNotNormalize();
                direction.y = Constants.Zero;
                _direction = direction;
            }
        }

        private void Rotate()
        {
            if (_direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            Quaternion fromRotation = _rigidbody.rotation;
            Quaternion toRotation = Quaternion.LookRotation(_direction);

            Quaternion rotation = Quaternion.RotateTowards(fromRotation, toRotation, _speed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(rotation);
        }
    }
}
