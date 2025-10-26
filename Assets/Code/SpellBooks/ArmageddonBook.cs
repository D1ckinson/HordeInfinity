using Assets.Code.CharactersLogic;
using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Tools;
using UnityEngine;

namespace Assets.Code.Spawners
{
    public class ArmageddonBook : SpellBook
    {
        [SerializeField][Min(1f)] private float _damage = 1000f;
        [SerializeField][Min(1f)] private float _range = 100f;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private ParticleSystem _effectPrefab;

        private readonly Collider[] _enemy = new Collider[70];

        private ParticleSystem _effect;

        private void Awake()
        {
            _effect = _effectPrefab.Instantiate(false);
        }

        protected override void Apply(HeroComponents hero)
        {
            _effect.SetPosition(hero.transform);
            _effect.SetActive(true);
            _effect.Play();

            int count = Physics.OverlapSphereNonAlloc(hero.transform.position, _range, _enemy, _layerMask);

            for (int i = Constants.Zero; i < count; i++)
            {
                if (_enemy[i].TryGetComponent(out Health health))
                {
                    health.TakeDamage(_damage);
                }
            }

            this.SetActive(false);
        }
    }
}
