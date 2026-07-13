using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JebbyJump.Release
{
    // Deterministic CLI/menu release builder. Validates (never fixes) via the
    // preflight, builds the Android AAB (Windows smoke only if the Android
    // toolchain is unavailable - never to mask a real Android failure), gates
    // post-build warnings, hashes the complete distributable, and ALWAYS writes
    // the report. Editor/build state is captured + restored in try/finally; menu
    // commands never Exit; CLI exits non-zero on failure.
    public static class JebbyJumpReleaseBuilder
    {
        private const string OutputRootRel = "Builds/P23";

        // Seam so tests can verify the CLI exit-code contract without quitting.
        internal static Action<int> CliExit = EditorApplication.Exit;

        // ---- entry points ----

        [MenuItem("Jebby Jump/Release/Build Release Candidate")]
        public static void BuildReleaseCandidateMenu()
        {
            var report = BuildReleaseCandidate();
            Debug.Log($"[Release] {report.ReadinessVerdict} | Android={report.AndroidBuildStatus} "
                + $"Preflight={report.PreflightStatus} WarningGate={report.WarningGateStatus} "
                + $"Hash={report.ArtifactHashingStatus}");
            // Menu commands intentionally do NOT call EditorApplication.Exit.
        }

        public static void BuildReleaseCandidateFromCommandLine()
        {
            ReleaseReport report;
            try { report = BuildReleaseCandidate(); }
            catch (Exception e)
            {
                Debug.LogError("[Release][CLI] unhandled: " + e);
                CliExit(1);
                return;
            }
            int code = ReleaseExitCode.From(report.ReadinessVerdict);
            Debug.Log($"[Release][CLI] verdict='{report.ReadinessVerdict}' exit={code}");
            CliExit(code);
        }

        public static void BuildDevelopmentDiagnosticsFromCommandLine()
        {
            // Separate, explicitly-development diagnostics build (not an RC).
            var state = CaptureState();
            try
            {
                SetReleaseFlags(false); // development = true for diagnostics
                EditorUserBuildSettings.development = true;
                string dir = AbsOutput("WindowsDev", PlayerSettings.bundleVersion);
                Directory.CreateDirectory(dir);
                ExecuteBuild(BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone,
                    Path.Combine(dir, "JebbyJump.exe"));
            }
            finally { RestoreState(state); }
            CliExit(0);
        }

        // ---- orchestration (no Exit; always returns a written report) ----

        public static ReleaseReport BuildReleaseCandidate()
        {
            var report = NewReportSkeleton();
            bool wantApk = ReadBuildFormat() == "apk";
            report.ArtifactFormat = wantApk ? "APK" : "AAB";
            report.OutputType = wantApk ? "APK" : "AAB";
            report.DistributionPurpose = wantApk
                ? "APK: directly installable (sideload / adb / internal sharing). Signed as-is; NOT processed by Play App Signing."
                : "AAB: Google Play distribution. Play re-signs with the App Signing key and generates per-device APKs.";
            // APK and AAB land in SEPARATE output dirs + reports (correction #11).
            string outputDir = AbsOutput(wantApk ? "AndroidApk" : "Android", report.BundleVersion);
            var state = CaptureState();
            var signing = JebbyJumpReleaseSigning.Capture();
            try
            {
                // 1. Preflight (validate only).
                var pf = ReleasePreflight.RunPreflight();
                report.PreflightChecks = pf.Checks.ToArray();
                report.PreflightStatus = pf.Passed ? "Passed" : "Failed";
                report.ErrorCount = pf.ErrorCount;
                report.PackageClassifications = ClassifyPackages();
                bool testsPassed = ReadTestsStatus(report);

                if (!pf.Passed)
                {
                    report.ReadinessVerdict = ReleaseReadiness.Blocked;
                    return report;
                }

                // 2. Choose action by toolchain availability.
                bool toolchain = IsAndroidToolchainAvailable();
                var action = ReleaseBuildDecision.ResolveAction(toolchain);

                SetReleaseFlags(true); // forces Development=false etc.

                bool androidOk = false, windowsOk = false, warningGateOk = true, hashingOk = true;

                if (action == AndroidBuildAction.BuildAab)
                {
                    // Resolve signing intent FIRST. Upload intent fails HARD on any
                    // missing/invalid input - it never silently falls back to debug.
                    var signRes = JebbyJumpReleaseSigning.ResolveFromEnvironment();
                    report.SigningIntent = signRes.Intent;
                    report.SigningResolutionReason = signRes.Reason;
                    report.SigningStatus = SigningResolution.StatusString(signRes);
                    if (signRes.BuildShouldFail)
                    {
                        // Signing config refused BEFORE any build attempt -> leave
                        // AndroidBuildStatus at NotRun (accurate: the build never ran);
                        // SigningStatus + verdict explain the refusal.
                        report.ReadinessVerdict = ReleaseReadiness.Blocked;
                        return report; // finally restores build + signing state
                    }
                    JebbyJumpReleaseSigning.ApplyResolved(signRes);

                    EditorUserBuildSettings.buildAppBundle = !wantApk;
                    string artifact = Path.Combine(outputDir, wantApk ? "JebbyJump.apk" : "JebbyJump.aab");
                    Directory.CreateDirectory(outputDir);
                    var br = ExecuteBuild(BuildTarget.Android, BuildTargetGroup.Android, artifact);
                    androidOk = br.summary.result == BuildResult.Succeeded;
                    FillBuildSummary(report, br);
                    warningGateOk = EvaluateWarnings(report, br);
                    if (androidOk)
                    {
                        hashingOk = HashSingleFile(report, br.summary.outputPath);
                        InspectSignedArtifact(report, br.summary.outputPath, wantApk);
                    }
                    else hashingOk = false; // nothing to hash
                }
                else
                {
                    // Toolchain unavailable -> Windows smoke (pipeline proof only).
                    string winDir = AbsOutput("Windows", report.BundleVersion);
                    Directory.CreateDirectory(winDir);
                    var br = ExecuteBuild(BuildTarget.StandaloneWindows64,
                        BuildTargetGroup.Standalone, Path.Combine(winDir, "JebbyJump.exe"));
                    windowsOk = br.summary.result == BuildResult.Succeeded;
                    report.Target = "Android(blocked)->WindowsSmoke";
                    report.OutputType = "StandaloneWindows64";
                    FillBuildSummary(report, br);
                    warningGateOk = EvaluateWarnings(report, br);
                    report.SigningStatus = "NotApplicable";
                    if (windowsOk) hashingOk = HashDirectory(report, winDir);
                    else hashingOk = false;
                }

                var android = ReleaseBuildDecision.MapStatus(toolchain, androidOk, windowsOk);
                report.AndroidBuildStatus = android.ToString();
                report.WindowsSmokeStatus = toolchain ? "NotRun" : (windowsOk ? "Passed" : "Failed");
                report.WarningGateStatus = warningGateOk ? "Passed" : "Failed";
                report.ArtifactHashingStatus = hashingOk ? "Passed" : "Failed";

                report.ReadinessVerdict = ReleaseReadiness.Verdict(
                    pf.Passed, testsPassed, android, warningGateOk, hashingOk, configMatches: true);
                return report;
            }
            catch (Exception e)
            {
                report.ReadinessVerdict = ReleaseReadiness.Failed;
                report.BuildResult = "Exception: " + e.Message;
                return report;
            }
            finally
            {
                RestoreState(state);
                // Byte-for-byte restore of the signing config (correction #3); nothing
                // secret is ever persisted to ProjectSettings.asset.
                JebbyJumpReleaseSigning.Restore(signing);
                report.SigningConfigRestored =
                    JebbyJumpReleaseSigning.VerifyRestored(signing) ? "Restored" : "DRIFT";
                try { ReleaseReportWriter.Write(report, outputDir); }
                catch (Exception e) { Debug.LogError("[Release] report write failed: " + e); }
            }
        }

        // ---- build state guard (fix #6) ----

        public struct BuildStateSnapshot
        {
            public BuildTarget Target;
            public BuildTargetGroup Group;
            public bool AppBundle, Development, AllowDebugging, ConnectProfiler, DeepProfiling;
        }

        public static BuildStateSnapshot CaptureState() => new BuildStateSnapshot
        {
            Target = EditorUserBuildSettings.activeBuildTarget,
            Group = EditorUserBuildSettings.selectedBuildTargetGroup,
            AppBundle = EditorUserBuildSettings.buildAppBundle,
            Development = EditorUserBuildSettings.development,
            AllowDebugging = EditorUserBuildSettings.allowDebugging,
            ConnectProfiler = EditorUserBuildSettings.connectProfiler,
            DeepProfiling = EditorUserBuildSettings.buildWithDeepProfilingSupport,
        };

        public static void RestoreState(BuildStateSnapshot s)
        {
            EditorUserBuildSettings.buildAppBundle = s.AppBundle;
            EditorUserBuildSettings.development = s.Development;
            EditorUserBuildSettings.allowDebugging = s.AllowDebugging;
            EditorUserBuildSettings.connectProfiler = s.ConnectProfiler;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = s.DeepProfiling;
            if (EditorUserBuildSettings.activeBuildTarget != s.Target)
                EditorUserBuildSettings.SwitchActiveBuildTarget(s.Group, s.Target);
        }

        private static void SetReleaseFlags(bool release)
        {
            EditorUserBuildSettings.development = !release;
            EditorUserBuildSettings.allowDebugging = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
        }

        // ---- toolchain detection (reflection so a missing module can't break compile) ----

        public static bool IsAndroidToolchainAvailable()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
                return false;
            try
            {
                var t = Type.GetType(
                    "UnityEditor.Android.AndroidExternalToolsSettings, UnityEditor.Android.Extensions");
                if (t == null) return false; // cannot positively confirm -> treat as unavailable
                string sdk = StaticString(t, "sdkRootPath");
                string ndk = StaticString(t, "ndkRootPath");
                string jdk = StaticString(t, "jdkRootPath");
                return PathOk(sdk) && PathOk(ndk) && PathOk(jdk);
            }
            catch { return false; }
        }

        private static string StaticString(Type t, string prop)
            => t.GetProperty(prop, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as string;

        private static bool PathOk(string p) => !string.IsNullOrEmpty(p) && Directory.Exists(p);

        // ---- build execution (uses the immutable scene contract directly, fix #9) ----

        private static BuildReport ExecuteBuild(
            BuildTarget target, BuildTargetGroup group, string locationPathName)
        {
            var opts = new BuildPlayerOptions
            {
                scenes = ReleaseSceneContract.Copy(),
                locationPathName = locationPathName,
                target = target,
                targetGroup = group,
                options = BuildOptions.None, // non-development release
            };
            return BuildPipeline.BuildPlayer(opts);
        }

        // ---- report population ----

        private static ReleaseReport NewReportSkeleton()
        {
            var r = new ReleaseReport
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                UnityVersion = Application.unityVersion,
                Target = "Android",
                OutputType = "AAB",
                CompanyName = PlayerSettings.companyName,
                ProductName = PlayerSettings.productName,
                ApplicationIdentifier = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android),
                BundleVersion = string.IsNullOrEmpty(PlayerSettings.bundleVersion) ? "0.0" : PlayerSettings.bundleVersion,
                AndroidVersionCode = PlayerSettings.Android.bundleVersionCode,
                DevelopmentBuild = false,
                ScriptingBackend = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android).ToString(),
                Architectures = PlayerSettings.Android.targetArchitectures.ToString(),
                ManagedStripping = PlayerSettings.GetManagedStrippingLevel(NamedBuildTarget.Android).ToString(),
                Scenes = ReleaseSceneContract.Copy(),
                StoreUploadReady = false,
                ManualQaStatus = "DEFERRED / NOT VERIFIED",
            };
            FillGit(r);
            return r;
        }

        private static void FillGit(ReleaseReport r)
        {
            r.GitCommit = RunGit("rev-parse --short HEAD") ?? "unknown";
            string status = RunGit("status --porcelain");
            r.GitDirty = !string.IsNullOrEmpty(status);
        }

        private static string RunGit(string args)
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo("git", args)
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
                    return outp.Trim();
                }
            }
            catch { return null; }
        }

        private static bool ReadTestsStatus(ReleaseReport r)
        {
            // The authoritative verification runs tests first and exports
            // JJ_TESTS_PASSED=1 (+ optional counts) so the verdict can include the
            // test gate honestly. Absent => tests not verified in this run.
            string passed = Environment.GetEnvironmentVariable("JJ_TESTS_PASSED");
            int.TryParse(Environment.GetEnvironmentVariable("JJ_TESTS_TOTAL") ?? "0", out r.TestsTotal);
            int.TryParse(Environment.GetEnvironmentVariable("JJ_TESTS_FAILED") ?? "0", out r.TestsFailed);
            bool ok = passed == "1";
            r.TestsPassed = ok ? r.TestsTotal : 0;
            r.TestsStatus = ok ? "Passed" : "NotVerifiedInThisRun";
            return ok;
        }

        private static string[] ClassifyPackages()
        {
            var snap = ReleasePreflight.GatherSnapshot();
            var lines = new List<string>();
            foreach (var id in snap.PackageIds)
            {
                var info = ReleasePackageClassification.Classify(id);
                lines.Add($"{id} = {info.Category} ({info.Reason})");
            }
            return lines.ToArray();
        }

        private static void FillBuildSummary(ReleaseReport r, BuildReport br)
        {
            r.BuildResult = br.summary.result.ToString();
            r.BuildDurationSeconds = br.summary.totalTime.TotalSeconds;
            r.TotalSizeBytes = (long)br.summary.totalSize;
        }

        private static bool EvaluateWarnings(ReleaseReport r, BuildReport br)
        {
            var classified = new List<string>();
            var unclassified = new List<string>();
            int warnings = 0, errors = 0;
            foreach (var step in br.steps)
                foreach (var msg in step.messages)
                {
                    if (msg.type == LogType.Warning)
                    {
                        warnings++;
                        if (ReleaseWarningAllowlist.IsAllowed(msg.content)) classified.Add(msg.content);
                        else unclassified.Add(msg.content);
                    }
                    else if (msg.type == LogType.Error || msg.type == LogType.Exception || msg.type == LogType.Assert)
                    {
                        errors++;
                    }
                }
            r.WarningCount = warnings;
            r.ErrorCount += errors;
            r.ClassifiedWarnings = classified.ToArray();
            r.UnclassifiedWarnings = unclassified.ToArray();
            return unclassified.Count == 0;
        }

        private static string ReadBuildFormat()
        {
            string f = (Environment.GetEnvironmentVariable("JJ_BUILD_FORMAT") ?? "aab")
                .Trim().ToLowerInvariant();
            return f == "apk" ? "apk" : "aab";
        }

        // Verifies the ACTUAL artifact signature (+ APK target SDK & 16KB pages) using
        // the real toolchain; degrades to an honest Skipped when a tool is unavailable.
        private static void InspectSignedArtifact(ReleaseReport r, string artifactPath, bool isApk)
        {
            var sig = isApk
                ? AndroidArtifactInspector.VerifyApk(artifactPath)
                : AndroidArtifactInspector.VerifyAab(artifactPath);
            r.SignatureVerifyStatus = sig.Status;
            r.SignatureVerifyTool = sig.Tool;
            r.SignerCertSha256 = sig.CertSha256 ?? "";
            r.StoreUploadReady = false; // never claimed
            if (isApk)
            {
                var tsdk = AndroidArtifactInspector.ReadApkTargetSdk(artifactPath);
                r.ResolvedArtifactTargetSdk = tsdk.ResolvedTargetSdk;
                r.ResolvedTargetSdkStatus = tsdk.Status;
                r.PageSize16kStatus = AndroidArtifactInspector.CheckApk16kAlignment(artifactPath).Status;
            }
            else
            {
                // The AAB target SDK lives in a protobuf manifest; the resolved value is
                // read from the APK artifact / installed platforms (store-compliance audit).
                r.ResolvedTargetSdkStatus = "Skipped(aab-protobuf-manifest)";
                r.PageSize16kStatus = "Skipped(aab)";
            }
        }

        private static bool HashSingleFile(ReleaseReport r, string artifactPath)
        {
            try
            {
                if (string.IsNullOrEmpty(artifactPath) || !File.Exists(artifactPath)) return false;
                string root = AbsOutputRoot();
                r.PrimaryArtifactPath = ArtifactHasher.Relative(root, artifactPath);
                r.PrimaryArtifactBytes = new FileInfo(artifactPath).Length;
                r.PrimaryArtifactSha256 = ArtifactHasher.Sha256File(artifactPath);
                return true;
            }
            catch (Exception e) { Debug.LogError("[Release] hash failed: " + e); return false; }
        }

        private static bool HashDirectory(ReleaseReport r, string dir)
        {
            try
            {
                if (!Directory.Exists(dir)) return false;
                string root = AbsOutputRoot();
                r.ArtifactManifest = ArtifactHasher.Sha256Directory(dir, root);
                r.PrimaryArtifactPath = ArtifactHasher.Relative(root, dir);
                long total = 0;
                foreach (var f in r.ArtifactManifest) total += f.Bytes;
                r.PrimaryArtifactBytes = total;
                return r.ArtifactManifest.Length > 0;
            }
            catch (Exception e) { Debug.LogError("[Release] dir hash failed: " + e); return false; }
        }

        // ---- paths ----

        private static string AbsOutputRoot()
            => Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputRootRel));

        private static string AbsOutput(string target, string version)
            => Path.Combine(AbsOutputRoot(), target, string.IsNullOrEmpty(version) ? "0.0" : version);

        [MenuItem("Jebby Jump/Release/Open Latest Build Report")]
        public static void OpenLatestReport()
        {
            string root = AbsOutputRoot();
            if (Directory.Exists(root)) EditorUtility.RevealInFinder(root);
            else Debug.Log("[Release] no Builds/P23 output yet.");
        }
    }
}
