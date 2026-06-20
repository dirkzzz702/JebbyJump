using System;
using System.Collections.Generic;

namespace JebbyJump.Performance
{
    public enum PerfRegression { Ok, Leak, UnexpectedGcAlloc, TimingRegression }

    // P24 regression policy (correction #5 strictness): leaks + unexpected
    // steady-state GC = zero tolerance; timing = RELATIVE regression only (never
    // an absolute device-certification limit for editor/headless runs).
    public static class PerformanceRegressionPolicy
    {
        public static bool IsLeak(long baselineCount, long currentCount)
            => currentCount > baselineCount;

        public static bool IsUnexpectedGcAlloc(bool expectedZero, bool allocated)
            => expectedZero && allocated;

        public static bool IsTimingRegression(double baselineMs, double currentMs, double tolerancePct)
            => baselineMs > 0.0 && currentMs > baselineMs * (1.0 + tolerancePct / 100.0);

        public static PerfRegression Classify(bool leak, bool unexpectedGc, bool timingRegressed)
        {
            if (leak) return PerfRegression.Leak;
            if (unexpectedGc) return PerfRegression.UnexpectedGcAlloc;
            if (timingRegressed) return PerfRegression.TimingRegression;
            return PerfRegression.Ok;
        }

        // P24 never claims physical-device certification regardless of editor timings.
        public static bool ClaimsDeviceCertification => false;
    }

    public static class BuildSizeMath
    {
        public static double Mb(long bytes) => bytes / 1_000_000.0;   // 10^6
        public static double MiB(long bytes) => bytes / 1048576.0;    // 2^20

        // Top-N contributors by descending packed size. Pure + testable.
        public static BuildSizeContributor[] TopContributors(
            IList<KeyValuePair<string, long>> packed, int n)
        {
            if (packed == null || packed.Count == 0 || n <= 0)
                return Array.Empty<BuildSizeContributor>();
            var list = new List<BuildSizeContributor>(packed.Count);
            foreach (var kv in packed)
                list.Add(new BuildSizeContributor { Path = kv.Key, PackedBytes = kv.Value });
            list.Sort((a, b) => b.PackedBytes.CompareTo(a.PackedBytes));
            if (list.Count > n) list.RemoveRange(n, list.Count - n);
            return list.ToArray();
        }
    }
}
