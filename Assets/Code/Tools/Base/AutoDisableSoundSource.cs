using UnityEngine;

namespace Assets.Code.Tools.Base
{
    public class AutoDisableSoundSource : MonoBehaviour
    {
        private const int Zero = 0;

        [field: SerializeField] private AudioSource[] _sources;

        private void Update()
        {
            for (int i = Zero; i < _sources.Length; i++)
            {
                AudioSource source = _sources[i];

                if (source.isPlaying)
                {
                    return;
                }
            }

            gameObject.SetActive(false);
        }
    }
}
