using System;

namespace JebbyJump.Release
{
    // Dated Play-policy snapshot with an explicit verification status (correction #3):
    // policy values age, so each entry records whether it was Reverified against the
    // official source, CarriedForward from a prior phase, or NotVerifiedThisRun.
    public enum PolicyVerificationStatus { Reverified, CarriedForward, NotVerifiedThisRun }

    [Serializable]
    public struct PolicySnapshotEntry
    {
        public string PolicyName;
        public string OfficialSource;
        public string DateChecked;
        public string EffectiveDate;
        public string RequiredValue;
        public string ConfiguredValue;
        public string ResolvedValue;
        public string Result;             // Pass | Flag | Unknown
        public string VerificationStatus; // PolicyVerificationStatus name
    }

    public static class PolicySnapshotCatalog
    {
        // Carried forward from P26 with explicit verification status. No live policy fetch
        // happens here, so target/16KB are NotVerifiedThisRun and must be re-verified
        // against the official source before any real upload.
        public static PolicySnapshotEntry[] Default(int resolvedTargetSdk, string pageSize16k) => new[]
        {
            new PolicySnapshotEntry
            {
                PolicyName = "Play target API level (new apps)",
                OfficialSource = "Google Play target API level requirements",
                DateChecked = "assistant knowledge cutoff (early 2026)",
                EffectiveDate = "new apps 2024-08-31; updates 2025-08-31",
                RequiredValue = ">= 35",
                ConfiguredValue = "Automatic (0)",
                ResolvedValue = resolvedTargetSdk > 0 ? resolvedTargetSdk.ToString() : "unknown",
                Result = resolvedTargetSdk >= StoreCompliancePolicy.AssumedPlayMinTargetSdk ? "Pass" : "Flag",
                VerificationStatus = nameof(PolicyVerificationStatus.NotVerifiedThisRun),
            },
            new PolicySnapshotEntry
            {
                PolicyName = "16 KB memory page support",
                OfficialSource = "Android 15 16 KB page size / Google Play guidance",
                DateChecked = "assistant knowledge cutoff (early 2026)",
                EffectiveDate = "~2025-11 (apps targeting Android 15+)",
                RequiredValue = "supported",
                ConfiguredValue = "n/a",
                ResolvedValue = string.IsNullOrEmpty(pageSize16k) ? "unknown" : pageSize16k,
                Result = pageSize16k == "Aligned16k" ? "Pass" : "Unknown",
                VerificationStatus = nameof(PolicyVerificationStatus.NotVerifiedThisRun),
            },
            new PolicySnapshotEntry
            {
                PolicyName = "App distribution + signing",
                OfficialSource = "Google Play (AAB requirement) / Play App Signing",
                DateChecked = "assistant knowledge cutoff (early 2026)",
                EffectiveDate = "2021-08 (new apps require AAB)",
                RequiredValue = "AAB + Play App Signing",
                ConfiguredValue = "AAB; upload key not configured",
                ResolvedValue = "Play App Signing not configured",
                Result = "Flag",
                VerificationStatus = nameof(PolicyVerificationStatus.CarriedForward),
            },
        };

        public static bool AllHaveSourceAndDate(PolicySnapshotEntry[] entries)
        {
            if (entries == null || entries.Length == 0) return false;
            foreach (var e in entries)
                if (string.IsNullOrEmpty(e.OfficialSource)
                    || string.IsNullOrEmpty(e.DateChecked)
                    || string.IsNullOrEmpty(e.VerificationStatus)) return false;
            return true;
        }
    }

    // Gates the Play declarations (correction #6): everything stays a DRAFT/WORKSHEET
    // until artifact audit + explicit user approval; placeholders never pass; the IARC
    // rating is never predicted.
    public static class DistributionDeclarationPolicy
    {
        public const int TitleMax = 30;
        public const int ShortDescMax = 80;
        public const int FullDescMax = 4000;

        public static bool DataSafetyCanFinalize(bool artifactAudited, bool userApproved)
            => artifactAudited && userApproved;

        public static bool PrivacyPolicyValid(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            string u = url.Trim().ToLowerInvariant();
            if (u.Contains("example.com") || u.Contains("placeholder") || u.Contains("todo")
                || u.Contains("<") || u.Contains("your-")) return false;
            return u.StartsWith("https://");
        }

        public static bool TargetAudienceExplicit(string audience)
            => !string.IsNullOrEmpty(audience)
                && audience.Trim().ToLowerInvariant() != "unknown"
                && audience.Trim().ToLowerInvariant() != "tbd";

        // A non-empty rating claim means someone PREDICTED it - not allowed.
        public static bool IsPredictedRating(string ratingClaim)
            => !string.IsNullOrEmpty(ratingClaim);

        public static bool ListingCopyWithinLimits(string title, string shortDesc, string fullDesc)
            => (title == null || title.Length <= TitleMax)
            && (shortDesc == null || shortDesc.Length <= ShortDescMax)
            && (fullDesc == null || fullDesc.Length <= FullDescMax);

        private static readonly string[] UnverifiedClaimTerms =
        {
            "final", "certified", "guaranteed", "100%", "fastest", "best-in-class",
            "no bugs", "fully accessible", "lag-free",
        };

        public static bool ListingCopyHasUnverifiedClaim(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            string t = text.ToLowerInvariant();
            foreach (var term in UnverifiedClaimTerms) if (t.Contains(term)) return true;
            return false;
        }
    }
}
