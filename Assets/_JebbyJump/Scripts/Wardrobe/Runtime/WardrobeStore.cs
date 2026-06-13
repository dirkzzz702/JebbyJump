using UnityEngine;

namespace JebbyJump.Wardrobe
{
    // Local, PlayerPrefs-backed wardrobe state. P9 persists ONLY the
    // equipped outfit id; ownership/unlock is derived from Stars at display
    // time (no owned-id list, no migration, no wallet). String id only.
    //
    // PlayerPrefs key:
    //   jebby.wardrobe.equippedOutfit  (string outfit id)
    public static class WardrobeStore
    {
        // Same literal as before; centralized in WardrobePersistenceKeys.
        private const string EquippedKey = WardrobePersistenceKeys.EquippedOutfit;

        // Returns the stored equipped id, or the default if missing/unknown.
        // Does NOT validate unlock state (that is the service layer's job
        // via NormalizeEquippedId, which needs the player's star total).
        public static string GetEquippedOutfitId()
        {
            string id = PlayerPrefs.GetString(
                EquippedKey, WardrobeCatalog.DefaultOutfitId);
            return WardrobeCatalog.Exists(id)
                ? id
                : WardrobeCatalog.DefaultOutfitId;
        }

        // Stores a known outfit id; unknown/null falls back to default.
        // Returns the value actually stored.
        public static string SetEquippedOutfitId(string outfitId)
        {
            string id = WardrobeCatalog.Exists(outfitId)
                ? outfitId
                : WardrobeCatalog.DefaultOutfitId;
            PlayerPrefs.SetString(EquippedKey, id);
            PlayerPrefs.Save();
            return id;
        }

        // Dev-only: clears the equipped key (returns to default).
        public static void Reset()
        {
            PlayerPrefs.DeleteKey(EquippedKey);
            PlayerPrefs.Save();
        }
    }
}
