using Assets.Code.Tools;
using System;

namespace Assets.Scripts
{
    public class HeroLevel
    {
        private const float DefaultTransferValue = 30f;

        private readonly Func<int, int> _experienceFormula;

        private float _buffer;
        private float _transferValue = DefaultTransferValue;

        public int Level { get; private set; } = 1;
        public float Value { get; private set; } = 0;
        public float LevelUpValue { get; private set; }
        public int LevelUpsCount { get; private set; } = 0;

        public HeroLevel(Func<int, int> experienceFormula)
        {
            _experienceFormula = experienceFormula.ThrowIfNull();
            LevelUpValue = _experienceFormula.Invoke(Level);
            CalculateTransferValue();
        }

        public event Action<int> LevelChanged;
        public event Action ValueChanged;

        public void Add(float value)
        {
            _buffer += value.ThrowIfNegative();
        }

        public void Reset()
        {
            _buffer = Constants.Zero;
            Level = Constants.One;
            Value = Constants.Zero;
            LevelUpValue = _experienceFormula.Invoke(Level);

            LevelChanged?.Invoke(Level);
            ValueChanged?.Invoke();
            LevelUpsCount = Constants.Zero;
            CalculateTransferValue();
        }

        public void Transfer()
        {
            if (_buffer <= Constants.One)
            {
                return;
            }

            if (_buffer >= _transferValue)
            {
                _buffer -= _transferValue;
                Value += _transferValue;
            }
            else
            {
                Value += _buffer;
                _buffer -= _buffer;
            }

            ValueChanged?.Invoke();


            if (Value >= LevelUpValue)
            {
                LevelUp();
                LevelUpsCount++;
            }
        }

        public void DecreaseLevelUpsCount()
        {
            LevelUpsCount--;
        }

        private void LevelUp()
        {
            Value -= LevelUpValue;
            LevelUpValue = _experienceFormula.Invoke(Level);

            ValueChanged?.Invoke();

            Level++;
            LevelChanged?.Invoke(Level);
            CalculateTransferValue();
        }

        private void CalculateTransferValue()
        {
            _transferValue = DefaultTransferValue + DefaultTransferValue * Level;
        }
    }
}
