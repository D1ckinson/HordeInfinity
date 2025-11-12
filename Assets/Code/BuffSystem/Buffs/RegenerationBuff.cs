using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data;

namespace Assets.Code.BuffSystem
{
    public class RegenerationBuff : Buff
    {
        private readonly IValueEffect _effect;

        public RegenerationBuff(BuffConfig config, HeroComponents hero, int level = 1) : base(config, hero, level)
        {
            _effect = new SumEffect(Value);
        }

        public override void Apply()
        {
            Hero.Health.Regenerator.AddEffect(_effect);
        }

        protected override void OnLevelUp()
        {
            _effect.SetValue(Value);
        }
    }
}
