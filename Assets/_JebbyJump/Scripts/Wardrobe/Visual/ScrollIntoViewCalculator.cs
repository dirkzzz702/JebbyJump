using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Pure ScrollRect "bring item into view" math for a vertical, top-anchored
    // content list. Returns the verticalNormalizedPosition (1 = top, 0 = bottom)
    // that minimally scrolls so a focused row is fully visible. Used when
    // keyboard/gamepad focus moves to a dynamic outfit row. Content-fits or
    // degenerate inputs return the (clamped) current position.
    public static class ScrollIntoViewCalculator
    {
        // itemTop / itemHeight are measured from the CONTENT TOP, y increasing
        // downward (i.e. row 0 has itemTop ~0).
        public static float ComputeVerticalNormalized(
            float contentHeight, float viewportHeight,
            float itemTop, float itemHeight, float current)
        {
            float scrollable = contentHeight - viewportHeight;
            if (scrollable <= 0f) return Mathf.Clamp01(current);

            float v = Mathf.Clamp01(current);
            float topVisible = (1f - v) * scrollable;
            float bottomVisible = topVisible + viewportHeight;
            float itemBottom = itemTop + itemHeight;

            if (itemTop < topVisible)
                v = 1f - (itemTop / scrollable);
            else if (itemBottom > bottomVisible)
                v = 1f - ((itemBottom - viewportHeight) / scrollable);

            return Mathf.Clamp01(v);
        }
    }
}
