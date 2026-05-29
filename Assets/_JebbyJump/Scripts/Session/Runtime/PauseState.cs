namespace JebbyJump.Session
{
    // Session-scoped pause flag. Set by PauseMenuController and read by
    // gameplay input handlers (jump, skill activation, move polling).
    //
    // Time.timeScale = 0 already freezes the timer, skill cooldowns, effect
    // durations, physics, and the memory-phase coroutine. This flag exists
    // only to cover input EVENTS, which the Input System still delivers on
    // unscaled time while the game is paused.
    public static class PauseState
    {
        public static bool IsPaused { get; private set; }

        public static void SetPaused(bool paused) => IsPaused = paused;

        // Read by input handlers to decide whether to ignore input.
        public static bool BlocksGameplayInput => IsPaused;
    }
}
