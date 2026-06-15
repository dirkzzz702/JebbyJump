namespace JebbyJump.Shell
{
    // Canonical shared shell/UI layout metric. P20 introduced the 90-unit touch
    // target for the Wardrobe; P21 promotes it to the shell-wide standard so
    // every menu/panel control uses ONE source (WardrobeLayoutMetrics now
    // references this). In CanvasScaler reference (1920x1080) units.
    public static class ShellLayoutMetrics
    {
        // ~48 dp comfort target on dense landscape phones at the 1080-height
        // reference. Interactive controls must be at least this in canvas units.
        public const float MinTouchTargetCanvasUnits = 90f;

        // Shared shell sizing (single-sourced by scaffolds + bounds tests).
        public const float EdgePadding = 40f;
        public const float StackSpacing = 24f;
        public const float TitleHeight = 90f;
        public const float ButtonHeight = 90f;       // >= touch min
        public const float MenuButtonWidth = 360f;
        public const float SettingsRowWidth = 700f;
        public const float ResultTextHeight = 48f;

        // Level Select grid - matches BuildLevelSelectPanel's GridLayoutGroup
        // (5 columns; cell 180x220; spacing + padding 24). Fits 4:3 landscape.
        public const int LevelSelectColumns = 5;
        public const float LevelSelectCellWidth = 180f;
        public const float LevelSelectCellHeight = 220f;
        public const float GridSpacing = 24f;
        public const float GridPadding = 24f;
    }
}
