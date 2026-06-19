using System.Text;

namespace JebbyJump.Release
{
    public static class ReleasePreflightChecks
    {
        public const string Identity = "identity";
        public const string Version = "version";
        public const string Scenes = "scenes.contract";
        public const string SceneNoTest = "scenes.no_test_scene";
        public const string SceneNoDup = "scenes.no_duplicates";
        public const string InputHandler = "input.new_system_only";
        public const string Orientation = "orientation.landscape_only";
        public const string Backend = "android.backend_il2cpp";
        public const string Architecture = "android.architecture_arm64";
        public const string DevFlag = "build.development_flag";
        public const string Packages = "packages.classified";
    }

    // Pure preflight: validates a gathered snapshot against the immutable scene
    // contract + approved identity/target invariants. NO file IO / editor calls
    // (the editor wrapper supplies the snapshot and does read-only existence
    // checks). Fed synthetic snapshots in tests; never mutates global state.
    public static class ReleasePreflightCore
    {
        public static ReleasePreflightResult Evaluate(ReleaseConfigSnapshot s, string[] contract)
        {
            var r = new ReleasePreflightResult();
            if (s == null) { r.Add(ReleaseCheckResult.Error("snapshot", "null snapshot")); return r; }

            // Identity (must match approved exactly).
            if (string.IsNullOrEmpty(s.CompanyName) || string.IsNullOrEmpty(s.ApplicationIdentifier)
                || s.CompanyName != ReleaseIdentity.CompanyName
                || s.ApplicationIdentifier != ReleaseIdentity.ApplicationIdentifier
                || s.ProductName != ReleaseIdentity.ProductName)
                r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.Identity,
                    $"identity drift: company='{s.CompanyName}' id='{s.ApplicationIdentifier}' product='{s.ProductName}' "
                    + $"(expected '{ReleaseIdentity.CompanyName}'/'{ReleaseIdentity.ApplicationIdentifier}'/'{ReleaseIdentity.ProductName}')"));
            else
                r.Add(ReleaseCheckResult.Info(ReleasePreflightChecks.Identity, "identity matches approved"));

            // Version.
            if (string.IsNullOrWhiteSpace(s.BundleVersion))
                r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.Version, "bundleVersion is empty"));
            else if (s.AndroidVersionCode < 1)
                r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.Version, "AndroidBundleVersionCode < 1"));
            else
                r.Add(ReleaseCheckResult.Info(ReleasePreflightChecks.Version, $"version {s.BundleVersion} (code {s.AndroidVersionCode})"));

            // Scene list == contract (order matters).
            if (!SequenceEqual(s.EnabledScenePaths, contract))
                r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.Scenes,
                    $"enabled scenes != contract. got=[{Join(s.EnabledScenePaths)}] expected=[{Join(contract)}]"));
            else
                r.Add(ReleaseCheckResult.Info(ReleasePreflightChecks.Scenes, "scene list matches the immutable contract"));

            if (HasDuplicate(s.EnabledScenePaths))
                r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.SceneNoDup, "duplicate scene in the build list"));

            foreach (var p in s.EnabledScenePaths)
                if (LooksLikeTestScene(p))
                {
                    r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.SceneNoTest, $"test/sample scene enabled: {p}"));
                    break;
                }

            // Input handling.
            if (s.ActiveInputHandler != ReleaseTargetInvariants.NewInputSystemOnly)
                r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.InputHandler,
                    $"activeInputHandler={s.ActiveInputHandler}, expected {ReleaseTargetInvariants.NewInputSystemOnly} (new Input System only)"));
            else
                r.Add(ReleaseCheckResult.Info(ReleasePreflightChecks.InputHandler, "new Input System only"));

            // Orientation.
            if (!s.LandscapeOnly)
                r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.Orientation, "not landscape-only"));
            else
                r.Add(ReleaseCheckResult.Info(ReleasePreflightChecks.Orientation, "landscape-only"));

            // Android backend / architecture.
            if (s.AndroidScriptingBackend != ReleaseTargetInvariants.IL2CPP)
                r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.Backend,
                    $"Android backend={s.AndroidScriptingBackend}, expected IL2CPP({ReleaseTargetInvariants.IL2CPP})"));
            else
                r.Add(ReleaseCheckResult.Info(ReleasePreflightChecks.Backend, "Android IL2CPP"));

            if (s.AndroidArchitectures != ReleaseTargetInvariants.ARM64)
                r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.Architecture,
                    $"Android architectures={s.AndroidArchitectures}, expected ARM64({ReleaseTargetInvariants.ARM64})"));
            else
                r.Add(ReleaseCheckResult.Info(ReleasePreflightChecks.Architecture, "Android ARM64"));

            // Development flag (builder forces release; warn only).
            if (s.DevelopmentBuild)
                r.Add(ReleaseCheckResult.Warn(ReleasePreflightChecks.DevFlag,
                    "development build flag is set in EditorUserBuildSettings (the release builder forces it off)"));

            // Packages (explicit classification; only Unknown/UnexpectedRuntimeSdk fail).
            foreach (var id in s.PackageIds)
            {
                var info = ReleasePackageClassification.Classify(id);
                if (ReleasePackageClassification.IsHardFail(info.Category))
                    r.Add(ReleaseCheckResult.Error(ReleasePreflightChecks.Packages,
                        $"unexpected/unknown package: {id} ({info.Category}: {info.Reason})"));
                else if (info.Category == ReleasePackageCategory.ApprovedPresentUnused)
                    r.Add(ReleaseCheckResult.Warn(ReleasePreflightChecks.Packages, $"{id}: {info.Reason}"));
            }

            return r;
        }

        private static bool SequenceEqual(string[] a, string[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        private static bool HasDuplicate(string[] a)
        {
            if (a == null) return false;
            for (int i = 0; i < a.Length; i++)
                for (int j = i + 1; j < a.Length; j++)
                    if (a[i] == a[j]) return true;
            return false;
        }

        private static bool LooksLikeTestScene(string path)
        {
            if (string.IsNullOrEmpty(path)) return true;
            return path.Contains("SampleScene")
                || path.Contains("InitTestScene")
                || path.Contains("/Tests/")
                || path.Contains("SceneTemplate");
        }

        private static string Join(string[] a)
        {
            if (a == null) return "";
            var sb = new StringBuilder();
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(a[i]);
            }
            return sb.ToString();
        }
    }
}
