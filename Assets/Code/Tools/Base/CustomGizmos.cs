#if UNITY_EDITOR
namespace Assets.Code.Tools.Base
{
    using UnityEngine;

    public sealed class CustomGizmos
    {
        private const int Zero = 0;
        private const int One = 1;
        private const int CircleSegmentsCount = 32;
        private const int ConeSegmentsCount = 20;
        private const int FullCircleDegrees = 360;
        private const float Half = 0.5f;

        private static readonly Color _baseColor = Color.yellow;

        public static void DrawCircle(Vector3 center, float radius, Color? color = null)
        {
            center.ThrowIfNull();
            radius.ThrowIfZeroOrLess();

            Gizmos.color = color ?? _baseColor;
            float angleStep = FullCircleDegrees / CircleSegmentsCount;

            Vector3 startPoint = CalculatePoint(Zero);

            for (int i = One; i <= CircleSegmentsCount + One; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 endPoint = CalculatePoint(angle);

                Gizmos.DrawLine(startPoint, endPoint);
                startPoint = endPoint;
            }

            Vector3 CalculatePoint(float angle)
            {
                Vector3 point = center + new Vector3(Mathf.Cos(angle), Zero, Mathf.Sin(angle)) * radius;

                return point;
            }
        }

        public static void DrawCone(Vector3 center, Vector3 direction, float radius, float angle, Color? color = null)
        {
            center.ThrowIfNull();
            direction.ThrowIfNotNormalize();
            radius.ThrowIfZeroOrLess();
            angle.ThrowIfZeroOrLess();

            Gizmos.color = color ?? _baseColor;

            float halfAngle = angle * Half;

            Vector3 rightDirection = Quaternion.Euler(Zero, halfAngle, Zero) * direction;
            Vector3 leftDirection = Quaternion.Euler(Zero, -halfAngle, Zero) * direction;

            Gizmos.DrawLine(center, center + rightDirection * radius);
            Gizmos.DrawLine(center, center + leftDirection * radius);

            float angleStep = angle / ConeSegmentsCount;
            Vector3 previousPoint = center + leftDirection * radius;

            for (int i = One; i <= ConeSegmentsCount; i++)
            {
                float currentAngle = -halfAngle + angleStep * i;
                Vector3 nextPoint = center + Quaternion.Euler(Zero, currentAngle, Zero) * direction * radius;

                Gizmos.DrawLine(previousPoint, nextPoint);

                previousPoint = nextPoint;
            }
        }
    }
}
#endif
