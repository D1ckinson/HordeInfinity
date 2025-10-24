using Assets.Code.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.AbilitySystem
{
    public class AbilityContainer
    {
        private readonly Dictionary<AbilityType, Ability> _abilities = new();

        public IEnumerable<AbilityType> MaxedAbilities => _abilities.Values.Where(ability => ability.IsMaxed).Select(ability => ability.Type);

        public void Run()
        {
            _abilities.ForEachValues(ability => ability.Run());
        }

        public void Stop()
        {
            _abilities.ForEachValues(_abilities => _abilities.Stop());
        }

        public void Add(Ability ability)
        {
            ability.ThrowIfNull().Run();

            _abilities.Add(ability.Type, ability);
        }

        public void RemoveAll()
        {
            Stop();

            _abilities.ForEachValues(ability => ability.Dispose());
            _abilities.Clear();
        }

        public void Upgrade(AbilityType abilityType)
        {
            _abilities.GetValueOrThrow(abilityType.ThrowIfNull()).LevelUp();
        }

        public bool HasAbility(AbilityType abilityType)
        {
            return _abilities.ContainsKey(abilityType.ThrowIfNull());
        }

        public int GetAbilityLevel(AbilityType abilityType)
        {
            _abilities.TryGetValue(abilityType.ThrowIfNull(), out Ability ability);

            return ability.IsNull() ? Constants.Zero : ability.Level;
        }
    }
}
