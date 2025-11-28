using UnityEngine;

namespace Assets.Code.LootSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class Loot : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }

        [field: SerializeField][field: Min(1)] public int Value { get; private set; } = 1;
        [field: SerializeField] public LootType Type { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}
