using Assets.Code.Core;
using Assets.Code.Tools.Base;
using UnityEngine;

namespace Assets.Code.Data.Base
{
    [CreateAssetMenu(menuName = "Game/CharacterConfig")]
    public class CharacterConfig : ScriptableObject
    {
        [field: Header("Character Settings")]
        [field: SerializeField] public CharacterType Type { get; private set; }
        [field: SerializeField] public Transform Prefab { get; private set; }

        [field: Header("Health Settings")]
        [field: SerializeField][field: Min(1f)] public float MaxHealth { get; private set; } = 100;
        [field: SerializeField][field: Min(0f)] public float Regeneration { get; private set; } = 1f;
        [field: SerializeField][field: Min(0.1f)] public float InvincibilityDuration { get; private set; } = 0.1f;
        [field: SerializeField][field: Range(0f, 100f)] public float InvincibilityTriggerPercent { get; private set; } = 50;
        [field: SerializeField][field: Min(0)] public float Resist { get; private set; } = 0;

        [field: Header("Movement Settings")]
        [field: SerializeField][field: Min(1f)] public float MoveSpeed { get; private set; } = 10;
        [field: SerializeField][field: Min(1f)] public float RotationSpeed { get; private set; } = 200;

        [field: Header("Contact Damage Settings")]
        [field: SerializeField][field: Min(10)] public int Damage { get; private set; } = 10;
        [field: SerializeField] public LayerMask DamageLayer { get; private set; }

        [field: Header("loot Collection Settings")]
        [field: SerializeField][field: Min(1f)] public float AttractionRadius { get; private set; } = 10f;
        [field: SerializeField][field: Min(1f)] public float PullSpeed { get; private set; } = 5f;

        [field: Header("Loot Droop Settings")]
        [field: SerializeField] public LootDropInfo[] Loot { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            DamageLayer.ThrowIfNull();
        }
#endif
    }
}
