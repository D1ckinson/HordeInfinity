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
        public bool IsMaxed => Level >= _abilityUnlockLevel[_config.Type];

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

        public void Dispose()
        {
            _effects.ForEach(effect => effect.ValueChanged -= ModifyStats);
            OnDispose();
        }

        protected abstract void OnDispose();

        protected void RecordHitResult(HitResult result)
        {
            _battleMetrics.Record(result, Type);
        }

        protected abstract void OnStatsUpdate();

        protected abstract void Apply();

        public void AddEffect(IEffect effect)
        {
            effect.ThrowIfNull().ValueChanged += ModifyStats;

            _effects.Add(effect);
            ModifyStats();
        }

        public void AddEffect(List<IEffect> effects)
        {
            effects.ThrowIfNullOrEmpty()
                .ForEach(effect => effect.ValueChanged += ModifyStats);

            _effects.AddRange(effects);
            ModifyStats();
        }

        public void RemoveEffect(IEffect effect)
        {
            effect.ThrowIfNull().ValueChanged -= ModifyStats;

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
