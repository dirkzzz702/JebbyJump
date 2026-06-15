using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Pure reproduction of UnityEngine.UI.CanvasScaler "Scale With Screen Size"
    // + "Match Width Or Height" logical-canvas sizing, so layout validation can
    // compute the reference-unit canvas size for any device resolution without
    // a live Canvas. Mirrors Unity's log2-lerp scale-factor formula.
    public static class CanvasScalerMath
    {
        public static Vector2 LogicalCanvasSize(
            Vector2 screenSize, Vector2 referenceResolution, float match)
        {
            if (screenSize.x <= 0f || screenSize.y <= 0f
                || referenceResolution.x <= 0f || referenceResolution.y <= 0f)
                return referenceResolution;

            float logWidth = Mathf.Log(screenSize.x / referenceResolution.x, 2f);
            float logHeight = Mathf.Log(screenSize.y / referenceResolution.y, 2f);
            float logWeighted =
                Mathf.Lerp(logWidth, logHeight, Mathf.Clamp01(match));
            float scaleFactor = Mathf.Pow(2f, logWeighted);
            return screenSize / scaleFactor;
        }
    }
}
