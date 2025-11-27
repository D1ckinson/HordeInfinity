using UnityEngine;

namespace Assets.Code.Tools.Base
{
    public static class Utilities
    {
        private const int Zero = 0;
        private const int FullCircleDegrees = 360;

        public static Vector3 GenerateRandomDirection(float positionY = 0)
        {
            float randomAngle = Random.Range(Zero, FullCircleDegrees) * Mathf.Deg2Rad;

            return new(Mathf.Cos(randomAngle), positionY, Mathf.Sin(randomAngle));
        }
    }
}
