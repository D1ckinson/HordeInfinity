using Assets.Code.Data.Value;
using Assets.Code.Tools.Base;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.CharactersLogic.GeneralLogic
{
    public class Resist
    {
        private readonly float _baseValue;
        private readonly List<IValueEffect> _effects = new();

        public Resist(float baseValue)
        {
            _baseValue = baseValue;
        }

        public float Affect(float value)
        {
            value -= _baseValue;
            _effects.ForEach(effect => value = effect.Apply(value));

            return value;
        }

        public void AddEffect(IValueEffect effect)
        {
            _effects.Add(effect.ThrowIfNull());
            _effects.OrderBy(effect => effect.Priority);
        }

        public void RemoveEffect(IValueEffect effect)
        {
            _effects.Remove(effect.ThrowIfNull());
        }

        public void Reset()
        {
            _effects.Clear();
        }
    }
}
