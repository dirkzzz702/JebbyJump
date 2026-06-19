using System;
using System.Collections.Generic;

namespace JebbyJump.Release
{
    // P23 release-readiness models. Editor-only (the whole asmdef is Editor-only),
    // but kept pure + [Serializable] with PUBLIC FIELDS so they are unit-testable
    // and serialize cleanly with JsonUtility (no interfaces / read-only props /
    // IReadOnlyList in serialized shapes).
    public enum ReleaseCheckSeverity { Info, Warning, Error }

    [Serializable]
    public struct ReleaseCheckResult
    {
        public string CheckId;
        public ReleaseCheckSeverity Severity;
        public string Message;

        public ReleaseCheckResult(string checkId, ReleaseCheckSeverity severity, string message)
        {
            CheckId = checkId;
            Severity = severity;
            Message = message;
        }

        public static ReleaseCheckResult Info(string id, string m)
            => new ReleaseCheckResult(id, ReleaseCheckSeverity.Info, m);
        public static ReleaseCheckResult Warn(string id, string m)
            => new ReleaseCheckResult(id, ReleaseCheckSeverity.Warning, m);
        public static ReleaseCheckResult Error(string id, string m)
            => new ReleaseCheckResult(id, ReleaseCheckSeverity.Error, m);
    }

    // In-memory aggregate (not serialized directly; the report copies the list
    // into a field array). Helpers are plain methods/properties, not serialized.
    public sealed class ReleasePreflightResult
    {
        public readonly List<ReleaseCheckResult> Checks = new List<ReleaseCheckResult>();

        public void Add(ReleaseCheckResult r) => Checks.Add(r);

        public int ErrorCount
        {
            get
            {
                int n = 0;
                foreach (var c in Checks)
                    if (c.Severity == ReleaseCheckSeverity.Error) n++;
                return n;
            }
        }

        public int WarningCount
        {
            get
            {
                int n = 0;
                foreach (var c in Checks)
                    if (c.Severity == ReleaseCheckSeverity.Warning) n++;
                return n;
            }
        }

        public bool Passed => ErrorCount == 0;
    }
}
