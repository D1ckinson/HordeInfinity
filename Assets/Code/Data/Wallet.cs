using Assets.Code.Tools;
using System;

namespace Assets.Scripts
{
    [Serializable]
    public class Wallet
    {
        private float _lootMultiplier = 1;

        public event Action<float> ValueChanged;

        public float CoinsQuantity { get; private set; } = 20;

        public void Add(int value)
        {
            CoinsQuantity += value.ThrowIfZeroOrLess();
            ValueChanged?.Invoke(CoinsQuantity);
        }

        public void Spend(int value)
        {
            CoinsQuantity -= value.ThrowIfNegative().ThrowIfMoreThan((int)CoinsQuantity + Constants.One);
            ValueChanged?.Invoke(CoinsQuantity);
        }

        public void SetLootPercent(int percent)
        {
            _lootMultiplier = Constants.PercentToMultiplier(percent.ThrowIfNegative());
        }
    }
}
