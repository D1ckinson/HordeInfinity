using Assets.Code.CharactersLogic.Movement.Configs;
using Assets.Code.CharactersLogic.Movement.Interfaces;
using Assets.Code.Tools.Base;
using System;
using UnityEngine;

namespace Assets.Code.CharactersLogic.Movement.DirectionSources
{
    public class SocialDirectionTeller : ITellDirection
    {
        private readonly Transform _owner;
        private readonly Transform _target;
        private readonly SocialDirectionTellerConfig _config;
        private readonly Collider _collider;
        private readonly LayerMask _layer;
        private readonly Collider[] _neighbors;

        private Vector3 _direction;

        public SocialDirectionTeller(
            Transform owner,
            Transform target,
            SocialDirectionTellerConfig config,
            Collider collider,
            LayerMask layer)
        {
            _owner = owner.ThrowIfNull();
            _target = target.ThrowIfNull();
            _config = config.ThrowIfNull();
            _collider = collider.ThrowIfNull();
            _layer = layer;

            _neighbors = new Collider[_config.NeighborCount + Constants.One];
        }

        ~SocialDirectionTeller()
        {
            TimerService.StopTimer(this, UpdateDirection);
        }

        public event Action<Vector3> DirectionChanged;

        public void Enable()
        {
            TimerService.StartTimer(_config.Delay, UpdateDirection, this, true);
        }

        public void Disable()
        {
            TimerService.StopTimer(this, UpdateDirection);

            _direction = Vector3.zero;
            DirectionChanged?.Invoke(_direction);
        }

        private void UpdateDirection()
        {
            Vector3 direction = _target.position - _owner.position;
            float sqrDistance = direction.sqrMagnitude;

            if (sqrDistance > _config.StopDistance)
            {
                Vector3 separationForce = Vector3.zero;
                int count = Physics.OverlapSphereNonAlloc(_owner.position, _config.NeighborDistance, _neighbors, _layer);

                if (count > Constants.One)
                {
                    separationForce += CalculateSeparationForce(count);
                    separationForce += CalculateAlignmentForce(count);
                    separationForce += CalculateCohesion(count);

                    separationForce *= CalculateSocialScale(sqrDistance);
                }

                _direction = (direction + separationForce).normalized;
            }
            else
            {
                _direction = Vector3.zero;
            }

            DirectionChanged?.Invoke(_direction);
        }

        private float CalculateSocialScale(float sqrDistanceToTarget)
        {
            float stopDistanceSquared = _config.StopDistance * _config.StopDistance;
            float neighborDistanceSquared = _config.NeighborDistance * _config.NeighborDistance;
            float denominator = neighborDistanceSquared - stopDistanceSquared;

            if (denominator <= Mathf.Epsilon)
            {
                return sqrDistanceToTarget > stopDistanceSquared ? Constants.One : Constants.Zero;
            }

            float normalizedDistance = (sqrDistanceToTarget - stopDistanceSquared) / denominator;

            return Mathf.Clamp01(normalizedDistance);
        }

        private Vector3 CalculateCohesion(int count)
        {
            Vector3 averagePosition = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                averagePosition += _neighbors[i].transform.position;
            }

            averagePosition /= count;

            return (averagePosition - _owner.position).normalized * _config.CohesionWeight;
        }

        private Vector3 CalculateAlignmentForce(int count)
        {
            Vector3 result = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                Collider collider = _neighbors[i];

                result += collider.transform.forward;
            }

            if (result == Vector3.zero)
            {
                return result;
            }

            return result.normalized * _config.AlignmentWeight;
        }

        private Vector3 CalculateSeparationForce(int count)
        {
            Vector3 result = Vector3.zero;

            for (int i = 0; i < count; i++)
            {
                Collider collider = _neighbors[i];

                if (collider == _collider)
                {
                    continue;
                }

                Vector3 direction = collider.transform.position - _owner.position;
                float distance = direction.magnitude;

                if (distance > 0)
                {
                    result += -direction.normalized / distance * _config.SeparationWeight;
                }
            }

            return result;
        }
    }
}
