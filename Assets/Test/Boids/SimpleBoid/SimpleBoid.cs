using UnityEngine;

namespace Assets.Test.Boids.SimpleBoid
{
    public class SimpleBoid : MonoBehaviour
    {
        private const float Boundary = 10f;
        private const float MaxPositionY = 5f;

        [Header("Movement Settings")]
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 3f;

        private Vector3 _velocity;

        private void Start()
        {
            _velocity = Random.insideUnitSphere * _speed;
            _velocity.y = 0;
        }

        private void Update()
        {
            Move();
            Rotate();
            BounceOffWalls();
        }

        private void Move()
        {
            transform.position += _velocity * Time.deltaTime;
        }

        private void Rotate()
        {
            if (_velocity == Vector3.zero)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(_velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        private void BounceOffWalls()
        {
            Vector3 position = transform.position;

            if (Mathf.Abs(position.x) > Boundary) _velocity.x *= -1;
            if (Mathf.Abs(position.z) > Boundary) _velocity.z *= -1;

            position.y = Mathf.Clamp(position.y, 1f, MaxPositionY);
            transform.position = position;
        }
    }
}