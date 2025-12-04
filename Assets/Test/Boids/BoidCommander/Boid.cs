using System.Collections.Generic;
using UnityEngine;

namespace Assets.Test.Boids.BoidCommander
{
    [RequireComponent(typeof(Rigidbody))]
    public class Boid : MonoBehaviour
    {
        private const float MinPositionY = 0.5f;

        [Header("Movement Settings")]
        [SerializeField] private float _minSpeed = 0.5f;
        [SerializeField] private float _maxSpeed = 5f;
        [SerializeField] private float _maxAcceleration = 0.5f;
        [SerializeField] private float _rotationSpeed = 3f;
        [SerializeField] private float _perceptionRadius = 5f;
        [SerializeField] private float _desiredSeparation = 2f;

        private BoidCommander _commander;
        private Rigidbody _rigidbody;

        public Vector3 Velocity { get; private set; }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 5f);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Velocity.normalized * 2);

            if (_commander != null)
            {
                List<Boid> neighbors = GetNeighbors();
                if (neighbors.Count > 0)
                {
                    Vector3 center = Vector3.zero;
                    foreach (var n in neighbors) center += n.transform.position;
                    center /= neighbors.Count;

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, center);
                }
            }
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            Velocity = GenerateVelocity();

            if (Velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(Velocity);
            }
        }

        private void Update()
        {
            List<Boid> neighbors = GetNeighbors();
            Vector3 acceleration = CalculateAcceleration(neighbors);

            ApplyAcceleration(acceleration);
            Move();
            Rotate();
            ClampPositionY();
        }

        private void ClampPositionY()
        {
            Vector3 position = transform.position;
            position.y = Mathf.Max(position.y, MinPositionY);
            transform.position = position;
        }

        private void ApplyAcceleration(Vector3 acceleration)
        {
            Velocity += acceleration;
            Velocity = Vector3.ClampMagnitude(Velocity, _maxSpeed);

            if (Velocity.magnitude < _minSpeed)
            {
                Velocity = transform.forward * _minSpeed;
            }
        }

        private void Move()
        {
            transform.position += Velocity * Time.deltaTime;
        }

        private void Rotate()
        {
            if (Velocity == Vector3.zero)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(Velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        private Vector3 GenerateVelocity()
        {
            Vector3 velocity = Random.insideUnitSphere * _maxSpeed;
            velocity.y = 0;

            return velocity;
        }

        private Vector3 CalculateAcceleration(List<Boid> neighbors)
        {
            Vector3 separation = CalculateSeparation(neighbors) * _commander.SeparationWeight;
            Vector3 alignment = CalculateAlignment(neighbors) * _commander.AlignmentWeight;
            Vector3 cohesion = CalculateCohesion(neighbors) * _commander.CohesionWeight;
            Vector3 acceleration = separation + alignment + cohesion;

            return Vector3.ClampMagnitude(acceleration, _maxAcceleration);
        }

        public Boid SetCommander(BoidCommander commander)
        {
            _commander = commander;

            return this;
        }

        private List<Boid> GetNeighbors()
        {
            IReadOnlyList<Boid> boids = _commander.GetAllBoids();
            List<Boid> neighbors = new();

            foreach (Boid boid in boids)
            {
                if (boid == this)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, boid.transform.position);

                if (distance < _perceptionRadius)
                {
                    neighbors.Add(boid);
                }
            }

            return neighbors;
        }

        private Vector3 CalculateSeparation(List<Boid> neighbors)
        {
            if (neighbors.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 steer = Vector3.zero;

            foreach (Boid boid in neighbors)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);

                if (distance >= _desiredSeparation || distance <= 0)
                {
                    continue;
                }

                Vector3 difference = transform.position - boid.transform.position;
                difference.Normalize();
                difference /= distance;

                steer += difference;
            }

            return steer;
        }

        private Vector3 CalculateAlignment(List<Boid> neighbors)
        {
            if (neighbors.Count == 0)
            {
                return Velocity.normalized;
            }

            Vector3 averageVelocity = Vector3.zero;

            foreach (Boid other in neighbors)
            {
                averageVelocity += other.Velocity;
            }

            averageVelocity /= neighbors.Count;
            averageVelocity.Normalize();

            Vector3 steer = averageVelocity - Velocity.normalized;

            return steer;
        }

        private Vector3 CalculateCohesion(List<Boid> neighbors)
        {
            if (neighbors.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 centerOfMass = Vector3.zero;

            foreach (Boid other in neighbors)
            {
                centerOfMass += other.transform.position;
            }

            centerOfMass /= neighbors.Count;

            Vector3 desiredDirection = centerOfMass - transform.position;
            desiredDirection.Normalize();

            Vector3 steer = desiredDirection - Velocity.normalized;

            return steer;
        }
    }
}
