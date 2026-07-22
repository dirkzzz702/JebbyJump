using System.Collections.Generic;
using UnityEngine;

namespace JebbyJump.Rewards
{
    // Local PlayerPrefs record of the per-world "World Gem" trophy, granted on
    // the FIRST clear of a world's finale (WorldExpansion100, phase P34G,
    // reward model C). A gem is a non-spendable collection marker: set-once,
    // never decreases, never consumed, and it does not affect progression or
    // Stars. Mirrors the StarRewardStore / acknowledgement pattern.
    //
    // Per-world key: jebby.rewards.worldGem.<worldId>   (worldId = "W01".."W10")
    //
    // This type intentionally has NO dependency on WorldMapping: the caller
    // decides which world's finale was cleared and passes the stable worldId.
    public static class WorldGemStore
    {
        private const string KeyPrefix = "jebby.rewards.worldGem.";

        public static bool IsGranted(string worldId)
        {
            if (string.IsNullOrEmpty(worldId)) return false;
            return PlayerPrefs.GetInt(KeyPrefix + worldId, 0) == 1;
        }

        // Grants the gem if not already held. Returns true only when NEWLY
        // granted (so a finale replay grants nothing). Idempotent; null/empty
        // ids are ignored.
        public static bool TryGrant(string worldId)
        {
            if (string.IsNullOrEmpty(worldId)) return false;
            if (IsGranted(worldId)) return false;
            PlayerPrefs.SetInt(KeyPrefix + worldId, 1);
            PlayerPrefs.Save();
            return true;
        }

        // Count of gems held across the given world ids.
        public static int TotalGranted(IReadOnlyList<string> worldIds)
        {
            if (worldIds == null) return 0;
            int total = 0;
            for (int i = 0; i < worldIds.Count; i++)
                if (IsGranted(worldIds[i])) total++;
            return total;
        }

        public static void Clear(string worldId)
        {
            if (string.IsNullOrEmpty(worldId)) return;
            PlayerPrefs.DeleteKey(KeyPrefix + worldId);
            PlayerPrefs.Save();
        }

        public static void ResetAll(IReadOnlyList<string> worldIds)
        {
            if (worldIds == null) return;
            for (int i = 0; i < worldIds.Count; i++)
                if (!string.IsNullOrEmpty(worldIds[i]))
                    PlayerPrefs.DeleteKey(KeyPrefix + worldIds[i]);
            PlayerPrefs.Save();
        }
    }
}
