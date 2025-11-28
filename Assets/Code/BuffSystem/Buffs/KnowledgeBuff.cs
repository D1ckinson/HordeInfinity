using Assets.Code.BuffSystem.Base;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data.Value;
using Assets.Code.LootSystem;
using Assets.Code.Tools.Base;

namespace Assets.Code.BuffSystem.Buffs
{
    public class KnowledgeBuff : Buff
    {
        private readonly IValueEffect _effect;

        public KnowledgeBuff(BuffConfig config, HeroComponents hero, int level = 1) : base(config, hero, level)
        {
            _effect = new MultiplyEffect(CalculateValue());
        }

        public override void Apply()
        {
            Hero.LootCollector.LootAffecter.Add(_effect, LootType.Experience);
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
