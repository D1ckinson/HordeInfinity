using UnityEngine;

namespace Assets.Test.Boids.BoidsFinal
{
    [CreateAssetMenu(menuName = "Game/BoidSettings")]
    public class BoidSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        [Tooltip("Максимальная скорость бойдов")]
        public float Speed = 8f; // Увеличил для быстрого движения
        [Tooltip("Скорость вращения бойдов")]
        public float RotationSpeed = 10f; // Увеличил для быстрого поворота
        [Tooltip("Максимальное ускорение")]
        public float MaxAcceleration = 2f; // Увеличил для резкого движения

        [Header("Behavior Settings")]
        [Tooltip("Радиус восприятия других бойдов")]
        public float PerceptionRadius = 3f;
        [Tooltip("Желаемое расстояние разделения")]
        public float DesiredSeparation = 1f; // Уменьшил для меньшего разделения
        [Tooltip("Задержка обновления поведения (оптимизация)")]
        public float UpdateBehaviorDelay = 0.1f; // Чаще обновляем

        [Header("Social Settings")]
        [Tooltip("Вес силы разделения")]
        public float SeparationWeight = 0.3f; // Ослабил
        [Tooltip("Вес силы выравнивания")]
        public float AlignmentWeight = 0.2f; // Ослабил
        [Tooltip("Вес силы сплочения")]
        public float CohesionWeight = 0.1f; // Ослабил

        [Header("Target Following")]
        [Tooltip("Дистанция остановки у цели (очень маленькая)")]
        public float TargetStopDistance = 0.1f; // Очень маленькая для близкого подхода
        [Tooltip("Сила притяжения к цели (высокий приоритет)")]
        public float TargetWeight = 3.0f; // Увеличил для приоритета цели
    }
}