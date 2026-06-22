namespace JebbyJump.Balance
{
    // Pure P28 balance-readiness policy. Heuristic analysis NEVER authorizes an applied
    // tuning change; a "validated"/"complete" claim requires recorded attempts; with no
    // attempts + no applied tuning the decision is exactly the NOT RUN wording.
    public static class BalancePlaytestPolicy
    {
        public const string PreparationNotRun =
            "P28 preparation complete - manual balance playtest NOT RUN";
        public const string PlaytestNoTuning =
            "P28 balance playtest complete - no tuning changes applied";
        public const string TuningPassComplete = "P28 balance tuning pass complete";
        public const string Blocked = "P28 blocked - balance decisions or tester unavailable";

        public static bool CanClaimValidated(int sampleSize) => sampleSize > 0;

        // Heuristic-only evidence can never authorize an applied tuning change (corr #2/#3/#7).
        public static bool HeuristicCanAuthorizeTuning() => false;

        public static string Decide(int sampleSize, bool tuningApplied)
        {
            if (tuningApplied) return TuningPassComplete;
            if (sampleSize > 0) return PlaytestNoTuning;
            return PreparationNotRun;
        }

        // Guard (corr #7): in P28 no review candidate may be authorized for apply, and all
        // must be deferred pending playtest.
        public static bool AllCandidatesDeferred(BalanceReviewCandidate[] candidates)
        {
            if (candidates == null) return true;
            foreach (var c in candidates)
                if (c.AuthorizedForApply || !c.DeferredPendingPlaytest) return false;
            return true;
        }

        public static bool AllObservationsDeferred(ProgressionEconomyObservation[] observations)
        {
            if (observations == null) return true;
            foreach (var o in observations)
                if (o.AuthorizedForApply || !o.DeferredPendingPlaytest) return false;
            return true;
        }
    }
}
