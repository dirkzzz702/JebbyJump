namespace JebbyJump.Shell
{
    // P22 correction #1: pure decision for whether the gameplay input layer
    // (movement / jump / skills + the mobile-control CanvasGroup) should be
    // blocked. Blocked under ANY shell modal — pause, pause->settings, level
    // result, or game over. Normal Playing and the memory show phase are NOT
    // blocked (the gate passes false for all four there).
    public static class GameplayInputBlockPolicy
    {
        public static bool ShouldBlock(
            bool isPaused,
            bool isSettingsOpen,
            bool isResultOpen,
            bool isGameOverOpen)
            => isPaused || isSettingsOpen || isResultOpen || isGameOverOpen;
    }
}
