using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data;
using Assets.Code.Loot;
using Assets.Code.Tools;

namespace Assets.Code.BuffSystem
{
    public class ExtractionBuff : Buff
    {
        private readonly IValueEffect _effect;

        public ExtractionBuff(BuffConfig config, HeroComponents hero, int level = 1) : base(config, hero, level)
        {
            _effect = new MultiplyEffect(CalculateValue());
        }

        public override void Apply()
        {
            Hero.LootCollector.LootAffecter.Add(_effect, LootType.LowCoin);
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
