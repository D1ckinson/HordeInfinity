using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Assets.Test.Boids.BoidsFinal
{
    [BurstCompile]
    public struct BoidJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BoidData> BoidData;
        [ReadOnly] public float PerceptionRadius;
        [ReadOnly] public float DesiredSeparation;
        [ReadOnly] public float3 TargetPosition;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public float SeparationWeight;
        [ReadOnly] public float AlignmentWeight;
        [ReadOnly] public float CohesionWeight;
        [ReadOnly] public float TargetWeight;
        [ReadOnly] public float TargetStopDistance;

        public NativeArray<float3> Accelerations;

        public void Execute(int index)
        {
            BoidData current = BoidData[index];

            // ИГНОРИРУЕМ Y координату для 2D
            float3 currentPos2D = current.Position;
            currentPos2D.y = 0;

            float3 separation = float3.zero;
            float3 alignment = float3.zero;
            float3 cohesion = float3.zero;
            int neighborCount = 0;

            float perceptionSqr = PerceptionRadius * PerceptionRadius;

            for (int i = 0; i < BoidData.Length; i++)
            {
                if (i == index) continue;

                BoidData data = BoidData[i];

                // 2D вектор (игнорируем Y)
                float3 dataPos2D = data.Position;
                dataPos2D.y = 0;

                float3 offset = dataPos2D - currentPos2D;
                float distanceSqr = math.lengthsq(offset);

                if (distanceSqr < perceptionSqr)
                {
                    neighborCount++;

                    // Separation (только при слишком близком расстоянии)
                    if (distanceSqr < DesiredSeparation * DesiredSeparation)
                    {
                        float distance = math.sqrt(distanceSqr);
                        separation -= math.normalizesafe(offset) / (distance + 0.001f);
                    }

                    // Alignment (выравнивание скорости)
                    float3 dataVel2D = data.Velocity;
                    dataVel2D.y = 0;
                    alignment += dataVel2D;

                    // Cohesion (центр группы)
                    cohesion += dataPos2D;
                }
            }

            // НОРМАЛИЗАЦИЯ СИЛ
            if (neighborCount > 0)
            {
                alignment = math.normalizesafe(alignment / neighborCount);
                cohesion = math.normalizesafe((cohesion / neighborCount) - currentPos2D);
            }

            // СИЛА К ЦЕЛИ (ГЛАВНЫЙ ПРИОРИТЕТ)
            float3 targetPos2D = TargetPosition;
            targetPos2D.y = 0;

            float3 toTarget = targetPos2D - currentPos2D;
            float targetDistance = math.length(toTarget);

            float3 targetForce;

            // УПРОЩЕННЫЙ ПОДХОД: либо полная сила, либо плавное торможение у самой цели
            if (targetDistance > TargetStopDistance)
            {
                // Полная сила к цели
                targetForce = math.normalizesafe(toTarget) * TargetWeight;
            }
            else
            {
                // На самой маленькой дистанции - слабое торможение
                float slowdownFactor = math.max(targetDistance / TargetStopDistance, 0.1f);
                targetForce = math.normalizesafe(toTarget) * TargetWeight * slowdownFactor;
            }

            // КОМБИНАЦИЯ СИЛ (цель имеет приоритет)
            float3 acceleration = float3.zero;

            // Балансируем социальные силы
            if (neighborCount > 0)
            {
                acceleration += separation * SeparationWeight * 0.5f; // Ослабляем разделение
                acceleration += alignment * AlignmentWeight * 0.3f;   // Ослабляем выравнивание
                acceleration += cohesion * CohesionWeight * 0.2f;    // Ослабляем сплочение
            }

            // Цель - главная сила
            acceleration += targetForce;

            // Нормализация и масштабирование
            float accelMagnitude = math.length(acceleration);
            if (accelMagnitude > 0.001f)
            {
                acceleration = math.normalize(acceleration) * math.min(accelMagnitude, 2.0f) * DeltaTime;
            }

            Accelerations[index] = acceleration;
        }
    }
}