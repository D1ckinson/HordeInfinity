using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace Assets.Test.Boids.BoidsFinal
{
    public class BoidSpawnerRaycast : MonoBehaviour
    {
        [Header("Boid Settings")]
        [SerializeField] private Boid _prefab;
        [SerializeField] private int _boidCount = 10;
        [SerializeField] private float _spawnRadius = 5f;
        [SerializeField] private BoidSettings _settings;

        [Header("Target Settings")]
        [SerializeField] private Transform _target;

        [Header("Raycast Settings")]
        [SerializeField] private float _visionDistance = 8f;
        [SerializeField] private float _visionAngle = 60f;
        [SerializeField] private LayerMask _obstacleLayer = ~0;

        private NativeArray<BoidData> _boidData;
        private NativeArray<float3> _accelerations;
        private TransformAccessArray _boidTransforms;
        private Vector3[] _velocities;

        private NativeArray<RaycastHit> _raycastHits;
        private NativeArray<RaycastCommand> _raycastCommands;
        private JobHandle _raycastJobHandle;

        private float _timeSinceLastUpdate;
        private JobHandle _boidJobHandle;
        private bool _jobRunning;

        private void Start()
        {
            SpawnBoids();
            InitializeRaycasts();
            _timeSinceLastUpdate = 0f;
            _jobRunning = false;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // Обновляем с фиксированной частотой
            _timeSinceLastUpdate += deltaTime;
            if (_timeSinceLastUpdate >= _settings.UpdateBehaviorDelay && !_jobRunning)
            {
                StartBoidUpdate();
                _timeSinceLastUpdate = 0f;
            }

            // Если Job завершился - применяем результаты
            if (_jobRunning && _boidJobHandle.IsCompleted)
            {
                CompleteBoidUpdate();
            }

            // Движение каждый кадр
            MoveBoids(deltaTime);
            RotateBoids(deltaTime);

            // Визуализация лучей (только в редакторе)
#if UNITY_EDITOR
            DebugDrawRays();
#endif
        }

        private void OnDestroy()
        {
            if (_jobRunning)
                _boidJobHandle.Complete();

            if (_boidData.IsCreated)
                _boidData.Dispose();

            if (_accelerations.IsCreated)
                _accelerations.Dispose();

            if (_boidTransforms.isCreated)
                _boidTransforms.Dispose();

            DisposeRaycasts();
        }

        private void SpawnBoids()
        {
            _boidData = new NativeArray<BoidData>(_boidCount, Allocator.Persistent);
            _accelerations = new NativeArray<float3>(_boidCount, Allocator.Persistent);
            _velocities = new Vector3[_boidCount];
            Transform[] transforms = new Transform[_boidCount];

            for (int i = 0; i < _boidCount; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * _spawnRadius;
                Vector3 position = new Vector3(
                    transform.position.x + randomCircle.x,
                    0,
                    transform.position.z + randomCircle.y
                );

                Boid boid = Instantiate(_prefab, position, Quaternion.identity);
                transforms[i] = boid.transform;

                Vector2 randomDir = Random.insideUnitCircle.normalized;
                _velocities[i] = new Vector3(randomDir.x, 0, randomDir.y) * _settings.Speed;

                if (_velocities[i].magnitude > 0.1f)
                {
                    transforms[i].rotation = Quaternion.LookRotation(_velocities[i]);
                }

                _boidData[i] = new BoidData
                {
                    Position = position,
                    Velocity = _velocities[i]
                };
            }

            _boidTransforms = new TransformAccessArray(transforms);
        }

        private void InitializeRaycasts()
        {
            // 3 луча на каждого бойда: вперед, влево, вправо
            int totalRays = _boidCount * 3;
            _raycastHits = new NativeArray<RaycastHit>(totalRays, Allocator.Persistent);
            _raycastCommands = new NativeArray<RaycastCommand>(totalRays, Allocator.Persistent);
        }

        private void ScheduleRaycasts()
        {
            int rayIndex = 0;

            // Создаем QueryParameters один раз для всех RaycastCommand
            var queryParameters = new QueryParameters
            {
                layerMask = _obstacleLayer,
                hitMultipleFaces = false,
                hitBackfaces = false
            };

            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 position = _boidTransforms[i].position;
                Vector3 forward = _velocities[i].normalized;

                if (forward.magnitude < 0.1f)
                {
                    forward = Vector3.forward;
                }

                // Луч вперед
                _raycastCommands[rayIndex++] = new RaycastCommand(
                    position,
                    forward,
                    queryParameters,
                    _visionDistance
                );

                // Луч влево (под углом)
                Vector3 leftDir = Quaternion.Euler(0, -_visionAngle, 0) * forward;
                _raycastCommands[rayIndex++] = new RaycastCommand(
                    position,
                    leftDir,
                    queryParameters,
                    _visionDistance
                );

                // Луч вправо (под углом)
                Vector3 rightDir = Quaternion.Euler(0, _visionAngle, 0) * forward;
                _raycastCommands[rayIndex++] = new RaycastCommand(
                    position,
                    rightDir,
                    queryParameters,
                    _visionDistance
                );
            }

            // Запускаем параллельные Raycast'ы
            _raycastJobHandle = RaycastCommand.ScheduleBatch(
                _raycastCommands,
                _raycastHits,
                32,
                default(JobHandle)
            );
        }

        private void StartBoidUpdate()
        {
            // 1. Обновляем данные бойдов
            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 pos = _boidTransforms[i].position;
                pos.y = 0;

                _boidData[i] = new BoidData
                {
                    Position = pos,
                    Velocity = _velocities[i]
                };
            }

            // 2. Запускаем Raycast'ы
            ScheduleRaycasts();

            // 3. Ждем завершения Raycast'ов
            _raycastJobHandle.Complete();

            // 4. Запускаем Job обработки
            BoidRaycastJob job = new()
            {
                BoidData = _boidData,
                TargetPosition = _target != null ? _target.position : Vector3.zero,
                DeltaTime = _settings.UpdateBehaviorDelay,
                VisionDistance = _visionDistance,
                VisionAngle = _visionAngle,
                AvoidanceWeight = _settings.SeparationWeight, // Переиспользуем настройку
                TargetWeight = _settings.TargetWeight,
                TargetStopDistance = _settings.TargetStopDistance,
                Speed = _settings.Speed,
                RaycastHits = _raycastHits,
                Accelerations = _accelerations
            };

            _boidJobHandle = job.Schedule(_boidCount, 32);
            _jobRunning = true;
        }

        private void CompleteBoidUpdate()
        {
            _boidJobHandle.Complete();
            _jobRunning = false;

            // Применяем ускорения
            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 acceleration = _accelerations[i];

                // Сильное ускорение к цели
                _velocities[i] += acceleration * _settings.MaxAcceleration * 3f;

                // Поддержание скорости
                Vector3 toTarget = (_target.position - _boidTransforms[i].position);
                toTarget.y = 0;

                if (toTarget.magnitude > _settings.TargetStopDistance)
                {
                    if (_velocities[i].magnitude < _settings.Speed * 0.7f)
                    {
                        Vector3 boostDir = toTarget.normalized;
                        _velocities[i] += boostDir * _settings.MaxAcceleration;
                    }
                }

                // Ограничение скорости и 2D
                _velocities[i] = Vector3.ClampMagnitude(_velocities[i], _settings.Speed);
                _velocities[i].y = 0;
            }
        }

        private void MoveBoids(float deltaTime)
        {
            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 newPos = _boidTransforms[i].position + _velocities[i] * deltaTime;
                newPos.y = 0;
                _boidTransforms[i].position = newPos;
            }
        }

        private void RotateBoids(float deltaTime)
        {
            for (int i = 0; i < _boidCount; i++)
            {
                if (_velocities[i].magnitude > 0.1f)
                {
                    Vector3 lookDir = _velocities[i];
                    lookDir.y = 0;

                    Quaternion targetRot = Quaternion.LookRotation(lookDir);
                    _boidTransforms[i].rotation = Quaternion.Slerp(
                        _boidTransforms[i].rotation,
                        targetRot,
                        _settings.RotationSpeed * deltaTime
                    );
                }
            }
        }

        private void DisposeRaycasts()
        {
            if (_raycastHits.IsCreated)
                _raycastHits.Dispose();

            if (_raycastCommands.IsCreated)
                _raycastCommands.Dispose();
        }

        private void DebugDrawRays()
        {
            if (!Application.isPlaying) return;

            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 pos = _boidTransforms[i].position;
                Vector3 forward = _velocities[i].normalized;

                if (forward.magnitude < 0.1f) continue;

                // Луч вперед
                Debug.DrawRay(pos, forward * _visionDistance,
                    Color.green);

                // Луч влево
                Vector3 leftDir = Quaternion.Euler(0, -_visionAngle, 0) * forward;
                Debug.DrawRay(pos, leftDir * _visionDistance,
                    Color.yellow);

                // Луч вправо
                Vector3 rightDir = Quaternion.Euler(0, _visionAngle, 0) * forward;
                Debug.DrawRay(pos, rightDir * _visionDistance,
                    Color.yellow);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);

            if (_target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_target.position, 0.5f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_target.position, _settings.TargetStopDistance);
            }
        }
    }
}