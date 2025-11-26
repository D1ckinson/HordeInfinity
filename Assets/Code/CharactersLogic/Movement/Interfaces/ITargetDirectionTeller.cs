using Assets.Scripts.Movement;
using UnityEngine;

namespace Assets.Code.CharactersLogic.Movement.Direction_Sources.Interfaces
{
    public interface ITargetDirectionTeller : ITellDirection
    {
        public void SetTarget(Transform target);
    }
}
