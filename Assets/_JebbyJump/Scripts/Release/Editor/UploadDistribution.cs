using System;
using System.Text.RegularExpressions;

namespace JebbyJump.Release
{
    // P32 upload-distribution helpers (pure, EditMode-testable). Separates the five
    // distinct upload/console statuses (corr #7), proves a fingerprint match, and gates
    // "internal-track complete" behind real Console evidence (it can never be claimed from
    // local state). No secrets/values are ever handled here.

    public static class UploadStatus
    {
        public const string NotProvided = "NotProvided";
        public const string NotBuilt = "NotBuilt";
        public const string Blocked = "Blocked";
        public const string NotRun = "NotRun";
        public const string NotConfigured = "NotConfigured";
        public const string Passed = "Passed";
        public const string NotVerified = "NotVerified";
    }

    public static class UploadDistributionPolicy
    {
        public const string Blocked = "P32 blocked - see Play distribution blocker report";
        public const string CompleteNoDevice =
            "Play internal-testing distribution complete - physical install validation NOT RUN";
        public const string ValidatedOnDevices =
            "Play internal-testing distribution validated on tested devices";

        // Normalized, case-insensitive cert-fingerprint comparison (ignores ':' / spaces).
        public static bool FingerprintMatches(string expected, string actual)
        {
            if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(actual)) return false;
            return Norm(expected) == Norm(actual);
        }

        private static string Norm(string s) => Regex.Replace(s, "[^0-9A-Fa-f]", "").ToLowerInvariant();

        // "internal-track complete" requires real Console evidence AND an upload-signed
        // artifact; local/heuristic state can never claim it.
        public static bool CanClaimInternalTrackComplete(bool consoleEvidencePresent, bool uploadKeySignedArtifact)
            => consoleEvidencePresent && uploadKeySignedArtifact;

        // Readiness decision. This run -> Blocked (no keystore, no Console, externals missing).
        public static string Decide(bool uploadKeySignedArtifact, bool internalTrackUploaded,
            bool consoleEvidencePresent, bool deviceSmokePassed, bool anyBlockerRemains)
        {
            if (anyBlockerRemains || !uploadKeySignedArtifact || !internalTrackUploaded
                || !consoleEvidencePresent)
                return Blocked;
            return deviceSmokePassed ? ValidatedOnDevices : CompleteNoDevice;
        }
    }

    // [Serializable] evidence model. FIVE separate statuses (corr #7) + Console
    // submission/verification flags (corr #8) + env-presence (never values, corr #3).
    [Serializable]
    public sealed class UploadDistributionReport
    {
        public string Commit = "";
        public string ExecutionMode = "PreparationOnly-Blocked";
        public string ReadinessDecision = "";

        // five separate statuses (corr #7)
        public string UploadKeyStatus = UploadStatus.NotProvided;
        public string UploadKeySignedArtifactStatus = UploadStatus.NotBuilt;
        public string PlayAppSigningStatus = UploadStatus.NotConfigured;
        public string PlayConsoleActionStatus = UploadStatus.NotRun;
        public string InternalTrackUploadStatus = UploadStatus.NotRun;

        // supporting statuses
        public string TargetSdkStatus = UploadStatus.NotRun;
        public string PageSize16KbStatus = UploadStatus.NotRun;
        public string PrivacyPolicyStatus = UploadStatus.Blocked;
        public string GraphicAssetsStatus = UploadStatus.Blocked;
        public string DataSafetyStatus = "Draft";
        public string VersionCodeStatus = UploadStatus.NotVerified; // corr #5 (no Console evidence)
        public string PhysicalInstallStatus = UploadStatus.NotRun;

        // Console flags (corr #8)
        public bool SubmittedInConsole;
        public bool VerifiedInConsole;

        // env PRESENT/MISSING only, never values (corr #3)
        public string SigningEnvPresence = "";
        public string SignedArtifactFingerprint = ""; // public cert fp if a signed AAB built; else ""
        public string ExpectedUploadFingerprint = "";  // N/A without an upload key
        public string FailHardProof = "";

        public string[] Blockers = Array.Empty<string>();

        // guards (corr #9)
        public static bool ContainsSecretLike(string t) => ReleaseReport.ContainsSecretLike(t);
        public static bool ContainsTesterEmail(string t) => DistributionReport.ContainsTesterEmail(t);

        public static bool ContainsLocalPathOrEnvDump(string t)
        {
            if (string.IsNullOrEmpty(t)) return false;
            if (Regex.IsMatch(t, @"[A-Za-z]:\\")) return true;             // drive path
            if (Regex.IsMatch(t, @"JJ_ANDROID_\w+\s*=\s*\S")) return true;  // raw env-var dump (name=value)
            return false;
        }
    }
}
