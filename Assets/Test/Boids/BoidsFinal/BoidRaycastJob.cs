using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Test.Boids.BoidsFinal
{
    [BurstCompile]
    public struct BoidRaycastJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BoidData> BoidData;
        [ReadOnly] public float3 TargetPosition;
        [ReadOnly] public float DeltaTime;

        // Настройки "зрения"
        [ReadOnly] public float VisionDistance;
        [ReadOnly] public float VisionAngle;
        [ReadOnly] public float AvoidanceWeight;
        [ReadOnly] public float TargetWeight;
        [ReadOnly] public float TargetStopDistance;
        [ReadOnly] public float Speed;

        // Результаты Raycast (заполняются извне)
        [ReadOnly] public NativeArray<RaycastHit> RaycastHits;

        public NativeArray<float3> Accelerations;

        public void Execute(int index)
        {
            BoidData current = BoidData[index];

            // 2D позиция и направление
            float3 currentPos = current.Position;
            currentPos.y = 0;

            float3 currentVel = current.Velocity;
            currentVel.y = 0;

            float3 currentForward = math.normalizesafe(currentVel);
            if (math.length(currentForward) < 0.001f)
            {
                currentForward = new float3(0, 0, 1);
            }

            float3 totalForce = float3.zero;

            // 1. СИЛА К ЦЕЛИ (главный приоритет)
            float3 targetPos2D = TargetPosition;
            targetPos2D.y = 0;

            float3 toTarget = targetPos2D - currentPos;
            float targetDistance = math.length(toTarget);

            if (targetDistance > 0.001f)
            {
                float3 targetDir = math.normalize(toTarget);
                float targetInfluence = 1.0f;

                if (targetDistance < TargetStopDistance * 2f)
                {
                    // Плавное торможение только у самой цели
                    targetInfluence = math.max(targetDistance / TargetStopDistance, 0.2f);
                }

                totalForce += targetDir * TargetWeight * targetInfluence;
            }

            // 2. ИЗБЕГАНИЕ ПРЕПЯТСТВИЙ (по Raycast данным)
            // Используем 3 луча: вперед, влево, вправо
            for (int rayIndex = 0; rayIndex < 3; rayIndex++)
            {
                int hitIndex = index * 3 + rayIndex;

                if (hitIndex < RaycastHits.Length && RaycastHits[hitIndex].distance > 0)
                {
                    float hitDistance = RaycastHits[hitIndex].distance;
                    float3 hitNormal = RaycastHits[hitIndex].normal;
                    hitNormal.y = 0;

                    if (math.length(hitNormal) > 0.001f)
                    {
                        // Чем ближе препятствие, тем сильнее сила избегания
                        float avoidanceStrength = 1.0f - (hitDistance / VisionDistance);
                        avoidanceStrength = math.clamp(avoidanceStrength, 0.1f, 1.0f);

                        // Отталкиваемся от нормали препятствия
                        float3 avoidDir = math.normalize(hitNormal);
                        totalForce += avoidDir * AvoidanceWeight * avoidanceStrength;
                    }
                }
            }

            // 3. ПОДДЕРЖАНИЕ СКОРОСТИ
            // Если движемся медленно - ускоряемся вперед
            float currentSpeed = math.length(currentVel);
            float desiredSpeed = Speed;

            if (currentSpeed < desiredSpeed * 0.8f && targetDistance > TargetStopDistance)
            {
                float3 forwardForce = currentForward * (desiredSpeed - currentSpeed) * 0.5f;
                totalForce += forwardForce;
            }

            // Нормализация и применение
            float forceMagnitude = math.length(totalForce);
            if (forceMagnitude > 0.001f)
            {
                float3 acceleration = math.normalize(totalForce) *
                                    math.min(forceMagnitude, 2.0f) *
                                    DeltaTime;
                Accelerations[index] = acceleration;
            }
            else
            {
                // Минимальное движение вперед если нет сил
                Accelerations[index] = currentForward * 0.1f * DeltaTime;
            }
        }
    }
}