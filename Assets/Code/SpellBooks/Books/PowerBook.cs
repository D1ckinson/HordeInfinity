using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Spawners;
using Assets.Code.Tools;
using Assets.Code.Ui.Buff_View;
using UnityEngine;

namespace Assets.Code.SpellBooks
{
    public class PowerBook : SpellBook
    {
        [SerializeField] private MultiplyEffect _effect;
        [SerializeField] private BuffType _type = BuffType.Power;
        [SerializeField][Min(1f)] private float _time = 15f;

        protected override void Apply(HeroComponents hero)
        {
            hero.AbilityContainer.AddEffect(_effect, _time);
            hero.BuffView.AddBuff(_type, _time);

            this.SetActive(false);
        }
    }
}
