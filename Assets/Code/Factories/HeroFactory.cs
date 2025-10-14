using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.InputActions;
using Assets.Code.Tools;
using Assets.Scripts.Configs;
using Assets.Scripts.Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Factories
{
    public class HeroFactory
    {
        private readonly CharacterConfig _config;
        private readonly Wallet _wallet;
        private readonly HeroLevel _heroLevel;
        private readonly IInputService _inputService;

        public HeroFactory(CharacterConfig config, Wallet wallet, HeroLevel heroLevel, IInputService inputService)
        {
            _config = config.ThrowIfNull();
            _wallet = wallet.ThrowIfNull();
            _heroLevel = heroLevel.ThrowIfNull();
            _inputService = inputService.ThrowIfNull();
        }

        public HeroComponents Create(Vector3 position)
        {
            Transform hero = Object.Instantiate(_config.Prefab, position, Quaternion.identity);
            HeroComponents heroComponents = hero.GetComponentOrThrow<HeroComponents>();

            heroComponents.CharacterMovement.Initialize(_config.MoveSpeed, _config.RotationSpeed, _inputService);
            heroComponents.Health.Initialize(_config.MaxHealth, _config.InvincibilityDuration, _config.InvincibilityTriggerPercent, _config.Regeneration);
            heroComponents.LootCollector.Initialize(_config.AttractionRadius, _config.PullSpeed, _wallet, _heroLevel);

            Camera.main.GetComponentOrThrow<Follower>().Follow(hero);

            return heroComponents;
        }
    }
}
