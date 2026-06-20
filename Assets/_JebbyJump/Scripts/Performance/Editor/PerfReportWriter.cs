using System.IO;
using System.Text;
using UnityEngine;

namespace JebbyJump.Performance
{
    // Writes P24 reports to an ignored Builds/P24 path. Relative artifact paths,
    // no secrets (the reports carry only metrics + asset paths).
    public static class PerfReportWriter
    {
        public static void WritePerf(PerfReport report, string dir)
        {
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "performance-baseline.json"),
                JsonUtility.ToJson(report, true));
        }

        public static void WriteBuildSize(BuildSizeAudit audit, string dir)
        {
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "build-size-audit.json"),
                JsonUtility.ToJson(audit, true));
            File.WriteAllText(Path.Combine(dir, "build-size-audit.md"), ToMarkdown(audit));
        }

        public static string ToMarkdown(BuildSizeAudit a)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# Jebby Jump — Build Size Audit (P24)");
            sb.AppendLine();
            sb.AppendLine($"- Timestamp: {a.Timestamp}  |  Unity: {a.UnityVersion}  |  Git: {a.GitCommit}");
            sb.AppendLine($"- **Compressed AAB (authoritative, P23):** {a.CompressedAabBytes} bytes "
                + $"= {a.CompressedAabMB:F2} MB (10^6) = {a.CompressedAabMiB:F2} MiB (2^20)");
            sb.AppendLine($"- P24 compressed AAB: {a.P24CompressedAabBytes} bytes  |  "
                + $"**delta: {a.AabDeltaBytes:+#;-#;0} bytes**");
            sb.AppendLine($"- Total PACKED (uncompressed-in-package): {a.TotalPackedBytes} bytes "
                + $"= {BuildSizeMath.Mb(a.TotalPackedBytes):F2} MB");
            sb.AppendLine($"- _{a.Note}_");
            sb.AppendLine();
            sb.AppendLine("## Largest packed contributors");
            foreach (var c in a.LargestContributors)
                sb.AppendLine($"- {c.PackedBytes} B ({BuildSizeMath.Mb(c.PackedBytes):F2} MB) — {c.Path}");
            return sb.ToString();
        }
    }
}
