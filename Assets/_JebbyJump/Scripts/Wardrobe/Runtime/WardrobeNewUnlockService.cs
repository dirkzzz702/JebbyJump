using System;
using System.Collections.Generic;

namespace JebbyJump.Wardrobe
{
    // Pure classifier: which outfits are currently unlocked by total Stars but
    // have NOT yet had their unlock ceremony acknowledged. Returns them in
    // WardrobeCatalog order; excludes the AlwaysUnlocked default; excludes
    // locked and acknowledged outfits. No PlayerPrefs writes, no Star or
    // WardrobeStore mutation - acknowledgement is read through the predicate.
    public static class WardrobeNewUnlockService
    {
        public static IReadOnlyList<CosmeticItemDefinition> GetNewlyUnlocked(
            int totalStars, Func<string, bool> isAcknowledged)
        {
            var result = new List<CosmeticItemDefinition>();
            foreach (var def in WardrobeCatalog.Outfits)
            {
                if (def.AlwaysUnlocked) continue;
                if (!WardrobeUnlockService.IsUnlocked(def, totalStars)) continue;
                if (isAcknowledged != null && isAcknowledged(def.Id)) continue;
                result.Add(def);
            }
            return result;
        }
    }
}
