using System;

namespace Assets.Code.Data
{
    public interface IValueEffect
    {
        public int Priority { get; }

        public event Action Changed;

        public float Apply(float value);

        public void SetValue(float value);
    }
}
