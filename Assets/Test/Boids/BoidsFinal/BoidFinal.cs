using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Test.Boids.BoidsFinal
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoidFinal : MonoBehaviour
    {
        private const float MinPositionY = 0.5f;

        [Header("Movement Settings")]
        [SerializeField] private float _minSpeed = 0.5f;
        [SerializeField] private float _maxSpeed = 5f;
        [SerializeField] private float _maxAcceleration = 0.5f;
        [SerializeField] private float _rotationSpeed = 3f;

        [Header("Behavior Settings")]
        [SerializeField] private float _perceptionRadius = 5f;
        [SerializeField] private float _desiredSeparation = 2f;

        [Header("Optimization Settings")]
        [SerializeField] private float _behaviorUpdateDelay = 0.1f;

        [Header("Obstacle Avoidance")]
        [SerializeField] private float _avoidanceDistance = 3f;
        [SerializeField] private float _avoidanceWeight = 2f;
        [SerializeField] private LayerMask _obstacleLayer = ~0;

        [Header("2D Movement Settings")]
        [SerializeField] private bool _isMove2D = true;
        [SerializeField] private float _fixedHeight = 0.5f;

        [Header("Target Following")]
        [SerializeField] private float _targetStopDistance = 1f;

        private Vector3[] _rayDirections;
        private BoidCommanderFinal _commander;
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
                List<BoidFinal> neighbors = GetNeighbors();
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

            if (_isMove2D)
            {
                Vector3 position = transform.position;
                position.y = _fixedHeight;
                transform.position = position;
            }

            if (Velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(Velocity);
            }

            FillRayDirections();
            StartCoroutine(UpdateBehaviorCoroutine());
        }

        private void Update()
        {
            Move();
            Rotate();
        }

        private void FillRayDirections()
        {
            if (_isMove2D)
            {
                _rayDirections = new Vector3[]
                {
                    Vector3.forward,
                    Quaternion.Euler(0, -45, 0) * Vector3.forward,
                    Quaternion.Euler(0, 45, 0) * Vector3.forward
                };
            }
            else
            {
                _rayDirections = new Vector3[]
                {
                    Vector3.forward,
                    Quaternion.Euler(0, -30, 0) * Vector3.forward,
                    Quaternion.Euler(0, 30, 0) * Vector3.forward,
                    Quaternion.Euler(20, 0, 0) * Vector3.forward,
                    Quaternion.Euler(-20, 0, 0) * Vector3.forward
                };
            }
        }

        private IEnumerator UpdateBehaviorCoroutine()
        {
            WaitForSeconds wait = new(_behaviorUpdateDelay);

            while (true)
            {
                List<BoidFinal> neighbors = GetNeighbors();
                Vector3 acceleration = CalculateAcceleration(neighbors);

                ApplyAcceleration(acceleration);

                yield return wait;
            }
        }

        private void ClampHeight()
        {
            Vector3 position = transform.position;
            position.y = _isMove2D ? _fixedHeight : Mathf.Max(position.y, MinPositionY);

            transform.position = position;
        }

        private void ApplyAcceleration(Vector3 acceleration)
        {
            if (_isMove2D)
            {
                acceleration.y = 0;
            }

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
            ClampHeight();
        }

        private void Rotate()
        {
            if (Velocity == Vector3.zero)
            {
                return;
            }

            Quaternion targetRotation;

            if (_isMove2D)
            {
                Vector3 flatVelocity = Velocity;
                flatVelocity.y = 0;

                if (flatVelocity.magnitude > 0.01f)
                {
                    targetRotation = Quaternion.LookRotation(flatVelocity);
                }
                else
                {
                    return;
                }
            }
            else
            {
                targetRotation = Quaternion.LookRotation(Velocity);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }

        private Vector3 GenerateVelocity()
        {
            Vector3 velocity = Random.insideUnitSphere * _maxSpeed;

            if (_isMove2D)
            {
                velocity.y = 0;
            }

            return velocity;
        }

        private Vector3 CalculateAcceleration(List<BoidFinal> neighbors)
        {
            Vector3 separation = CalculateSeparation(neighbors) * _commander.SeparationWeight;
            Vector3 alignment = CalculateAlignment(neighbors) * _commander.AlignmentWeight;
            Vector3 cohesion = CalculateCohesion(neighbors) * _commander.CohesionWeight;
            Vector3 avoidance = CalculateObstacleAvoidance() * _avoidanceWeight;
            Vector3 target = CalculateTargetForce() * _commander.TargetWeight;

            Vector3 acceleration = separation + alignment + cohesion + avoidance + target;

            return Vector3.ClampMagnitude(acceleration, _maxAcceleration);
        }

        private Vector3 CalculateTargetForce()
        {
            if (_commander == null || _commander.TargetWeight <= 0.01f)
            {
                return Vector3.zero;
            }

            Vector3 targetPosition = _commander.TargetPosition;
            Vector3 direction = targetPosition - transform.position;

            float distance = direction.magnitude;

            return distance < _targetStopDistance ?
                -Velocity.normalized * Mathf.Clamp01(distance / _targetStopDistance) 
                : direction.normalized;
        }

        private Vector3 CalculateObstacleAvoidance()
        {
            if (_rayDirections == null)
            {
                return Vector3.zero;
            }

            Vector3 avoidanceForce = Vector3.zero;

            foreach (Vector3 direction in _rayDirections)
            {
                Vector3 worldDirection = transform.TransformDirection(direction);

                if (Physics.Raycast(transform.position, worldDirection, out RaycastHit hit, _avoidanceDistance, _obstacleLayer))
                {
                    float distanceFactor = 1f - (hit.distance / _avoidanceDistance);

                    Vector3 avoidanceDirection = (transform.position - hit.point).normalized;
                    avoidanceForce += avoidanceDirection * distanceFactor;
                }
            }

            DrawDebugRays();

            return avoidanceForce;
        }

        private void DrawDebugRays()
        {
            if (_rayDirections == null)
            {
                return;
            }

            foreach (Vector3 direction in _rayDirections)
            {
                Vector3 worldDirection = transform.TransformDirection(direction);
                Debug.DrawRay(transform.position, worldDirection * _avoidanceDistance, Color.magenta, _behaviorUpdateDelay);
            }
        }

        public BoidFinal SetCommander(BoidCommanderFinal commander)
        {
            _commander = commander;

            return this;
        }

        private List<BoidFinal> GetNeighbors()
        {
            IReadOnlyList<BoidFinal> boids = _commander.Boids;
            List<BoidFinal> neighbors = new();

            foreach (BoidFinal boid in boids)
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

        private Vector3 CalculateSeparation(List<BoidFinal> neighbors)
        {
            if (neighbors.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 steer = Vector3.zero;
            int closeNeighbors = 0;

            foreach (BoidFinal boid in neighbors)
            {
                float distance = Vector3.Distance(transform.position, boid.transform.position);

                if (distance < _desiredSeparation && distance > 0.1f)
                {
                    Vector3 difference = transform.position - boid.transform.position;
                    difference.Normalize();

                    difference /= Mathf.Max(distance, 0.5f);

                    steer += difference;
                    closeNeighbors++;
                }
            }

            if (closeNeighbors > 0)
            {
                steer /= closeNeighbors;
            }

            return steer;
        }

        private Vector3 CalculateAlignment(List<BoidFinal> neighbors)
        {
            if (neighbors.Count == 0)
            {
                return Velocity.normalized;
            }

            Vector3 averageVelocity = Vector3.zero;

            foreach (BoidFinal other in neighbors)
            {
                averageVelocity += other.Velocity;
            }

            averageVelocity /= neighbors.Count;
            averageVelocity.Normalize();

            Vector3 steer = averageVelocity - Velocity.normalized;

            return steer;
        }

        private Vector3 CalculateCohesion(List<BoidFinal> neighbors)
        {
            if (neighbors.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 centerOfMass = Vector3.zero;

            foreach (BoidFinal other in neighbors)
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
