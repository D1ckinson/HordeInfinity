using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.SpellBooks.Base;
using Assets.Code.Tools.Base;
using System.Collections;
using UnityEngine;

namespace Assets.Code.SpellBooks.Books
{
    public class MagnetBook : SpellBook
    {
        [SerializeField][Min(1f)] private float _additionalRadius = 150f;

        private readonly WaitForSeconds _wait = new(1);

        protected override void Apply(HeroComponents hero)
        {
            CoroutineService.StartCoroutine(IncreaseRadius(hero.LootCollector));

            this.SetActive(false);
        }

        private IEnumerator IncreaseRadius(LootCollector collector)
        {
            collector.AttractionRadius.Increase(_additionalRadius);

            yield return _wait;

            collector.AttractionRadius.Decrease(_additionalRadius);
        }
    }
}
