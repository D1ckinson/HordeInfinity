using Assets.Code;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class PlayerData
    {
        public Wallet Wallet;
        public AbilityType StartAbility;
        public Dictionary<AbilityType, int> AbilityUnlockLevel;
        public Dictionary<AbilityType, float> DamageDealt;
        public Dictionary<AbilityType, int> KillCount;
        public float ScoreRecord;

        public PlayerData()
        {
            Wallet = new();
            StartAbility = AbilityType.SwordStrike;

            //AbilityUnlockLevel = new()
            //{
            //    [AbilityType.SwordStrike] = 10,
            //    [AbilityType.GhostSwords] = 10,
            //    [AbilityType.HolyGround] = 10,
            //    [AbilityType.MidasHand] = 10,
            //    [AbilityType.Bombard] = 10,
            //    [AbilityType.BlackHole] = 10,
            //    [AbilityType.StoneSpikes] = 10,
            //    [AbilityType.IceStuff] = 10,
            //    [AbilityType.Shuriken] = 10,
            //    [AbilityType.Fireball] = 10,
            //    [AbilityType.WindFlow] = 10
            //};

            AbilityUnlockLevel = new()
            {
                [AbilityType.SwordStrike] = 5,
                [AbilityType.GhostSwords] = 5,
                [AbilityType.HolyGround] = 2,
                [AbilityType.MidasHand] = 0,
                [AbilityType.Bombard] = 1,
                [AbilityType.BlackHole] = 0,
                [AbilityType.StoneSpikes] = 0,
                [AbilityType.IceStaff] = 0,
                [AbilityType.Shuriken] = 0,
                [AbilityType.Fireball] = 0,
                [AbilityType.WindFlow] = 0
            };

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
    }
}
