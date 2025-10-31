using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Tools;
using System;
using System.Collections.Generic;

namespace Assets.Code.BuffSystem
{
    public class BuffFactory
    {
        private readonly Dictionary<BuffType, Func<Buff>> _createFunc;
        private readonly Dictionary<BuffType, BuffConfig> _configs;
        private readonly HeroComponents _hero;

        public BuffFactory(Dictionary<BuffType, BuffConfig> configs, HeroComponents hero)
        {
            _configs = configs.ThrowIfNullOrEmpty();
            _hero = hero.ThrowIfNull();

            _createFunc = new()
            {
                [BuffType.Health] = CreateHealthBuff,
            };
        }

        public Buff Create(BuffType type)
        {
            return _createFunc[type.ThrowIfNull()].Invoke();
        }

        private Buff CreateHealthBuff()
        {
            return new HealthBuff(_configs[BuffType.Health], _hero);
        }
    }
}
