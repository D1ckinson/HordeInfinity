using Assets.Code.AbilitySystem.Base;
using Assets.Code.BuffSystem.Base;
using Assets.Code.Tools.Base;
using System;

namespace Assets.Code.LevelUpSystem
{
    public class Upgrader
    {
        private readonly AbilityContainer _abilityContainer;
        private readonly AbilityFactory _abilityFactory;
        private readonly BuffContainer _buffContainer;
        private readonly BuffFactory _buffFactory;

        public Upgrader(
            AbilityContainer container,
            AbilityFactory abilityFactory,
            BuffContainer buffContainer,
            BuffFactory buffFactory)
        {
            _abilityContainer = container.ThrowIfNull();
            _abilityFactory = abilityFactory.ThrowIfNull();
            _buffContainer = buffContainer.ThrowIfNull();
            _buffFactory = buffFactory.ThrowIfNull();
        }

        public void Upgrade(Enum type)
        {
            switch (type)
            {
                case AbilityType abilityType:
                    UpgradeAbility(abilityType);
                    break;

                case BuffType buffType:
                    UpgradeBuff(buffType);
                    break;

                default:
                    throw new ArgumentException();
            }
        }

        private void UpgradeAbility(AbilityType abilityType)
        {
            switch (_abilityContainer.HasAbility(abilityType))
            {
                case true:
                    _abilityContainer.Upgrade(abilityType);
                    break;

                case false:
                    _abilityContainer.Add(_abilityFactory.Create(abilityType));
                    break;
            }
        }

        private void UpgradeBuff(BuffType buffType)
        {
            switch (_buffContainer.HasBuff(buffType))
            {
                case true:
                    _buffContainer.Upgrade(buffType);
                    break;

                case false:
                    _buffContainer.Add(_buffFactory.Create(buffType));
                    break;
            }
        }
    }
}
