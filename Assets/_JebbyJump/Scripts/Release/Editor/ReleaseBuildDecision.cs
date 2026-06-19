namespace JebbyJump.Release
{
    public enum AndroidBuildAction
    {
        BuildAab,
        ToolchainBlockedWindowsSmoke,
    }

    public enum AndroidBuildStatus
    {
        NotRun,
        AndroidAabBuilt,
        AndroidToolchainBlocked_WindowsSmokePassed,
        AndroidToolchainBlocked_WindowsSmokeFailed,
        AndroidBuildFailed,
    }

    // Pure build-decision + status mapping (fix #2): the Windows smoke build is
    // chosen ONLY when the Android toolchain is unavailable, and a real Android
    // build failure can never be masked by Windows.
    public static class ReleaseBuildDecision
    {
        public static AndroidBuildAction ResolveAction(bool androidToolchainAvailable)
            => androidToolchainAvailable
                ? AndroidBuildAction.BuildAab
                : AndroidBuildAction.ToolchainBlockedWindowsSmoke;

        public static AndroidBuildStatus MapStatus(
            bool androidToolchainAvailable,
            bool androidBuildSucceeded,
            bool windowsSmokeSucceeded)
        {
            if (androidToolchainAvailable)
                return androidBuildSucceeded
                    ? AndroidBuildStatus.AndroidAabBuilt
                    : AndroidBuildStatus.AndroidBuildFailed; // never falls back
            return windowsSmokeSucceeded
                ? AndroidBuildStatus.AndroidToolchainBlocked_WindowsSmokePassed
                : AndroidBuildStatus.AndroidToolchainBlocked_WindowsSmokeFailed;
        }
    }

    // Pure readiness verdict (exact approved wording).
    public static class ReleaseReadiness
    {
        public const string AndroidComplete =
            "Automated Android release-candidate build readiness complete";
        public const string WindowsSmokeOnly =
            "Android RC blocked by toolchain; generic build pipeline smoke test passed";
        public const string Blocked = "Release candidate blocked - see report";
        public const string Failed = "Build failed - see reported errors";

        public static string Verdict(
            bool preflightPassed, bool testsPassed, AndroidBuildStatus android,
            bool warningGatePassed, bool hashingOk, bool configMatches)
        {
            bool gates = preflightPassed && testsPassed && warningGatePassed && hashingOk && configMatches;
            if (android == AndroidBuildStatus.AndroidAabBuilt && gates)
                return AndroidComplete;
            if (android == AndroidBuildStatus.AndroidToolchainBlocked_WindowsSmokePassed && gates)
                return WindowsSmokeOnly;
            if (android == AndroidBuildStatus.AndroidBuildFailed
                || android == AndroidBuildStatus.AndroidToolchainBlocked_WindowsSmokeFailed)
                return Failed;
            return Blocked;
        }

        public static bool IsReady(string verdict)
            => verdict == AndroidComplete || verdict == WindowsSmokeOnly;
    }

    // CLI exit-code mapping (fix #6): non-zero on any non-ready outcome.
    public static class ReleaseExitCode
    {
        public static int From(string verdict) => ReleaseReadiness.IsReady(verdict) ? 0 : 1;
    }
}
