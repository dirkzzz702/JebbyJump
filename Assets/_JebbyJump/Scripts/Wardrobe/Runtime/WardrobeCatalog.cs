using System.Collections.Generic;

namespace JebbyJump.Wardrobe
{
    // The fixed P9 outfit catalog (the P8 initial five). Ids are stable
    // snake_case wire keys; display names match P8. Star thresholds are
    // P8 PLACEHOLDERS (pending P4B + level count + balance review) and
    // are stored here as data only - not tuned. Outfits are cosmetic-only
    // with no gameplay effect.
    //
    // "Classic Color Knight" maps to the Art Bible / GDD "Classic Cavalier"
    // default identity.
    public static class WardrobeCatalog
    {
        public const string DefaultOutfitId = "classic_color_knight";

        private static readonly CosmeticItemDefinition[] _outfits =
        {
            new CosmeticItemDefinition(
                "classic_color_knight", "Classic Color Knight",
                CosmeticCategory.Outfit,
                "Default Jebby the Color Knight.",
                0, true),
            new CosmeticItemDefinition(
                "forest_cavalier", "Forest Cavalier",
                CosmeticCategory.Outfit,
                "Woodland theme.",
                8, false),   // PLACEHOLDER threshold
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
