namespace JebbyJump.Session
{
    // Session-scoped mirror of the current level index, set by
    // LevelSessionController. Lets emitters that don't hold a
    // LevelSessionController reference (e.g. PauseMenuController) attach
    // level_index / level_number to analytics events without new scene
    // wiring. Read-only view of gameplay state - it does not drive
    // progression or any gameplay decision.
    public static class LevelContext
    {
        // Zero-based; -1 means "no active level" (e.g. in the Main Menu).
        public static int CurrentIndex { get; private set; } = -1;

        // One-based for display/analytics; 0 when no active level.
        public static int CurrentNumber =>
            CurrentIndex >= 0 ? CurrentIndex + 1 : 0;

        public static void Set(int index) => CurrentIndex = index;

        public static void Clear() => CurrentIndex = -1;
    }
}
