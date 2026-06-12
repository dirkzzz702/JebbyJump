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

        // Library-aware resolution: same id normalization as above, then the
        // serialized library supplies the per-outfit override controller (if
        // any). Outfits without a library entry stay no-op (default visuals).
        public static OutfitVisualDefinition GetVisualForOutfit(
            string outfitId, OutfitVisualLibrary library)
        {
            var def = GetVisualForOutfit(outfitId);
            if (library != null
                && library.TryGetOverride(def.OutfitId, out var controller))
            {
                return new OutfitVisualDefinition(
                    def.OutfitId, def.DisplayName,
                    hasVisualOverride: true,
                    animatorControllerOverride: controller);
            }
            return def;
        }

        // True only when the resolved definition carries a visual override.
        // Always false without a library (the static layer is asset-free).
        public static bool HasVisual(string outfitId)
            => GetVisualForOutfit(outfitId).HasVisualOverride;
    }
}
