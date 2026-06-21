using System;

namespace JebbyJump.Release
{
    // Pure distribution-preparation helpers (P27). UnityEditor-free, EditMode-testable.
    // No secrets, no Play Console / device claims.

    // Approved tracked distribution config. Approved defaults FALSE so the writer
    // dry-runs / refuses until version name + version code + target SDK are explicitly
    // approved (correction #5).
    [Serializable]
    public struct DistributionConfig
    {
        public bool Approved;
        public string VersionName;
        public int VersionCode;
        public int TargetSdk;   // 0 = leave Automatic (no pin)

        public static DistributionConfig CurrentBaseline() => new DistributionConfig
        {
            Approved = false, VersionName = "1.0", VersionCode = 1, TargetSdk = 0,
        };
    }

    public struct DistributionConfigDecision
    {
        public bool Apply;
        public string Reason;
    }

    public static class DistributionConfigGate
    {
        // Refuses to write unless explicitly approved AND valid (correction #5).
        // lastUploadedCode = highest version code already used on Play (0 = first upload).
        public static DistributionConfigDecision ShouldApply(DistributionConfig c, int lastUploadedCode)
        {
            if (!c.Approved)
                return Refuse("DRY-RUN: distribution config not explicitly approved; "
                    + "no ProjectSettings written.");
            if (string.IsNullOrEmpty(c.VersionName))
                return Refuse("Refused: version name is empty.");
            if (!VersionCodePolicy.IsValidNextCode(c.VersionCode, lastUploadedCode))
                return Refuse($"Refused: version code {c.VersionCode} must be explicit and greater "
                    + $"than the last uploaded code {lastUploadedCode}.");
            if (c.TargetSdk != 0 && c.TargetSdk < StoreCompliancePolicy.AssumedPlayMinTargetSdk)
                return Refuse($"Refused: pinned target SDK {c.TargetSdk} is below the assumed Play "
                    + $"minimum {StoreCompliancePolicy.AssumedPlayMinTargetSdk}.");
            return new DistributionConfigDecision
            {
                Apply = true,
                Reason = "Approved: applying tracked distribution config.",
            };
        }

        private static DistributionConfigDecision Refuse(string reason)
            => new DistributionConfigDecision { Apply = false, Reason = reason };
    }

    public static class VersionCodePolicy
    {
        // Explicit + strictly increasing; never auto-incremented.
        public static bool IsValidNextCode(int candidate, int lastUploadedCode)
            => candidate > 0 && candidate > lastUploadedCode;
    }

    // Working-tree cleanliness from `git status --porcelain`. Any non-empty line means
    // dirty - it NEVER reports clean while tracked files are modified (correction #1/#10).
    public static class TreeStatePolicy
    {
        public static bool IsClean(string porcelain)
        {
            if (string.IsNullOrEmpty(porcelain)) return true;
            foreach (var raw in porcelain.Replace("\r", "").Split('\n'))
                if (raw.Trim().Length > 0) return false;
            return true;
        }
    }

    // Upload-readiness by signing mode (correction #2): a debug-signed artifact is a
    // release-pipeline REGRESSION-GATE artifact, NOT an upload/distribution candidate.
    public static class UploadReadinessPolicy
    {
        public const string RegressionGate =
            "regression-gate (debug-signed; NOT upload/distribution-ready)";
        public const string UploadReady =
            "upload-key-signed (distribution candidate)";

        public static bool IsUploadReady(string signingMode)
            => signingMode == nameof(SigningMode.EnvUploadKeySigned);

        public static string ArtifactPurpose(string signingMode)
            => IsUploadReady(signingMode) ? UploadReady : RegressionGate;
    }
}
