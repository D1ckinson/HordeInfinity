using Assets.Code.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public class Pool<T> where T : Component
    {
        private const int Zero = 0;
        private const int DefaultPreCreatedCount = 10;

        private readonly List<T> _items = new();
        private readonly Func<T> _createFunc;

        public Pool(Func<T> createFunc, int preCreatedCount = DefaultPreCreatedCount)
        {
            _createFunc = createFunc.ThrowIfNull();

            for (int i = Zero; i < preCreatedCount; i++)
            {
                Create();
            }
        }

        public int ReleaseCount => _items.Count(item => item.gameObject.activeSelf);

        public List<T> GetAllActive()
        {
            return _items.FindAll(item => item.IsActive());
        }

        public T Get(bool isActive = true)
        {
            T item = _items.FirstOrDefault(item => item.gameObject.activeSelf == false) ?? Create();
            item.gameObject.SetActive(isActive);

            return item;
        }

        public T Get(Transform position, bool isActive = true)
        {
            T item = _items.FirstOrDefault(item => item.gameObject.activeSelf == false) ?? Create();

            item.transform.position = position.position;
            item.gameObject.SetActive(isActive);

            return item;
        }

        public T Get(Vector3 position, bool isActive = true)
        {
            T item = _items.FirstOrDefault(item => item.gameObject.activeSelf == false) ?? Create();

            item.transform.position = position;
            item.gameObject.SetActive(isActive);

            return item;
        }

        public void DisableAll()
        {
            _items.ForEach(item => item.SetActive(false));
        }

        private T Create()
        {
            T item = _createFunc.Invoke();
            _items.Add(item);
            item.gameObject.SetActive(false);

            return item;
        }

        public void ForEach(Action<T> action)
        {
            _items.ForEach(action);
        }

        public void DestroyAll()
        {
            foreach (T item in _items)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }
    }
}
