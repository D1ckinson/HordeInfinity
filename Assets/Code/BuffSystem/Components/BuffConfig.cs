using Assets.Code.Tools;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Assets.Code.BuffSystem
{
    [CreateAssetMenu(menuName = "Game/BuffConfig")]
    public class BuffConfig : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<int, int> _valueOnLevel;

        public BuffType Type;
        public Sprite Icon;

        public int MaxLevel => _valueOnLevel.Count;

        public int GetValue(int level)
        {
            return _valueOnLevel[level - Constants.One];
        }
    }
}
