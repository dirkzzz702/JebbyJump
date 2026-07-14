using System.Collections.Generic;

namespace JebbyJump.Wardrobe
{
    // The fixed runtime outfit catalog: 7 outfits total - the default plus
    // six unlockable variants. Ids are stable snake_case wire keys. Star
    // thresholds are PLACEHOLDERS (pending P4B + level count + balance
    // review) and are stored here as data only - not tuned. Outfits are
    // cosmetic-only with no gameplay effect; Jebby remains one playable
    // character.
    //
    // The default outfit keeps the stable id "classic_color_knight" (save
    // keys / normalization depend on it) but has NO art of its own: the base
    // JebbyAnimator clips, the Jebby prefab's initial sprite, and the preview
    // library's default entry all point at a VARIANT's asset set (currently
    // RookiePage - hence the display name). Retarget the default look with
    // "Jebby Jump/Wardrobe/Set Default Look"; the former separate rookie_page
    // catalog entry was folded into the default because its art WAS the
    // default look.
    public static class WardrobeCatalog
    {
        public const string DefaultOutfitId = "classic_color_knight";

        // Ordered by ascending Star threshold (panel row order). All
        // thresholds remain PLACEHOLDERS.
        private static readonly CosmeticItemDefinition[] _outfits =
        {
            new CosmeticItemDefinition(
                "classic_color_knight", "Rookie Page",
                CosmeticCategory.Outfit,
                "Default Jebby - cheerful starter theme.",
                0, true),
            new CosmeticItemDefinition(
                "forest_cavalier", "Forest Cavalier",
                CosmeticCategory.Outfit,
                "Woodland theme.",
                8, false),   // PLACEHOLDER threshold
            new CosmeticItemDefinition(
                "crimson_hero", "Crimson Hero",
                CosmeticCategory.Outfit,
                "Bold crimson theme.",
                12, false),  // PLACEHOLDER threshold
            new CosmeticItemDefinition(
                "sunshine_knight", "Sunshine Knight",
                CosmeticCategory.Outfit,
                "Sunny theme.",
                15, false),  // PLACEHOLDER threshold
            new CosmeticItemDefinition(
                "aqua_knight", "Aqua Knight",
                CosmeticCategory.Outfit,
                "Water theme.",
                22, false),  // PLACEHOLDER threshold
            new CosmeticItemDefinition(
                "pastel_prince", "Pastel Prince",
                CosmeticCategory.Outfit,
                "Soft pastel theme.",
                26, false),  // PLACEHOLDER threshold
            new CosmeticItemDefinition(
                "silver_dreamer", "Silver Dreamer",
                CosmeticCategory.Outfit,
                "Night / dream theme.",
                30, false),  // PLACEHOLDER threshold
        };

        public static IReadOnlyList<CosmeticItemDefinition> Outfits => _outfits;

        public static CosmeticItemDefinition GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            for (int i = 0; i < _outfits.Length; i++)
                if (_outfits[i].Id == id) return _outfits[i];
            return null;
        }

        public static bool Exists(string id) => GetById(id) != null;
    }
}
