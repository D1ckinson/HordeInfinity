using UnityEngine;

namespace Assets.Code.CharactersLogic.Movement.Configs
{
    [CreateAssetMenu(menuName = "Game/SocialDirectionTellerConfig")]
    public class SocialDirectionTellerConfig : ScriptableObject
    {
        [Header("Optimization settings")]
        [Min(0.1f)] public float Delay = 0.1f;
        [Min(0.1f)] public float NeighborDistance = 1f;
        [Min(0.1f)] public float StopDistance = 0.5f;
        [Min(1)] public int NeighborCount = 10;

        [Header("Weights")]
        [Min(0.1f)] public float SeparationWeight = 1f;
        [Min(0.1f)] public float AlignmentWeight = 1f;
        [Min(0.1f)] public float CohesionWeight = 1f;
    }
}
