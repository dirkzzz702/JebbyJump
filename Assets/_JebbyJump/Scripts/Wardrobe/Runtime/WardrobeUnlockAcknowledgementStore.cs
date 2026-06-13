using UnityEngine;

namespace JebbyJump.Wardrobe
{
    // Local PlayerPrefs record of which outfit UNLOCK CEREMONIES the player
    // has seen/dismissed. This is NOT ownership: ownership stays derived from
    // total Stars via WardrobeUnlockService. Acknowledgement never unlocks an
    // outfit, never consumes Stars, and is kept separate from WardrobeStore.
    //
    // Per-outfit key: jebby.wardrobe.unlockAcknowledged.<outfitId>
    //
    // The AlwaysUnlocked/default outfit is treated as already acknowledged and
    // never gets a key. null / empty / unknown ids are ignored on read+write.
    public static class WardrobeUnlockAcknowledgementStore
    {
        // Same literal as before; centralized in WardrobePersistenceKeys.
        private const string KeyPrefix =
            WardrobePersistenceKeys.UnlockAcknowledgementPrefix;

        public static bool IsAcknowledged(string outfitId)
        {
            var item = WardrobeCatalog.GetById(outfitId);
            if (item == null) return false;        // null / empty / unknown
            if (item.AlwaysUnlocked) return true;  // default = already seen
            return PlayerPrefs.GetInt(KeyPrefix + outfitId, 0) == 1;
        }

        // Ignores null/empty/unknown ids and the AlwaysUnlocked default (no
        // key is ever created for it).
        public static void MarkAcknowledged(string outfitId)
        {
            var item = WardrobeCatalog.GetById(outfitId);
            if (item == null || item.AlwaysUnlocked) return;
            PlayerPrefs.SetInt(KeyPrefix + outfitId, 1);
            PlayerPrefs.Save();
        }

        public static void ClearAcknowledged(string outfitId)
        {
            if (string.IsNullOrEmpty(outfitId)) return;
            PlayerPrefs.DeleteKey(KeyPrefix + outfitId);
            PlayerPrefs.Save();
        }

        // Clears every per-outfit acknowledgement key (catalog-driven).
        public static void ResetAll()
        {
            foreach (var o in WardrobeCatalog.Outfits)
                PlayerPrefs.DeleteKey(KeyPrefix + o.Id);
            PlayerPrefs.Save();
        }
    }
}
