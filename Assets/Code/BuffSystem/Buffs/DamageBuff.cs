using Assets.Code.AbilitySystem.StatTypes;
using Assets.Code.BuffSystem.Base;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.SpellBooks;
using Assets.Code.SpellBooks.Base;
using Assets.Code.Tools.Base;

namespace Assets.Code.BuffSystem.Buffs
{
    public class DamageBuff : Buff
    {
        private readonly IEffect _effect;

        public DamageBuff(BuffConfig config, HeroComponents hero, int level = 1) : base(config, hero, level)
        {
            _effect = new MultiplyEffect(CalculateValue(), FloatStatType.Damage, Constants.Two);
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
            return (float)(Constants.One + Constants.PercentToMultiplier(Value));
        }
    }
}
