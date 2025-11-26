using Assets.Code.Tools;
using Assets.Scripts.Configs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Spawners
{
    public class SpellBookSpawner
    {
        private readonly Transform _hero;
        private readonly List<SpellBook> _books = new();
        private readonly GameAreaSettings _areaSettings;

        private readonly float _minDelay;
        private readonly float _maxDelay;
        private readonly float _range;

        private float _delay;

        public SpellBookSpawner(Transform hero, SpellBook[] prefabs, GameAreaSettings areaSettings, BooksSpawnerSettings spawnerSettings)
        {
            _hero = hero.ThrowIfNull();
            prefabs.ThrowIfNullOrEmpty().ForEach(prefab => _books.Add(prefab.Instantiate(false)));
            _areaSettings = areaSettings.ThrowIfNull();

            _minDelay = spawnerSettings.MinDelay.ThrowIfNegative();
            _maxDelay = spawnerSettings.MaxDelay.ThrowIfLessThan(_minDelay);
            _range = spawnerSettings.Range.ThrowIfNegative();
        }

        public void Run()
        {
            UpdateService.RegisterUpdate(SpawnBook);
            _delay = Random.Range(_minDelay, _maxDelay);
        }

        public void Pause()
        {
            UpdateService.UnregisterUpdate(SpawnBook);
        }

        public void Continue()
        {
            UpdateService.RegisterUpdate(SpawnBook);
        }

        public void Stop()
        {
            UpdateService.UnregisterUpdate(SpawnBook);
            _books.ForEach(book => book.SetActive(false));
        }

        public void SpawnBook(float deltaTime)
        {
            _delay -= deltaTime;

            if (_delay > Constants.Zero)
            {
                return;
            }

            _delay = Random.Range(_minDelay, _maxDelay);

            IEnumerable<SpellBook> disableBooks = _books.Where(book => book.IsActive() == false);
            float totalWeight = disableBooks.Sum(book => book.SpawnWeight);

            if (totalWeight <= Constants.Zero)
            {
                return;
            }

            float randomValue = Random.Range(Constants.Zero, totalWeight);
            float currentSum = Constants.Zero;

            for (int i = Constants.Zero; i < disableBooks.Count(); i++)
            {
                SpellBook book = disableBooks.ElementAt(i);
                currentSum += book.SpawnWeight;

                if (randomValue <= currentSum)
                {
                    book.SetPosition(GenerateRandomPoint());
                    book.SetActive(true);

                    return;
                }
            }
        }

        private Vector3 GenerateRandomPoint()
        {
            Vector3 distance = Utilities.GenerateRandomDirection() * _range;
            Vector3 point = _hero.position + distance;

            return IsPositionInGameArea(point) ? point : _hero.position - distance;
        }

        private bool IsPositionInGameArea(Vector3 position)
        {
            float distance = Vector3.Distance(_areaSettings.Center, position);

            return distance <= _areaSettings.Radius;
        }
    }
}
