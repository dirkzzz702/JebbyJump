using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace JebbyJump.Release
{
    [Serializable]
    public sealed class StoreComplianceReport
    {
        public string Timestamp = "";
        public string GitCommit = "";
        public string Note =
            "Compliance scaffolding audit — NOT a store-readiness certification. "
            + "Play policies change; verify every dated assumption against current policy.";
        public StoreComplianceSnapshot Snapshot;
        public StoreComplianceFinding[] Findings = Array.Empty<StoreComplianceFinding>();
        public PlayPolicyAssumption[] PolicyAssumptions = Array.Empty<PlayPolicyAssumption>();
        public int FlagCount;
    }

    // Read-only Play store-compliance audit (P26-Store, corrections #4/#5/#8). Gathers
    // PlayerSettings + installed-platform data, runs the pure StoreCompliancePolicy, and
    // writes a dated JSON+MD report under the ignored Builds/P26 path. Mutates NOTHING.
    public static class StoreComplianceAuditTool
    {
        private const string OutputRootRel = "Builds/P26";

        [MenuItem("Jebby Jump/Release/Store Compliance Audit")]
        public static void RunMenu()
        {
            var report = Run();
            Debug.Log($"[StoreCompliance] flags={report.FlagCount} "
                + $"configuredTargetSdk={report.Snapshot.ConfiguredTargetSdk} "
                + $"resolvedTargetSdk={report.Snapshot.ResolvedTargetSdk}");
        }

        public static StoreComplianceReport Run()
        {
            var snap = GatherSnapshot();
            var findings = StoreCompliancePolicy.Evaluate(snap);
            var report = new StoreComplianceReport
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                GitCommit = ShortGitCommit(),
                Snapshot = snap,
                Findings = findings,
                PolicyAssumptions = StoreCompliancePolicy.DefaultAssumptions(),
                FlagCount = StoreCompliancePolicy.FlagCount(findings),
            };
            Write(report);
            return report;
        }

        public static StoreComplianceSnapshot GatherSnapshot()
        {
            var (adaptive, legacy) = DetectAndroidIcons();
            return new StoreComplianceSnapshot
            {
                ConfiguredTargetSdk = (int)PlayerSettings.Android.targetSdkVersion,
                ResolvedTargetSdk = AndroidArtifactInspector.HighestInstalledPlatformApi(),
                MinSdk = (int)PlayerSettings.Android.minSdkVersion,
                LandscapeOnly = ComputeLandscapeOnly(),
                AndroidVersionCode = PlayerSettings.Android.bundleVersionCode,
                ApplicationId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android),
                HasAdaptiveIcon = adaptive,
                HasLegacyIcon = legacy,
            };
        }

        private static bool ComputeLandscapeOnly()
        {
            var d = PlayerSettings.defaultInterfaceOrientation;
            if (d == UIOrientation.LandscapeLeft || d == UIOrientation.LandscapeRight) return true;
            if (d == UIOrientation.AutoRotation)
                return !PlayerSettings.allowedAutorotateToPortrait
                    && !PlayerSettings.allowedAutorotateToPortraitUpsideDown
                    && (PlayerSettings.allowedAutorotateToLandscapeLeft
                        || PlayerSettings.allowedAutorotateToLandscapeRight);
            return false;
        }

        // Reflection-based icon detection: compile-safe + free of obsolete-API warnings
        // across PlayerSettings icon API revisions.
        private static (bool adaptive, bool legacy) DetectAndroidIcons()
        {
            bool adaptive = false, legacy = false;
            try
            {
                var m = typeof(PlayerSettings).GetMethod("GetIconsForTargetGroup",
                    new[] { typeof(BuildTargetGroup) });
                if (m != null && m.Invoke(null, new object[] { BuildTargetGroup.Android }) is Texture2D[] icons)
                    foreach (var t in icons) if (t != null) { legacy = true; break; }
            }
            catch { }
            try
            {
                // AndroidPlatformIconKind lives in the Android extensions assembly, which
                // this asmdef does not reference - resolve the type + method reflectively.
                var kindType = FindType("UnityEditor.AndroidPlatformIconKind");
                if (kindType != null)
                {
                    object adaptiveKind = Enum.Parse(kindType, "Adaptive");
                    var m = typeof(PlayerSettings).GetMethod("GetPlatformIcons",
                        new[] { typeof(BuildTargetGroup), kindType });
                    if (m != null
                        && m.Invoke(null, new object[] { BuildTargetGroup.Android, adaptiveKind }) is Array arr)
                        foreach (var pi in arr)
                        {
                            var gt = pi?.GetType().GetMethod("GetTextures");
                            if (gt?.Invoke(pi, null) is Texture2D[] layers)
                                foreach (var l in layers) if (l != null) { adaptive = true; break; }
                            if (adaptive) break;
                        }
                }
            }
            catch { }
            return (adaptive, legacy);
        }

        private static Type FindType(string fullName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try { var t = asm.GetType(fullName); if (t != null) return t; }
                catch { }
            }
            return null;
        }

        // ---- output ----

        private static void Write(StoreComplianceReport r)
        {
            string dir = Path.GetFullPath(Path.Combine(
                Application.dataPath, "..", OutputRootRel, r.GitCommit));
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "store-compliance.json"), JsonUtility.ToJson(r, true));
            File.WriteAllText(Path.Combine(dir, "store-compliance.md"), ToMarkdown(r));
        }

        public static string ToMarkdown(StoreComplianceReport r)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Jebby Jump — Play Store Compliance Audit (P26)");
            sb.AppendLine();
            sb.AppendLine($"- Timestamp: {r.Timestamp}  |  Git: {r.GitCommit}");
            sb.AppendLine($"- {r.Note}");
            sb.AppendLine($"- FLAGs: {r.FlagCount}");
            sb.AppendLine();
            sb.AppendLine("## Snapshot");
            sb.AppendLine($"- Configured target SDK: {(r.Snapshot.ConfiguredTargetSdk == 0 ? "Automatic (0)" : r.Snapshot.ConfiguredTargetSdk.ToString())}");
            sb.AppendLine($"- Resolved target SDK (highest installed platform): {(r.Snapshot.ResolvedTargetSdk > 0 ? r.Snapshot.ResolvedTargetSdk.ToString() : "unknown")}");
            sb.AppendLine($"- min SDK: {r.Snapshot.MinSdk}  |  versionCode: {r.Snapshot.AndroidVersionCode}");
            sb.AppendLine($"- Application id: {r.Snapshot.ApplicationId}  |  Landscape-only: {r.Snapshot.LandscapeOnly}");
            sb.AppendLine($"- Launcher icon: adaptive={r.Snapshot.HasAdaptiveIcon}, legacy={r.Snapshot.HasLegacyIcon}");
            sb.AppendLine();
            sb.AppendLine("## Findings");
            foreach (var f in r.Findings)
            {
                sb.AppendLine($"- **[{f.Status}] {f.CheckId }** — {f.Message}"
                    + (string.IsNullOrEmpty(f.Recommendation) ? "" : $"  _→ {f.Recommendation}_"));
            }
            sb.AppendLine();
            sb.AppendLine("## Play-policy assumptions (verify — these age)");
            foreach (var a in r.PolicyAssumptions)
                sb.AppendLine($"- **{a.Name}**: {a.AssumedValue}  |  source: {a.Source}  |  effective: {a.EffectiveDate}  |  as-of: {a.AsOf}");
            sb.AppendLine();
            sb.AppendLine("_This audit is scaffolding only and never certifies store-readiness._");
            return sb.ToString();
        }

        private static string ShortGitCommit()
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo("git", "rev-parse --short HEAD")
                {
                    WorkingDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..")),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (var p = System.Diagnostics.Process.Start(psi))
                {
                    string outp = p.StandardOutput.ReadToEnd();
                    p.WaitForExit(5000);
                    string s = outp.Trim();
                    return string.IsNullOrEmpty(s) ? "unknown" : s;
                }
            }
            catch { return "unknown"; }
        }
    }
}
