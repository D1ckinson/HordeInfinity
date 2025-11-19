using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Tools;
using System;

namespace Assets.Code.BuffSystem
{
    public abstract class Buff
    {
        private readonly BuffConfig _config;

        public Buff(BuffConfig config, HeroComponents hero, int level = 1)
        {
            _config = config.ThrowIfNull();
            Hero = hero.ThrowIfNull();

            Level = level.ThrowIfZeroOrLess().ThrowIfMoreThan(_config.MaxLevel);
            Value = _config.GetValue(Level);
        }

        public BuffType Type => _config.Type;
        public bool IsMaxed => Level == _config.MaxLevel;
        public int Level { get; private set; }
        public int Value { get; private set; }
        public HeroComponents Hero { get; private set; }

        public void LevelUp()
        {
            Level++;
            Value = _config.GetValue(Level);
            OnLevelUp();
        }

        public abstract void Apply();

        protected abstract void OnLevelUp();
    }
}
