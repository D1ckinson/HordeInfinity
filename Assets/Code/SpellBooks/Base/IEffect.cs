using System;

namespace Assets.Code.SpellBooks
{
    public interface IEffect
    {
        public int Priority { get; }

        public event Action ValueChanged;

        public AbilityStats Apply(AbilityStats stats);

        public void ChangeValue(float value);
    }
}
