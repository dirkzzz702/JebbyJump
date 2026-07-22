namespace JebbyJump.Rewards
{
    // Maps each world to the themed cosmetic it awards on world mastery
    // (WorldExpansion100, phase P34G, reward model C). Ids match the
    // cosmetic_<id>_01.png art filenames in each world's UI folder. This is the
    // reward RECORD; surfacing/equipping these in the wardrobe UI is a separate
    // increment (it depends on the wardrobe outfit system + final art).
    public static class WorldCosmeticCatalog
    {
        // Index = worldNumber-1. Stable snake_case wire ids.
        private static readonly string[] _ids =
        {
            "cloudpuff_cape",       // W01 Cloud Meadow
            "leafcrown_hood",       // W02 Enchanted Forest
            "geode_pauldrons",      // W03 Crystal Caves
            "sunwrap_scarf",        // W04 Sunshine Desert
            "tidal_fin_cloak",      // W05 Ocean Sky
            "frosting_crown",       // W06 Candy Cloud Kingdom
            "gearwork_goggles",     // W07 Clockwork Heights
            "starlace_veil",        // W08 Moonlit Dreamscape
            "emberguard_mantle",    // W09 Stormfire Volcano
            "radiant_heirloom_set", // W10 Rainbow Tower Castle
        };

        private static readonly string[] _names =
        {
            "Cloudpuff Cape", "Leafcrown Hood", "Geode Pauldrons",
            "Sunwrap Scarf", "Tidal Fin Cloak", "Frosting Crown",
            "Gearwork Goggles", "Starlace Veil", "Emberguard Mantle",
            "Radiant Heirloom Set",
        };

        public static int Count => _ids.Length;
        public static System.Collections.Generic.IReadOnlyList<string> AllIds => _ids;

        // 1-based world number -> cosmetic id / display name. Empty when invalid.
        public static string CosmeticIdForWorld(int worldNumber)
            => IsValid(worldNumber) ? _ids[worldNumber - 1] : string.Empty;

        public static string DisplayNameForWorld(int worldNumber)
            => IsValid(worldNumber) ? _names[worldNumber - 1] : string.Empty;

        private static bool IsValid(int worldNumber)
            => worldNumber >= 1 && worldNumber <= _ids.Length;
    }
}
