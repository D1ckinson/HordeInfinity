using Assets.Code.CharactersLogic.Movement;
using Assets.Code.Tools;
using System;
using UnityEngine;

namespace Assets.Code.CharactersLogic.EnemyLogic
{
    public class EnemyBooster : MonoBehaviour
    {
        [SerializeField][Range(0f, 100f)] private float _speedBoost = 5f;
        [SerializeField][Min(0f)] private float _healthBoost = 1f;
        [SerializeField][Min(1f)] private float _healthBoostTime = 10f;
        [SerializeField][Min(1f)] private float _speedBoostTime = 10f;

        private Health _health;
        private Mover _mover;

        private void OnEnable()
        {
            if (_healthBoost > Constants.Zero && TimerService.IsTimerExists(this, BoostHealth) == false)
            {
                TimerService.StartTimer(_healthBoostTime, BoostHealth, this, true);
            }

            TimerService.StartTimer(_speedBoostTime, BoostSpeed, this, true);
        }

        private void OnDisable()
        {
            TimerService.StopTimer(this, BoostSpeed);
            _mover?.Speed.Reset();
        }

        public void Initialize(Mover mover, Health health)
        {
            _mover = mover.ThrowIfNull();
            _health = health.ThrowIfNull();
        }

        public void ResetHealthBoost()
        {
            TimerService.StopTimer(this, BoostHealth);
            _health.ResetValues();
        }

        private void BoostHealth()
        {
            _health.SetMaxValue(_health.MaxValue + _healthBoost);
        }

        private void BoostSpeed()
        {
            _mover.Speed.Increase(_speedBoost);
        }
    }
}
