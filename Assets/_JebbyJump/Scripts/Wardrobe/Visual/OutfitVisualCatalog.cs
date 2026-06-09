namespace JebbyJump.Wardrobe.Visual
{
    // Resolves a wardrobe outfit id to a visual definition. Pure and
    // asset-free: no Resources, no Addressables, no filesystem paths, no
    // sprite refs. Every WardrobeCatalog outfit resolves; null/unknown ids
    // fall back to the default outfit.
    //
    // P11: all outfits resolve to a safe no-op definition (HasVisualOverride
    // =false, AnimatorControllerOverride=null) because no outfit art exists
    // yet. This is the seam where future art delivery (e.g. per-outfit
    // AnimatorOverrideControllers) would return real overrides without
    // changing PlayerOutfitVisualController.
    //
    // Deliberately unlock-agnostic: it does not consult Stars / unlock
    // state. Lock-aware normalization stays with the wardrobe panel
    // (WardrobeUnlockService.NormalizeEquippedId). In P11 every definition
    // is a no-op anyway, so a (hypothetically) locked-but-equipped id still
    // leaves Jebby visually unchanged.
    public static class OutfitVisualCatalog
    {
        // Resolves an outfit id to its visual definition. null/unknown ids
        // fall back to the default outfit (which always exists).
        public static OutfitVisualDefinition GetVisualForOutfit(string outfitId)
        {
            var item = WardrobeCatalog.GetById(outfitId)
                ?? WardrobeCatalog.GetById(WardrobeCatalog.DefaultOutfitId);

            // P11: no art yet -> safe no-op definition for every outfit.
            return new OutfitVisualDefinition(
                item.Id,
                item.DisplayName,
                hasVisualOverride: false,
                animatorControllerOverride: null);
        }

        // True only when the resolved definition carries a visual override.
        // Always false in P11 (no art).
        public static bool HasVisual(string outfitId)
            => GetVisualForOutfit(outfitId).HasVisualOverride;
    }
}
