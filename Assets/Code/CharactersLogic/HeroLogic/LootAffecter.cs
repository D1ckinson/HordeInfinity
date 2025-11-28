using Assets.Code.Data.Value;
using Assets.Code.LootSystem;
using Assets.Code.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.CharactersLogic.HeroLogic
{
    public class LootAffecter
    {
        private readonly List<IValueEffect> _goldEffects = new();
        private readonly List<IValueEffect> _experienceEffects = new();

        public void Add(IValueEffect effect, LootType type)
        {
            switch (type)
            {
                case LootType.Coin:
                    AddEffect(_goldEffects, effect);
                    break;

                case LootType.Experience:
                    AddEffect(_experienceEffects, effect);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void Reset()
        {
            _goldEffects.Clear();
            _experienceEffects.Clear();
        }

        public float Affect(int value, LootType type)
        {
            float result = value.ThrowIfNegative();

            switch (type)
            {
                case LootType.Coin:
                    _goldEffects.ForEach(effect => result = effect.Apply(result));
                    break;

                case LootType.Experience:
                    _experienceEffects.ForEach(effect => result = effect.Apply(result));
                    break;

                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        private void AddEffect(List<IValueEffect> effects, IValueEffect effect)
        {
            effects.Add(effect);
            effects = effects.OrderBy(effect => effect.Priority).ToList();
        }
    }
}
