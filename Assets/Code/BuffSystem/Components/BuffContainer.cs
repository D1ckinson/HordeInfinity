using Assets.Code.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.BuffSystem
{
    public class BuffContainer
    {
        private readonly Dictionary<BuffType, Buff> _buffs = new();

        public IEnumerable<BuffType> MaxedBuffs => _buffs.Values
            .Where(buff => buff.IsMaxed)
            .Select(buff => buff.Type);

        public void Add(Buff buff)
        {
            buff.ThrowIfNull()
                .Apply();

            _buffs.Add(buff.Type, buff);
        }

        public void Reset()
        {
            _buffs.Clear();
        }

        public int GetBuffLevel(BuffType buffType)
        {
            _buffs.TryGetValue(buffType, out Buff buff);

            return buff.IsNull() ? Constants.Zero : buff.Level;
        }

        public void Upgrade(BuffType buffType)
        {
            _buffs[buffType.ThrowIfNull()].LevelUp();
        }

        public bool HasBuff(BuffType buffType)
        {
            return _buffs.ContainsKey(buffType.ThrowIfNull());
        }
    }
}
