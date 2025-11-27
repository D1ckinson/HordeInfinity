using Assets.Code.Tools.Base;
using UnityEngine;

namespace Assets.Code.BuffSystem.Base
{
    [CreateAssetMenu(menuName = "Game/BuffConfig")]
    public class BuffConfig : ScriptableObject
    {
        [SerializeField] private int[] _value;

        public BuffType Type;
        public Sprite Icon;

        [field: SerializeField] public bool IsMultiplier { get; private set; } = false;
        [field: SerializeField] public bool IsPositive { get; private set; } = true;

        public int MaxLevel => _value.Length;

        public int GetValue(int level)
        {
            level.ThrowIfZeroOrLess().ThrowIfMoreThan(MaxLevel);

            return _value[level - Constants.One];
        }
    }
}
