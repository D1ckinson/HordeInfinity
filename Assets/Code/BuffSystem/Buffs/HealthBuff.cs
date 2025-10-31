using Assets.Code.CharactersLogic;
using Assets.Code.CharactersLogic.HeroLogic;

namespace Assets.Code.BuffSystem
{
    public class HealthBuff : Buff
    {
        public HealthBuff(BuffConfig config, HeroComponents hero, int level = 1) : base(config, hero, level) { }

        public override void Apply()
        {
            Health health = Hero.Health;
            health.SetMaxValue(health.MaxValue + Value);
        }
    }
}
