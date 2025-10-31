using Assets.Code.CharactersLogic;
using System;

namespace Assets.Code.AbilitySystem.Abilities
{
    public interface IProjectile
    {
        public event Action<HitResult> Hit;
    }
}
