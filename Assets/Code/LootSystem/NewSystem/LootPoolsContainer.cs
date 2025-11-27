using Assets.Code.Tools.Base;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Code.LootSystem.NewSystem
{
    public class LootPoolsContainer
    {
        private readonly Dictionary<int, List<Pool<NewLoot>>> _pools = new();

        private List<int> _sortedValues = new();

        public void AddLoot(NewLoot loot)
        {
            int lootValue = loot.ThrowIfNull().Value;
            Pool<NewLoot> pool = new(() => loot.Instantiate());

            if (_pools.ContainsKey(lootValue) == false)
            {
                _pools.Add(lootValue, new());
            }

            _pools[lootValue].Add(pool);

            if (_sortedValues.Contains(lootValue) == false)
            {
                _sortedValues = _pools.Keys.OrderByDescending(value => value).ToList();
            }
        }

        public List<NewLoot> Get(int lootValue)
        {
            int remainingValue = lootValue.ThrowIfNegative();
            List<NewLoot> loots = new();

            foreach (int value in _sortedValues)
            {
                while (remainingValue >= value)
                {
                    remainingValue -= value;

                    List<Pool<NewLoot>> pools = _pools[value];
                    Pool<NewLoot> pool = pools[Random.Range(Constants.Zero, pools.Count)];
                    loots.Add(pool.Get());
                }

                if (remainingValue <= Constants.Zero)
                {
                    break;
                }
            }

            if (remainingValue > Constants.Zero)
            {
                NewLoot loot = _pools[_sortedValues.Last()]
                    .First()
                    .Get();

                loots.Add(loot);
            }

            return loots;
        }

        public void DisableAll()
        {
            _pools.ForEachValues(pools => pools.ForEach(pool => pool.DisableAll()));
        }
    }
}
