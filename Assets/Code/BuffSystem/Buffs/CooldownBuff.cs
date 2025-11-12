using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.SpellBooks;
using Assets.Code.Tools;

namespace Assets.Code.BuffSystem
{
    public class CooldownBuff : Buff
    {
        private readonly IEffect _effect;

        public CooldownBuff(BuffConfig config, HeroComponents hero, int level = 1) : base(config, hero, level)
        {
            _effect = new MultiplyEffect(CalculateValue(), FloatStatType.Cooldown, Constants.One);
        }

        public override void Apply()
        {
            Hero.AbilityContainer.AddEffect(_effect);
        }

        protected override void OnLevelUp()
        {
            _effect.ChangeValue(CalculateValue());
        }

        private float CalculateValue()
        {
            return (float)(Constants.One - Constants.PercentToMultiplier(Value));
        }
    }
}
