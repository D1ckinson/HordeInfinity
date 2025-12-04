using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
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
        private NativeArray<float3> _accelerations;
        private TransformAccessArray _boidTransforms;
        private Vector3[] _velocities;

        private float _timeSinceLastJob;
        private JobHandle _jobHandle;
        private bool _jobRunning;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);

            if (_target != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_target.position, 0.3f);

                // Показываем радиус остановки
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_target.position, _settings.TargetStopDistance);
            }
        }

        private void Start()
        {
            SpawnBoids();
            _timeSinceLastJob = 0f;
            _jobRunning = false;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // 1. Запускаем Job для вычислений с нужной частотой
            _timeSinceLastJob += deltaTime;
            if (_timeSinceLastJob >= _settings.UpdateBehaviorDelay && !_jobRunning)
            {
                StartBoidJob();
                _timeSinceLastJob = 0f;
            }

            // 2. Если Job завершился - применяем результаты
            if (_jobRunning && _jobHandle.IsCompleted)
            {
                CompleteBoidJob();
            }

            // 3. Двигаем бойдов КАЖДЫЙ КАДР (чистое 2D движение)
            MoveBoids(deltaTime);

            // 4. Вращаем бойдов КАЖДЫЙ КАДР (чистое 2D вращение)
            RotateBoids(deltaTime);
        }

        private void OnDestroy()
        {
            // Ждем завершения Job перед очисткой
            if (_jobRunning)
            {
                _jobHandle.Complete();
            }

            if (_boidData.IsCreated)
            {
                _boidData.Dispose();
            }

            if (_accelerations.IsCreated)
            {
                _accelerations.Dispose();
            }

            if (_boidTransforms.isCreated)
            {
                _boidTransforms.Dispose();
            }
        }

        private void SpawnBoids()
        {
            _boidData = new NativeArray<BoidData>(_boidCount, Allocator.Persistent);
            _accelerations = new NativeArray<float3>(_boidCount, Allocator.Persistent);
            _velocities = new Vector3[_boidCount];
            Transform[] transforms = new Transform[_boidCount];

            for (int i = 0; i < _boidCount; i++)
            {
                // 2D спавн (XZ плоскость)
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * _spawnRadius;
                Vector3 randomPosition = new Vector3(
                    transform.position.x + randomCircle.x,
                    0, // Всегда Y = 0 для 2D
                    transform.position.z + randomCircle.y
                );

                Boid boid = Instantiate(_prefab, randomPosition, Quaternion.identity);
                transforms[i] = boid.transform;

                // 2D начальная скорость
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
                _velocities[i] = new Vector3(randomDirection.x, 0, randomDirection.y) * _settings.Speed;

                if (_velocities[i].magnitude > 0.1f)
                {
                    transforms[i].rotation = Quaternion.LookRotation(_velocities[i]);
                }

                _boidData[i] = new BoidData
                {
                    Position = randomPosition,
                    Velocity = _velocities[i],
                };
            }

            _boidTransforms = new TransformAccessArray(transforms);
        }

        private void StartBoidJob()
        {
            // Обновляем данные бойдов (2D позиции)
            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 pos = _boidTransforms[i].position;
                pos.y = 0; // Принудительно 2D

                _boidData[i] = new BoidData
                {
                    Position = pos,
                    Velocity = _velocities[i],
                };
            }

            // Создаем и запускаем Job
            BoidJob job = new()
            {
                BoidData = _boidData,
                PerceptionRadius = _settings.PerceptionRadius,
                DesiredSeparation = _settings.DesiredSeparation,
                TargetPosition = _target != null ? _target.position : Vector3.zero,
                DeltaTime = _settings.UpdateBehaviorDelay,
                Accelerations = _accelerations,
                SeparationWeight = _settings.SeparationWeight,
                AlignmentWeight = _settings.AlignmentWeight,
                CohesionWeight = _settings.CohesionWeight,
                TargetWeight = _settings.TargetWeight,
                TargetStopDistance = _settings.TargetStopDistance
            };

            _jobHandle = job.Schedule(_boidCount, 32);
            _jobRunning = true;
        }

        private void CompleteBoidJob()
        {
            _jobHandle.Complete();
            _jobRunning = false;

            // Применяем ускорения, вычисленные в Job
            for (int i = 0; i < _boidCount; i++)
            {
                Vector3 acceleration = _accelerations[i];

                // УСТРАНЯЕМ ЗАМЕДЛЕНИЕ: добавляем ускорение без потери скорости
                _velocities[i] += acceleration * _settings.MaxAcceleration;

                // Поддерживаем МИНИМАЛЬНУЮ скорость к цели
                float3 toTarget = (_target.position - _boidTransforms[i].position);
                toTarget.y = 0;

                if (Vector3.Magnitude(toTarget) > _settings.TargetStopDistance && _velocities[i].magnitude < _settings.Speed * 0.7f)
                {
                    // Усиливаем скорость к цели если она слишком низкая
                    float3 boostDirection = math.normalize(toTarget);
                    _velocities[i] += (Vector3)(_settings.MaxAcceleration * 0.5f * boostDirection);
                }

                // Ограничиваем максимальную скорость
                _velocities[i] = Vector3.ClampMagnitude(_velocities[i], _settings.Speed);

                // Убираем Y компонент для чистого 2D
                _velocities[i].y = 0;
            }
        }

        private void MoveBoids(float deltaTime)
        {
            for (int i = 0; i < _boidCount; i++)
            {
                // ЧИСТОЕ 2D ДВИЖЕНИЕ
                Vector3 newPos = _boidTransforms[i].position + _velocities[i] * deltaTime;
                newPos.y = 0; // Фиксируем Y на 0
                _boidTransforms[i].position = newPos;
            }
        }

        private void RotateBoids(float deltaTime)
        {
            for (int i = 0; i < _boidCount; i++)
            {
                if (_velocities[i].magnitude <= 0.1f)
                    continue;

                // 2D ВРАЩЕНИЕ (только вокруг Y оси)
                Vector3 lookDirection = _velocities[i];
                lookDirection.y = 0;

                if (lookDirection.magnitude <= 0.1f)
                    continue;

                Quaternion targetRot = Quaternion.LookRotation(lookDirection, Vector3.up);
                _boidTransforms[i].rotation = Quaternion.Slerp(
                    _boidTransforms[i].rotation,
                    targetRot,
                    _settings.RotationSpeed * deltaTime
                );
            }
        }
    }
}