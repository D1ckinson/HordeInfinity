using Assets.Code.Tools;
using System.Collections.Generic;

namespace Assets.Code.BuffSystem
{
    public class BuffContainer
    {
        private readonly Dictionary<BuffType, Buff> _buffs = new();

        public void Add(Buff buff)
        {
            buff.ThrowIfNull();
            _buffs.Add(buff.Type, buff);
        }

        public void Reset()
        {
            _buffs.Clear();
        }
    }
}
