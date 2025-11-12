using Assets.Code.Tools;
using System;

namespace Assets.Scripts
{
    [Serializable]
    public class Wallet
    {
        public event Action<float> ValueChanged;

        public int CoinsQuantity { get; private set; } = 20;

        public void Add(int value)
        {
            CoinsQuantity += value.ThrowIfZeroOrLess();
            ValueChanged?.Invoke(CoinsQuantity);
        }

        public void Spend(int value)
        {
            CoinsQuantity -= value.ThrowIfNegative()
                .ThrowIfMoreThan(CoinsQuantity + Constants.One);

            ValueChanged?.Invoke(CoinsQuantity);
        }
    }
}
