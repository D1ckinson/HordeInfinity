using UnityEngine;

namespace Assets.Test.Boids
{
    [RequireComponent(typeof(Rigidbody))]
    public class Boid : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _stopDistance = 0.5f;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 5f;

        [Header("Boids")]
        [SerializeField] private Collider _collider;
        [SerializeField] private float _neighborDistance = 1f;
        [SerializeField] private float _separationWeight = 1f;
        [SerializeField] private float _alignmentWeight = 1f;
        [SerializeField] private float _cohesionWeight = 1f;

        private readonly Collider[] _neighbors = new Collider[21];

        private Vector3 _direction;
        private LayerMask _layer;

        private void Awake()
        {
            _layer = 1 << gameObject.layer;
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (_target != null)
            {
                FollowTarget();
            }
        }

        private void FixedUpdate()
        {
            if (_target != null)
            {
                UpdateDirection();
            }
        }

        private void FollowTarget()
        {
            if (_direction != Vector3.zero)
            {
                Move();
                Rotate();
            }
        }

        private void UpdateDirection()
        {
            Vector3 direction = _target.position - transform.position;
            float sqrDistance = direction.sqrMagnitude;

            if (sqrDistance > _stopDistance)
            {
                Vector3 separationForce = Vector3.zero;
                int count = Physics.OverlapSphereNonAlloc(transform.position, _neighborDistance, _neighbors, _layer);

                if (count > 1)
                {
                    separationForce += CalculateSeparationForce(count);
                    separationForce += CalculateAlignmentForce(count);
                    separationForce += CalculateCohesion(count);
                }

                _direction = (direction + separationForce).normalized;
            }
            else
            {
                _direction = Vector3.zero;
            }
        }

        private Vector3 CalculateCohesion(int count)
        {
            Vector3 averagePosition = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                averagePosition += _neighbors[i].transform.position;
            }

            averagePosition /= count;

            return ((averagePosition - transform.position).normalized) * _cohesionWeight;
        }

        private Vector3 CalculateAlignmentForce(int count)
        {
            Vector3 result = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                Collider collider = _neighbors[i];

                result += collider.transform.forward;
            }

            if (result == Vector3.zero)
            {
                return result;
            }

            return result.normalized * _alignmentWeight;
        }

        private Vector3 CalculateSeparationForce(int count)
        {
            Vector3 result = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                Collider collider = _neighbors[i];

                if (collider == _collider)
                {
                    continue;
                }

                Vector3 direction = collider.transform.position - transform.position;
                float distance = direction.magnitude;

                if (distance > 0)
                {
                    result += (-direction.normalized / distance) * _separationWeight;
                }
            }

            return result;
        }

        private void Move()
        {
            transform.position += _moveSpeed * Time.deltaTime * _direction;
        }

        private void Rotate()
        {
            Quaternion rotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);
        }
    }
}
