using System.IO;
using System.Text;
using UnityEngine;

namespace JebbyJump.Release
{
    // Writes the RC report (JSON + Markdown) under an ignored Builds/P23 path.
    // Refuses to write if the serialized text looks like it carries secrets.
    public static class ReleaseReportWriter
    {
        public static void Write(ReleaseReport report, string outputDir)
        {
            Directory.CreateDirectory(outputDir);
            string json = JsonUtility.ToJson(report, true);
            if (ReleaseReport.ContainsSecretLike(json))
                throw new System.InvalidOperationException(
                    "[Release] report serialization contains secret-like text; aborting write.");
            File.WriteAllText(Path.Combine(outputDir, "release-report.json"), json);
            File.WriteAllText(Path.Combine(outputDir, "release-report.md"), ToMarkdown(report));
        }

        public static string ToMarkdown(ReleaseReport r)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Jebby Jump — Release Candidate Report");
            sb.AppendLine();
            sb.AppendLine($"- Verdict: **{r.ReadinessVerdict}**");
            sb.AppendLine($"- Timestamp: {r.Timestamp}");
            sb.AppendLine($"- Unity: {r.UnityVersion}  |  Git: {r.GitCommit} (dirty={r.GitDirty})");
            sb.AppendLine($"- Reproducibility level tested: {r.ReproducibilityLevel}");
            sb.AppendLine($"- Target: {r.Target} ({r.OutputType})  |  Backend: {r.ScriptingBackend}  Arch: {r.Architectures}  Stripping: {r.ManagedStripping}");
            sb.AppendLine($"- Identity: {r.CompanyName} / {r.ProductName} / {r.ApplicationIdentifier}  v{r.BundleVersion} ({r.AndroidVersionCode})");
            sb.AppendLine($"- Development build: {r.DevelopmentBuild}");
            sb.AppendLine();
            sb.AppendLine("## Statuses");
            sb.AppendLine($"- Preflight: {r.PreflightStatus}");
            sb.AppendLine($"- Tests: {r.TestsStatus} ({r.TestsPassed}/{r.TestsTotal}, failed {r.TestsFailed})");
            sb.AppendLine($"- Android build: {r.AndroidBuildStatus}");
            sb.AppendLine($"- Windows smoke: {r.WindowsSmokeStatus}");
            sb.AppendLine($"- Signing: {r.SigningStatus}  |  Store-upload ready: {r.StoreUploadReady}");
            sb.AppendLine($"- Signing intent: {r.SigningIntent}  |  Signature: {r.SignatureVerifyStatus} ({r.SignatureVerifyTool})  |  Config restored: {r.SigningConfigRestored}");
            if (!string.IsNullOrEmpty(r.SignerCertSha256))
                sb.AppendLine($"- Signer cert SHA-256: {r.SignerCertSha256}");
            sb.AppendLine($"- Artifact format: {r.ArtifactFormat} — {r.DistributionPurpose}");
            sb.AppendLine($"- Resolved artifact target SDK: {(r.ResolvedArtifactTargetSdk > 0 ? r.ResolvedArtifactTargetSdk.ToString() : r.ResolvedTargetSdkStatus)}  |  16 KB pages: {r.PageSize16kStatus}");
            sb.AppendLine($"- Warning gate: {r.WarningGateStatus} (warnings {r.WarningCount}, errors {r.ErrorCount})");
            sb.AppendLine($"- Artifact hashing: {r.ArtifactHashingStatus}");
            sb.AppendLine($"- Manual QA: {r.ManualQaStatus}");
            sb.AppendLine();
            sb.AppendLine("## Scenes");
            foreach (var s in r.Scenes) sb.AppendLine($"- {s}");
            sb.AppendLine();
            sb.AppendLine("## Artifact");
            sb.AppendLine($"- Path (relative to Builds/P23): {r.PrimaryArtifactPath}");
            sb.AppendLine($"- Size: {r.PrimaryArtifactBytes} bytes  |  Total: {r.TotalSizeBytes} bytes");
            sb.AppendLine($"- SHA-256: {r.PrimaryArtifactSha256}");
            if (r.ArtifactManifest != null && r.ArtifactManifest.Length > 0)
            {
                sb.AppendLine($"- File manifest ({r.ArtifactManifest.Length} files):");
                foreach (var f in r.ArtifactManifest)
                    sb.AppendLine($"  - {f.RelativePath}  {f.Bytes}B  {f.Sha256}");
            }
            sb.AppendLine();
            sb.AppendLine("## Preflight checks");
            foreach (var c in r.PreflightChecks)
                sb.AppendLine($"- [{c.Severity}] {c.CheckId}: {c.Message}");
            sb.AppendLine();
            sb.AppendLine("## Packages");
            foreach (var p in r.PackageClassifications) sb.AppendLine($"- {p}");
            if (r.UnclassifiedWarnings != null && r.UnclassifiedWarnings.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("## Unclassified build warnings");
                foreach (var w in r.UnclassifiedWarnings) sb.AppendLine($"- {w}");
            }
            sb.AppendLine();
            sb.AppendLine("_Manual device / visual / performance / accessibility / balance / art-final / "
                + "signing / store-submission status remain DEFERRED / NOT VERIFIED._");
            return sb.ToString();
        }
    }
}
