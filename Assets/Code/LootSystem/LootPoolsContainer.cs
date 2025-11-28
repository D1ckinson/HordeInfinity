using Assets.Code.Tools.Base;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.LootSystem
{
    public class LootPoolsContainer
    {
        private readonly Dictionary<int, PoolGroup<Loot>> _poolGroups = new();

        private List<int> _sortedKeys = new();

        public void AddLoot(Loot loot)
        {
            int lootValue = loot.ThrowIfNull().Value;
            Pool<Loot> pool = new(() => loot.Instantiate());

            if (_poolGroups.ContainsKey(lootValue) == false)
            {
                _poolGroups.Add(lootValue, new());
            }

            _poolGroups[lootValue].Add(pool);

            if (_sortedKeys.Contains(lootValue) == false)
            {
                _sortedKeys = _poolGroups.Keys.OrderByDescending(value => value).ToList();
            }
        }

        public List<Loot> Get(int lootValue)
        {
            int remainingValue = lootValue.ThrowIfNegative();
            List<Loot> loots = new();

            foreach (int value in _sortedKeys)
            {
                while (remainingValue >= value)
                {
                    remainingValue -= value;
                    loots.Add(_poolGroups[value].GetRandom());
                }

                if (remainingValue <= Constants.Zero)
                {
                    break;
                }
            }

            if (remainingValue > Constants.Zero)
            {
                loots.Add(_poolGroups[_sortedKeys.Min()].GetRandom());
            }

            return loots;
        }

        public void DisableAll()
        {
            _poolGroups.ForEachValues(group => group.DisableAll());
        }
    }
}
