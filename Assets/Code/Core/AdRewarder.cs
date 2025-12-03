using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Tools.Base;
using YG;

namespace Assets.Code.Core
{
    public class AdRewarder
    {
        private const string HealRewardId = "heal";
        private const float AdditionalAttractionRadius = 100f;

        private readonly HeroComponents _hero;

        public AdRewarder(HeroComponents hero)
        {
            _hero = hero.ThrowIfNull();
        }

        public void Heal()
        {
            YG2.RewardedAdvShow(HealRewardId, HealReward);
        }

        private void HealReward()
        {
            _hero.Health.Heal(_hero.Health.MaxValue);
            _hero.LootCollector.AttractionRadius.Increase(AdditionalAttractionRadius);
            TimerService.StartTimer(Constants.One, () => _hero.LootCollector.AttractionRadius.Decrease(AdditionalAttractionRadius), this);
        }
    }
}
