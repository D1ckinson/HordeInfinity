using Assets.Code.AbilitySystem.Interfaces;
using Assets.Code.Tools.Base;
using UnityEngine;

namespace Assets.Code.AbilitySystem.Base
{
    public class BounceTargetSelector : ITargetSelector
    {
        private readonly float _searchRadius;
        private readonly Collider[] _targets;

        public BounceTargetSelector(float searchRadius, LayerMask layer, int maxTargets = 10)
        {
            _searchRadius = searchRadius.ThrowIfNegative();
            DamageLayer = layer;

            _targets = new Collider[maxTargets.ThrowIfZeroOrLess()];
        }

        public Collider Target { get; private set; }
        public LayerMask DamageLayer { get; }

        public void FindTarget(Vector3 fromPosition)
        {
            int count = Physics.OverlapSphereNonAlloc(fromPosition, _searchRadius, _targets, DamageLayer);
            Collider collider = null;

            if (count == Constants.Zero)
            {
                Target = collider;

                return;
            }

            float minDistance = float.MaxValue;

            for (int i = Constants.Zero; i < count; i++)
            {
                Collider target = _targets[i];

                if (target == Target)
                {
                    continue;
                }

                Vector3 direction = fromPosition - target.transform.position;
                float sqrDistance = direction.sqrMagnitude;

                if (minDistance <= sqrDistance)
                {
                    continue;
                }

                minDistance = sqrDistance;
                collider = target;
            }

            Target = collider;
        }

        public void ResetTarget()
        {
            Target = null;
        }
    }
}
