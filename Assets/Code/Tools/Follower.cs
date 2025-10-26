using Assets.Code.Tools;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public class Follower : MonoBehaviour
    {
        [SerializeField] private Vector3 _offset;
        [SerializeField][Range(0.01f, 2f)] private float _offsetChangeDuration = 0.5f;

        private Transform _transform;
        private Transform _target;
        private Coroutine _coroutine;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        private void LateUpdate()
        {
            if (_target.NotNull())
            {
                _transform.position = _target.position + _offset;
            }
        }

        public void Follow(Transform target)
        {
            _target = target.ThrowIfNull();
        }

        public void SetOffset(Vector3 offset)
        {
            if (_coroutine.NotNull())
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(ChangeOffset(offset));
        }

        private IEnumerator ChangeOffset(Vector3 targetOffset)
        {
            float elapsed = Constants.Zero;
            Vector3 startOffset = _offset;

            while (elapsed < _offsetChangeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / _offsetChangeDuration;
                float sqrProgress = progress * progress;

                float x = Mathf.Lerp(startOffset.x, targetOffset.x, sqrProgress);
                float y = Mathf.Lerp(startOffset.y, targetOffset.y, sqrProgress);
                float z = Mathf.Lerp(startOffset.z, targetOffset.z, sqrProgress);

                _offset = new(x, y, z);

                yield return null;
            }

            _offset = targetOffset;
            _coroutine = null;
        }
    }
}
