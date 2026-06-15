using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Computed region rects (canvas units, origin bottom-left) for the wardrobe
    // panel inside an already-safe-area-fitted content area.
    public readonly struct WardrobeLayout
    {
        public readonly Rect Header;
        public readonly Rect List;
        public readonly Rect Preview;
        public readonly Rect Actions;
        public readonly bool IsCompact;

        public WardrobeLayout(
            Rect header, Rect list, Rect preview, Rect actions, bool isCompact)
        {
            Header = header;
            List = list;
            Preview = preview;
            Actions = actions;
            IsCompact = isCompact;
        }
    }

    // Pure responsive region layout for the wardrobe panel within a content
    // rect of (0,0)-(contentSize) in canvas units (the safe-area-fitted area).
    // Landscape: Header pinned top, Actions pinned bottom; the middle holds the
    // outfit List + the selected Preview side by side. On short screens (e.g.
    // 20:9 landscape, logical height ~966) it switches to a COMPACT layout that
    // collapses the preview to a band so the list + full-size touch targets are
    // preserved. Engine-light + deterministic for cross-aspect validation tests.
    public static class WardrobeResponsiveLayout
    {
        public static WardrobeLayout Compute(Vector2 contentSize)
        {
            float w = Mathf.Max(0f, contentSize.x);
            float h = Mathf.Max(0f, contentSize.y);
            float pad = WardrobeLayoutMetrics.EdgePadding;
            float gap = WardrobeLayoutMetrics.RegionSpacing;
            float headerH = WardrobeLayoutMetrics.HeaderHeight;
            float actionsH = WardrobeLayoutMetrics.ActionRegionHeight;
            float innerW = Mathf.Max(0f, w - 2f * pad);

            var header = new Rect(
                pad, Mathf.Max(0f, h - pad - headerH), innerW, headerH);
            var actions = new Rect(pad, pad, innerW, actionsH);

            float midBottom = actions.yMax + gap;
            float midTop = header.yMin - gap;
            float midH = Mathf.Max(0f, midTop - midBottom);

            bool compact = h < WardrobeLayoutMetrics.CompactHeightThreshold;

            Rect list, preview;
            if (!compact)
            {
                float previewW =
                    innerW * WardrobeLayoutMetrics.PreviewWidthFractionStandard;
                float listW = Mathf.Max(0f, innerW - previewW - gap);
                list = new Rect(pad, midBottom, listW, midH);
                preview = new Rect(pad + listW + gap, midBottom, previewW, midH);
            }
            else
            {
                float previewH = Mathf.Clamp(
                    WardrobeLayoutMetrics.PreviewHeightCompact, 0f,
                    Mathf.Max(0f,
                        midH - WardrobeLayoutMetrics.MinListHeight - gap));
                float listH = Mathf.Max(0f, midH - previewH - gap);
                preview = new Rect(pad, midTop - previewH, innerW, previewH);
                list = new Rect(pad, midBottom, innerW, listH);
            }

            return new WardrobeLayout(header, list, preview, actions, compact);
        }
    }
}
