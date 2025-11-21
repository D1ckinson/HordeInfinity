using System;

namespace Assets.Scripts
{
    public interface IWalletService
    {
        public event Action<int> ValueChanged;

        public int CoinsQuantity { get; }

        public void Add(int value);

        public void Spend(int value);
    }
}
