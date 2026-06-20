using System;

namespace JebbyJump.Performance
{
    // P24 editor-only, [Serializable] field-based report models (JsonUtility-safe).
    [Serializable]
    public sealed class PerfSample
    {
        public string Metric = "";
        public double Value;
        public string Unit = "";
    }

    [Serializable]
    public sealed class PerfFlowResult
    {
        public string Flow = "";
        public int Iterations;
        public PerfSample[] Samples = Array.Empty<PerfSample>();
    }

    // Environment-tagged baseline/report (correction #5). Editor/headless metrics
    // are regression signals only — never device certification.
    [Serializable]
    public sealed class PerfReport
    {
        public string Timestamp = "";
        public string UnityVersion = "";
        public string GitCommit = "";
        public bool GitDirty;
        public string Environment = "";       // e.g. "Editor (Mono, headless batchmode)"
        public string OperatingSystem = "";
        public string QualityLevel = "";
        public int TargetFrameRate;
        public int VSyncCount;
        public string DeviceCertification =
            "NONE - editor/headless metrics are not physical-device certification";
        public PerfFlowResult[] Flows = Array.Empty<PerfFlowResult>();
        public string[] LeakChecks = Array.Empty<string>();

        public static bool IsRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            string p = path.Replace('\\', '/');
            return !p.Contains(":") && !p.StartsWith("/");
        }
    }

    [Serializable]
    public sealed class BuildSizeContributor
    {
        public string Path = "";
        public long PackedBytes;
    }

    // Distinguishes the authoritative COMPRESSED AAB size (from the persisted P23
    // report) from detailed PACKED (uncompressed-in-package) per-asset sizes from a
    // fresh BuildReport (correction #8).
    [Serializable]
    public sealed class BuildSizeAudit
    {
        public string Timestamp = "";
        public string UnityVersion = "";
        public string GitCommit = "";

        public long CompressedAabBytes;       // authoritative, from P23 report
        public double CompressedAabMB;        // 10^6
        public double CompressedAabMiB;       // 2^20
        public long P24CompressedAabBytes;    // fresh P24 AAB (if rebuilt)
        public long AabDeltaBytes;            // P24 - P23 (reported, not assumed 0)

        public long TotalPackedBytes;
        public BuildSizeContributor[] LargestContributors = Array.Empty<BuildSizeContributor>();
        public string Note =
            "Packed sizes are uncompressed-in-package and do NOT sum to the compressed AAB.";
    }
}
