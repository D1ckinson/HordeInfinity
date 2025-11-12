using Assets.Code.Tools;
using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Code.CharactersLogic.Movement
{
    public class Rotator
    {
        private readonly ITellDirection _directionSource;
        private readonly Rigidbody _rigidbody;
        private readonly float _speed;

        private Vector3 _direction;

        public Rotator(ITellDirection directionSource, Rigidbody rigidbody, float speed)
        {
            _directionSource = directionSource.ThrowIfNull();
            _rigidbody = rigidbody.ThrowIfNull();
            _speed = speed.ThrowIfNegative();
        }

        ~Rotator()
        {
            if (_directionSource.IsNotNull())
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

        private void Rotate(float fixedDeltaTime)
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
