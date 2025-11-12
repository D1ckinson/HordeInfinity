using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data;
using Assets.Code.Tools;

namespace Assets.Code.BuffSystem
{
    public class SpeedBuff : Buff
    {
        private readonly IValueEffect _effect;

        public SpeedBuff(BuffConfig config, HeroComponents hero, int level = 1) : base(config, hero, level)
        {
            _effect = new MultiplyEffect(CalculateValue());
        }

        public override void Apply()
        {
            Hero.Mover.Speed.AddEffect(_effect);
        }

        protected override void OnLevelUp()
        {
            _effect.SetValue(CalculateValue());
        }

        private float CalculateValue()
        {
            return (float)(Constants.One + Constants.PercentToMultiplier(Value));
        }
    }
}
