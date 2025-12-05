using UnityEngine;

namespace Assets.Test.Boids.BoidsFinal
{
    [CreateAssetMenu(menuName = "Game/BoidRaycastSettings")]
    public class BoidRaycastSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        public float Speed = 10f;              // Быстро!
        public float RotationSpeed = 15f;      // Быстро поворачивают
        public float MaxAcceleration = 3f;     // Резко ускоряются

        [Header("Vision Settings")]
        public float VisionDistance = 8f;      // Дальность зрения
        public float VisionAngle = 60f;        // Угол обзора
        public float UpdateBehaviorDelay = 0.05f; // Часто обновляем

        [Header("Behavior Weights")]
        public float AvoidanceWeight = 2f;     // Сила избегания препятствий
        public float TargetWeight = 4f;        // Очень сильное стремление к цели

        [Header("Target Following")]
        public float TargetStopDistance = 0.2f; // Подходят очень близко
    }
}