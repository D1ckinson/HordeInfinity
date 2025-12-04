using UnityEngine;

namespace Assets.Test.Boids.BoidsFinal
{
    [CreateAssetMenu(menuName = "Game/BoidSettings")]
    public class BoidSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        public float Speed = 3f;
        public float RotationSpeed = 5f;
        public float MaxAcceleration = 1f;

        [Header("Behavior Settings")]
        public float PerceptionRadius = 4f;
        public float DesiredSeparation = 1.5f;
        public float UpdateBehaviorDelay = 0.15f;

        [Header("Social Settings")]
        public float SeparationWeight = 0.2f;
        public float AlignmentWeight = 0.1f;
        public float CohesionWeight = 0.1f;

        [Header("Target Following")]
        public float TargetStopDistance = 0.5f;
    }
}