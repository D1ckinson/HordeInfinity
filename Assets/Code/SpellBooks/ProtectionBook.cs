using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Spawners;
using Assets.Code.Tools;
using UnityEngine;

namespace Assets.Code.SpellBooks
{
    public class ProtectionBook : SpellBook
    {
        [SerializeField][Min(1f)] private float _additionalResist = 100f;
        [SerializeField][Min(1f)] private float _time = 15f;

        protected override void Apply(HeroComponents hero)
        {
            hero.Health.IncreaseResist(_additionalResist, _time);
            this.SetActive(false);
        }
    }
}
