using System;

namespace Assets.Code.Data
{
    public class MultiplyEffect : IValueEffect
    {
        private float _value;

        public MultiplyEffect(float value, int priority = 1)
        {
            _value = value;
            Priority = priority;
        }

        public int Priority { get; }

        public event Action Changed;

        public float Apply(float value)
        {
            return value * _value;
        }

        public void SetValue(float value)
        {
            _value = value;
            Changed?.Invoke();
        }
    }
}
