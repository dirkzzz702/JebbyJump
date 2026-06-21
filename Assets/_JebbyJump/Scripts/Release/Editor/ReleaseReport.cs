using System;

namespace JebbyJump.Release
{
    // [Serializable], PUBLIC FIELDS only (fix #1) so JsonUtility serializes it
    // cleanly. Independent per-area statuses (fix #11) - no single broad "Passed".
    [Serializable]
    public sealed class ReleaseReport
    {
        // metadata
        public string Timestamp = "";
        public string UnityVersion = "";
        public string GitCommit = "";
        public bool GitDirty;
        public string ReproducibilityLevel = "clean-working-tree";

        // target / identity
        public string Target = "";
        public string OutputType = "";
        public string CompanyName = "";
        public string ProductName = "";
        public string ApplicationIdentifier = "";
        public string BundleVersion = "";
        public int AndroidVersionCode;
        public bool DevelopmentBuild;
        public string ScriptingBackend = "";
        public string Architectures = "";
        public string ManagedStripping = "";
        public string[] Scenes = Array.Empty<string>();

        // independent statuses
        public string PreflightStatus = "NotRun";
        public string TestsStatus = "NotRun";
        public string AndroidBuildStatus = AndroidBuildStatusValues.NotRun;
        public string WindowsSmokeStatus = "NotRun";
        public string SigningStatus = "NotApplicable";
        public bool StoreUploadReady;            // always false in P23
        public string WarningGateStatus = "NotRun";
        public string ArtifactHashingStatus = "NotRun";
        public string ManualQaStatus = "DEFERRED / NOT VERIFIED";
        public string ReadinessVerdict = ReleaseReadiness.Blocked;

        // ---- P26: artifact format + signing intent / signature verification ----
        public string ArtifactFormat = "AAB";              // "AAB" | "APK"
        public string DistributionPurpose = "";
        public string SigningIntent = "Debug";
        public string SigningResolutionReason = "";
        public string SignatureVerifyStatus = "NotRun";    // Verified | Failed | Skipped
        public string SignatureVerifyTool = "";
        public string SignerCertSha256 = "";               // public signer fingerprint (not secret)
        public string SigningConfigRestored = "NotChecked";// "Restored" | "DRIFT"
        public int ResolvedArtifactTargetSdk;              // from aapt2 on the APK (0 = not read)
        public string ResolvedTargetSdkStatus = "NotRun";
        public string PageSize16kStatus = "NotRun";        // Aligned16k | NotAligned16k | Skipped

        // details
        public int TestsTotal;
        public int TestsPassed;
        public int TestsFailed;
        public ReleaseCheckResult[] PreflightChecks = Array.Empty<ReleaseCheckResult>();
        public string[] PackageClassifications = Array.Empty<string>();
        public string BuildResult = "";
        public double BuildDurationSeconds;
        public long TotalSizeBytes;
        public string PrimaryArtifactPath = "";
        public long PrimaryArtifactBytes;
        public string PrimaryArtifactSha256 = "";
        public ArtifactHasher.FileHash[] ArtifactManifest = Array.Empty<ArtifactHasher.FileHash>();
        public int WarningCount;
        public int ErrorCount;
        public string[] ClassifiedWarnings = Array.Empty<string>();
        public string[] UnclassifiedWarnings = Array.Empty<string>();

        // ---- pure validators (used by the writer + tests) ----

        // Guards the report can never carry credentials/secrets.
        public static bool ContainsSecretLike(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            string t = text.ToLowerInvariant();
            return t.Contains("password")
                || t.Contains("keystorepass")
                || t.Contains("keypass")
                || t.Contains("-----begin")
                || t.Contains(".keystore")
                || t.Contains(".jks")
                || t.Contains("provisioningprofile");
        }

        // A reported artifact path must be relative (no drive root / no leading slash).
        public static bool IsRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            string p = path.Replace('\\', '/');
            if (p.Contains(":")) return false;     // C:/...
            if (p.StartsWith("/")) return false;    // absolute posix
            return true;
        }
    }

    public static class AndroidBuildStatusValues
    {
        public const string NotRun = "NotRun";
        // mirrors AndroidBuildStatus enum names for serialized clarity
    }
}
