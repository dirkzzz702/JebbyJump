using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Pure safe-area -> anchor math for a full-stretch RectTransform that should
    // hug Screen.safeArea. Engine-free/deterministic so it is unit-testable with
    // synthetic rectangles (no device). Degenerate / inverted inputs fall back
    // to full screen (0,0)-(1,1) so the UI is never collapsed off-screen.
    public static class SafeAreaCalculator
    {
        public static void ComputeAnchors(
            Rect safeArea, Vector2 screenSize,
            out Vector2 anchorMin, out Vector2 anchorMax)
        {
            if (screenSize.x <= 0f || screenSize.y <= 0f)
            {
                anchorMin = Vector2.zero;
                anchorMax = Vector2.one;
                return;
            }

            anchorMin = new Vector2(
                Mathf.Clamp01(safeArea.xMin / screenSize.x),
                Mathf.Clamp01(safeArea.yMin / screenSize.y));
            anchorMax = new Vector2(
                Mathf.Clamp01(safeArea.xMax / screenSize.x),
                Mathf.Clamp01(safeArea.yMax / screenSize.y));

            // Empty / inverted safe area -> full screen (never collapse).
            if (anchorMax.x <= anchorMin.x || anchorMax.y <= anchorMin.y)
            {
                anchorMin = Vector2.zero;
                anchorMax = Vector2.one;
            }
        }
    }
}
