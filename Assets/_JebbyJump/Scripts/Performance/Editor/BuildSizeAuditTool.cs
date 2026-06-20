using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JebbyJump.Performance
{
    // P24 build-size audit (corrections #8,#9): authoritative COMPRESSED AAB size
    // from the persisted P23 report; detailed PACKED (uncompressed-in-package)
    // per-asset contributors from the latest BuildReport (no rebuild needed for the
    // audit). Distinguishes bytes / MB / MiB / packed / compressed; reports the AAB
    // delta without assuming zero. Writes to ignored Builds/P24.
    public static class BuildSizeAuditTool
    {
        [MenuItem("Jebby Jump/Performance/Build Size Audit")]
        public static void Run()
        {
            var audit = new BuildSizeAudit
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                UnityVersion = Application.unityVersion,
                GitCommit = GitShort(),
            };

            long p23 = ReadP23AabBytes();
            audit.CompressedAabBytes = p23;
            audit.CompressedAabMB = BuildSizeMath.Mb(p23);
            audit.CompressedAabMiB = BuildSizeMath.MiB(p23);

            var report = BuildReport.GetLatestReport();
            if (report != null)
            {
                var packed = new List<KeyValuePair<string, long>>();
                long total = 0;
                foreach (var pa in report.packedAssets)
                    foreach (var info in pa.contents)
                    {
                        long sz = (long)info.packedSize;
                        if (!string.IsNullOrEmpty(info.sourceAssetPath))
                            packed.Add(new KeyValuePair<string, long>(info.sourceAssetPath, sz));
                        total += sz;
                    }
                audit.TotalPackedBytes = total;
                audit.LargestContributors = BuildSizeMath.TopContributors(packed, 25);

                string outPath = report.summary.outputPath;
                if (!string.IsNullOrEmpty(outPath) && File.Exists(outPath))
                    audit.P24CompressedAabBytes = new FileInfo(outPath).Length;
                audit.AabDeltaBytes = audit.P24CompressedAabBytes - p23;
            }
            else
            {
                Debug.LogWarning("[P24] No BuildReport.GetLatestReport(); run an AAB build first for packed detail.");
            }

            string dir = Path.Combine(OutputRoot(), string.IsNullOrEmpty(audit.GitCommit) ? "local" : audit.GitCommit);
            PerfReportWriter.WriteBuildSize(audit, dir);
            Debug.Log($"[P24] Build-size audit -> {dir}: compressed AAB baseline {p23} B "
                + $"({audit.CompressedAabMiB:F1} MiB), packed total {audit.TotalPackedBytes} B, "
                + $"{audit.LargestContributors.Length} contributors, AAB delta {audit.AabDeltaBytes} B.");
        }

        private static long ReadP23AabBytes()
        {
            string path = Path.GetFullPath(Path.Combine(
                Application.dataPath, "..", "Builds/P23/Android/1.0/release-report.json"));
            if (!File.Exists(path)) return 0;
            try
            {
                var shape = JsonUtility.FromJson<P23Shape>(File.ReadAllText(path));
                return shape != null ? shape.PrimaryArtifactBytes : 0;
            }
            catch { return 0; }
        }

        [Serializable] private sealed class P23Shape { public long PrimaryArtifactBytes; }

        private static string OutputRoot()
            => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Builds/P24"));

        private static string GitShort()
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
                using var p = System.Diagnostics.Process.Start(psi);
                string outp = p.StandardOutput.ReadToEnd();
                p.WaitForExit(5000);
                return outp.Trim();
            }
            catch { return ""; }
        }
    }
}
