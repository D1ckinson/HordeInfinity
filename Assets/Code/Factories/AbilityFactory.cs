using Assets.Code.AbilitySystem;
using Assets.Code.AbilitySystem.Abilities;
using Assets.Code.Tools;
using Assets.Scripts.Factories;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class AbilityFactory
    {
        private readonly Dictionary<AbilityType, AbilityConfig> _configs;
        private readonly Dictionary<AbilityType, Func<Ability>> _createFunctions;
        private readonly Transform _hero;

        private readonly Transform _swingEffectPoint;
        private readonly Dictionary<AbilityType, int> _abilityUnlockLevel;
        private readonly LootFactory _lootFactory;

        public AbilityFactory(Dictionary<AbilityType, AbilityConfig> configs, Transform hero, Transform swingEffectPoint, Dictionary<AbilityType, int> abilityUnlockLevel, LootFactory lootFactory)
        {
            _configs = configs.ThrowIfNullOrEmpty();
            _hero = hero.ThrowIfNull();

            _swingEffectPoint = swingEffectPoint.ThrowIfNull();
            _abilityUnlockLevel = abilityUnlockLevel.ThrowIfNull();
            _lootFactory = lootFactory.ThrowIfNull();

            _createFunctions = new()
            {
                [AbilityType.SwordStrike] = CreateSwordStrike,
                [AbilityType.GhostSwords] = CreateGhostSwords,
                [AbilityType.HolyGround] = CreateHolyGround,
                [AbilityType.MidasHand] = CreateMidasHand,
                [AbilityType.Bombard] = CreateBombard,
                [AbilityType.BlackHole] = CreateBlackHole,
                [AbilityType.StoneSpikes] = CreateStoneSpikes,
                [AbilityType.IceStuff] = CreateIceStuff,
            };
        }

        public Ability Create(AbilityType abilityType)
        {
            return _createFunctions[abilityType].Invoke();
        }

        private SwordStrike CreateSwordStrike()
        {
            AbilityConfig config = _configs[AbilityType.SwordStrike];

            return new SwordStrike(config, _swingEffectPoint, _hero.GetComponentOrThrow<Animator>(), _abilityUnlockLevel);
        }

        private Ability CreateGhostSwords()
        {
            AbilityConfig config = _configs[AbilityType.GhostSwords];

            return new GhostSwords(config, _swingEffectPoint, _abilityUnlockLevel);
        }

        private Ability CreateHolyGround()
        {
            AbilityConfig config = _configs[AbilityType.HolyGround];

            return new HolyGround(config, _hero, _abilityUnlockLevel);
        }

        private Ability CreateMidasHand()
        {
            AbilityConfig config = _configs[AbilityType.MidasHand];

            return new MidasHand(config, _hero, _abilityUnlockLevel, _lootFactory);
        }

        private Ability CreateBombard()
        {
            AbilityConfig config = _configs[AbilityType.Bombard];

            return new Bombard(config, _hero, _abilityUnlockLevel);
        }

        private Ability CreateBlackHole()
        {
            AbilityConfig config = _configs[AbilityType.BlackHole];

            return new BlackHole(config, _hero, _abilityUnlockLevel, _swingEffectPoint);
        }

        private Ability CreateStoneSpikes()
        {
            AbilityConfig config = _configs[AbilityType.StoneSpikes];

            return new StoneSpikes(config, _hero, _abilityUnlockLevel);
        }

        private Ability CreateIceStuff()
        {
            AbilityConfig config = _configs[AbilityType.IceStuff];

            return new IceStuff(config, _hero, _abilityUnlockLevel);
        }
    }
}