using UnityEngine;

namespace JebbyJump.Level
{
    // PlayerPrefs-backed best-time store keyed per level. Returns NaN when no record.
    public static class BestTimeStore
    {
        private const string Prefix = "JebbyJump.BestTime.";

        public static float GetBest(string levelKey)
        {
            if (string.IsNullOrEmpty(levelKey)) return float.NaN;
            return PlayerPrefs.HasKey(Prefix + levelKey)
                ? PlayerPrefs.GetFloat(Prefix + levelKey)
                : float.NaN;
        }

        // Returns true if the new time beat the previous best (or there was none).
        public static bool TrySetBest(string levelKey, float seconds)
        {
            if (string.IsNullOrEmpty(levelKey) || seconds <= 0f) return false;
            float prev = GetBest(levelKey);
            if (float.IsNaN(prev) || seconds < prev)
            {
                PlayerPrefs.SetFloat(Prefix + levelKey, seconds);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }
    }
}
