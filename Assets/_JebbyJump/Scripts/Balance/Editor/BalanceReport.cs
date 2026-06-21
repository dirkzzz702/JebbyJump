using System;
using System.Globalization;
using System.Text;

namespace JebbyJump.Balance
{
    [Serializable]
    public sealed class BalanceReport
    {
        public string Timestamp = "";
        public string GitCommit = "";
        public string Formula = "";
        public string Confidence =
            "LOW — cross-level scoring is a heuristic pending the P4B human playtest. "
            + "Only intra-level S<A<B threshold ordering is a hard invariant.";
        public LevelDifficultySnapshot[] Levels = Array.Empty<LevelDifficultySnapshot>();
        public float[] Scores = Array.Empty<float>();
        public BalanceFinding[] Findings = Array.Empty<BalanceFinding>();
        public int InvariantFailCount;
        public string ReadOnlyProof = "";   // before/after asset-hash equality (correction #10)
    }

    // Pure markdown rendering (no file IO) so the Assembly-CSharp-Editor tool can write
    // both the ignored Builds/P26 report and the committed analysis doc.
    public static class BalanceReportFormat
    {
        public static string ToMarkdown(BalanceReport r, bool committedDoc)
        {
            var sb = new StringBuilder();
            sb.AppendLine(committedDoc
                ? "# Jebby Jump — Level Difficulty Analysis v0.1 (P26)"
                : "# Jebby Jump — Level Difficulty Audit (P26)");
            sb.AppendLine();
            sb.AppendLine("Data-driven, **read-only** analysis of the 10 LevelConfig + TimeRankConfig "
                + "assets. Proposals are heuristic and **LOW-CONFIDENCE**; no balance values are "
                + "changed by this audit (thresholds stay as authored until the P4B playtest).");
            sb.AppendLine();
            sb.AppendLine($"- Timestamp: {r.Timestamp}  |  Git: {r.GitCommit}");
            sb.AppendLine($"- Read-only proof: **{r.ReadOnlyProof}**");
            sb.AppendLine($"- Confidence: {r.Confidence}");
            sb.AppendLine($"- Heuristic: `{r.Formula}`");
            sb.AppendLine($"- Hard-invariant (S<A<B) failures: **{r.InvariantFailCount}**");
            sb.AppendLine();

            sb.AppendLine("## Difficulty curve");
            sb.AppendLine();
            sb.AppendLine("| Lvl | seq | colors | mem(s) | lives | cactus | per-row | S | A | B | heuristic |");
            sb.AppendLine("| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |");
            for (int i = 0; i < r.Levels.Length; i++)
            {
                var s = r.Levels[i];
                float score = (r.Scores != null && i < r.Scores.Length) ? r.Scores[i] : 0f;
                sb.AppendLine($"| {s.Index} | {s.SequenceLength} | {s.ColorCount} | {F(s.MemorySeconds)} | "
                    + $"{s.StartingLives} | {F(s.CactusChance)} | {s.PlatformsPerRow} | {F(s.SThreshold)} | "
                    + $"{F(s.AThreshold)} | {F(s.BThreshold)} | {F(score)} |");
            }
            sb.AppendLine();

            sb.AppendLine("## Findings");
            if (r.Findings == null || r.Findings.Length == 0)
                sb.AppendLine("- (none)");
            else
                foreach (var f in r.Findings)
                    sb.AppendLine($"- **[{f.Severity}] {f.CheckId}** — {f.Message}");
            sb.AppendLine();

            sb.AppendLine("## Proposals (LOW-CONFIDENCE — for P4B playtest, not applied)");
            sb.AppendLine("- Treat any `curve.monotonicity` / `threshold.trend` observation as a *question*, "
                + "not a defect: confirm the intended pacing by playtesting before changing any value.");
            sb.AppendLine("- Any `INVARIANT_FAIL` is a real ordering bug and should be fixed (S<A<B).");
            sb.AppendLine("- Cross-level numeric tuning is DEFERRED to P4B with human playtest data + approval.");
            sb.AppendLine();
            sb.AppendLine("_No LevelConfig/TimeRankConfig asset is modified by this audit._");
            return sb.ToString();
        }

        private static string F(float v) => v.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
