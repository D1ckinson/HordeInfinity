using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

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

        private NativeArray<BoidData> _boidData;
        private NativeArray<Vector3> _accelerations;
        private TransformAccessArray _boidTransforms;
        private Vector3[] _velocities;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);

            if (_target != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_target.position, 0.3f);
            }
        }

        private void Start()
        {
            SpawnBoids();
            StartCoroutine(UpdateBoidsJobCoroutine());
        }

        private void Update()
        {
            for (int i = 0; i < _boidCount; i++)
            {
                _boidTransforms[i].position += _velocities[i] * Time.deltaTime;

                if (_velocities[i].magnitude <= 0.1f)
                {
                    continue;
                }

                Vector3 lookDirection = _velocities[i];
                lookDirection.y = 0;

                if (lookDirection.magnitude <= 0.1f)
                {
                    continue;
                }

                Quaternion targetRot = Quaternion.LookRotation(lookDirection);
                _boidTransforms[i].rotation = Quaternion.Slerp(_boidTransforms[i].rotation, targetRot, _settings.RotationSpeed * Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            if (_boidData.IsCreated)
            {
                _boidData.Dispose();
            }

            if (_accelerations.IsCreated)
            {
                _accelerations.Dispose();
            }
        }

        private void SpawnBoids()
        {
            _boidData = new NativeArray<BoidData>(_boidCount, Allocator.Persistent);
            _accelerations = new NativeArray<Vector3>(_boidCount, Allocator.Persistent);
            _velocities = new Vector3[_boidCount];
            Transform[] transforms = new Transform[_boidCount];

            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 randomPosition = transform.position + Random.insideUnitSphere * _spawnRadius;
                randomPosition.y = 1f;

                Boid boid = Instantiate(_prefab, randomPosition, Quaternion.identity);
                transforms[i] = boid.transform;

                _velocities[i] = Random.insideUnitSphere * _settings.Speed;
                _velocities[i].y = 0;

                _boidData[i] = new BoidData
                {
                    Position = randomPosition,
                    Velocity = _velocities[i],
                    Index = i
                };
            }

            _boidTransforms = new TransformAccessArray(transforms);
        }

        private IEnumerator UpdateBoidsJobCoroutine()
        {
            WaitForSeconds wait = new(_settings.UpdateBehaviorDelay);

            while (true)
            {
                UpdateBoidsJob();

                yield return wait;
            }
        }

        private void UpdateBoidsJob()
        {
            for (int i = 0; i < _boidCount; i++)
            {
                _boidData[i] = new BoidData
                {
                    Position = _boidTransforms[i].position,
                    Velocity = _velocities[i],
                    Index = i
                };
            }

            BoidJob job = new()
            {
                BoidData = _boidData,
                PerceptionRadius = _settings.PerceptionRadius,
                DesiredSeparation = _settings.DesiredSeparation,
                TargetPosition = _target.position,
                DeltaTime = Time.deltaTime,
                Accelerations = _accelerations,
                SeparationWeight = _settings.SeparationWeight,
                AlignmentWeight = _settings.AlignmentWeight,
                CohesionWeight = _settings.CohesionWeight,

            };

            JobHandle handle = job.Schedule(_boidCount, 0);
            handle.Complete();

            for (int i = 0; i < _boidCount; i++)
            {
                _velocities[i] += _settings.MaxAcceleration * _settings.UpdateBehaviorDelay * (Vector3)_accelerations[i];
                _velocities[i] = Vector3.ClampMagnitude(_velocities[i], _settings.Speed);

                if (_velocities[i].magnitude < 0.5f)
                    _velocities[i] = _boidTransforms[i].forward * 0.5f;
            }
        }
    }
}
