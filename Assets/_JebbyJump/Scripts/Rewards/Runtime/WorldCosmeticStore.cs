using System.Collections.Generic;
using UnityEngine;

namespace JebbyJump.Rewards
{
    // Local PlayerPrefs record of which world-mastery cosmetics are unlocked
    // (WorldExpansion100, phase P34G, reward model C). Unlock is set-once and
    // never consumes Stars. Mirrors WorldGemStore. Non-spendable; does not
    // affect progression. The existing star-threshold wardrobe outfits are a
    // separate system and are untouched.
    //
    // Per-cosmetic key: jebby.rewards.worldCosmetic.<cosmeticId>
    public static class WorldCosmeticStore
    {
        private const string KeyPrefix = "jebby.rewards.worldCosmetic.";

        public static bool IsUnlocked(string cosmeticId)
        {
            if (string.IsNullOrEmpty(cosmeticId)) return false;
            return PlayerPrefs.GetInt(KeyPrefix + cosmeticId, 0) == 1;
        }

        // Unlocks if not already. Returns true only when NEWLY unlocked.
        public static bool TryUnlock(string cosmeticId)
        {
            if (string.IsNullOrEmpty(cosmeticId)) return false;
            if (IsUnlocked(cosmeticId)) return false;
            PlayerPrefs.SetInt(KeyPrefix + cosmeticId, 1);
            PlayerPrefs.Save();
            return true;
        }

        public static int TotalUnlocked(IReadOnlyList<string> cosmeticIds)
        {
            if (cosmeticIds == null) return 0;
            int total = 0;
            for (int i = 0; i < cosmeticIds.Count; i++)
                if (IsUnlocked(cosmeticIds[i])) total++;
            return total;
        }

        public static void Clear(string cosmeticId)
        {
            if (string.IsNullOrEmpty(cosmeticId)) return;
            PlayerPrefs.DeleteKey(KeyPrefix + cosmeticId);
            PlayerPrefs.Save();
        }

        public static void ResetAll(IReadOnlyList<string> cosmeticIds)
        {
            if (cosmeticIds == null) return;
            for (int i = 0; i < cosmeticIds.Count; i++)
                if (!string.IsNullOrEmpty(cosmeticIds[i]))
                    PlayerPrefs.DeleteKey(KeyPrefix + cosmeticIds[i]);
            PlayerPrefs.Save();
        }
    }
}
