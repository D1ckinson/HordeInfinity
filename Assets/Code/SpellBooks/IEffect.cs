namespace Assets.Code.SpellBooks
{
    public interface IEffect
    {
        public int Priority { get; }

        public AbilityStats Apply(AbilityStats stats);
    }
}
