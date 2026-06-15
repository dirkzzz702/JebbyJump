namespace JebbyJump.Wardrobe.Visual
{
    // Single source of truth for wardrobe layout sizing, in CanvasScaler
    // reference (1920x1080) units. Shared by WardrobePanelController, the
    // responsive layout calculator, the BuildWardrobePanel scaffold, and tests
    // so the touch-target minimum and region sizes never drift. P20 mobile
    // (landscape) accessibility hardening - sizing only, no gameplay/semantics.
    public static class WardrobeLayoutMetrics
    {
        // ~48 dp comfort target on dense landscape phones at the 1080-height
        // reference (derived in the Accessibility checklist). Interactive
        // controls must be at least this in canvas units.
        public const float MinTouchTargetCanvasUnits = 90f;

        // Runtime outfit-row height (was 64; raised to meet the touch minimum).
        public const float RowMinHeight = 90f;

        // Outfit list VerticalLayoutGroup spacing + padding (shared by the
        // scaffold and the panel's scroll-into-view math so they never drift).
        public const float RowSpacing = 12f;
        public const float ListPadding = 12f;

        // Action / ceremony button sizes (>= the touch minimum).
        public const float ActionButtonWidth = 220f;
        public const float ActionButtonHeight = 90f;
        public const float CeremonyButtonWidth = 260f;
        public const float CeremonyButtonHeight = 90f;

        // Region paddings / fixed-band sizes (canvas units).
        public const float EdgePadding = 40f;
        public const float RegionSpacing = 24f;
        public const float HeaderHeight = 96f;
        public const float ActionRegionHeight = 120f;

        // Below this usable (post-safe-area) height, switch to the compact
        // layout - e.g. a 20:9 phone in landscape has a logical canvas height
        // of ~966 with this 1920x1080 reference + match 0.5.
        public const float CompactHeightThreshold = 1000f;

        // Standard side-by-side: preview occupies this fraction of the middle
        // width; the list takes the rest.
        public const float PreviewWidthFractionStandard = 0.38f;

        // Compact: preview collapses to a band above the list.
        public const float PreviewHeightCompact = 150f;

        // List height we try to preserve in any layout.
        public const float MinListHeight = 240f;
    }
}
