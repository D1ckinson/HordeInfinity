using Assets.Code.AbilitySystem.Base;
using System.Collections.Generic;

namespace Assets.Code.Data.Base
{
    public class BattleMetrics
    {
        public Dictionary<AbilityType, int> DamageDealt;
        public Dictionary<AbilityType, int> KillCount;

        public BattleMetrics()
        {
            DamageDealt = new()
            {
                [AbilityType.SwordStrike] = 0,
                [AbilityType.GhostSwords] = 0,
                [AbilityType.HolyGround] = 0,
                [AbilityType.MidasHand] = 0,
                [AbilityType.Bombard] = 0,
                [AbilityType.BlackHole] = 0,
                [AbilityType.StoneSpikes] = 0,
                [AbilityType.IceStaff] = 0,
                [AbilityType.Shuriken] = 0,
                [AbilityType.Fireball] = 0,
                [AbilityType.WindFlow] = 0
            };

            KillCount = new()
            {
                [AbilityType.SwordStrike] = 0,
                [AbilityType.GhostSwords] = 0,
                [AbilityType.HolyGround] = 0,
                [AbilityType.MidasHand] = 0,
                [AbilityType.Bombard] = 0,
                [AbilityType.BlackHole] = 0,
                [AbilityType.StoneSpikes] = 0,
                [AbilityType.IceStaff] = 0,
                [AbilityType.Shuriken] = 0,
                [AbilityType.Fireball] = 0,
                [AbilityType.WindFlow] = 0
            };
        }

        public void Record(HitResult result, AbilityType type)
        {
            DamageDealt[type] += result.DealtDamage;

            if (result.IsKilled)
            {
                KillCount[type]++;
            }
        }
    }
}
