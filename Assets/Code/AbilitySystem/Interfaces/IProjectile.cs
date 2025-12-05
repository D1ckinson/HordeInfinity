using Assets.Code.Data.Base;
using System;

namespace Assets.Code.AbilitySystem.Interfaces
{
    public interface IProjectile
    {
        public event Action<HitResult> Hit;
    }
}
