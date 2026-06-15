using System.Collections.Generic;
using UnityEngine;

namespace JebbyJump.Shell
{
    // A centered, padded vertical stack of elements (title, text lines,
    // buttons) - Main Menu, Pause, Settings rows, Result, Game Over. Each
    // surface supplies its OWN real element heights + max width, so bounds are
    // validated per surface (not one uniform proof for all). Canvas reference
    // units.
    public static class ShellStackLayoutPolicy
    {
        private const float Eps = 0.5f;

        public static float RequiredHeight(
            float padding, float spacing, IReadOnlyList<float> elementHeights)
        {
            if (elementHeights == null || elementHeights.Count == 0)
                return 2f * padding;
            float sum = 0f;
            for (int i = 0; i < elementHeights.Count; i++)
                sum += elementHeights[i];
            return 2f * padding + sum
                + Mathf.Max(0, elementHeights.Count - 1) * spacing;
        }

        public static bool Fits(
            Vector2 content, float padding, float spacing,
            IReadOnlyList<float> elementHeights, float maxElementWidth)
            => content.x > 0f && content.y > 0f
               && maxElementWidth <= content.x + Eps
               && RequiredHeight(padding, spacing, elementHeights)
                  <= content.y + Eps;
    }
}
