using Assets.Code.Tools.Base;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Code.LootSystem
{
    public class PoolGroup<T> where T : Component
    {
        private readonly List<Pool<T>> _pools = new();

        public void Add(Pool<T> pool)
        {
            _pools.Add(pool.ThrowIfNull());
        }

        public T GetRandom()
        {
            return _pools[Random.Range(Constants.Zero, _pools.Count)].Get();
        }

        public void DisableAll()
        {
            _pools.ForEach(pool => pool.DisableAll());
        }
    }
}
