using Assets.Code.Tools;
using Assets.Scripts.Movement;
using System;
using UnityEngine;

namespace Assets.Code.CharactersLogic.EnemyLogic
{
    public class EnemyBooster : MonoBehaviour
    {
        [SerializeField] private CharacterMovement _movement;
        [SerializeField][Range(0.1f, 100f)] private float _speedForMinute = 5f;

        private readonly Timer _timer = new();

        private void OnEnable()
        {
            _timer.Start(Constants.SecondsInMinute);
            _timer.Completed += Boost;
        }

        private void OnDisable()
        {
            _movement.ResetSpeed();
            _timer.Stop();
            _timer.Completed -= Boost;
        }

        private void Boost()
        {
            _movement.AddSpeed(_speedForMinute);
            _timer.Start(Constants.SecondsInMinute);
        }
    }
}
