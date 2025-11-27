using Assets.Code.Data.Value;
using Assets.Code.LootSystem.Legacy;
using System;
using System.Collections.Generic;

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
                case LootType.LowExperience:
                case LootType.MediumExperience:
                case LootType.HighExperience:
                    _experienceEffects.Add(effect);
                    _experienceEffects.Sort();
                    break;

                case LootType.LowCoin:
                case LootType.MediumCoin:
                case LootType.HighCoin:
                    _goldEffects.Add(effect);
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

        public float Affect(float amount, LootType type)
        {
            switch (type)
            {
                case LootType.LowExperience:
                case LootType.MediumExperience:
                case LootType.HighExperience:
                    _experienceEffects.ForEach(effect => amount = effect.Apply(amount));
                    break;

                case LootType.LowCoin:
                case LootType.MediumCoin:
                case LootType.HighCoin:
                    _goldEffects.ForEach(effect => amount = effect.Apply(amount));
                    break;

                default:
                    throw new NotImplementedException();
            }

            return amount;
        }
    }
}
