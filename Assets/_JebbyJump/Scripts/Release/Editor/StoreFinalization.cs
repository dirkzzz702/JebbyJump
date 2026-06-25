namespace JebbyJump.Release
{
    // P31 store-finalization pure helpers (UnityEditor-free, EditMode-testable). Reuses
    // DistributionDeclarationPolicy for listing/privacy/data-safety/rating checks; adds
    // graphic-asset separation + tiered readiness (correction #5) and a placeholder guard
    // so unfinished external content can never pass final readiness (correction #8).

    public enum GraphicAssetKind
    {
        // In-artifact (ships inside the APK/AAB via Player Settings)
        LauncherIcon, AdaptiveForeground, AdaptiveBackground,
        // Console-listing (uploaded to Play, never in the artifact)
        ListingIcon512, FeatureGraphic1024x500, PhoneScreenshot, TabletScreenshot,
    }

    public enum ReleaseTier { InternalTesting, PublicListing, ProductionRelease }

    public static class StoreGraphicsPolicy
    {
        public static bool IsInArtifact(GraphicAssetKind k)
            => k == GraphicAssetKind.LauncherIcon
            || k == GraphicAssetKind.AdaptiveForeground
            || k == GraphicAssetKind.AdaptiveBackground;

        public static bool IsConsoleListing(GraphicAssetKind k) => !IsInArtifact(k);

        // Graphics readiness is evaluated per tier (correction #5).
        public static ReleaseTier[] Tiers() => new[]
        {
            ReleaseTier.InternalTesting, ReleaseTier.PublicListing, ReleaseTier.ProductionRelease,
        };
    }

    public static class StoreReadinessPolicy
    {
        public const string Blocked = "Store listing package blocked - missing external content";
        public const string Ready = "Store listing package ready for user Play Console entry";

        // A placeholder marks unfinished external content; it may appear only in
        // blocked/draft sections and can never pass final readiness (correction #8).
        public static bool ContainsPlaceholder(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            string t = text.ToUpperInvariant();
            return t.Contains("<PROVIDE") || t.Contains("PROVIDE:") || t.Contains("TODO")
                || t.Contains("TBD") || t.Contains("<YOUR-") || t.Contains("PLACEHOLDER");
        }

        // Ready ONLY when every required external item is present, the user approved, and no
        // placeholder remains; otherwise blocked (corrections #8/#10).
        public static string Decide(bool privacyUrlProvided, bool supportEmailProvided,
            bool listingGraphicsProvided, bool userApproved, bool anyPlaceholderRemains)
        {
            if (anyPlaceholderRemains || !privacyUrlProvided || !supportEmailProvided
                || !listingGraphicsProvided || !userApproved)
                return Blocked;
            return Ready;
        }
    }
}
