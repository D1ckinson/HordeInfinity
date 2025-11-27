using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.SpellBooks.Base;
using Assets.Code.Tools.Base;
using Assets.Code.Ui.BuffView;
using UnityEngine;

namespace Assets.Code.SpellBooks.Books
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
