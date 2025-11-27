using Assets.Code.CharactersLogic.GeneralLogic;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data.Value;
using Assets.Code.SpellBooks.Base;
using Assets.Code.Tools.Base;
using Assets.Code.Ui.BuffView;
using UnityEngine;

namespace Assets.Code.SpellBooks.Books
{
    public class ProtectionBook : SpellBook
    {
        [SerializeField][Min(1f)] private float _additionalResist = 100f;
        [SerializeField][Min(1f)] private float _time = 15f;
        [SerializeField] private BuffType _type = BuffType.Armor;

        private IValueEffect _effect;

        private void Awake()
        {
            _effect = new SumEffect(-_additionalResist);
        }

        protected override void Apply(HeroComponents hero)
        {
            Resist resist = hero.Health.Resist;

            resist.AddEffect(_effect);
            hero.BuffView.AddBuff(_type, _time);

            TimerService.StartTimer(_time, () => resist.RemoveEffect(_effect));
            this.SetActive(false);
        }
    }
}
