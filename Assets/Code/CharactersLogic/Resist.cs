using Assets.Code.Tools;

namespace Assets.Code.CharactersLogic
{
    public class Resist
    {
        private readonly float _defaultValue;
        private readonly float _minValue;
        private readonly float _maxValue;

        public Resist(
            float value,
            float minValue = 0,
            float maxValue = float.MaxValue)
        {
            _defaultValue = value.ThrowIfNegative();
            Value = _defaultValue;

            _minValue = minValue.ThrowIfNegative();
            _maxValue = maxValue.ThrowIfLessThan(_minValue);
        }

        public float Value { get; private set; }

        public void Increase(float value)
        {
            Value = (Value + value.ThrowIfNegative())
                .Clamp(_minValue, _maxValue);
        }

        public void Decrease(float value)
        {
            Value = (Value - value.ThrowIfNegative())
                .Clamp(_minValue, _maxValue);
        }

        public void Reset()
        {
            Value = _defaultValue;
        }
    }
}
