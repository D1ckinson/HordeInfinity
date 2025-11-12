using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Data;
using Assets.Code.Spawners;
using Assets.Code.Tools;
using Assets.Code.Ui.Buff_View;
using UnityEngine;

namespace Assets.Code.SpellBooks
{
    public class ProtectionBook : SpellBook
    {
        [SerializeField][Min(1f)] private float _additionalResist = 100f;
        [SerializeField][Min(1f)] private float _time = 15f;
        [SerializeField] private BuffType _type = BuffType.Armor;

        protected override void Apply(HeroComponents hero)
        {
            IValueContainer resist = hero.Health.Resist;

            resist.Increase(_additionalResist);
            hero.BuffView.AddBuff(_type, _time);

            TimerService.StartTimer(_time, () => resist.Decrease(_additionalResist));
            this.SetActive(false);
        }
    }
}
