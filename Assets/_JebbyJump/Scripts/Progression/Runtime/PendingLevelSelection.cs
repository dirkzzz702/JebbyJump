namespace JebbyJump.Progression
{
    // Session-only hand-off slot between MainMenu and Game scenes.
    // LevelSelectController sets Index before SceneLoader.LoadGame().
    // LevelSessionController.Awake reads it in the Game scene and resets.
    // MainMenuController.Awake also resets it so a return-to-menu cannot
    // silently replay the previous selection.
    public static class PendingLevelSelection
    {
        public static int Index;

        // How the upcoming level was launched, for analytics labelling
        // ("continue" / "level_select"). Defaults to "default" and is
        // cleared back to it on Reset so a stale source is never replayed.
        // Purely descriptive; does not affect progression.
        public const string DefaultSource = "default";
        public static string Source = DefaultSource;

        public static void Reset()
        {
            Index = 0;
            Source = DefaultSource;
        }
    }
}
