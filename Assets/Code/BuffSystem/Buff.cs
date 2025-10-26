using Assets.Code.AbilitySystem;
using Assets.Code.CharactersLogic;
using Assets.Code.Tools;
using Assets.Scripts;
using Assets.Scripts.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.BuffSystem
{
    public abstract class Buff<T>
    {
        private readonly Dictionary<int, int> _valueOnLevel;
        private readonly List<T> _components;

        public Buff(Dictionary<int, int> valueOnLevel, T component)
        {
            _valueOnLevel = valueOnLevel.ThrowIfNullOrEmpty();
            _components = new()
            {
                component.ThrowIfNull()
            };

            Amplify(component, _valueOnLevel[Level]);
        }

        public int Level { get; private set; } = 1;

        public void Add(T component)
        {
            _components.Add(component.ThrowIfNull());
            Amplify(component, _valueOnLevel[Level]);
        }

        public void LevelUp()
        {
            Level++;
            AmplifyAll();
        }

        protected abstract void Amplify(T component, int value);

        private void AmplifyAll()
        {
            int value = _valueOnLevel[Level];

            _components.ForEach(component => Amplify(component, value));
        }
    }

    public class HealthAmplification : Buff<Health>
    {
        public HealthAmplification(Dictionary<int, int> valueOnLevel, Health health) : base(valueOnLevel, health) { }

        protected override void Amplify(Health health, int value)
        {
            health.SetAdditionalValue(value);
        }
    }

    public class RegenAmplification : Buff<Health>
    {
        public RegenAmplification(Dictionary<int, int> valueOnLevel, Health health) : base(valueOnLevel, health) { }

        protected override void Amplify(Health health, int value)
        {
            health.SetRegeneration(value);
        }
    }

    public class DamageAmplification : Buff<Ability>
    {
        public DamageAmplification(Dictionary<int, int> valueOnLevel, Ability ability) : base(valueOnLevel, ability) { }

        protected override void Amplify(Ability ability, int value)
        {
            ability.SetAdditionalDamage(value);
        }
    }

    public class CooldownAmplification : Buff<Ability>
    {
        public CooldownAmplification(Dictionary<int, int> valueOnLevel, Ability ability) : base(valueOnLevel, ability) { }

        protected override void Amplify(Ability ability, int percent)
        {
            ability.SetCooldownPercent(percent);
        }
    }

    public class SpeedAmplification : Buff<CharacterMovement>
    {
        public SpeedAmplification(Dictionary<int, int> valueOnLevel, CharacterMovement characterMovement) : base(valueOnLevel, characterMovement) { }

        protected override void Amplify(CharacterMovement characterMovement, int value)
        {
            characterMovement.SetAdditionalSpeed(value);
        }
    }

    public class LootAmplification : Buff<Wallet>
    {
        public LootAmplification(Dictionary<int, int> valueOnLevel, Wallet valueContainer) : base(valueOnLevel, valueContainer) { }

        protected override void Amplify(Wallet valueContainer, int percent)
        {
            valueContainer.SetLootPercent(percent);
        }
    }

    public class LootAttractionRadiusAmplification : Buff<LootCollector>
    {
        public LootAttractionRadiusAmplification(Dictionary<int, int> valueOnLevel, LootCollector lootCollector) : base(valueOnLevel, lootCollector) { }

        protected override void Amplify(LootCollector lootCollector, int value)
        {
            lootCollector.AddAttractionRadius(value);
        }
    }

    public class ResistAmplification : Buff<Health>
    {
        public ResistAmplification(Dictionary<int, int> valueOnLevel, Health health) : base(valueOnLevel, health) { }

        protected override void Amplify(Health health, int resistPercent)
        {
            //health.AddResist(resistPercent);
        }
    }

    public class ExperienceAmplification : Buff<HeroLevel>
    {
        public ExperienceAmplification(Dictionary<int, int> valueOnLevel, HeroLevel heroExperience) : base(valueOnLevel, heroExperience) { }

        protected override void Amplify(HeroLevel heroExperience, int additionalExperiencePercent)
        {
            heroExperience.SetLootPercent(additionalExperiencePercent);
        }
    }

    public class AmplificationFactory
    {
        public void Create()
        {

        }
    }

    [Serializable]
    public struct AmplificationConfig
    {
        [SerializeField] private int _amplificationValue;

    }
}
