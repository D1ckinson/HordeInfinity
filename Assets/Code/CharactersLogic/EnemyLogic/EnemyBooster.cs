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

        private Health _health;
        private Mover _mover;

        private void OnEnable()
        {
            if (TimerService.IsTimerExists(this, BoostHealth) == false)
            {
                TimerService.StartTimer(Constants.SecondsInMinute, BoostHealth, this, true);
            }

            TimerService.StartTimer(Constants.SecondsInMinute, BoostSpeed, this, true);
        }

        private void OnDisable()
        {
            TimerService.StopTimer(this, BoostSpeed);
            _mover?.ResetSpeed();
        }

        public void Initialize(Mover mover, Health health)
        {
            _mover = mover.ThrowIfNull();
            _health = health.ThrowIfNull();
        }

        public void ResetHealthBoost()
        {
            TimerService.StopTimer(this, BoostHealth);
            _health.ResetValue();
        }

        private void BoostHealth()
        {
            _health.AddMaxValue(_healthForMinute);
        }

        private void BoostSpeed()
        {
            _mover.AddSpeed(_speedForMinute);
        }
    }
}
