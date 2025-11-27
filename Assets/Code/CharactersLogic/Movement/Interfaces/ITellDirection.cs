using System;
using UnityEngine;

namespace Assets.Code.CharactersLogic.Movement.Interfaces
{
    public interface ITellDirection
    {
        public event Action<Vector3> DirectionChanged;

        public void Enable();

        public void Disable();
    }
}
