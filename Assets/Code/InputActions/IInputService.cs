using Assets.Code.CharactersLogic.Movement.Interfaces;
using System;

namespace Assets.Code.InputActions
{
    public interface IInputService : ITellDirection
    {
        public event Action BackPressed;
    }
}
