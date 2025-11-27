using Assets.Code.BuffSystem.Base;
using Assets.Code.CharactersLogic.GeneralLogic;
using Assets.Code.CharactersLogic.HeroLogic;

namespace Assets.Code.BuffSystem.Buffs
{
    public class HealthBuff : Buff
    {
        public HealthBuff(BuffConfig config, HeroComponents hero, int level = 1) : base(config, hero, level) { }

        public override void Apply()
        {
            Health health = Hero.Health;
            health.SetMaxValue(health.DefaultMaxValue + Value);
        }

        protected override void OnLevelUp()
        {
            Health health = Hero.Health;
            health.SetMaxValue(health.DefaultMaxValue + Value);
        }
    }
}
