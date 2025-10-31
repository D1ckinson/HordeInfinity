using Assets.Code.CharactersLogic;
using Assets.Code.SpellBooks;
using Assets.Code.Tools;
using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.AbilitySystem.Abilities
{
    public abstract class Ability : IDisposable
    {
        private readonly AbilityConfig _config;
        private readonly Dictionary<AbilityType, int> _abilityUnlockLevel;
        private readonly BattleMetrics _battleMetrics;
        private readonly List<IEffect> _effects = new();

        protected Ability(AbilityConfig config, Dictionary<AbilityType, int> abilityUnlockLevel, BattleMetrics battleMetrics, int level = 1)
        {
            _config = config.ThrowIfNull();
            Level = level.ThrowIfZeroOrLess().ThrowIfMoreThan(_config.MaxLevel);
            _abilityUnlockLevel = abilityUnlockLevel.ThrowIfNullOrEmpty();
            _battleMetrics = battleMetrics.ThrowIfNull();

            BaseStats = _config.GetStats(Level);
            CurrentStats = new(BaseStats);
        }

        public AbilityStats CurrentStats { get; private set; }
        public AbilityStats BaseStats { get; private set; }
        public AbilityType Type => _config.Type;
        public int Level { get; private set; }
        public bool IsMaxed => Level == _abilityUnlockLevel[_config.Type];

        public void Run()
        {
            TimerService.StartTimer(CurrentStats.Get(FloatStatType.Cooldown), Apply, this, true);
        }

        public void Stop()
        {
            TimerService.StopTimer(this, Apply);
        }

        public void LevelUp()
        {
            Level++;
            BaseStats = _config.GetStats(Level);
            CurrentStats = new AbilityStats(BaseStats);
            ModifyStats();

            TimerService.StartTimer(CurrentStats.Get(FloatStatType.Cooldown), Apply, this, true);
        }

        public abstract void Dispose();

        protected void RecordHitResult(HitResult result)
        {
            _battleMetrics.Record(result, Type);
        }

        protected abstract void OnStatsUpdate();

        protected abstract void Apply();

        public void AddEffect(IEffect effect)
        {
            _effects.Add(effect);
            ModifyStats();
        }

        public void AddEffect(List<IEffect> effects)
        {
            _effects.AddRange(effects);
            ModifyStats();
        }

        public void RemoveEffect(IEffect effect)
        {
            _effects.Remove(effect);
            ModifyStats();
        }

        private void ModifyStats()
        {
            AbilityStats stats = new(BaseStats);

            _effects.OrderBy(effect => effect.Priority)
                .ForEach(effect => effect.Apply(stats));

            CurrentStats = stats;
            OnStatsUpdate();
        }
    }
}

namespace VisitorExample.Effects
{
    public class AddFlatEffect : IEffect
    {
        private readonly int _value;

        public AddFlatEffect(int value, int priority)
        {
            _value = value;
            Priority = priority;
        }

        public int Priority { get; }

        public void Effect(StatsVisitor visitor)
        {
            visitor.AffectFlat(x => x + _value);
        }
    }

    public class ComplexEffect : IEffect
    {
        private readonly float _addFlat;
        private readonly float _divideMultiplier;

        public ComplexEffect(float addFlat, float divideMultiplier, int priority)
        {
            _addFlat = addFlat;
            _divideMultiplier = divideMultiplier;
            Priority = priority;
        }

        public int Priority { get; }

        public void Effect(StatsVisitor visitor)
        {
            visitor.AffectFlat(x => x + _addFlat);
            visitor.AffectMultiplier(x => x / _divideMultiplier);
        }
    }

    public interface IEffect
    {
        public int Priority { get; }

        public void Effect(StatsVisitor visitor);
    }

    public class MultiplyFlatEffect : IEffect
    {
        private readonly float _value;

        public MultiplyFlatEffect(float value, int priority)
        {
            _value = value;
            Priority = priority;
        }

        public int Priority { get; }

        public void Effect(StatsVisitor visitor)
        {
            visitor.AffectFlat(x => x * _value);
        }
    }

    public class StatsCalculator
    {
        private List<IEffect> _effects = new List<IEffect>();

        public void AddEffect(IEffect effect)
        {
            _effects.Add(effect);
        }

        public float Calculate(float @base)
        {
            StatsVisitor visitor = new StatsVisitor(@base);

            foreach (IEffect effect in _effects.OrderBy(effect => effect.Priority))
                effect.Effect(visitor);

            return visitor.Result;
        }
    }

    public class StatsVisitor
    {
        private float _base;
        private float _multiplier;
        private float _flat;

        public StatsVisitor(float @base)
        {
            _base = @base;
        }

        public float Result => _base + _flat * _multiplier;

        public void AffectMultiplier(Func<float, float> func) =>
            _multiplier = func.Invoke(_multiplier);

        public void AffectFlat(Func<float, float> func) =>
            _flat = func.Invoke(_flat);
    }
}
