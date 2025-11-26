using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Spawners;
using Assets.Code.Tools;
using Assets.Scripts;
using System.Collections;
using UnityEngine;

namespace Assets.Code.SpellBooks
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
