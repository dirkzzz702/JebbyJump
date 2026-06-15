using System.Collections.Generic;

namespace JebbyJump.Shell
{
    // Pure focus-decision logic shared by the shell panels (the controllers map
    // the returned index to an actual Selectable; this stays engine-light and
    // unit-testable). Covers list/grid initial focus and the Result/Game Over
    // fallback order.
    public static class ShellFocusResolver
    {
        // First entry whose control is available (interactable/active), else -1.
        // Used for Main Menu (Continue->...->Quit) and Result/Game Over
        // (Level Complete: Next->Retry->Menu; Game Over: Retry->Menu).
        public static int FirstAvailableIndex(IReadOnlyList<bool> available)
        {
            if (available == null) return -1;
            for (int i = 0; i < available.Count; i++)
                if (available[i]) return i;
            return -1;
        }

        // Preferred index if valid, else the first item, else -1 (empty).
        // Used for Level Select (current level if valid, else highest unlocked
        // / first), computed by the caller and validated here.
        public static int ResolvePreferredOrFirst(int count, int preferred)
        {
            if (count <= 0) return -1;
            if (preferred >= 0 && preferred < count) return preferred;
            return 0;
        }
    }
}
