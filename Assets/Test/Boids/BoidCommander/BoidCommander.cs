using System.Collections.Generic;
using UnityEngine;

namespace Assets.Test.Boids.BoidCommander
{
    public class BoidCommander : MonoBehaviour
    {
        [Header("Boid Settings")]
        [SerializeField] private Boid _prefab;
        [SerializeField] private int _boidCount = 10;
        [SerializeField] private float _spawnRadius = 5f;

        [field: Header("Behavior Weights")]
        [field: SerializeField] public float SeparationWeight { get; private set; } = 1.5f;
        [field: SerializeField] public float AlignmentWeight { get; private set; } = 1f;
        [field: SerializeField] public float CohesionWeight { get; private set; } = 1f;

        private readonly List<Boid> _boids = new();

        private void Start()
        {
            SpawnBoids();
        }

        private void SpawnBoids()
        {
            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 randomPos = Random.insideUnitSphere * _spawnRadius;
                randomPos += transform.position;

                Boid boid = Instantiate(_prefab, randomPos, Quaternion.identity, transform)
                    .SetCommander(this);

                _boids.Add(boid);
            }
        }

        public IReadOnlyList<Boid> GetAllBoids()
        {
            return _boids;
        }
    }
}
