using Assets.Code.Tools;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Assets.Code.AbilitySystem
{
    public class PseudoRandomUpgradeSelector
    {
        private const int MinWeight = 0;
        private const int MaxWeight = 100;
        private const int AbilityStep = 5;
        private const int BuffStep = -50;

        private readonly AbilityUpgradeGenerator _abilityGenerator;
        private readonly BuffUpgradeGenerator _buffGenerator;

        private int _weight = 0;

        public PseudoRandomUpgradeSelector(AbilityUpgradeGenerator abilityGenerator, BuffUpgradeGenerator buffGenerator)
        {
            _abilityGenerator = abilityGenerator.ThrowIfNull();
            _buffGenerator = buffGenerator.ThrowIfNull();
        }

        public List<UpgradeOption> Generate(int count)
        {
            _abilityGenerator.RefreshAvailableAbilities();
            _buffGenerator.RefreshAvailableBuffs();

            List<UpgradeOption> upgradeOptions = new();

            for (int i = Constants.Zero; i < count; i++)
            {
                UpgradeOption? option = GenerateNext();

                if (option.IsNull())
                {
                    break;
                }
                else
                {
                    upgradeOptions.Add((UpgradeOption)option);
                }
            }

            return upgradeOptions;
        }

        public void Reset()
        {
            _weight = Constants.Zero;
        }

        private UpgradeOption? GenerateNext()
        {
            bool canAbility = _abilityGenerator.CanGenerate;
            bool canBuff = _buffGenerator.CanGenerate;

            return (canAbility, canBuff) switch
            {
                (false, false) => null,
                (true, false) => GenerateAbility(),
                (false, true) => GenerateBuff(),
                (true, true) => GenerateWithWeight()
            };
        }

        private UpgradeOption GenerateAbility()
        {
            (_weight += AbilityStep).Clamp(MinWeight, MaxWeight);

            return _abilityGenerator.Generate();
        }

        private UpgradeOption GenerateBuff()
        {
            (_weight += BuffStep).Clamp(MinWeight, MaxWeight);

            return _buffGenerator.Generate();
        }

        private UpgradeOption GenerateWithWeight()
        {
            return Random.Range(MinWeight, MaxWeight) >= _weight ? GenerateAbility() : GenerateBuff();
        }
    }
}
