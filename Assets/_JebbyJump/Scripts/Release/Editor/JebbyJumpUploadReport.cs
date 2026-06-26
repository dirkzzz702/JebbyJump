using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.Release
{
    // P32 upload-distribution evidence (PREPARATION ONLY / BLOCKED). Assembles the five
    // separate upload/console statuses + supporting compliance statuses + enumerated
    // blockers into an IGNORED Builds/P32 report. Reads signing env PRESENCE only (never
    // values, corr #3); refuses to write secrets, tester emails, local paths, or env dumps
    // (corr #9). It performs NO upload and NO Console action.
    public static class JebbyJumpUploadReport
    {
        private const string OutputRootRel = "Builds/P32";
        private const string ApkReportRel = "Builds/P23/AndroidApk/1.0/release-report.json";

        [MenuItem("Jebby Jump/Release/Upload Distribution Report")]
        public static void RunMenu()
        {
            var r = Run();
            Debug.Log($"[Upload] decision='{r.ReadinessDecision}' uploadKey={r.UploadKeyStatus} "
                + $"signedArtifact={r.UploadKeySignedArtifactStatus} console={r.PlayConsoleActionStatus} "
                + $"internalTrack={r.InternalTrackUploadStatus} blockers={r.Blockers.Length}");
        }

        public static UploadDistributionReport Run()
        {
            var apk = ReadReleaseReport(ApkReportRel);
            int resolvedSdk = apk != null ? apk.ResolvedArtifactTargetSdk : 0;
            string page16 = apk != null && !string.IsNullOrEmpty(apk.PageSize16kStatus)
                ? apk.PageSize16kStatus : "unknown";

            // env PRESENCE only - never values (corr #3)
            bool ks = HasEnv(JebbyJumpReleaseSigning.EnvKeystore);
            bool kp = HasEnv(JebbyJumpReleaseSigning.EnvKsPass);
            bool al = HasEnv(JebbyJumpReleaseSigning.EnvAlias);
            bool ap = HasEnv(JebbyJumpReleaseSigning.EnvAliasPass);
            bool allEnv = ks && kp && al && ap;

            var r = new UploadDistributionReport
            {
                Commit = ShortGitCommit(),
                ExecutionMode = "PreparationOnly-Blocked",
                UploadKeyStatus = allEnv ? "PresentFromEnv" : UploadStatus.NotProvided,
                UploadKeySignedArtifactStatus = UploadStatus.NotBuilt, // no upload-signed AAB built this run
                PlayAppSigningStatus = UploadStatus.NotConfigured,
                PlayConsoleActionStatus = UploadStatus.NotRun,
                InternalTrackUploadStatus = UploadStatus.NotRun,
                TargetSdkStatus = resolvedSdk >= StoreCompliancePolicy.AssumedPlayMinTargetSdk
                    ? UploadStatus.Passed : UploadStatus.NotRun,
                PageSize16KbStatus = page16 == "Aligned16k" ? UploadStatus.Passed : UploadStatus.NotRun,
                PrivacyPolicyStatus = UploadStatus.Blocked,
                GraphicAssetsStatus = UploadStatus.Blocked,
                DataSafetyStatus = "Draft",
                VersionCodeStatus = UploadStatus.NotVerified,   // corr #5: no Console evidence
                PhysicalInstallStatus = UploadStatus.NotRun,
                SubmittedInConsole = false,
                VerifiedInConsole = false,
                SigningEnvPresence =
                    $"upload-signing env (presence only): keystore={YN(ks)} secret1={YN(kp)} "
                    + $"alias={YN(al)} secret2={YN(ap)}",
                SignedArtifactFingerprint = "",
                ExpectedUploadFingerprint = "",  // N/A without an upload key
                FailHardProof = "env-upload + no keystore => build refused (Blocked); no AAB; signing "
                    + "state restored. See the Upload Signing Record + the release report.",
                Blockers = Blockers(allEnv),
            };
            r.ReadinessDecision = UploadDistributionPolicy.Decide(
                uploadKeySignedArtifact: false, internalTrackUploaded: false,
                consoleEvidencePresent: false, deviceSmokePassed: false, anyBlockerRemains: true);

            Write(r);
            return r;
        }

        private static string[] Blockers(bool allEnv) => new[]
        {
            allEnv ? "Upload keystore env present, but no upload-signed AAB built this run"
                   : "Upload keystore not provided (no signed AAB possible)",
            "Play Console account / authorization absent (no upload, no declarations)",
            "Hosted privacy-policy URL missing",
            "Store listing graphics missing (icon / feature / screenshots)",
            "Data Safety + content-rating not user-approved",
            "Tester list not provided",
            "Physical Play-distributed install NOT RUN (no device)",
        };

        private static bool HasEnv(string name) => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name));
        private static string YN(bool b) => b ? "PRESENT" : "MISSING";

        private static ReleaseReport ReadReleaseReport(string rel)
        {
            try
            {
                string full = Path.GetFullPath(Path.Combine(Application.dataPath, "..", rel));
                if (!File.Exists(full)) return null;
                return JsonUtility.FromJson<ReleaseReport>(File.ReadAllText(full));
            }
            catch { return null; }
        }

        private static void Write(UploadDistributionReport r)
        {
            string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", OutputRootRel, r.Commit));
            Directory.CreateDirectory(dir);
            string json = JsonUtility.ToJson(r, true);
            if (UploadDistributionReport.ContainsSecretLike(json))
                throw new InvalidOperationException("[Upload] secret-like text; aborting write.");
            if (UploadDistributionReport.ContainsTesterEmail(json))
                throw new InvalidOperationException("[Upload] email-like token; aborting write.");
            if (UploadDistributionReport.ContainsLocalPathOrEnvDump(json))
                throw new InvalidOperationException("[Upload] local path / env dump; aborting write.");
            File.WriteAllText(Path.Combine(dir, "signed-aab-report.json"), json);
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
