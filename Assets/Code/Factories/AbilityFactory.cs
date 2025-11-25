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
        private readonly BattleMetrics _battleMetrics;

        public AbilityFactory(
            Dictionary<AbilityType, AbilityConfig> configs,
            Transform hero,
            Transform heroCenter,
            Dictionary<AbilityType, int> abilityUnlockLevel,
            BattleMetrics battleMetrics,
            LootFactory lootFactory,
            Animator animator,
            ITimeService timeService)
        {
            _configs = configs.ThrowIfNullOrEmpty();
            _hero = hero.ThrowIfNull();
            _heroCenter = heroCenter.ThrowIfNull();
            _abilityUnlockLevel = abilityUnlockLevel.ThrowIfNull();
            _lootFactory = lootFactory.ThrowIfNull();
            _animator = animator.ThrowIfNull();
            _timeService = timeService.ThrowIfNull();
            _battleMetrics = battleMetrics.ThrowIfNull();

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
            return new SwordStrike(_configs[AbilityType.SwordStrike], _abilityUnlockLevel, _heroCenter, _animator, _battleMetrics, 7);
        }

        private Ability CreateGhostSwords()
        {
            return new GhostSwords(_configs[AbilityType.GhostSwords], _abilityUnlockLevel, _heroCenter, _battleMetrics, 7);
        }

        private Ability CreateHolyGround()
        {
            return new HolyGround(_configs[AbilityType.HolyGround], _abilityUnlockLevel, _hero, _timeService, _battleMetrics, 7);
        }

        private Ability CreateMidasHand()
        {
            return new MidasHand(_configs[AbilityType.MidasHand], _abilityUnlockLevel, _heroCenter, _lootFactory, _battleMetrics);
        }

        private Ability CreateBombard()
        {
            return new Bombard(_configs[AbilityType.Bombard], _abilityUnlockLevel, _heroCenter, _battleMetrics, 7);
        }

        private Ability CreateBlackHole()
        {
            return new BlackHole(_configs[AbilityType.BlackHole], _abilityUnlockLevel, _heroCenter, _timeService, _battleMetrics, 7);
        }

        private Ability CreateStoneSpikes()
        {
            return new StoneSpikes(_configs[AbilityType.StoneSpikes], _abilityUnlockLevel, _hero, _battleMetrics, 7);
        }

        private Ability CreateIceStaff()
        {
            return new IceStaff(_configs[AbilityType.IceStaff], _abilityUnlockLevel, _heroCenter, _battleMetrics, 7);
        }

        private Ability CreateShuriken()
        {
            return new Shuriken(_configs[AbilityType.Shuriken], _abilityUnlockLevel, _heroCenter, _timeService, _battleMetrics, 7);
        }

        private Ability CreateFireball()
        {
            return new Fireball(_configs[AbilityType.Fireball], _abilityUnlockLevel, _heroCenter, _timeService, _battleMetrics, 7);
        }

        private Ability CreateWindFlow()
        {
            return new WindFlow(_configs[AbilityType.WindFlow], _abilityUnlockLevel, _hero, _battleMetrics, 7);
        }
    }
}