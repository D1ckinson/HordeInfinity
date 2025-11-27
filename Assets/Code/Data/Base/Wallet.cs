using System;

namespace Assets.Code.Data.Base
{
    [Serializable]
    public class Wallet
    {
        public int Value;

        public Wallet()
        {
            Value = 20;
        }
    }
}
