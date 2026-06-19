namespace JebbyJump.Release
{
    // Narrow, commented allowlist for KNOWN-benign build warnings (fix #7). Any
    // build/compiler/IL2CPP/Gradle warning NOT matched here is "unclassified" and
    // invalidates RC readiness. Patterns must be specific (no broad catch-alls).
    // Seeded empty: entries are added only if a real benign warning appears.
    public static class ReleaseWarningAllowlist
    {
        public readonly struct Entry
        {
            public readonly string Pattern; // substring match against the warning text
            public readonly string Reason;
            public Entry(string pattern, string reason)
            {
                Pattern = pattern;
                Reason = reason;
            }
        }

        public static readonly Entry[] Entries = new Entry[]
        {
            new Entry(
                "To use Unity's dashboard services, you need to link your Unity project to a project ID",
                "Unity Services dashboard nag; the game uses no Unity backend services (intentional)."),
            new Entry(
                "Diagnostics Data is enabled, and it requires Debug Symbols",
                "Crash-report symbol-detail nag; Unity crash reporting is not used."),
            new Entry(
                "'TMP_Text.enableWordWrapping' is obsolete",
                "Pre-existing deprecated TMP API in the P15 wardrobe UI; still functional. Tracked for a future TMP-API cleanup (out of P23 scope)."),
            new Entry(
                "MobileControlsToggle._forceVisibleInEditor' is assigned but its value is never used",
                "Field is only read inside #if UNITY_EDITOR, so it is unused in the player build (benign). Tracked for cleanup (out of P23 scope)."),
        };

        public static bool IsAllowed(string warning)
        {
            if (string.IsNullOrEmpty(warning)) return false;
            foreach (var e in Entries)
                if (!string.IsNullOrEmpty(e.Pattern) && warning.Contains(e.Pattern))
                    return true;
            return false;
        }

        // A pattern is "narrow" if it is specific enough to not swallow unrelated
        // warnings: non-empty, reasonably long, and not a wildcard.
        public static bool IsNarrow(string pattern)
            => !string.IsNullOrWhiteSpace(pattern)
            && pattern.Trim().Length >= 12
            && pattern != "*"
            && pattern != ".*";
    }
}
