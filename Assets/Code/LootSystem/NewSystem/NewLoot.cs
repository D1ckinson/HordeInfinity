using UnityEngine;

namespace Assets.Code.LootSystem.NewSystem
{
    [RequireComponent(typeof(SphereCollider))]
    public class NewLoot : MonoBehaviour
    {
        [field: SerializeField][field: Min(1)] public int Value { get; private set; } = 1;
        [field: SerializeField] public NewLootType Type { get; private set; }


    }
}
