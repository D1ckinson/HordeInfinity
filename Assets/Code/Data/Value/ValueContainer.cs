using Assets.Code.Tools;
using System;
using System.Collections.Generic;

namespace Assets.Code.Data
{
    public class ValueContainer : IValueContainer
    {
        private readonly float _defaultMinValue;
        private readonly float _defaultMaxValue;
        private readonly List<IValueEffect> _effects = new();

        private float _additionalValue;
        private float _additionalMaxValue;

        public ValueContainer(
            float defaultValue = 0,
            float minValue = 0,
            float maxValue = float.MaxValue)
        {
            DefaultValue = defaultValue.ThrowIfMoreThan(maxValue).ThrowIfLessThan(minValue);
            _defaultMinValue = minValue.ThrowIfMoreThan(maxValue);
            _defaultMaxValue = maxValue.ThrowIfLessThan(minValue);

            Value = DefaultValue;
        }

        public event Action<ValueContainer> ValueChanged;

        public float DefaultValue { get; }
        public float Value { get; private set; }

        public void AddEffect(IValueEffect effect)
        {
            effect.ThrowIfNull().Changed += UpdateValue;

            _effects.Add(effect);
            UpdateValue();
        }

        public void RemoveEffect(IValueEffect effect)
        {
            effect.ThrowIfNull().Changed -= UpdateValue;

            _effects.Remove(effect);
            UpdateValue();
        }

        public void Increase(float value)
        {
            _additionalValue += value.ThrowIfNegative();
            UpdateValue();
        }

        public void Decrease(float value)
        {
            _additionalValue -= value.ThrowIfNegative();
            UpdateValue();
        }

        public void IncreaseMax(float value)
        {
            _additionalMaxValue += value.ThrowIfNegative();
            UpdateValue();
        }

        public void Reset()
        {
            Value = DefaultValue;

            _additionalValue = Constants.Zero;
            _additionalMaxValue = Constants.Zero;

            _effects.Clear();
            ValueChanged?.Invoke(this);
        }

        private void UpdateValue()
        {
            _effects.RemoveAll(effect => effect.IsNull());
            _effects.Sort();

            Value = DefaultValue + _additionalValue;

            _effects.ForEach(effect => Value = effect.Apply(Value));
            Value.Clamp(_defaultMinValue, _defaultMaxValue + _additionalMaxValue);
        }
    }
}
