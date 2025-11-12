using Assets.Code.Tools;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.Data;
using System;
using Random = UnityEngine.Random;

namespace Assets.Code.AbilitySystem
{
    public class AbilityUpgradeGenerator
    {
        private readonly AbilityContainer _container;
        private readonly Dictionary<AbilityType, int> _abilityUnlockLevel;
        private readonly Dictionary<AbilityType, AbilityConfig> _configs;

        private List<AbilityType> _availableAbilities;

        public AbilityUpgradeGenerator(
            AbilityContainer container,
            Dictionary<AbilityType, int> abilityUnlockLevel,
            Dictionary<AbilityType, AbilityConfig> configs)
        {
            _container = container.ThrowIfNull();
            _abilityUnlockLevel = abilityUnlockLevel.ThrowIfNullOrEmpty();
            _configs = configs.ThrowIfNullOrEmpty();

            RefreshAvailableAbilities();
        }

        public bool CanGenerate => _availableAbilities.Count > Constants.Zero;

        public UpgradeOption Generate()
        {
            if (CanGenerate == false)
            {
                throw new InvalidOperationException();
            }

            int index = Random.Range(Constants.Zero, _availableAbilities.Count);

            AbilityType abilityType = _availableAbilities[index];
            _availableAbilities.RemoveAt(index);

            return CreateUpgradeOption(abilityType);
        }

        public void RefreshAvailableAbilities()
        {
            _availableAbilities = Constants.GetEnums<AbilityType>()
                .Except(_container.MaxedAbilities)
                .Except(_abilityUnlockLevel.Where(pair => pair.Value == Constants.Zero).Select(pair => pair.Key))
                .ToList();
        }

        private UpgradeOption CreateUpgradeOption(AbilityType abilityType)
        {
            AbilityConfig config = _configs[abilityType];
            int level = _container.GetAbilityLevel(abilityType);
            AbilityStats nextStats = config.GetStats(level + Constants.One);

            List<string> statsDescription = level > Constants.Zero
                ? config.GetStats(level).GetStatsDifference(nextStats)
                : nextStats.GetStatsDescription();

            return new(abilityType, level, statsDescription, config.Icon, UIText.AbilityName[abilityType]);
        }
    }
}
