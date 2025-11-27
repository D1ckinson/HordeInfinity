using System;

namespace Assets.Code.Data.Value
{
    public interface IValueContainer
    {
        public float DefaultValue { get; }
        public float Value { get; }

        public event Action<ValueContainer> ValueChanged;

        public void AddEffect(IValueEffect effect);

        public void Decrease(float value);

        public void Increase(float value);

        public void RemoveEffect(IValueEffect effect);

        public void Reset();
    }
}
