namespace JebbyJump.Release
{
    // Read-only asset presence gathered by the editor preflight, evaluated by this
    // pure helper so the required-asset rules are unit-testable without touching
    // the AssetDatabase.
    public struct ReleaseAssetPresence
    {
        public bool LevelCatalog;
        public bool JebbyPrefab;
        public bool JebbyAnimator;
        public bool OutfitVisualLibrary;
        public bool WardrobePreviewLibrary;
        public bool InputActions;
        public bool PlatformPrefab;
        public bool TmpFont;
        public bool TmpDigits;
        public int OverrideControllers;
    }

    public static class ReleaseAssetChecks
    {
        public static void Evaluate(ReleaseAssetPresence p, ReleasePreflightResult r)
        {
            Require(r, "asset.level_catalog", p.LevelCatalog, "LevelCatalog");
            Require(r, "asset.jebby_prefab", p.JebbyPrefab, "Jebby.prefab");
            Require(r, "asset.jebby_animator", p.JebbyAnimator, "Jebby animator controller");
            Require(r, "asset.outfit_library", p.OutfitVisualLibrary, "OutfitVisualLibrary");
            Require(r, "asset.preview_library", p.WardrobePreviewLibrary, "WardrobePreviewLibrary");
            Require(r, "asset.input_actions", p.InputActions, "InputActionAsset");
            Require(r, "asset.platform_prefab", p.PlatformPrefab, "Platform.prefab");

            if (!p.TmpFont)
                r.Add(ReleaseCheckResult.Warn("tmp.font", "no default TMP font asset configured"));
            else if (!p.TmpDigits)
                r.Add(ReleaseCheckResult.Error("tmp.digits",
                    "default TMP font missing digit glyphs 0-9 (memory cues 1-6)"));
            else
                r.Add(ReleaseCheckResult.Info("tmp.digits", "default TMP font supports digits 0-9"));

            // Outfit override controllers are intentionally not required (prototype
            // art is not final-certified) - reported for visibility only.
            r.Add(ReleaseCheckResult.Info("asset.outfit_overrides",
                $"outfit override controllers present: {p.OverrideControllers} (art not final-certified)"));
        }

        private static void Require(ReleasePreflightResult r, string id, bool present, string label)
            => r.Add(present
                ? ReleaseCheckResult.Info(id, label + " present")
                : ReleaseCheckResult.Error(id, "required asset missing: " + label));
    }
}
