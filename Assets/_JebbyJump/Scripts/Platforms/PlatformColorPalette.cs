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
            // Yellow lifted / Orange deepened (2026-07-22, user-approved) to
            // widen the Yellow<->Orange separation, the tightest pair: the
            // platform tint darkens both into muddy warm tones (rendered dE
            // was ~21). Yellow #F9DE2C, Orange #E58A1B raise the min warm-pair
            // separation without colliding Orange into Red. Non-colour cue
            // glyphs remain the accessibility backstop for colour-blind play.
            PlatformColor.Yellow => new Color(0.976f, 0.871f, 0.173f), // #F9DE2C
            PlatformColor.Purple => new Color(0.60f, 0.22f, 0.90f),
            PlatformColor.Orange => new Color(0.898f, 0.541f, 0.106f), // #E58A1B
            _                    => Color.white,
        };
    }
}
