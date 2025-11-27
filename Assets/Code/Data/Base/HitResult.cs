using Assets.Code.Tools.Base;

namespace Assets.Code.Data.Base
{
    public readonly struct HitResult
    {
        public readonly int DealtDamage;
        public readonly bool IsKilled;

        public HitResult(float dealtDamage, bool isKilled)
        {
            DealtDamage = (int)dealtDamage.ThrowIfNegative();
            IsKilled = isKilled;
        }

        public HitResult(int dealtDamage, bool isKilled)
        {
            DealtDamage = dealtDamage.ThrowIfNegative();
            IsKilled = isKilled;
        }
    }
}
