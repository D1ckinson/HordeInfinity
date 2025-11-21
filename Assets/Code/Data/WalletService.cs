using Assets.Code.Tools;
using System;

namespace Assets.Scripts
{
    public class WalletService : IWalletService
    {
        private readonly Wallet _wallet;

        public WalletService(Wallet wallet)
        {
            _wallet = wallet.ThrowIfNull();
        }

        public int CoinsQuantity => _wallet.Value;

        public event Action<int> ValueChanged;

        public void Add(int value)
        {
            _wallet.Value += value.ThrowIfNegative();
            ValueChanged?.Invoke(_wallet.Value);
        }

        public void Spend(int value)
        {
            _wallet.Value -= value.ThrowIfNegative()
                .ThrowIfMoreThan(_wallet.Value);

            ValueChanged?.Invoke(CoinsQuantity);
        }
    }
}
