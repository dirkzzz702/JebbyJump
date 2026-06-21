using System;
using System.Text.RegularExpressions;

namespace JebbyJump.Release
{
    public static class DistStatus
    {
        public const string Passed = "Passed";
        public const string Failed = "Failed";
        public const string Blocked = "Blocked";
        public const string Draft = "Draft";
        public const string NotRun = "NotRun";
        public const string NotApplicable = "NotApplicable";
        public const string ExternalNotVerified = "ExternalNotVerified";
    }

    // [Serializable] field model (JsonUtility-friendly). Independent per-area statuses
    // (no single broad boolean); includes PlayConsoleActionStatus (correction #8) and an
    // enumerated MissingExternalItems list (correction #7).
    [Serializable]
    public sealed class DistributionReport
    {
        public string Timestamp = "";          // ONLY in the ignored Builds/P27 report
        public string GitCommit = "";
        public bool TreeClean;
        public string ExecutionMode = "PreparationOnly";

        public string ArtifactFormat = "";
        public string ArtifactSha256 = "";
        public string ArtifactPurpose = "";    // regression-gate vs upload-ready (corr #2)
        public string VersionName = "";
        public int VersionCode;

        // independent statuses
        public string TrackedConfigStatus = DistStatus.Draft;
        public string AutomatedTestStatus = DistStatus.NotRun;
        public string PreflightStatus = DistStatus.NotRun;
        public string PerformanceGateStatus = DistStatus.NotRun;
        public string UploadSigningStatus = DistStatus.NotRun;
        public string ArtifactSignatureStatus = DistStatus.NotRun;
        public string TargetSdkStatus = DistStatus.NotRun;
        public string PageSize16KbStatus = DistStatus.NotRun;
        public string DataSafetyStatus = DistStatus.Draft;
        public string PrivacyPolicyStatus = DistStatus.Blocked;
        public string StoreListingStatus = DistStatus.Draft;
        public string GraphicAssetsStatus = DistStatus.Blocked;
        public string PlayAppSigningStatus = DistStatus.NotRun;
        public string PlayConsoleActionStatus = DistStatus.NotRun; // corr #8
        public string InternalTrackUploadStatus = DistStatus.NotRun;
        public string TesterAccessStatus = DistStatus.NotRun;
        public string PhysicalInstallStatus = DistStatus.NotRun;
        public string ManualQaStatus = "DEFERRED / NOT VERIFIED";
        public string BalancePlaytestStatus = DistStatus.NotRun;
        public string ProductionRolloutStatus = DistStatus.NotRun;

        public string[] MissingExternalItems = Array.Empty<string>(); // corr #7
        public PolicySnapshotEntry[] PolicySnapshots = Array.Empty<PolicySnapshotEntry>();
        public string ReadinessDecision = "";

        // reuse the P23 guards; add a tester-email guard (corr #4)
        public static bool ContainsSecretLike(string t) => ReleaseReport.ContainsSecretLike(t);
        public static bool IsRelativePath(string p) => ReleaseReport.IsRelativePath(p);

        public static bool ContainsTesterEmail(string t)
        {
            if (string.IsNullOrEmpty(t)) return false;
            return Regex.IsMatch(t, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}");
        }
    }

    public static class DistributionReadinessPolicy
    {
        public const string PreparationComplete =
            "P27 preparation complete - upload requires authorized external action";
        public const string InternalComplete =
            "Play internal-testing distribution complete - physical install validation NOT RUN";
        public const string ValidatedOnDevices =
            "Play internal-testing distribution validated on tested devices";
        public const string Blocked = "P27 blocked - see distribution blocker report";
        public const string NotRun = "P27 NOT RUN - prerequisites or authorization unavailable";

        // Preparation-mode decision. "preparation complete" is allowed ONLY when the tree
        // is clean, internal gates pass, every missing external item is enumerated
        // (corr #7), and NO Console/internal-track/device action is claimed done (corr #8).
        public static string DecidePreparation(DistributionReport r)
        {
            if (!r.TreeClean) return Blocked;
            if (r.PageSize16KbStatus == DistStatus.Failed) return Blocked;
            if (r.AutomatedTestStatus == DistStatus.Failed
                || r.PreflightStatus == DistStatus.Failed) return Blocked;
            if (ClaimsConsoleOrDevice(r)) return Blocked;
            if (r.MissingExternalItems == null || r.MissingExternalItems.Length == 0) return Blocked;
            return PreparationComplete;
        }

        // True if any Console / internal-track / tester / device / upload status claims
        // an external action was completed (must never happen in preparation mode).
        public static bool ClaimsConsoleOrDevice(DistributionReport r)
        {
            return r.PlayConsoleActionStatus == DistStatus.Passed
                || r.InternalTrackUploadStatus == DistStatus.Passed
                || r.PlayAppSigningStatus == DistStatus.Passed
                || r.TesterAccessStatus == DistStatus.Passed
                || r.PhysicalInstallStatus == DistStatus.Passed
                || r.UploadSigningStatus == DistStatus.Passed;
        }
    }
}
