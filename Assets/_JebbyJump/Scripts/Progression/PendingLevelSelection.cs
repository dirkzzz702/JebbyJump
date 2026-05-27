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

        public static void Reset() => Index = 0;
    }
}
