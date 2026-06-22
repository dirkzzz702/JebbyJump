using System;

namespace JebbyJump.Balance
{
    // P28 P4B balance-playtest evidence system (preparation). Field-based, JsonUtility-
    // friendly, UnityEditor-free so they are EditMode-testable. No personal identity is
    // ever stored - TesterProfile is a category label only.

    // One recorded human attempt (populated only when a real playtest runs).
    [Serializable]
    public struct BalanceAttemptRecord
    {
        public string Commit;
        public string Platform;
        public string TesterProfile;   // "developer" | "adult casual" | "child (guardian-approved)" ...
        public int LevelIndex;
        public int Attempt;
        public string Result;          // "Completed" | "Failed" | ""
        public float CompletionTime;   // seconds; 0 if none
        public string Rank;            // "S"/"A"/"B"/"C" or ""
        public int LivesRemaining;
        public int Mistakes;
        public int Falls;
        public int Difficulty1to5;
        public int Frustration1to5;
        public int Clarity1to5;
        public string Notes;
        public string Confidence;      // "High"/"Medium"/"Low"
    }

    // Per-level baseline + evidence status. In preparation mode EvidenceStatus = "NotRun"
    // and the collected-metric fields stay empty/zero (correction #6: no fabricated data).
    [Serializable]
    public struct BalanceLevelSummary
    {
        public int LevelIndex;
        public string EvidenceStatus;  // "NotRun" until a real playtest collects data
        // baseline configuration (read from the real assets)
        public int SequenceLength;
        public int ColorCount;
        public float MemorySeconds;
        public int StartingLives;
        public float CactusChance;
        public int PlatformsPerRow;
        public float SThreshold, AThreshold, BThreshold;
        // collected metrics (zero/empty in preparation mode)
        public int AttemptsRecorded;
        public string Notes;
    }

    // A REVIEW CANDIDATE (hypothesis), NOT a tuning recommendation (correction #2). It
    // carries no exact replacement value - only a qualitative direction to investigate -
    // and is always deferred + not authorized for apply (correction #3).
    [Serializable]
    public struct BalanceReviewCandidate
    {
        public string Id;                    // stable, e.g. "RC-HAZ-01"
        public int LevelIndex;               // 0 = cross-level
        public string Category;              // "Hazard pressure" / "Memory load" / "Rank threshold" ...
        public string Hypothesis;            // what to investigate
        public string Evidence;              // heuristic/observation backing (low confidence)
        public string SuggestedDirection;    // QUALITATIVE only (never a number)
        public string AffectedAsset;
        public string Risk;
        public string Confidence;            // "Low" | "Observation"
        public bool AuthorizedForApply;      // ALWAYS false in P28
        public bool DeferredPendingPlaytest; // ALWAYS true in P28
    }

    // A progression-economy observation, kept SEPARATE from level-balance review
    // candidates (correction #8).
    [Serializable]
    public struct ProgressionEconomyObservation
    {
        public string Id;
        public string Topic;
        public string Observation;
        public string Evidence;
        public string Confidence;
        public bool AuthorizedForApply;      // ALWAYS false
        public bool DeferredPendingPlaytest; // ALWAYS true
    }

    [Serializable]
    public sealed class BalancePlaytestReport
    {
        public string Commit = "";
        public string ExecutionMode = "PreparationOnly";
        public string DifficultyObjectiveAssumption =
            "gentle-to-moderate family/casual (PREPARATION ASSUMPTION, not a final product decision)";
        public string ReadinessDecision = "";
        public int SampleSize;               // 0 in preparation
        public string[] TesterProfilesPresent = Array.Empty<string>();
        public BalanceLevelSummary[] LevelSummaries = Array.Empty<BalanceLevelSummary>();
        public BalanceAttemptRecord[] Attempts = Array.Empty<BalanceAttemptRecord>(); // empty in prep
        public BalanceReviewCandidate[] ReviewCandidates = Array.Empty<BalanceReviewCandidate>();
        public ProgressionEconomyObservation[] ProgressionObservations = Array.Empty<ProgressionEconomyObservation>();
        public string ReadOnlyProof = "";
        public string NoHiddenTuningProof = "";
        public string Timestamp = "";        // ONLY in the ignored Builds/P28 report
    }
}
