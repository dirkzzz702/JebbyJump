using UnityEngine;

namespace JebbyJump.Progression
{
    // Local unlock state, PlayerPrefs-backed. Tracks only the highest
    // unlocked level index. Best time stays in BestTimeStore. Rank is
    // intentionally NOT persisted here — Level Select recomputes it from
    // best time + the current LevelConfig.RankConfig so future threshold
    // tuning never strands a stale stored rank.
    //
    // PlayerPrefs key:
    //   jebby.level.highestUnlocked  (int, default 0 = Level 1 unlocked)
    public static class LevelProgressStore
    {
        private const string HighestUnlockedKey = "jebby.level.highestUnlocked";

        public static int HighestUnlockedIndex
        {
            get => PlayerPrefs.GetInt(HighestUnlockedKey, 0);
            private set
            {
                PlayerPrefs.SetInt(HighestUnlockedKey, value);
                PlayerPrefs.Save();
            }
        }

        // Defensive: negative or absurdly large stored values are treated
        // as "locked" rather than crashing the Level Select grid.
        public static bool IsUnlocked(int index)
        {
            if (index < 0) return false;
            int highest = HighestUnlockedIndex;
            if (highest < 0) return index == 0;
            return index <= highest;
        }

        // Called after a successful level clear. Monotonic — only advances.
        public static void UnlockNext(int completedIndex)
        {
            if (completedIndex < 0) return;
            int next = completedIndex + 1;
            if (next > HighestUnlockedIndex)
                HighestUnlockedIndex = next;
        }

        // Dev-only. Deletes the single unlock key so a fresh-install state
        // can be restored from the editor.
        public static void ResetLocalProgress()
        {
            PlayerPrefs.DeleteKey(HighestUnlockedKey);
            PlayerPrefs.Save();
        }
    }
}
