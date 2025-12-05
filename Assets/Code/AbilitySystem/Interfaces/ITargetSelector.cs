using UnityEngine;

namespace Assets.Code.AbilitySystem.Interfaces
{
    public interface ITargetSelector
    {
        public LayerMask DamageLayer { get; }
        public Collider Target {  get; }

        public void ResetTarget();
        public void FindTarget(Vector3 fromPosition);
    }
}
