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

        private readonly Transform _heroCenter;
        private readonly Dictionary<AbilityType, int> _abilityUnlockLevel;
        private readonly LootFactory _lootFactory;
        private readonly Animator _animator;

        public AbilityFactory(Dictionary<AbilityType, AbilityConfig> configs, Transform hero, Transform heroCenter, Dictionary<AbilityType, int> abilityUnlockLevel, LootFactory lootFactory, Animator animator)
        {
            _configs = configs.ThrowIfNullOrEmpty();
            _hero = hero.ThrowIfNull();

            _heroCenter = heroCenter.ThrowIfNull();
            _abilityUnlockLevel = abilityUnlockLevel.ThrowIfNull();
            _lootFactory = lootFactory.ThrowIfNull();
            _animator = animator.ThrowIfNull();

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
                [AbilityType.Shuriken] = CreateShuriken,
            };
        }

        public Ability Create(AbilityType abilityType)
        {
            return _createFunctions[abilityType].Invoke();
        }

        private SwordStrike CreateSwordStrike()
        {
            AbilityConfig config = _configs[AbilityType.SwordStrike];

            return new SwordStrike(config, _abilityUnlockLevel, _heroCenter, _animator);
        }

        private Ability CreateGhostSwords()
        {
            AbilityConfig config = _configs[AbilityType.GhostSwords];

            return new GhostSwords(config, _abilityUnlockLevel, _heroCenter);
        }

        private Ability CreateHolyGround()
        {
            AbilityConfig config = _configs[AbilityType.HolyGround];

            return new HolyGround(config, _abilityUnlockLevel, _hero);
        }

        private Ability CreateMidasHand()
        {
            AbilityConfig config = _configs[AbilityType.MidasHand];

            return new MidasHand(config, _abilityUnlockLevel, _heroCenter, _lootFactory);
        }

        private Ability CreateBombard()
        {
            AbilityConfig config = _configs[AbilityType.Bombard];

            return new Bombard(config, _abilityUnlockLevel, _heroCenter);
        }

        private Ability CreateBlackHole()
        {
            AbilityConfig config = _configs[AbilityType.BlackHole];

            return new BlackHole(config, _abilityUnlockLevel, _heroCenter);
        }

        private Ability CreateStoneSpikes()
        {
            AbilityConfig config = _configs[AbilityType.StoneSpikes];

            return new StoneSpikes(config, _abilityUnlockLevel, _hero);
        }

        private Ability CreateIceStuff()
        {
            AbilityConfig config = _configs[AbilityType.IceStuff];

            return new IceStuff(config, _abilityUnlockLevel, _heroCenter);
        }

        private Ability CreateShuriken()
        {
            AbilityConfig config = _configs[AbilityType.Shuriken];

            return new Shuriken(config, _abilityUnlockLevel, _heroCenter);
        }
    }
}