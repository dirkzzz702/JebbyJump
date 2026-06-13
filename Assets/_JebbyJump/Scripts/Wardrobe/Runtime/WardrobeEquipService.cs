namespace JebbyJump.Wardrobe
{
    public enum WardrobeEquipResult
    {
        Success = 0,
        AlreadyEquipped = 1,
        UnknownOutfit = 2,
        Locked = 3,
    }

    // The single validated equip path for all runtime equip requests (normal
    // Wardrobe Equip + the P16 ceremony Equip Now). Validates unlock
    // eligibility, writes through WardrobeStore (the persistence primitive),
    // and publishes a local change event on success so active player visuals
    // can re-sync. Pure/engine-light: no analytics (callers emit their own,
    // with the right source), no Star/acknowledgement mutation.
    public static class WardrobeEquipService
    {
        public static WardrobeEquipResult TryEquip(string outfitId, int totalStars)
        {
            var def = WardrobeCatalog.GetById(outfitId);
            if (def == null) return WardrobeEquipResult.UnknownOutfit;
            if (!WardrobeUnlockService.IsUnlocked(def, totalStars))
                return WardrobeEquipResult.Locked;

            string previous = WardrobeStore.GetEquippedOutfitId();
            if (previous == def.Id) return WardrobeEquipResult.AlreadyEquipped;

            WardrobeStore.SetEquippedOutfitId(def.Id);
            WardrobeAppearanceEvents.Publish(
                new WardrobeEquippedOutfitChanged(previous, def.Id));
            return WardrobeEquipResult.Success;
        }
    }
}
