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
        public Dictionary<AbilityType, int> DamageDealt;
        public Dictionary<AbilityType, int> UnlockDamage;
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
            //    [AbilityType.MidasHand] = 5,
            //    [AbilityType.Bombard] = 10,
            //    [AbilityType.BlackHole] = 10,
            //    [AbilityType.StoneSpikes] = 10,
            //    [AbilityType.IceStaff] = 10,
            //    [AbilityType.Shuriken] = 10,
            //    [AbilityType.Fireball] = 10,
            //    [AbilityType.WindFlow] = 10
            //};

            //добавить информацию об уровне прокачиваемой способности
            //сделать появление окна

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

            UnlockDamage = new()
            {
                [AbilityType.SwordStrike] = 0,
                [AbilityType.GhostSwords] = 100000,
                [AbilityType.HolyGround] = 1000000,
                [AbilityType.Bombard] = 1000000,
                [AbilityType.BlackHole] = 1000000,
                [AbilityType.StoneSpikes] = 100000,
                [AbilityType.IceStaff] = 1000000,
                [AbilityType.Shuriken] = 1000000,
                [AbilityType.Fireball] = 1000000,
            };

            //UnlockDamage = new()
            //{
            //    [AbilityType.SwordStrike] = 0,
            //    [AbilityType.GhostSwords] = 10,
            //    [AbilityType.HolyGround] = 100,
            //    [AbilityType.Bombard] = 1000,
            //    [AbilityType.BlackHole] = 10000,
            //    [AbilityType.StoneSpikes] = 10000,
            //    [AbilityType.IceStaff] = 10000,
            //    [AbilityType.Shuriken] = 10000,
            //    [AbilityType.Fireball] = 10000,
            //};
        }
    }
}
