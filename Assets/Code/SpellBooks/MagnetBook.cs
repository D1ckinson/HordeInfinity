using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Spawners;
using Assets.Code.Tools;
using UnityEngine;

namespace Assets.Code.SpellBooks
{
    public class MagnetBook : SpellBook
    {
        [SerializeField][Min(1f)] private float _additionalRadius = 150f;

        protected override void Apply(HeroComponents hero)
        {
            hero.LootCollector.AddAttractionRadius(_additionalRadius, Constants.One);
            this.SetActive(false);
        }
    }
}
