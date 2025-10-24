using Assets.Code.CharactersLogic.Movement;
using Assets.Code.Tools;
using System;
using UnityEngine;

namespace Assets.Code.CharactersLogic.EnemyLogic
{
    public class EnemyBooster : MonoBehaviour
    {
        [SerializeField][Range(0f, 100f)] private float _speedForMinute = 5f;
        [SerializeField][Min(1f)] private float _healthForMinute = 100f;

        private readonly Timer _timer = new();

        private Health _health;
        private NewMover _mover;

        private void OnEnable()
        {
            _timer.Start(Constants.SecondsInMinute);
            _timer.Completed += Boost;
        }

        private void OnDisable()
        {
            _mover?.ResetSpeed();

            _timer.Stop();
            _timer.Completed -= Boost;
        }

        public void Initialize(NewMover mover, Health health)
        {
            _mover = mover.ThrowIfNull();
            _health = health.ThrowIfNull();
        }

        private void Boost()
        {
            _mover.AddSpeed(_speedForMinute);
            _health.AddMaxHealth(_healthForMinute);
            _timer.Start(Constants.SecondsInMinute);
        }
    }
}
