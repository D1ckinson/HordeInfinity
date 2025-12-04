using System.Collections.Generic;
using UnityEngine;

namespace Assets.Test.Boids.BoidCommanderCut
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoidCut : MonoBehaviour
    {
        private const float MinPositionY = 0.5f;

        [Header("Movement Settings")]
        [SerializeField] private float _maxSpeed = 5f;
        [SerializeField] private float _maxAcceleration = 0.5f;
        [SerializeField] private float _rotationSpeed = 3f;

        private BoidCommanderCut _commander;
        private Rigidbody _rigidbody;
        private Vector3 _velocity;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _velocity = Random.insideUnitSphere * _maxSpeed;
            _velocity.y = 0;

            if (_velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(_velocity);
            }
        }

        private void Update()
        {
            List<BoidCut> neighbors = GetNeighbors();
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
            _velocity += acceleration;
            _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);
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

        private Vector3 CalculateAcceleration(List<BoidCut> neighbors)
        {
            Vector3 separation = CalculateSeparation(neighbors) * _commander.SeparationWeight;
            Vector3 alignment = CalculateAlignment(neighbors) * _commander.AlignmentWeight;
            Vector3 cohesion = CalculateCohesion(neighbors) * _commander.CohesionWeight;
            Vector3 acceleration = separation + alignment + cohesion;

            return Vector3.ClampMagnitude(acceleration, _maxAcceleration);
        }

        public BoidCut SetCommander(BoidCommanderCut commander)
        {
            _commander = commander;

            return this;
        }

        private List<BoidCut> GetNeighbors()
        {
            return new List<BoidCut>();
        }

        private Vector3 CalculateSeparation(List<BoidCut> neighbors)
        {
            return Vector3.zero;
        }

        private Vector3 CalculateAlignment(List<BoidCut> neighbors)
        {
            return Vector3.zero;
        }

        private Vector3 CalculateCohesion(List<BoidCut> neighbors)
        {
            return Vector3.zero;
        }
    }
}
