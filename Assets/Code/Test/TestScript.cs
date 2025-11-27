using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Test
{
    public class TestScript : MonoBehaviour
    {
        private void Start()
        {
            Dictionary<int, int> pairs = new()
            {
                [6] = 60,
                [4] = 40,
                [8] = 80,
                [7] = 70,
                [5] = 50,
                [0] = 00,
                [1] = 10,
                [2] = 20,
                [3] = 30
            };

            IOrderedEnumerable<KeyValuePair<int, int>> a = pairs.OrderByDescending(pair => pair.Key);
            Dictionary<int, int> b = a.ToDictionary(pair => pair.Key, pair => pair.Value);
            pairs.OrderByDescending(pair => pair.Key);

            pairs.Add(9, 90);
        }
    }
}