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
                [BuffType.Regeneration] = CreateRegenerationBuff,
                [BuffType.Damage] = CreateDamageBuff,
                [BuffType.Cooldown] = CreateCooldownBuff,
                [BuffType.Speed] = CreateSpeedBuff,
                [BuffType.Extraction] = CreateExtractionBuff,
                [BuffType.Knowledge] = CreateKnowledgeBuff,
                [BuffType.Collection] = CreateCollectionBuff,
                [BuffType.Armor] = CreateArmorBuff
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

        private Buff CreateRegenerationBuff()
        {
            return new RegenerationBuff(_configs[BuffType.Regeneration], _hero);
        }

        private Buff CreateDamageBuff()
        {
            return new DamageBuff(_configs[BuffType.Damage], _hero);
        }

        private Buff CreateCooldownBuff()
        {
            return new CooldownBuff(_configs[BuffType.Cooldown], _hero);
        }

        private Buff CreateSpeedBuff()
        {
            return new SpeedBuff(_configs[BuffType.Speed], _hero);
        }

        private Buff CreateExtractionBuff()
        {
            return new ExtractionBuff(_configs[BuffType.Extraction], _hero);
        }

        private Buff CreateKnowledgeBuff()
        {
            return new KnowledgeBuff(_configs[BuffType.Knowledge], _hero);
        }

        private Buff CreateCollectionBuff()
        {
            return new CollectionBuff(_configs[BuffType.Collection], _hero);
        }

        private Buff CreateArmorBuff()
        {
            return new ArmorBuff(_configs[BuffType.Armor], _hero);
        }
    }
}
