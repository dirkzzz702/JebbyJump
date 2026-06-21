using System;
using System.Collections.Generic;
using System.Globalization;

namespace JebbyJump.Balance
{
    [Serializable]
    public struct LevelDifficultySnapshot
    {
        public string LevelName;
        public int Index;            // 1-based level number
        public int SequenceLength;
        public int ColorCount;
        public float MemorySeconds;
        public int StartingLives;
        public float CactusChance;
        public int PlatformsPerRow;
        public float SThreshold, AThreshold, BThreshold;
    }

    [Serializable]
    public struct BalanceFinding
    {
        public string CheckId;
        public string Severity;      // "INVARIANT_FAIL" | "OBSERVATION" | "INFO"
        public string Message;
    }

    // Pure difficulty analysis (P26-Balance, correction #9).
    //
    //  - HARD INVARIANT (the ONLY thing that can "fail"): intra-level S < A < B
    //    threshold ordering. Asserted in tests; surfaced as INVARIANT_FAIL.
    //  - Everything CROSS-LEVEL (difficulty score, curve monotonicity, threshold trend)
    //    is a TRANSPARENT, DETERMINISTIC, LOW-CONFIDENCE heuristic pending the P4B human
    //    playtest. It only ever yields OBSERVATIONs - never invariant failures, never a
    //    claim that the real curve is "correct".
    public static class DifficultyCurvePolicy
    {
        // Transparent, fixed heuristic weights (higher score = assumed harder).
        public const float WSeq = 1.0f;          // per memory item
        public const float WColor = 0.5f;        // per available color
        public const float WMemPressure = 0.3f;  // per second BELOW the memory reference
        public const float WLives = 0.4f;        // per starting life (reduces difficulty)
        public const float WCactus = 2.0f;       // per unit cactus probability
        public const float WRow = 0.1f;          // per platform-per-row
        public const float MemReferenceSeconds = 10f;

        // ---- HARD invariant ----
        public static bool ThresholdOrderValid(LevelDifficultySnapshot s)
            => s.SThreshold < s.AThreshold && s.AThreshold < s.BThreshold;

        // ---- transparent deterministic heuristic ----
        public static float HeuristicScore(LevelDifficultySnapshot s)
        {
            float memPressure = WMemPressure * Math.Max(0f, MemReferenceSeconds - s.MemorySeconds);
            return WSeq * s.SequenceLength
                 + WColor * s.ColorCount
                 + memPressure
                 + WCactus * s.CactusChance
                 + WRow * s.PlatformsPerRow
                 - WLives * s.StartingLives;
        }

        public static string FormulaText()
            => $"score = {F(WSeq)}*seqLen + {F(WColor)}*colors + {F(WMemPressure)}*max(0,{F(MemReferenceSeconds)}-memorySec)"
             + $" + {F(WCactus)}*cactusChance + {F(WRow)}*platformsPerRow - {F(WLives)}*lives"
             + "  (transparent heuristic; LOW-CONFIDENCE pending P4B playtest)";

        public static BalanceFinding[] Analyze(LevelDifficultySnapshot[] levels)
        {
            var f = new List<BalanceFinding>();
            if (levels == null || levels.Length == 0)
            {
                f.Add(new BalanceFinding { CheckId = "levels.present", Severity = "INFO", Message = "no levels found" });
                return f.ToArray();
            }

            // 1. HARD invariant: intra-level threshold ordering (the only failable check).
            foreach (var s in levels)
                if (!ThresholdOrderValid(s))
                    f.Add(new BalanceFinding
                    {
                        CheckId = "threshold.order",
                        Severity = "INVARIANT_FAIL",
                        Message = $"{s.LevelName}: requires S({F(s.SThreshold)}) < A({F(s.AThreshold)}) < B({F(s.BThreshold)}).",
                    });

            // 2. Heuristic OBSERVATIONs (never failures): cross-level score monotonicity.
            float prevScore = float.NaN;
            string prevName = null;
            foreach (var s in levels)
            {
                float sc = HeuristicScore(s);
                if (!float.IsNaN(prevScore) && sc < prevScore)
                    f.Add(new BalanceFinding
                    {
                        CheckId = "curve.monotonicity",
                        Severity = "OBSERVATION",
                        Message = $"heuristic difficulty decreases {prevName} -> {s.LevelName} "
                                + $"({F(prevScore)} -> {F(sc)}); low-confidence, verify by playtest.",
                    });
                prevScore = sc;
                prevName = s.LevelName;
            }

            // 3. Cross-level S-threshold trend (observation only).
            for (int i = 1; i < levels.Length; i++)
                if (levels[i].SThreshold < levels[i - 1].SThreshold)
                    f.Add(new BalanceFinding
                    {
                        CheckId = "threshold.trend",
                        Severity = "OBSERVATION",
                        Message = $"S-threshold decreases {levels[i - 1].LevelName} -> {levels[i].LevelName} "
                                + $"({F(levels[i - 1].SThreshold)} -> {F(levels[i].SThreshold)}); verify intended pacing.",
                    });

            return f.ToArray();
        }

        public static int InvariantFailCount(BalanceFinding[] findings)
        {
            int n = 0;
            foreach (var x in findings) if (x.Severity == "INVARIANT_FAIL") n++;
            return n;
        }

        private static string F(float v) => v.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
