using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Test.Boids.BoidsFinal
{
    public class BoidCommanderFinal : MonoBehaviour
    {
        [Header("Boid Settings")]
        [SerializeField] private BoidFinal _prefab;
        [SerializeField] private int _boidCount = 10;
        [SerializeField] private float _spawnRadius = 5f;

        [Header("Optimization Settings")]
        [SerializeField] private float _updateDelay = 0.1f;

        [Header("Target Following")]
        [SerializeField] private Transform _target;
        [SerializeField] private float _targetUpdateDelay = 3f;
        [SerializeField] private float _targetRadius = 10f;

        [field: SerializeField] public float TargetWeight { get; private set; } = 0.5f;

        [field: Header("Behavior Weights")]
        [field: SerializeField] public float SeparationWeight { get; private set; } = 1.5f;
        [field: SerializeField] public float AlignmentWeight { get; private set; } = 1f;
        [field: SerializeField] public float CohesionWeight { get; private set; } = 1f;

        private readonly List<BoidFinal> _boids = new();

        public Vector3 TargetPosition { get; private set; }
        public IReadOnlyList<BoidFinal> Boids => _boids;

        private void OnDrawGizmosSelected()
        {
            if (_target == null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, _targetRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(TargetPosition, 0.5f);
                Gizmos.DrawLine(transform.position, TargetPosition);
            }
        }

        private void Start()
        {
            SpawnBoids();
            StartCoroutine(UpdateBoidsCoroutine());
            StartCoroutine(UpdateTargetPositionCoroutine());

            TargetPosition = _target == null ? transform.position : _target.position;
        }

        private IEnumerator UpdateTargetPositionCoroutine()
        {
            WaitForSeconds wait = new(_targetUpdateDelay);

            while (true)
            {
                UpdateTargetPosition();
                yield return wait;
            }
        }

        private void UpdateTargetPosition()
        {
            if (_target != null)
            {
                TargetPosition = _target.position;
            }
            else
            {
                Vector2 randomCircle = Random.insideUnitCircle * _targetRadius;
                TargetPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            }
        }

        private void SpawnBoids()
        {
            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 randomPos = Random.insideUnitSphere * _spawnRadius;
                randomPos += transform.position;

                BoidFinal boid = Instantiate(_prefab, randomPos, Quaternion.identity, transform)
                    .SetCommander(this);

                _boids.Add(boid);
            }
        }

        private IEnumerator UpdateBoidsCoroutine()
        {
            WaitForSeconds wait = new(_updateDelay);

            while (true)
            {
                UpdateAllBoids();

                yield return wait;
            }
        }

        private void UpdateAllBoids()
        {

        }
    }
}
