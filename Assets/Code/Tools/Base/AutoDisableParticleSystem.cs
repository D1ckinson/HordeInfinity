using UnityEngine;

namespace Assets.Code.Tools.Base
{
    public class AutoDisableParticleSystem : MonoBehaviour
    {
        private const int Zero = 0;
        private const int One = 1;

        [field: SerializeField] private ParticleSystem[] _systems;

        private void Update()
        {
            for (int i = Zero; i < _systems.Length; i++)
            {
                ParticleSystem system = _systems[i];

                if (system.isPlaying || system.particleCount > One)
                {
                    break;
                }

                gameObject.SetActive(false);
            }
        }
    }
}
