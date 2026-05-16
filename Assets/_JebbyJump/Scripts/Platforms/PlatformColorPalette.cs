using JebbyJump.Core;
using UnityEngine;

namespace JebbyJump.Platforms
{
    public static class PlatformColorPalette
    {
        public static Color GetColor(PlatformColor platformColor) => platformColor switch
        {
            PlatformColor.Red    => new Color(0.90f, 0.22f, 0.22f),
            PlatformColor.Blue   => new Color(0.22f, 0.47f, 0.90f),
            PlatformColor.Green  => new Color(0.22f, 0.75f, 0.35f),
            PlatformColor.Yellow => new Color(0.98f, 0.82f, 0.18f),
            PlatformColor.Purple => new Color(0.60f, 0.22f, 0.90f),
            PlatformColor.Orange => new Color(0.95f, 0.55f, 0.15f),
            _                    => Color.white,
        };
    }
}
