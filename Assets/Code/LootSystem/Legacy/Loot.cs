using UnityEngine;

namespace Assets.Code.LootSystem.Legacy
{
    [RequireComponent(typeof(Rigidbody))]
    public class Loot : MonoBehaviour
    {
        [SerializeField][Min(1)] private int _value = 1;

        [field: SerializeField] public LootType Type { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        public int Collect()
        {
            gameObject.SetActive(false);
            Rigidbody.velocity = Vector3.zero;

            return _value;
        }
    }
}
