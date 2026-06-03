using UnityEngine;

namespace JebbyJump.Rewards
{
    // Local, PlayerPrefs-backed mastery-star record. Per-level best stars
    // only; the total is summed on demand from the per-level keys (no
    // aggregate key, so there is no drift risk). Stars are a mastery
    // record, not a spendable/farmable currency: best-only, capped 0..3,
    // never decreasing, and they do not affect progression or rank.
    //
    // PlayerPrefs key:
    //   jebby.rewards.levelStars.<levelIndex>  (int 0..3)
    public static class StarRewardStore
    {
        private const string KeyPrefix = "jebby.rewards.levelStars.";
        private const int MaxStars = 3;

        public static int GetStars(int levelIndex)
        {
            if (levelIndex < 0) return 0;
            int stored = PlayerPrefs.GetInt(KeyPrefix + levelIndex, 0);
            return Mathf.Clamp(stored, 0, MaxStars);
        }

        // Stores stars only if higher than the current record. Clamps to
        // 0..3; negative index is a no-op. Returns the resulting stored
        // value (unchanged if not higher).
        public static int SetStarsIfHigher(int levelIndex, int stars)
        {
            if (levelIndex < 0) return 0;
            int clamped = Mathf.Clamp(stars, 0, MaxStars);
            int current = GetStars(levelIndex);
            if (clamped <= current) return current;
            PlayerPrefs.SetInt(KeyPrefix + levelIndex, clamped);
            PlayerPrefs.Save();
            return clamped;
        }

        // Sum of best stars across levels [0, levelCount). Cheap for the
        // current 10 (and a future 50) levels.
        public static int GetTotalStars(int levelCount)
        {
            if (levelCount <= 0) return 0;
            int total = 0;
            for (int i = 0; i < levelCount; i++)
                total += GetStars(i);
            return total;
        }

        // Dev-only: clears per-level star keys in [0, levelCount).
        public static void ResetAll(int levelCount)
        {
            if (levelCount <= 0) return;
            for (int i = 0; i < levelCount; i++)
                PlayerPrefs.DeleteKey(KeyPrefix + i);
            PlayerPrefs.Save();
        }
    }
}
