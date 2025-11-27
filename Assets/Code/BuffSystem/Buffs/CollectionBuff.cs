using Assets.Code.BuffSystem.Base;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data;
using Assets.Code.Data.Value;

namespace Assets.Code.BuffSystem.Buffs
{
    public class CollectionBuff : Buff
    {
        private readonly IValueEffect _effect;

        public CollectionBuff(BuffConfig config, HeroComponents hero, int level = 1) : base(config, hero, level)
        {
            _effect = new SumEffect(Value);
        }

        public override void Apply()
        {
            Hero.LootCollector.AttractionRadius.AddEffect(_effect);
        }

        protected override void OnLevelUp()
        {
            _effect.SetValue(Value);
        }
    }
}
