namespace JebbyJump.Wardrobe
{
    public enum WardrobeItemState
    {
        Locked = 0,
        Unlocked = 1,
        Equipped = 2,
    }

    // Pure Stars-gated unlock logic. Total stars are passed in (never read
    // from StarRewardStore here) so this stays engine-free and testable.
    // Stars are NOT consumed - thresholds only gate. No PlayerPrefs writes,
    // no gameplay effects.
    public static class WardrobeUnlockService
    {
        public static bool IsUnlocked(CosmeticItemDefinition item, int totalStars)
        {
            if (item == null) return false;
            if (item.AlwaysUnlocked || item.RequiredStars <= 0) return true;
            return totalStars >= item.RequiredStars;
        }

        public static WardrobeItemState GetState(
            CosmeticItemDefinition item, string equippedId, int totalStars)
        {
            if (!IsUnlocked(item, totalStars))
                return WardrobeItemState.Locked;
            return item.Id == equippedId
                ? WardrobeItemState.Equipped
                : WardrobeItemState.Unlocked;
        }

        // Returns storedId if it maps to a known, currently-unlocked outfit;
        // otherwise the default outfit id. Used to recover from an unknown
        // or now-locked stored equip without touching the store/Stars.
        public static string NormalizeEquippedId(string storedId, int totalStars)
        {
            var item = WardrobeCatalog.GetById(storedId);
            if (item != null && IsUnlocked(item, totalStars))
                return storedId;
            return WardrobeCatalog.DefaultOutfitId;
        }
    }
}
