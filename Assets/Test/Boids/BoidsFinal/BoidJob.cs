using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

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

        public NativeArray<Vector3> Accelerations;

        public void Execute(int index)
        {
            BoidData current = BoidData[index];
            float3 separation = float3.zero;
            float3 alignment = float3.zero;
            float3 cohesion = float3.zero;
            int neighborCount = 0;

            float perceptionSqr = PerceptionRadius * PerceptionRadius;

            for (int i = 0; i < BoidData.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }

                BoidData other = BoidData[i];
                float3 offset = other.Position - current.Position;
                float distanceSqr = math.lengthsq(offset);

                if (distanceSqr < perceptionSqr)
                {
                    neighborCount++;

                    if (distanceSqr < DesiredSeparation * DesiredSeparation)
                    {
                        separation -= math.normalize(offset) / (math.sqrt(distanceSqr) + 0.001f);
                    }

                    alignment += other.Velocity;
                    cohesion += other.Position;
                }
            }

            if (neighborCount > 0)
            {
                alignment = math.normalize(alignment / neighborCount);
                cohesion = math.normalize(cohesion / neighborCount - current.Position);
            }

            float3 toTarget = math.normalize(TargetPosition - current.Position);

            float3 acceleration = separation * SeparationWeight 
                + alignment * AlignmentWeight 
                + cohesion * CohesionWeight + toTarget;

            acceleration = math.normalize(acceleration) * math.min(math.length(acceleration), 1f);

            Accelerations[index] = acceleration;
        }
    }
}
