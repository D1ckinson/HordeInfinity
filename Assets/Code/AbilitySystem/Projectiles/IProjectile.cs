using Assets.Code.Data.Base;
using System;

namespace Assets.Code.AbilitySystem.Projectiles
{
    public interface IProjectile
    {
        public event Action<HitResult> Hit;
    }
}
