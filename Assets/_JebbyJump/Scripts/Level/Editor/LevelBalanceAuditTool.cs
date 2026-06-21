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
    // Read-only level-difficulty audit (P26-Balance, correction #10). Lives in
    // Assembly-CSharp-Editor (no asmdef) so it can read the gameplay LevelConfig /
    // TimeRankConfig ScriptableObjects directly, maps them to plain snapshots, and runs
    // the pure DifficultyCurvePolicy. It HASHES every level config asset before AND
    // after to PROVE it mutates nothing, then writes an ignored Builds/P26 report plus a
    // committed analysis doc. No .asset is ever modified.
    public static class LevelBalanceAuditTool
    {
        private const string OutputRootRel = "Builds/P26";
        private const string AnalysisDocRel =
            "Assets/_JebbyJump/Docs/Design/Jebby_Jump_Level_Difficulty_Analysis_v0.1.md";

        [MenuItem("Jebby Jump/Balance/Level Difficulty Audit")]
        public static void RunMenu()
        {
            var report = Run();
            Debug.Log($"[Balance] levels={report.Levels.Length} "
                + $"invariantFails={report.InvariantFailCount} | {report.ReadOnlyProof}");
        }

        public static BalanceReport Run()
        {
            // Read-only proof: hash the config assets BEFORE the analysis.
            var paths = CollectConfigAssetPaths();
            string beforeHash = HashFiles(paths);

            var snapshots = BuildSnapshots();
            var findings = DifficultyCurvePolicy.Analyze(snapshots);
            var scores = new float[snapshots.Length];
            for (int i = 0; i < snapshots.Length; i++)
                scores[i] = DifficultyCurvePolicy.HeuristicScore(snapshots[i]);

            // ...and AFTER. Equal => the audit did not touch any config asset.
            string afterHash = HashFiles(paths);
            bool readOnly = beforeHash == afterHash;

            var report = new BalanceReport
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                GitCommit = ShortGitCommit(),
                Formula = DifficultyCurvePolicy.FormulaText(),
                Levels = snapshots,
                Scores = scores,
                Findings = findings,
                InvariantFailCount = DifficultyCurvePolicy.InvariantFailCount(findings),
                ReadOnlyProof = readOnly
                    ? $"READ-ONLY VERIFIED (config asset hash unchanged: {Short(beforeHash)})"
                    : $"WARNING: config assets CHANGED during audit (before {Short(beforeHash)} != after {Short(afterHash)})",
            };

            WriteBuildsReport(report);
            WriteCommittedAnalysisDoc(report);
            return report;
        }

        private static LevelDifficultySnapshot[] BuildSnapshots()
        {
            var list = new List<LevelDifficultySnapshot>();
            foreach (var guid in AssetDatabase.FindAssets("t:LevelConfig"))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var cfg = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
                if (cfg == null) continue;

                float s = 0f, a = 0f, b = 0f;
                if (cfg.RankConfig != null)
                {
                    // Read private threshold fields via SerializedObject (read-only).
                    var so = new SerializedObject(cfg.RankConfig);
                    s = so.FindProperty("_sThreshold")?.floatValue ?? 0f;
                    a = so.FindProperty("_aThreshold")?.floatValue ?? 0f;
                    b = so.FindProperty("_bThreshold")?.floatValue ?? 0f;
                }

                list.Add(new LevelDifficultySnapshot
                {
                    LevelName = cfg.name,
                    Index = ParseIndex(cfg.name),
                    SequenceLength = cfg.SequenceLength,
                    ColorCount = cfg.AvailableColors != null ? cfg.AvailableColors.Length : 0,
                    MemorySeconds = cfg.MemoryTimeSeconds,
                    StartingLives = cfg.StartingLives,
                    CactusChance = cfg.CactusSpawnChance,
                    PlatformsPerRow = cfg.PlatformsPerRow,
                    SThreshold = s,
                    AThreshold = a,
                    BThreshold = b,
                });
            }
            list.Sort((x, y) => x.Index.CompareTo(y.Index));
            return list.ToArray();
        }

        private static int ParseIndex(string name)
        {
            var m = Regex.Match(name ?? "", @"(\d+)");
            return m.Success && int.TryParse(m.Groups[1].Value, out int n) ? n : 9999;
        }

        private static List<string> CollectConfigAssetPaths()
        {
            var paths = new List<string>();
            foreach (var g in AssetDatabase.FindAssets("t:LevelConfig"))
                paths.Add(AssetDatabase.GUIDToAssetPath(g));
            foreach (var g in AssetDatabase.FindAssets("t:TimeRankConfig"))
                paths.Add(AssetDatabase.GUIDToAssetPath(g));
            paths.Sort(StringComparer.Ordinal);
            return paths;
        }

        private static string HashFiles(List<string> assetPaths)
        {
            using (var sha = SHA256.Create())
            {
                var sb = new StringBuilder();
                foreach (var ap in assetPaths)
                {
                    string full = Path.GetFullPath(ap); // relative to the project root
                    if (!File.Exists(full)) continue;
                    sb.Append(ap).Append(':').Append(ToHex(sha.ComputeHash(File.ReadAllBytes(full)))).Append(';');
                }
                return ToHex(sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString())));
            }
        }

        private static void WriteBuildsReport(BalanceReport r)
        {
            string dir = Path.GetFullPath(Path.Combine(
                Application.dataPath, "..", OutputRootRel, r.GitCommit));
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "level-difficulty.json"), JsonUtility.ToJson(r, true));
            File.WriteAllText(Path.Combine(dir, "level-difficulty.md"),
                BalanceReportFormat.ToMarkdown(r, committedDoc: false));
        }

        private static void WriteCommittedAnalysisDoc(BalanceReport r)
        {
            string full = Path.GetFullPath(AnalysisDocRel);
            Directory.CreateDirectory(Path.GetDirectoryName(full));
            File.WriteAllText(full, BalanceReportFormat.ToMarkdown(r, committedDoc: true));
            AssetDatabase.ImportAsset(AnalysisDocRel);
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
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
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
