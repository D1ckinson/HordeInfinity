using Assets.Code.CharactersLogic.HeroLogic;
using Assets.Code.Tools;
using UnityEngine;

namespace Assets.Code.Spawners
{
    public abstract class SpellBook : MonoBehaviour
    {
        [SerializeField] private float _floatHeight = 0.5f;
        [SerializeField] private float _floatSpeed = 1f;
        [SerializeField] private float _rotationSpeed = 90f;

        private Vector3 _startPosition;
        private float _duration;

        [field: SerializeField][field: Range(1f, 100f)] public int SpawnWeight { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
            {
                return;
            }

            if (other.TryGetComponent(out HeroComponents hero))
            {
                Apply(hero);
            }
        }

        private void OnEnable()
        {
            _startPosition = transform.position;
            _duration = Constants.Zero;
        }

        private void Update()
        {
            _duration += Time.deltaTime;

            float yOffset = Mathf.Sin(_duration * _floatSpeed) * _floatHeight;
            transform.position = _startPosition + Vector3.up * yOffset;

            transform.Rotate(0, _rotationSpeed * Time.deltaTime, Constants.Zero);
        }

        protected abstract void Apply(HeroComponents hero);
    }
}
