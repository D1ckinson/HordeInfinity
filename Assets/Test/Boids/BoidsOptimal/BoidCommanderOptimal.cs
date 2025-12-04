using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Test.Boids.BoidsOptimal
{
    public class BoidCommanderOptimal : MonoBehaviour
    {
        [Header("Boid Settings")]
        [SerializeField] private BoidOptimal _prefab;
        [SerializeField] private int _boidCount = 10;
        [SerializeField] private float _spawnRadius = 5f;

        [Header("Optimization Settings")]
        [SerializeField] private float _updateDelay = 0.1f;

        [field: Header("Behavior Weights")]
        [field: SerializeField] public float SeparationWeight { get; private set; } = 1.5f;
        [field: SerializeField] public float AlignmentWeight { get; private set; } = 1f;
        [field: SerializeField] public float CohesionWeight { get; private set; } = 1f;

        private readonly List<BoidOptimal> _boids = new();

        private void Start()
        {
            SpawnBoids();
            StartCoroutine(UpdateBoidsCoroutine());
        }

        public IReadOnlyList<BoidOptimal> GetAllBoids()
        {
            return _boids;
        }

        private void SpawnBoids()
        {
            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 randomPos = Random.insideUnitSphere * _spawnRadius;
                randomPos += transform.position;

                BoidOptimal boid = Instantiate(_prefab, randomPos, Quaternion.identity, transform)
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