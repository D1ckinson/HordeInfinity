using Assets.Code.BuffSystem;
using Assets.Code.Tools;
using UnityEngine;

namespace Assets.Code.AbilitySystem
{
    public readonly struct BuffUpgradeOption
    {
        public readonly BuffType Type;
        public readonly int NextLevel;
        public readonly string Description;
        public readonly Sprite Icon;
        public readonly string Name;

        public BuffUpgradeOption(BuffType type, int nextLevel, string description, Sprite icon, string name)
        {
            Type = type.ThrowIfNull();
            NextLevel = nextLevel.ThrowIfNegative();
            Description = description.ThrowIfNullOrEmpty();
            Icon = icon.ThrowIfNull();
            Name = name.ThrowIfNullOrEmpty();
        }
    }
}