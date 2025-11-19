using Assets.Code.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.AbilitySystem
{
    public readonly struct UpgradeOption
    {
        public readonly Enum Type;
        public readonly int NextLevel;
        public readonly List<string> Stats;
        public readonly Sprite Icon;
        public readonly string Name;

        public UpgradeOption(
            Enum type,
            int level,
            List<string> statsDescription,
            Sprite icon,
            string name)
        {
            Type = type.ThrowIfNull();
            NextLevel = level.ThrowIfNegative() + Constants.One;
            Stats = statsDescription.ThrowIfNullOrEmpty();
            Icon = icon.ThrowIfNull();
            Name = name.ThrowIfNullOrEmpty();
        }
    }
}