using Assets.Code.AbilitySystem.Abilities;
using Assets.Code.SpellBooks;
using Assets.Code.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.AbilitySystem
{
    public class AbilityContainer
    {
        private readonly Dictionary<AbilityType, Ability> _abilities = new();
        private readonly List<IEffect> _effects = new();

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
            ability.ThrowIfNull();

            if (_effects.Count > Constants.Zero)
            {
                ability.AddEffect(_effects);
            }

            ability.Run();

            _abilities.Add(ability.Type, ability);
        }

        public void RemoveAll()
        {
            Stop();

            _abilities.ForEachValues(ability => ability.Dispose());
            _abilities.Clear();
            _effects.Clear();

            TimerService.StopAllTimersForOwner(this);
        }

        public void Upgrade(AbilityType abilityType)
        {
            _abilities[abilityType.ThrowIfNull()].LevelUp();
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

        public void AddEffect(IEffect effect, float time = -1)
        {
            _effects.Add(effect);
            _abilities.ForEachValues(ability => ability.AddEffect(effect));

            if (time > Constants.Zero)
            {
                TimerService.StartTimer(time, () => RemoveEffect(effect), this);
            }
        }

        public bool HasEffect(IEffect effect)
        {
            return _effects.Contains(effect);
        }

        private void RemoveEffect(IEffect effect)
        {
            _effects.Remove(effect);
            _abilities.ForEachValues(ability => ability.RemoveEffect(effect));
        }
    }
}
