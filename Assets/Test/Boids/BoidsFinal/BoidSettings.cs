using UnityEngine;

namespace Assets.Test.Boids.BoidsFinal
{
    [CreateAssetMenu(menuName = "Game/BoidSettings")]
    public class BoidSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        [Tooltip("Максимальная скорость бойдов")]
        public float Speed = 10f;
        [Tooltip("Скорость вращения бойдов")]
        public float RotationSpeed = 15f;
        [Tooltip("Максимальное ускорение")]
        public float MaxAcceleration = 3f;

        [Header("Behavior Settings")]
        [Tooltip("Радиус восприятия других бойдов (для классического подхода)")]
        public float PerceptionRadius = 4f;
        [Tooltip("Желаемое расстояние разделения")]
        public float DesiredSeparation = 1.5f;
        [Tooltip("Задержка обновления поведения (оптимизация)")]
        public float UpdateBehaviorDelay = 0.05f;

        [Header("Social Settings")]
        [Tooltip("Вес силы разделения")]
        public float SeparationWeight = 0.3f;
        [Tooltip("Вес силы выравнивания")]
        public float AlignmentWeight = 0.2f;
        [Tooltip("Вес силы сплочения")]
        public float CohesionWeight = 0.1f;

        [Header("Target Following")]
        [Tooltip("Дистанция остановки у цели")]
        public float TargetStopDistance = 0.2f;
        [Tooltip("Сила притяжения к цели")]
        public float TargetWeight = 4.0f;

        [Header("Raycast Settings (если используем Raycast подход)")]
        [Tooltip("Дальность зрения для Raycast")]
        public float VisionDistance = 8f;
        [Tooltip("Угол обзора для Raycast")]
        public float VisionAngle = 60f;
        [Tooltip("Сила избегания препятствий")]
        public float AvoidanceWeight = 2f;
    }
}