using JebbyJump.Release;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    public class UploadDistributionTests
    {
        [Test]
        public void AabSignature_MatchesExpectedUploadFingerprint_IgnoringColonsAndCase()
        {
            Assert.IsTrue(UploadDistributionPolicy.FingerprintMatches("AB:CD:ef", "abcdEF"));
            Assert.IsTrue(UploadDistributionPolicy.FingerprintMatches("c3 92 26 b2", "C39226B2"));
            Assert.IsFalse(UploadDistributionPolicy.FingerprintMatches("abcd", "abce"));
            Assert.IsFalse(UploadDistributionPolicy.FingerprintMatches("", "abcd"));
            Assert.IsFalse(UploadDistributionPolicy.FingerprintMatches("abcd", ""));
        }

        [Test]
        public void InternalTrack_CannotClaimCompleteWithoutConsoleEvidence()
        {
            Assert.IsFalse(UploadDistributionPolicy.CanClaimInternalTrackComplete(false, true));
            Assert.IsFalse(UploadDistributionPolicy.CanClaimInternalTrackComplete(true, false));
            Assert.IsTrue(UploadDistributionPolicy.CanClaimInternalTrackComplete(true, true));
        }

        [Test]
        public void Decide_ThisRunIsBlocked()
        {
            Assert.AreEqual("P32 blocked - see Play distribution blocker report", UploadDistributionPolicy.Blocked);
            Assert.AreEqual(UploadDistributionPolicy.Blocked, UploadDistributionPolicy.Decide(
                uploadKeySignedArtifact: false, internalTrackUploaded: false,
                consoleEvidencePresent: false, deviceSmokePassed: false, anyBlockerRemains: true));
        }

        [Test]
        public void Decide_CompleteWordingOnlyWithFullEvidence()
        {
            Assert.AreEqual(UploadDistributionPolicy.CompleteNoDevice, UploadDistributionPolicy.Decide(
                true, true, true, deviceSmokePassed: false, anyBlockerRemains: false));
            Assert.AreEqual(UploadDistributionPolicy.ValidatedOnDevices, UploadDistributionPolicy.Decide(
                true, true, true, deviceSmokePassed: true, anyBlockerRemains: false));
            Assert.AreEqual(UploadDistributionPolicy.Blocked, UploadDistributionPolicy.Decide(
                true, true, true, true, anyBlockerRemains: true));
        }

        [Test]
        public void VersionCode_MustBeIncreasing()
        {
            Assert.IsTrue(VersionCodePolicy.IsValidNextCode(2, 1));
            Assert.IsFalse(VersionCodePolicy.IsValidNextCode(1, 1));
        }

        [Test]
        public void UploadReport_DoesNotContainSecretsOrTesterEmails_OrPathsOrEnvDumps() // corr #9
        {
            Assert.IsTrue(UploadDistributionReport.ContainsSecretLike("keystorePass=hunter2"));
            Assert.IsTrue(UploadDistributionReport.ContainsTesterEmail("tester@example.com"));
            Assert.IsTrue(UploadDistributionReport.ContainsLocalPathOrEnvDump(@"C:\Users\me\upload.keystore"));
            Assert.IsTrue(UploadDistributionReport.ContainsLocalPathOrEnvDump("JJ_ANDROID_KEYSTORE_PASS=secretvalue"));
            Assert.IsFalse(UploadDistributionReport.ContainsLocalPathOrEnvDump("keystore=MISSING secret1=MISSING"));
            Assert.IsFalse(UploadDistributionReport.ContainsLocalPathOrEnvDump("Builds/P32/abc/report.json"));
        }

        [Test]
        public void Report_SeparatesFiveStatuses_AndConsoleFlagsDefaultFalse() // corr #5,#7,#8
        {
            var r = new UploadDistributionReport();
            string json = JsonUtility.ToJson(r);
            foreach (var f in new[]
            {
                "UploadKeyStatus", "UploadKeySignedArtifactStatus", "PlayAppSigningStatus",
                "PlayConsoleActionStatus", "InternalTrackUploadStatus", "SubmittedInConsole",
                "VerifiedInConsole", "VersionCodeStatus", "Blockers",
            })
                StringAssert.Contains(f, json);
            Assert.IsFalse(r.SubmittedInConsole);
            Assert.IsFalse(r.VerifiedInConsole);
            Assert.AreEqual(UploadStatus.NotVerified, r.VersionCodeStatus); // corr #5
        }
    }
}
