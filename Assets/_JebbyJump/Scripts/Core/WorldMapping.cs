namespace JebbyJump.Core
{
    // Pure level<->world arithmetic for the 100-level / 10-world structure
    // (WorldExpansion100, phase P34B). Lives in Core so it is unit-testable
    // without an Assembly-CSharp reference.
    //
    // Levels are contiguous: World n owns global level ids (n-1)*10+1 .. n*10,
    // i.e. 0-based level indices (n-1)*10 .. n*10-1. Existing Levels 1-10 are
    // World 1 and keep indices 0-9 (save-key safety - see doc 10).
    public static class WorldMapping
    {
        public const int LevelsPerWorld = 10;
        public const int WorldCount = 10;
        public const int TotalLevels = LevelsPerWorld * WorldCount;

        // 0-based level index -> 1-based world number. 0 when out of range.
        public static int WorldNumberForLevelIndex(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= TotalLevels) return 0;
            return levelIndex / LevelsPerWorld + 1;
        }

        // 1-based world number -> first/last 0-based level index. -1 when invalid.
        public static int FirstLevelIndexOfWorld(int worldNumber)
        {
            if (!IsValidWorldNumber(worldNumber)) return -1;
            return (worldNumber - 1) * LevelsPerWorld;
        }

        public static int LastLevelIndexOfWorld(int worldNumber)
        {
            if (!IsValidWorldNumber(worldNumber)) return -1;
            return worldNumber * LevelsPerWorld - 1;
        }

        // 1-based position of a level inside its own world (1..10). 0 when out of range.
        public static int LevelNumberWithinWorld(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= TotalLevels) return 0;
            return levelIndex % LevelsPerWorld + 1;
        }

        // A world's last level is its finale.
        public static bool IsFinaleLevelIndex(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= TotalLevels) return false;
            return (levelIndex + 1) % LevelsPerWorld == 0;
        }

        // Stable world id "W01".."W10". Empty when invalid.
        public static string WorldIdForNumber(int worldNumber)
        {
            if (!IsValidWorldNumber(worldNumber)) return string.Empty;
            return "W" + worldNumber.ToString("00");
        }

        public static bool IsValidWorldNumber(int worldNumber)
            => worldNumber >= 1 && worldNumber <= WorldCount;
    }
}
