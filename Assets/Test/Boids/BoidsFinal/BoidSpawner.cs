using System.Collections.Generic;
using UnityEngine;

namespace Assets.Test.Boids.BoidsFinal
{
    public class BoidSpawner : MonoBehaviour
    {
        [Header("Boid Settings")]
        [SerializeField] private Boid _prefab;
        [SerializeField] private int _boidCount = 10;
        [SerializeField] private float _spawnRadius = 5f;
        [SerializeField] private BoidSettings _settings;

        [Header("Target Settings")]
        [SerializeField] private Transform _target;

        public Vector3 TargetPosition => _target == null ? transform.position : _target.position;
        public IReadOnlyList<Boid> Boids => _boids;

        private readonly List<Boid> _boids = new();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(TargetPosition, 0.3f);
        }

        private void Start()
        {
            SpawnBoids();
        }

        private void SpawnBoids()
        {
            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 randomPosition = transform.position + Random.insideUnitSphere * _spawnRadius;
                randomPosition.y = 1f;

                Boid boid = Instantiate(_prefab, randomPosition, Quaternion.identity).Initialize(this, _settings);
                _boids.Add(boid);
            }
        }
    }
}
