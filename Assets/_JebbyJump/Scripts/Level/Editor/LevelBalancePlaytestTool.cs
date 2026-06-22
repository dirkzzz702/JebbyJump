using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using JebbyJump.Balance;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.Level
{
    // P28 balance evidence-system preparation (Assembly-CSharp-Editor so it can read the
    // real LevelConfig/TimeRankConfig assets). It builds the per-level baseline with every
    // level EvidenceStatus=NotRun (correction #6: it does NOT collect playtest evidence),
    // the LOW-confidence review-candidate hypotheses + separate progression-economy
    // observations, and a manual-attempt CSV template. It hashes LevelConfig +
    // TimeRankConfig + LevelCatalog + the wardrobe/star threshold sources before AND after
    // to prove it changed nothing (corrections #4/#7). Mutates no data asset.
    public static class LevelBalancePlaytestTool
    {
        private const string OutputRootRel = "Builds/P28";
        private const string WardrobeSourceRel =
            "Assets/_JebbyJump/Scripts/Wardrobe/Runtime/WardrobeCatalog.cs";
        private const string StarSourceRel =
            "Assets/_JebbyJump/Scripts/Rewards/Runtime/StarRewardCalculator.cs";

        [MenuItem("Jebby Jump/Balance/Playtest Kit + Baseline")]
        public static void RunMenu()
        {
            var r = Run();
            Debug.Log($"[Balance] decision='{r.ReadinessDecision}' levels={r.LevelSummaries.Length} "
                + $"sampleSize={r.SampleSize} | {r.NoHiddenTuningProof}");
        }

        public static BalancePlaytestReport Run()
        {
            var sources = CollectSourcePaths();
            string beforeHash = HashSources(sources);

            var summaries = BuildLevelSummaries();

            string afterHash = HashSources(sources);
            bool readOnly = beforeHash == afterHash;

            var report = new BalancePlaytestReport
            {
                Commit = ShortGitCommit(),
                ExecutionMode = "PreparationOnly",
                ReadinessDecision = BalancePlaytestPolicy.Decide(sampleSize: 0, tuningApplied: false),
                SampleSize = 0,
                TesterProfilesPresent = Array.Empty<string>(),
                LevelSummaries = summaries,
                Attempts = Array.Empty<BalanceAttemptRecord>(),
                ReviewCandidates = BalanceReviewCandidateCatalog.ReviewCandidates(),
                ProgressionObservations = BalanceReviewCandidateCatalog.ProgressionObservations(),
                ReadOnlyProof = readOnly
                    ? $"READ-ONLY VERIFIED ({sources.Count} sources; hash {Short(beforeHash)})"
                    : $"WARNING: sources changed ({Short(beforeHash)} != {Short(afterHash)})",
                NoHiddenTuningProof = readOnly
                    ? $"NO-HIDDEN-TUNING VERIFIED: LevelConfig + TimeRankConfig + LevelCatalog + "
                        + $"wardrobe/star threshold sources unchanged ({sources.Count} files)"
                    : "FAILED: a tuning/protected source changed during the run",
                Timestamp = DateTime.UtcNow.ToString("o"),
            };

            WriteReport(report);
            WriteCsvTemplate(report.Commit);
            return report;
        }

        private static BalanceLevelSummary[] BuildLevelSummaries()
        {
            var list = new List<BalanceLevelSummary>();
            foreach (var guid in AssetDatabase.FindAssets("t:LevelConfig"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var cfg = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
                if (cfg == null) continue;

                float s = 0f, a = 0f, b = 0f;
                if (cfg.RankConfig != null)
                {
                    var so = new SerializedObject(cfg.RankConfig);
                    s = so.FindProperty("_sThreshold")?.floatValue ?? 0f;
                    a = so.FindProperty("_aThreshold")?.floatValue ?? 0f;
                    b = so.FindProperty("_bThreshold")?.floatValue ?? 0f;
                }

                list.Add(new BalanceLevelSummary
                {
                    LevelIndex = ParseIndex(cfg.name),
                    EvidenceStatus = "NotRun", // correction #6: no playtest evidence collected
                    SequenceLength = cfg.SequenceLength,
                    ColorCount = cfg.AvailableColors != null ? cfg.AvailableColors.Length : 0,
                    MemorySeconds = cfg.MemoryTimeSeconds,
                    StartingLives = cfg.StartingLives,
                    CactusChance = cfg.CactusSpawnChance,
                    PlatformsPerRow = cfg.PlatformsPerRow,
                    SThreshold = s, AThreshold = a, BThreshold = b,
                    AttemptsRecorded = 0,
                    Notes = "",
                });
            }
            list.Sort((x, y) => x.LevelIndex.CompareTo(y.LevelIndex));
            return list.ToArray();
        }

        // ---- read-only proof over all data + threshold-source files (corr #4) ----

        private static List<string> CollectSourcePaths()
        {
            var paths = new List<string>();
            foreach (var g in AssetDatabase.FindAssets("t:LevelConfig")) paths.Add(AssetDatabase.GUIDToAssetPath(g));
            foreach (var g in AssetDatabase.FindAssets("t:TimeRankConfig")) paths.Add(AssetDatabase.GUIDToAssetPath(g));
            foreach (var g in AssetDatabase.FindAssets("t:LevelCatalog")) paths.Add(AssetDatabase.GUIDToAssetPath(g));
            paths.Add(WardrobeSourceRel);
            paths.Add(StarSourceRel);
            paths.Sort(StringComparer.Ordinal);
            return paths;
        }

        private static string HashSources(List<string> paths)
        {
            using (var sha = SHA256.Create())
            {
                var sb = new StringBuilder();
                foreach (var rel in paths)
                {
                    string full = Path.GetFullPath(rel);
                    if (!File.Exists(full)) continue;
                    sb.Append(rel).Append(':').Append(ToHex(sha.ComputeHash(File.ReadAllBytes(full)))).Append(';');
                }
                return ToHex(sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString())));
            }
        }

        // ---- output (ignored Builds/P28; the only place a timestamp lives) ----

        private static void WriteReport(BalancePlaytestReport r)
        {
            string dir = AbsDir(r.Commit);
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "balance-playtest-report.json"), JsonUtility.ToJson(r, true));
            File.WriteAllText(Path.Combine(dir, "balance-playtest-report.md"), ToMarkdown(r));
        }

        private static void WriteCsvTemplate(string commit)
        {
            string dir = AbsDir(commit);
            Directory.CreateDirectory(dir);
            var sb = new StringBuilder();
            sb.AppendLine("# Manual playtest template - fill one row per attempt. Use a tester PROFILE label "
                + "(developer / adult casual / child (guardian-approved)); never a personal name.");
            sb.AppendLine("TesterProfile,Platform,InputMethod,BuildCommit,Level,Attempt,Completed,CompletionTimeSec,"
                + "Rank,LivesRemaining,Retries,MemoryMistakes,Falls,Difficulty1to5,Frustration1to5,Clarity1to5,Notes");
            File.WriteAllText(Path.Combine(dir, "manual-playtest-template.csv"), sb.ToString());
        }

        private static string ToMarkdown(BalancePlaytestReport r)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Jebby Jump — P28 Balance Playtest (ignored raw report)");
            sb.AppendLine();
            sb.AppendLine($"- Decision: **{r.ReadinessDecision}**");
            sb.AppendLine($"- Mode: {r.ExecutionMode}  |  Git: {r.Commit}  |  Sample size: {r.SampleSize}");
            sb.AppendLine($"- Objective: {r.DifficultyObjectiveAssumption}");
            sb.AppendLine($"- Read-only: **{r.ReadOnlyProof}**");
            sb.AppendLine($"- No-hidden-tuning: **{r.NoHiddenTuningProof}**");
            sb.AppendLine();
            sb.AppendLine("## Per-level baseline (evidence status NotRun — system prepared for all 10 levels)");
            sb.AppendLine("| Lvl | status | seq | colors | mem | lives | cactus | /row | S | A | B |");
            sb.AppendLine("| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |");
            foreach (var s in r.LevelSummaries)
                sb.AppendLine($"| {s.LevelIndex} | {s.EvidenceStatus} | {s.SequenceLength} | {s.ColorCount} | "
                    + $"{s.MemorySeconds} | {s.StartingLives} | {s.CactusChance} | {s.PlatformsPerRow} | "
                    + $"{s.SThreshold} | {s.AThreshold} | {s.BThreshold} |");
            sb.AppendLine();
            sb.AppendLine("## Review candidates (hypotheses — deferred, NOT authorized for apply)");
            foreach (var c in r.ReviewCandidates)
                sb.AppendLine($"- **{c.Id}** [{c.Category}, L{c.LevelIndex}, {c.Confidence}] {c.Hypothesis} "
                    + $"→ _{c.SuggestedDirection}_ (apply={c.AuthorizedForApply}, deferred={c.DeferredPendingPlaytest})");
            sb.AppendLine();
            sb.AppendLine("## Progression-economy observations (separate from level-balance)");
            foreach (var o in r.ProgressionObservations)
                sb.AppendLine($"- **{o.Id}** [{o.Topic}, {o.Confidence}] {o.Observation} "
                    + $"(apply={o.AuthorizedForApply}, deferred={o.DeferredPendingPlaytest})");
            return sb.ToString();
        }

        // ---- helpers ----

        private static string AbsDir(string commit)
            => Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputRootRel, commit));

        private static int ParseIndex(string name)
        {
            var m = Regex.Match(name ?? "", @"(\d+)");
            return m.Success && int.TryParse(m.Groups[1].Value, out int n) ? n : 9999;
        }

        private static string Short(string hash)
            => string.IsNullOrEmpty(hash) ? "" : hash.Substring(0, Math.Min(16, hash.Length)) + "…";

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static string ShortGitCommit()
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo("git", "rev-parse --short HEAD")
                {
                    WorkingDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..")),
                    RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true,
                };
                using (var p = System.Diagnostics.Process.Start(psi))
                {
                    string outp = p.StandardOutput.ReadToEnd();
                    p.WaitForExit(5000);
                    string s = outp.Trim();
                    return string.IsNullOrEmpty(s) ? "unknown" : s;
                }
            }
            catch { return "unknown"; }
        }
    }
}
