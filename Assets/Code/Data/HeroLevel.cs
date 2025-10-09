using Assets.Code.Tools;
using System;

namespace Assets.Scripts
{
    [Serializable]
    public class HeroLevel
    {
        private const float TransferValue = 10f;

        private readonly Func<int, int> _experienceFormula;

        private float _buffer;
        private float _lootPercent = 1;

        public int Level { get; private set; } = 1;
        public float Value { get; private set; } = 0;
        public float LevelUpValue { get; private set; }

        public HeroLevel(Func<int, int> experienceFormula)
        {
            _experienceFormula = experienceFormula.ThrowIfNull();
            LevelUpValue = _experienceFormula.Invoke(Level);
        }

        public event Action<int> LevelChanged;
        public event Action ValueChanged;

        public void Add(int value)
        {
            _buffer += value.ThrowIfNegative() * _lootPercent;
        }

        public void Reset()
        {
            _buffer = Constants.Zero;
            Level = Constants.One;
            Value = Constants.Zero;
            LevelUpValue = _experienceFormula.Invoke(Level);

            LevelChanged?.Invoke(Level);
            ValueChanged?.Invoke();
        }

        public void SetLootPercent(int percent)
        {
            _lootPercent = Constants.One + Constants.PercentToMultiplier(percent);
        }

        public void Transfer()
        {
            if (_buffer < TransferValue)
            {
                return;
            }

            _buffer -= TransferValue;
            Value += TransferValue;
            ValueChanged?.Invoke();

            if (Value >= LevelUpValue)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Value -= LevelUpValue;
            LevelUpValue = _experienceFormula.Invoke(Level);

            ValueChanged?.Invoke();

            Level++;
            LevelChanged?.Invoke(Level);
        }
    }
}
