using UnityEngine;

namespace Assets.Code.CharactersLogic.Movement.Interfaces
{
    public interface ITargetDirectionTeller : ITellDirection
    {
        public void SetTarget(Transform target);
    }
}
