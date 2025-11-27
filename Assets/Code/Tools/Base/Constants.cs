using System;
using System.Collections.Generic;

namespace Assets.Code.Tools.Base
{
    public static class Constants
    {
        public const int Zero = 0;
        public const int One = 1;
        public const int Two = 2;
        public const int CompareAccuracy = 10;
        public const int FullCircleDegrees = 360;
        public const int Hundred = 100;
        public const float SecondsInMinute = 60;
        public const string LeaderboardName = "TimeLeaderboard";

        public static float PercentToMultiplier(float value, float from = 100)
        {
            if (value < Zero || from < Zero)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value / from;
        }

        public static float PercentToMultiplier(int value, float from = 100)
        {
            if (value < Zero || from < Zero)
            {
                throw new ArgumentOutOfRangeException();
            }

            return value / from;
        }

        public static IEnumerable<T> GetEnums<T>() where T : Enum
        {
            return (IEnumerable<T>)Enum.GetValues(typeof(T));
        }
    }
}
