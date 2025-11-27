using System;

namespace Assets.Code.Data.Value
{
    public class SumEffect : IValueEffect
    {
        private float _value;

        public SumEffect(float value, int priority = 0)
        {
            _value = value;
            Priority = priority;
        }

        public event Action Changed;

        public int Priority { get; }

        public float Apply(float value)
        {
            return value + _value;
        }

        public void SetValue(float value)
        {
            _value = value;
            Changed?.Invoke();
        }
    }
}
