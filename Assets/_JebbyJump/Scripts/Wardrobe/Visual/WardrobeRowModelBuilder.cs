using System.Collections.Generic;
using UnityEngine;

namespace JebbyJump.Wardrobe.Visual
{
    // Pure builder for the wardrobe panel's row view-models. Single source of
    // truth for per-row state/copy/preview so the panel MonoBehaviour only
    // renders. Engine-light (Sprite lookups via the preview library only);
    // no StarRewardStore/scene dependency - totalStars is passed in.
    public static class WardrobeRowModelBuilder
    {
        // Builds one row per WardrobeCatalog outfit, in catalog order.
        // equippedId is normalized here (an unknown/now-locked stored id maps
        // to the default), so callers may pass the raw stored id safely.
        // previews may be null (every row then has a null PreviewSprite).
        // isAcknowledged (optional) drives the "New" badge: null -> never New.
        public static IReadOnlyList<WardrobeOutfitRowModel> Build(
            string equippedId, int totalStars, WardrobePreviewLibrary previews,
            System.Func<string, bool> isAcknowledged = null)
        {
            string normalizedEquipped =
                WardrobeUnlockService.NormalizeEquippedId(equippedId, totalStars);

            var rows = new List<WardrobeOutfitRowModel>(
                WardrobeCatalog.Outfits.Count);
            foreach (var def in WardrobeCatalog.Outfits)
            {
                var state = WardrobeUnlockService.GetState(
                    def, normalizedEquipped, totalStars);
                bool unlocked = state != WardrobeItemState.Locked;
                bool equipped = state == WardrobeItemState.Equipped;

                Sprite preview = null;
                if (previews != null)
                    previews.TryGetPreview(def.Id, out preview);

                // "New" = unlocked, non-default, not yet acknowledged.
                bool isNew = unlocked && !def.AlwaysUnlocked
                    && isAcknowledged != null && !isAcknowledged(def.Id);

                rows.Add(new WardrobeOutfitRowModel(
                    def.Id, def.DisplayName, def.RequiredStars,
                    unlocked, equipped, preview, StateText(def, state), isNew));
            }
            return rows;
        }

        // Exact wardrobe-panel copy (unchanged from P9):
        // Equipped / Unlocked / "Locked (N Stars)".
        public static string StateText(
            CosmeticItemDefinition def, WardrobeItemState state)
        {
            switch (state)
            {
                case WardrobeItemState.Equipped: return "Equipped";
                case WardrobeItemState.Unlocked: return "Unlocked";
                default: return "Locked (" + def.RequiredStars + " Stars)";
            }
        }
    }
}
