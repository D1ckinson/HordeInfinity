using Assets.Code.AbilitySystem;
using Assets.Code.AbilitySystem.Abilities;
using Assets.Code.Tools;
using Assets.Scripts;
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
        private readonly ITimeService _timeService;
        private readonly Dictionary<AbilityType, int> _damageDealt;
        private readonly Dictionary<AbilityType, int> _killCount;

        public AbilityFactory(Dictionary<AbilityType, AbilityConfig> configs, Transform hero, Transform heroCenter,
            Dictionary<AbilityType, int> abilityUnlockLevel, Dictionary<AbilityType, int> damageDealt,
            Dictionary<AbilityType, int> killCount, LootFactory lootFactory, Animator animator, ITimeService timeService)
        {
            _configs = configs.ThrowIfNullOrEmpty();
            _hero = hero.ThrowIfNull();

            _heroCenter = heroCenter.ThrowIfNull();
            _abilityUnlockLevel = abilityUnlockLevel.ThrowIfNull();
            _lootFactory = lootFactory.ThrowIfNull();
            _animator = animator.ThrowIfNull();
            _timeService = timeService.ThrowIfNull();
            _damageDealt = damageDealt.ThrowIfNullOrEmpty();
            _killCount = killCount.ThrowIfNullOrEmpty();

            _createFunctions = new()
            {
                [AbilityType.SwordStrike] = CreateSwordStrike,
                [AbilityType.GhostSwords] = CreateGhostSwords,
                [AbilityType.HolyGround] = CreateHolyGround,
                [AbilityType.MidasHand] = CreateMidasHand,
                [AbilityType.Bombard] = CreateBombard,
                [AbilityType.BlackHole] = CreateBlackHole,
                [AbilityType.StoneSpikes] = CreateStoneSpikes,
                [AbilityType.IceStaff] = CreateIceStaff,
                [AbilityType.Shuriken] = CreateShuriken,
                [AbilityType.Fireball] = CreateFireball,
                [AbilityType.WindFlow] = CreateWindFlow
            };
        }

        public Ability Create(AbilityType abilityType)
        {
            return _createFunctions[abilityType].Invoke();
        }

        private SwordStrike CreateSwordStrike()
        {
            AbilityConfig config = _configs[AbilityType.SwordStrike];

            return new SwordStrike(config, _abilityUnlockLevel, _heroCenter, _animator, _damageDealt, _killCount);
        }

        private Ability CreateGhostSwords()
        {
            AbilityConfig config = _configs[AbilityType.GhostSwords];

            return new GhostSwords(config, _abilityUnlockLevel, _heroCenter, _damageDealt, _killCount);
        }

        private Ability CreateHolyGround()
        {
            AbilityConfig config = _configs[AbilityType.HolyGround];

            return new HolyGround(config, _abilityUnlockLevel, _hero, _timeService, _damageDealt, _killCount);
        }

        private Ability CreateMidasHand()
        {
            AbilityConfig config = _configs[AbilityType.MidasHand];

            return new MidasHand(config, _abilityUnlockLevel, _heroCenter, _lootFactory, _killCount);
        }

        private Ability CreateBombard()
        {
            AbilityConfig config = _configs[AbilityType.Bombard];

            return new Bombard(config, _abilityUnlockLevel, _heroCenter, _damageDealt, _killCount);
        }

        private Ability CreateBlackHole()
        {
            AbilityConfig config = _configs[AbilityType.BlackHole];

            return new BlackHole(config, _abilityUnlockLevel, _heroCenter, _timeService, _damageDealt, _killCount);
        }

        private Ability CreateStoneSpikes()
        {
            AbilityConfig config = _configs[AbilityType.StoneSpikes];

            return new StoneSpikes(config, _abilityUnlockLevel, _hero, _damageDealt, _killCount);
        }

        private Ability CreateIceStaff()
        {
            AbilityConfig config = _configs[AbilityType.IceStaff];

            return new IceStaff(config, _abilityUnlockLevel, _heroCenter, _damageDealt, _killCount);
        }

        private Ability CreateShuriken()
        {
            AbilityConfig config = _configs[AbilityType.Shuriken];

            return new Shuriken(config, _abilityUnlockLevel, _heroCenter, _timeService, _damageDealt, _killCount);
        }

        private Ability CreateFireball()
        {
            AbilityConfig config = _configs[AbilityType.Fireball];

            return new Fireball(config, _abilityUnlockLevel, _heroCenter, _timeService, _damageDealt, _killCount);
        }

        private Ability CreateWindFlow()
        {
            AbilityConfig config = _configs[AbilityType.WindFlow];

            return new WindFlow(config, _abilityUnlockLevel, _hero, _damageDealt, _killCount);
        }
    }
}