using System.Collections.Generic;

namespace JebbyJump.Release
{
    public enum ReleasePackageCategory
    {
        BuiltInModule,        // com.unity.modules.* — always present
        ApprovedRuntime,      // intentionally used at runtime
        ApprovedPresentUnused,// present but no runtime code refs (flagged, not fatal)
        EditorOnly,           // editor/IDE/test tooling, not in the player
        EditorOnlyService,    // a Unity service used only at build/editor time
        TargetIrrelevant,     // a platform package irrelevant to the Android target
        UnexpectedRuntimeSdk, // an unapproved runtime SDK (HARD FAIL)
        Unknown,              // unclassified (HARD FAIL — forces explicit review)
    }

    // Explicit per-package classification (fix #8): never judged by substring
    // (e.g. names containing "services"/"cloud"/"purchasing"). Only Unknown or
    // UnexpectedRuntimeSdk are hard failures; everything else is reported with a
    // reason. P23 never adds or removes packages.
    public static class ReleasePackageClassification
    {
        public readonly struct Info
        {
            public readonly ReleasePackageCategory Category;
            public readonly string Reason;
            public Info(ReleasePackageCategory category, string reason)
            {
                Category = category;
                Reason = reason;
            }
        }

        private const string ModulePrefix = "com.unity.modules.";

        private static readonly Dictionary<string, Info> Map = new Dictionary<string, Info>
        {
            { "com.coplaydev.coplay", new Info(ReleasePackageCategory.EditorOnly, "AI editor plugin; not compiled into the player") },
            { "com.unity.2d.animation", new Info(ReleasePackageCategory.ApprovedRuntime, "2D sprite animation pipeline") },
            { "com.unity.2d.aseprite", new Info(ReleasePackageCategory.EditorOnly, "Aseprite importer (asset import only)") },
            { "com.unity.2d.psdimporter", new Info(ReleasePackageCategory.EditorOnly, "PSD importer (asset import only)") },
            { "com.unity.2d.sprite", new Info(ReleasePackageCategory.ApprovedRuntime, "2D sprite support") },
            { "com.unity.2d.spriteshape", new Info(ReleasePackageCategory.ApprovedRuntime, "2D sprite shape") },
            { "com.unity.2d.tilemap", new Info(ReleasePackageCategory.ApprovedRuntime, "2D tilemap") },
            { "com.unity.2d.tilemap.extras", new Info(ReleasePackageCategory.ApprovedRuntime, "2D tilemap extras") },
            { "com.unity.2d.tooling", new Info(ReleasePackageCategory.EditorOnly, "2D editor tooling") },
            { "com.unity.cinemachine", new Info(ReleasePackageCategory.ApprovedRuntime, "Camera framework") },
            { "com.unity.collab-proxy", new Info(ReleasePackageCategory.EditorOnly, "Version control UI") },
            { "com.unity.ide.rider", new Info(ReleasePackageCategory.EditorOnly, "Rider IDE integration") },
            { "com.unity.ide.visualstudio", new Info(ReleasePackageCategory.EditorOnly, "Visual Studio IDE integration") },
            { "com.unity.inputsystem", new Info(ReleasePackageCategory.ApprovedRuntime, "New Input System (the game's input)") },
            { "com.unity.microsoft.gdk", new Info(ReleasePackageCategory.TargetIrrelevant, "Microsoft GDK (Xbox/PC); irrelevant to Android") },
            { "com.unity.microsoft.gdk.tools", new Info(ReleasePackageCategory.TargetIrrelevant, "Microsoft GDK tools; irrelevant to Android") },
            { "com.unity.multiplayer.center", new Info(ReleasePackageCategory.EditorOnly, "Multiplayer setup center (editor only)") },
            { "com.unity.purchasing", new Info(ReleasePackageCategory.ApprovedPresentUnused, "IAP package present but no runtime code references; not initialized") },
            { "com.unity.render-pipelines.universal", new Info(ReleasePackageCategory.ApprovedRuntime, "URP 2D render pipeline") },
            { "com.unity.services.cloud-build", new Info(ReleasePackageCategory.EditorOnlyService, "Unity Cloud Build service; build/editor-time only") },
            { "com.unity.test-framework", new Info(ReleasePackageCategory.EditorOnly, "Test framework; excluded from the player") },
            { "com.unity.timeline", new Info(ReleasePackageCategory.ApprovedRuntime, "Timeline") },
            { "com.unity.ugui", new Info(ReleasePackageCategory.ApprovedRuntime, "uGUI / TextMeshPro UI") },
        };

        public static Info Classify(string packageId)
        {
            if (string.IsNullOrEmpty(packageId))
                return new Info(ReleasePackageCategory.Unknown, "empty id");
            if (packageId.StartsWith(ModulePrefix))
                return new Info(ReleasePackageCategory.BuiltInModule, "built-in engine module");
            return Map.TryGetValue(packageId, out var info)
                ? info
                : new Info(ReleasePackageCategory.Unknown, "no explicit classification");
        }

        public static bool IsHardFail(ReleasePackageCategory c)
            => c == ReleasePackageCategory.Unknown
            || c == ReleasePackageCategory.UnexpectedRuntimeSdk;
    }
}
