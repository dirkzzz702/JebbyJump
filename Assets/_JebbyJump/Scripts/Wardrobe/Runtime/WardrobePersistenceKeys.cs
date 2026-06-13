namespace JebbyJump.Wardrobe
{
    // Central registry of wardrobe PlayerPrefs keys. These are stable wire
    // keys - do NOT rename. Shared by WardrobeStore,
    // WardrobeUnlockAcknowledgementStore, and WardrobePersistenceMigrator so
    // the literals never drift. (Values match the originals from P9/P16.)
    public static class WardrobePersistenceKeys
    {
        public const string EquippedOutfit = "jebby.wardrobe.equippedOutfit";
        public const string SchemaVersion  = "jebby.wardrobe.schemaVersion";
        public const string UnlockAcknowledgementPrefix =
            "jebby.wardrobe.unlockAcknowledged.";
    }
}
