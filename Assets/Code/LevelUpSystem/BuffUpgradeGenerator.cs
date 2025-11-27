using Assets.Code.BuffSystem.Base;
using Assets.Code.Data.Base;
using Assets.Code.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Code.LevelUpSystem
{
    public class BuffUpgradeGenerator
    {
        private const char Plus = '+';
        private const char Minus = '-';

        private readonly BuffContainer _container;
        private readonly Dictionary<BuffType, BuffConfig> _configs;

        private List<BuffType> _availableBuffs;

        public BuffUpgradeGenerator(BuffContainer container, Dictionary<BuffType, BuffConfig> configs)
        {
            _container = container.ThrowIfNull();
            _configs = configs.ThrowIfNullOrEmpty();

            RefreshAvailableBuffs();
        }

        public bool CanGenerate => _availableBuffs.Count > Constants.Zero;

        public UpgradeOption Generate()
        {
            if (CanGenerate == false)
            {
                throw new InvalidOperationException();
            }

            int index = Random.Range(Constants.Zero, _availableBuffs.Count);

            BuffType type = _availableBuffs[index];
            _availableBuffs.RemoveAt(index);

            return CreateUpgradeOption(type);
        }

        public void RefreshAvailableBuffs()
        {
            _availableBuffs = Constants.GetEnums<BuffType>()
                .Except(_container.MaxedBuffs)
                .ToList();
        }

        private UpgradeOption CreateUpgradeOption(BuffType type)
        {
            BuffConfig config = _configs[type];

            int level = _container.GetBuffLevel(type);
            int stat = config.GetValue(level + Constants.One);

            string statDescription = config.IsMultiplier ? $"{stat}%" : stat.ToString();
            char sign = config.IsPositive ? Plus : Minus;

            List<string> statsDescription = new()
            {
                $"{UIText.BuffStatDescription[type]} {sign}{statDescription}"
            };

            return new(type, level, statsDescription, config.Icon, UIText.BuffName[type]);
        }
    }
}
