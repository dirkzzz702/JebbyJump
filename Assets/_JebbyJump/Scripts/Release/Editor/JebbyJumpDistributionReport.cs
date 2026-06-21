using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.Release
{
    // Read-only P27 distribution-readiness audit. Aggregates the existing P23 release
    // reports + PlayerSettings + git state into the independent status model, enumerates
    // every missing external item, attaches dated policy snapshots, computes the
    // preparation-mode decision, and writes an IGNORED Builds/P27 report (the only place a
    // timestamp lives). Mutates nothing; refuses to write secrets or tester emails.
    public static class JebbyJumpDistributionReport
    {
        private const string OutputRootRel = "Builds/P27";
        private const string AabReportRel = "Builds/P23/Android/1.0/release-report.json";
        private const string ApkReportRel = "Builds/P23/AndroidApk/1.0/release-report.json";

        [MenuItem("Jebby Jump/Release/Distribution Readiness Audit")]
        public static void RunMenu()
        {
            var r = Run();
            Debug.Log($"[Distribution] decision='{r.ReadinessDecision}' treeClean={r.TreeClean} "
                + $"missingExternals={r.MissingExternalItems.Length} artifactPurpose='{r.ArtifactPurpose}'");
        }

        public static DistributionReport Run()
        {
            var aab = ReadReleaseReport(AabReportRel);
            var apk = ReadReleaseReport(ApkReportRel);

            string porcelain = RunGit("status --porcelain");
            string commit = RunGit("rev-parse --short HEAD");

            int resolvedSdk = apk != null ? apk.ResolvedArtifactTargetSdk : 0;
            string page16 = apk != null && !string.IsNullOrEmpty(apk.PageSize16kStatus)
                ? apk.PageSize16kStatus : "unknown";
            bool uploadSigned = aab != null && aab.SigningStatus != null
                && aab.SigningStatus.StartsWith("EnvUploadKeySigned");

            var r = new DistributionReport
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                GitCommit = string.IsNullOrEmpty(commit) ? "unknown" : commit,
                TreeClean = TreeStatePolicy.IsClean(porcelain),
                ExecutionMode = "PreparationOnly",

                ArtifactFormat = "AAB",
                ArtifactSha256 = aab != null ? aab.PrimaryArtifactSha256 : "",
                ArtifactPurpose = UploadReadinessPolicy.ArtifactPurpose(
                    uploadSigned ? nameof(SigningMode.EnvUploadKeySigned) : nameof(SigningMode.DebugSigned)),
                VersionName = PlayerSettings.bundleVersion,
                VersionCode = PlayerSettings.Android.bundleVersionCode,

                TrackedConfigStatus = DistStatus.Draft,    // writer ready; values not yet approved
                AutomatedTestStatus = aab != null && aab.TestsStatus == "Passed" ? DistStatus.Passed : DistStatus.NotRun,
                PreflightStatus = aab != null && aab.PreflightStatus == "Passed" ? DistStatus.Passed : DistStatus.NotRun,
                PerformanceGateStatus = DistStatus.NotApplicable, // no runtime change in P27
                UploadSigningStatus = DistStatus.NotRun,          // no upload key supplied
                ArtifactSignatureStatus = aab != null && aab.SignatureVerifyStatus == "Verified"
                    ? DistStatus.Passed : DistStatus.NotRun,
                TargetSdkStatus = resolvedSdk >= StoreCompliancePolicy.AssumedPlayMinTargetSdk ? DistStatus.Passed
                    : (resolvedSdk > 0 ? DistStatus.Failed : DistStatus.NotRun),
                PageSize16KbStatus = page16 == "Aligned16k" ? DistStatus.Passed
                    : (page16 == "NotAligned16k" ? DistStatus.Failed : DistStatus.NotRun),
                DataSafetyStatus = DistStatus.Draft,
                PrivacyPolicyStatus = DistStatus.Blocked,
                StoreListingStatus = DistStatus.Draft,
                GraphicAssetsStatus = DistStatus.Blocked,
                PlayAppSigningStatus = DistStatus.NotRun,
                PlayConsoleActionStatus = DistStatus.NotRun,
                InternalTrackUploadStatus = DistStatus.NotRun,
                TesterAccessStatus = DistStatus.NotRun,
                PhysicalInstallStatus = DistStatus.NotRun,
                BalancePlaytestStatus = DistStatus.NotRun,
                ProductionRolloutStatus = DistStatus.NotRun,

                MissingExternalItems = MissingExternals(),
                PolicySnapshots = PolicySnapshotCatalog.Default(resolvedSdk, page16),
            };
            r.ReadinessDecision = DistributionReadinessPolicy.DecidePreparation(r);

            Write(r);
            return r;
        }

        private static string[] MissingExternals() => new[]
        {
            "Google Play Console developer account + JebbyJump app record",
            "Real upload keystore (supplied only via env; not yet provided)",
            "Play App Signing configuration (first-release opt-in)",
            "Public privacy-policy URL",
            "Final store-listing copy approval",
            "Play listing graphics (512 icon, 1024x500 feature, screenshots)",
            "Approved internal tester list / Google Group",
            "Physical Android device + tester for Play-distributed install",
            "Regions/pricing + ads/IAP runtime declarations",
            "Final Data Safety approval (after artifact audit)",
            "Content-rating (IARC) questionnaire answers",
        };

        private static ReleaseReport ReadReleaseReport(string rel)
        {
            try
            {
                string full = Path.GetFullPath(Path.Combine(Application.dataPath, "..", rel));
                if (!File.Exists(full)) return null;
                return JsonUtility.FromJson<ReleaseReport>(File.ReadAllText(full));
            }
            catch { return null; }
        }

        private static void Write(DistributionReport r)
        {
            string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputRootRel, r.GitCommit));
            Directory.CreateDirectory(dir);
            string json = JsonUtility.ToJson(r, true);
            if (DistributionReport.ContainsSecretLike(json))
                throw new InvalidOperationException("[Distribution] report contains secret-like text; aborting write.");
            if (DistributionReport.ContainsTesterEmail(json))
                throw new InvalidOperationException("[Distribution] report contains an email-like token; aborting write.");
            File.WriteAllText(Path.Combine(dir, "distribution-report.json"), json);
            File.WriteAllText(Path.Combine(dir, "distribution-report.md"), ToMarkdown(r));
        }

        private static string ToMarkdown(DistributionReport r)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Jebby Jump — P27 Distribution Readiness (ignored raw report)");
            sb.AppendLine();
            sb.AppendLine($"- Decision: **{r.ReadinessDecision}**");
            sb.AppendLine($"- Execution mode: {r.ExecutionMode}  |  Git: {r.GitCommit}  |  Tree clean: {r.TreeClean}");
            sb.AppendLine($"- Artifact: {r.ArtifactFormat} {r.ArtifactSha256}");
            sb.AppendLine($"- Artifact purpose: **{r.ArtifactPurpose}**");
            sb.AppendLine($"- Version: {r.VersionName} ({r.VersionCode})");
            sb.AppendLine();
            sb.AppendLine("## Statuses");
            foreach (var kv in new (string, string)[]
            {
                ("TrackedConfig", r.TrackedConfigStatus), ("AutomatedTest", r.AutomatedTestStatus),
                ("Preflight", r.PreflightStatus), ("PerformanceGate", r.PerformanceGateStatus),
                ("UploadSigning", r.UploadSigningStatus), ("ArtifactSignature", r.ArtifactSignatureStatus),
                ("TargetSdk", r.TargetSdkStatus), ("PageSize16Kb", r.PageSize16KbStatus),
                ("DataSafety", r.DataSafetyStatus), ("PrivacyPolicy", r.PrivacyPolicyStatus),
                ("StoreListing", r.StoreListingStatus), ("GraphicAssets", r.GraphicAssetsStatus),
                ("PlayAppSigning", r.PlayAppSigningStatus), ("PlayConsoleAction", r.PlayConsoleActionStatus),
                ("InternalTrackUpload", r.InternalTrackUploadStatus), ("TesterAccess", r.TesterAccessStatus),
                ("PhysicalInstall", r.PhysicalInstallStatus), ("ManualQa", r.ManualQaStatus),
                ("BalancePlaytest", r.BalancePlaytestStatus), ("ProductionRollout", r.ProductionRolloutStatus),
            })
                sb.AppendLine($"- {kv.Item1}: {kv.Item2}");
            sb.AppendLine();
            sb.AppendLine("## Missing external items (must be enumerated to claim preparation-complete)");
            foreach (var m in r.MissingExternalItems) sb.AppendLine($"- {m}");
            sb.AppendLine();
            sb.AppendLine("## Play-policy snapshots (verify — these age)");
            foreach (var p in r.PolicySnapshots)
                sb.AppendLine($"- {p.PolicyName}: required {p.RequiredValue}, resolved {p.ResolvedValue} -> {p.Result} "
                    + $"[{p.VerificationStatus}] (source: {p.OfficialSource}; checked {p.DateChecked})");
            return sb.ToString();
        }

        private static string RunGit(string args)
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo("git", args)
                {
                    WorkingDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..")),
                    RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true,
                };
                using (var p = System.Diagnostics.Process.Start(psi))
                {
                    string outp = p.StandardOutput.ReadToEnd();
                    p.WaitForExit(5000);
                    return outp.Trim();
                }
            }
            catch { return ""; }
        }
    }
}
