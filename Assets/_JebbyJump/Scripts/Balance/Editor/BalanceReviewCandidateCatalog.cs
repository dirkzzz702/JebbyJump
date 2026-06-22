namespace JebbyJump.Balance
{
    // The fixed P28 review-candidate hypotheses + progression-economy observations.
    // Pure data (no asset access) so it is EditMode-testable. Every entry is a hypothesis
    // to investigate WITH human data - never a tuning recommendation, no exact replacement
    // value, always deferred + not authorized for apply (corrections #2/#3/#8).
    public static class BalanceReviewCandidateCatalog
    {
        public static BalanceReviewCandidate[] ReviewCandidates() => new[]
        {
            Candidate("RC-RANK-01", 0, "Rank threshold",
                "S/A/B thresholds are placeholders; fair values require multiple human completion times per level.",
                "P26: thresholds increase + are internally consistent, but unvalidated against real play.",
                "Derive thresholds from real median/best human times in P4B (not from expert-only runs).",
                "TimeRankConfig", "Tuning to expert/developer times would over-tighten ranks."),
            Candidate("RC-HAZ-01", 9, "Hazard pressure",
                "Cactus chance is non-monotonic (L9 spike ~0.40; dips on L5/L7/L8); may read as an unfair spike.",
                "P26 baseline hazard sequence across L4-L10.",
                "Review for a gentler rising hazard curve consistent with the family/casual assumption.",
                "LevelConfig", "Hazard is not the right lever to fix a rank issue."),
            Candidate("RC-MEM-01", 6, "Memory load",
                "Memory time drops to 4.5s only on L6/L7/L9; confirm an intentional pulse vs. accidental drift.",
                "P26 baseline memory-time sequence.",
                "Confirm intended memory pacing once observed in real play.",
                "LevelConfig", "Memory changes alter core difficulty; change cautiously."),
            Candidate("RC-CURVE-01", 0, "Learning / onboarding",
                "Overall heuristic ramp looks reasonable; two tiny dips (L4->L5, L6->L7) are within noise.",
                "P26 heuristic scores 5.0 -> 10.2.",
                "No change unless a real playtest shows an actual difficulty spike.",
                "", "Acting on noise-level dips risks churn without benefit."),
        };

        // Kept SEPARATE from level-balance candidates (correction #8).
        public static ProgressionEconomyObservation[] ProgressionObservations() => new[]
        {
            new ProgressionEconomyObservation
            {
                Id = "PE-01", Topic = "Wardrobe star economy",
                Observation = "The top outfit unlock requires all 30 stars (S/A on every one of the 10 "
                    + "levels), which is steep for a gentle family/casual curve.",
                Evidence = "StarReward S/A=3, B=2, C=1 -> max 30; wardrobe unlock thresholds "
                    + "0/4/8/12/15/22/26/30 are PLACEHOLDERS.",
                Confidence = "Low",
                AuthorizedForApply = false, DeferredPendingPlaytest = true,
            },
        };

        private static BalanceReviewCandidate Candidate(string id, int level, string category,
            string hypothesis, string evidence, string direction, string asset, string risk)
            => new BalanceReviewCandidate
            {
                Id = id, LevelIndex = level, Category = category, Hypothesis = hypothesis,
                Evidence = evidence, SuggestedDirection = direction, AffectedAsset = asset,
                Risk = risk, Confidence = level == 0 ? "Observation" : "Low",
                AuthorizedForApply = false, DeferredPendingPlaytest = true,
            };
    }
}
