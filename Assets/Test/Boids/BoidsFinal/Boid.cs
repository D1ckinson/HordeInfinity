using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Test.Boids.BoidsFinal
{
    [RequireComponent(typeof(Rigidbody))]
    public class Boid : MonoBehaviour
    {
        private BoidSpawner _commander;
        private BoidSettings _settings;
        private Vector3 _velocity;
        private List<Boid> _neighbors = new(20);

        private void Start()
        {
            _velocity = Random.insideUnitSphere * _settings.Speed;
            _velocity.y = 0;

            if (_velocity.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(_velocity);
            }

            StartCoroutine(UpdateBehaviorCoroutine());
        }

        private void Update()
        {
            Move();
            Rotate();
        }

        public Boid Initialize(BoidSpawner commander, BoidSettings settings)
        {
            _commander = commander;
            _settings = settings;

            return this;
        }

        private IEnumerator UpdateBehaviorCoroutine()
        {
            WaitForSeconds wait = new(_settings.UpdateBehaviorDelay);

            while (true)
            {
                UpdateBehavior();

                yield return wait;
            }
        }

        private void UpdateBehavior()
        {
            Vector3 targetForce = CalculateTargetForce();

            UpdateNeighbors();
            Vector3 socialForces = Vector3.zero;

            if (_neighbors.Count > 0)
            {
                Vector3 separation = CalculateSeparation(_neighbors) * _settings.SeparationWeight;
                Vector3 alignment = CalculateAlignment(_neighbors) * _settings.AlignmentWeight;
                Vector3 cohesion = CalculateCohesion(_neighbors) * _settings.CohesionWeight;

                socialForces = separation + alignment + cohesion;
            }

            Vector3 acceleration = targetForce + socialForces;

            acceleration = Vector3.ClampMagnitude(acceleration, _settings.MaxAcceleration);
            acceleration.y = 0;

            _velocity += acceleration;
            _velocity = Vector3.ClampMagnitude(_velocity, _settings.Speed);

            if (_velocity.magnitude < 0.5f)
            {
                _velocity = transform.forward * 0.5f;
            }
        }

        private Vector3 CalculateTargetForce()
        {
            if (_commander == null)
            {
                return Vector3.zero;
            }

            Vector3 targetPos = _commander.TargetPosition;
            Vector3 direction = targetPos - transform.position;

            return direction.magnitude < _settings.TargetStopDistance ? -_velocity.normalized * 0.5f : direction.normalized;
        }

        private void Move()
        {
            transform.position += _velocity * Time.deltaTime;
        }

        private void Rotate()
        {
            if (_velocity.magnitude > 0.1f)
            {
                Vector3 lookDirection = _velocity;
                lookDirection.y = 0;

                if (lookDirection.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _settings.RotationSpeed * Time.deltaTime);
                }
            }
        }

        private Vector3 CalculateSeparation(List<Boid> neighbors)
        {
            if (neighbors.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 steer = Vector3.zero;
            int count = 0;
            float desiredSeparationSqr = _settings.DesiredSeparation * _settings.DesiredSeparation;

            foreach (Boid neighbor in neighbors)
            {
                Vector3 offset = neighbor.transform.position - transform.position;
                float distanceSqr = offset.sqrMagnitude;

                if (distanceSqr < desiredSeparationSqr && distanceSqr > 0.01f)
                {
                    float distance = Mathf.Sqrt(distanceSqr);
                    Vector3 difference = transform.position - neighbor.transform.position;
                    difference.Normalize();
                    difference /= distance;

                    steer += difference;
                    count++;
                }
            }

            return count > 0 ? steer /= count : steer;
        }

        private void UpdateNeighbors()
        {
            _neighbors.Clear();

            IReadOnlyList<Boid> boids = _commander.Boids;
            float perceptionSqr = _settings.PerceptionRadius * _settings.PerceptionRadius;

            foreach (Boid boid in boids)
            {
                if (boid == this)
                {
                    continue;
                }

                Vector3 offset = boid.transform.position - transform.position;

                if (offset.sqrMagnitude < perceptionSqr)
                {
                    _neighbors.Add(boid);
                }
            }
        }

        private Vector3 CalculateAlignment(List<Boid> neighbors)
        {
            if (neighbors.Count == 0)
            {
                return _velocity.normalized;
            }

            Vector3 averageVelocity = Vector3.zero;

            foreach (Boid other in neighbors)
            {
                averageVelocity += other._velocity;
            }

            averageVelocity /= neighbors.Count;
            averageVelocity.Normalize();

            Vector3 steer = averageVelocity - _velocity.normalized;

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

            Vector3 steer = desiredDirection - _velocity.normalized;

            return steer;
        }
    }
}
