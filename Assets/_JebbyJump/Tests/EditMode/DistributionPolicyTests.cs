using JebbyJump.Release;
using NUnit.Framework;

namespace JebbyJump.Tests.EditMode
{
    public class DistributionPolicyTests
    {
        // ---- config writer dry-run / refuse by default (corr #5,#10) ----
        [Test]
        public void ConfigWriter_DryRunsByDefault()
        {
            var d = DistributionConfigGate.ShouldApply(DistributionConfig.CurrentBaseline(), 0);
            Assert.IsFalse(d.Apply);
            StringAssert.Contains("DRY-RUN", d.Reason);
        }

        [Test]
        public void ConfigWriter_RefusesNonIncreasingVersionCode()
        {
            var c = new DistributionConfig { Approved = true, VersionName = "1.0", VersionCode = 5, TargetSdk = 0 };
            Assert.IsFalse(DistributionConfigGate.ShouldApply(c, 5).Apply); // not greater than last
            Assert.IsTrue(DistributionConfigGate.ShouldApply(c, 4).Apply);  // greater than last
        }

        [Test]
        public void ConfigWriter_RefusesTooLowPinnedTargetSdk()
        {
            var c = new DistributionConfig { Approved = true, VersionName = "1.0", VersionCode = 2, TargetSdk = 30 };
            Assert.IsFalse(DistributionConfigGate.ShouldApply(c, 1).Apply);
        }

        // ---- version code explicit + increasing (corr #10) ----
        [Test]
        public void VersionCode_MustBeExplicitAndIncreasing()
        {
            Assert.IsTrue(VersionCodePolicy.IsValidNextCode(2, 1));
            Assert.IsFalse(VersionCodePolicy.IsValidNextCode(1, 1));
            Assert.IsFalse(VersionCodePolicy.IsValidNextCode(0, 0));
            Assert.IsFalse(VersionCodePolicy.IsValidNextCode(1, 2));
        }

        // ---- dirty-tree reporting (corr #1,#10) ----
        [Test]
        public void TreeState_ReportsDirtyWhenTrackedModified()
        {
            Assert.IsTrue(TreeStatePolicy.IsClean(""));
            Assert.IsTrue(TreeStatePolicy.IsClean("   "));
            Assert.IsFalse(TreeStatePolicy.IsClean(" M .gitignore"));
            Assert.IsFalse(TreeStatePolicy.IsClean("?? newfile.cs"));
        }

        // ---- debug artifact is a regression gate, not upload-ready (corr #2,#10) ----
        [Test]
        public void DebugArtifact_IsRegressionGate_NotUploadReady()
        {
            Assert.IsFalse(UploadReadinessPolicy.IsUploadReady(nameof(SigningMode.DebugSigned)));
            Assert.IsTrue(UploadReadinessPolicy.IsUploadReady(nameof(SigningMode.EnvUploadKeySigned)));
            StringAssert.Contains("NOT upload",
                UploadReadinessPolicy.ArtifactPurpose(nameof(SigningMode.DebugSigned)));
        }

        // ---- policy snapshot is dated + has a verification status (corr #3) ----
        [Test]
        public void PolicySnapshot_HasSourceDateAndVerificationStatus()
        {
            var snaps = PolicySnapshotCatalog.Default(36, "Aligned16k");
            Assert.IsTrue(PolicySnapshotCatalog.AllHaveSourceAndDate(snaps));
            foreach (var s in snaps)
                Assert.That(s.VerificationStatus, Is.EqualTo("Reverified")
                    .Or.EqualTo("CarriedForward").Or.EqualTo("NotVerifiedThisRun"));
        }

        // ---- declarations stay worksheets/drafts (corr #6) ----
        [Test]
        public void DataSafety_CannotFinalizeWithoutAuditAndApproval()
        {
            Assert.IsFalse(DistributionDeclarationPolicy.DataSafetyCanFinalize(false, false));
            Assert.IsFalse(DistributionDeclarationPolicy.DataSafetyCanFinalize(true, false));
            Assert.IsFalse(DistributionDeclarationPolicy.DataSafetyCanFinalize(false, true));
            Assert.IsTrue(DistributionDeclarationPolicy.DataSafetyCanFinalize(true, true));
        }

        [Test]
        public void PrivacyPolicy_PlaceholderCannotPass()
        {
            Assert.IsFalse(DistributionDeclarationPolicy.PrivacyPolicyValid(""));
            Assert.IsFalse(DistributionDeclarationPolicy.PrivacyPolicyValid("https://example.com/privacy"));
            Assert.IsFalse(DistributionDeclarationPolicy.PrivacyPolicyValid("http://realsite.dev/privacy")); // not https
            Assert.IsTrue(DistributionDeclarationPolicy.PrivacyPolicyValid("https://sparklibrary.dev/privacy"));
        }

        [Test]
        public void TargetAudience_MustBeExplicit()
        {
            Assert.IsFalse(DistributionDeclarationPolicy.TargetAudienceExplicit(""));
            Assert.IsFalse(DistributionDeclarationPolicy.TargetAudienceExplicit("unknown"));
            Assert.IsTrue(DistributionDeclarationPolicy.TargetAudienceExplicit("Mixed (children + older) -> Families"));
        }

        [Test]
        public void IarcRating_IsNotPredicted()
        {
            Assert.IsFalse(DistributionDeclarationPolicy.IsPredictedRating(""));      // we leave it empty
            Assert.IsTrue(DistributionDeclarationPolicy.IsPredictedRating("Everyone")); // a prediction = violation
        }

        [Test]
        public void ListingCopy_RespectsLimitsAndHasNoUnverifiedClaims()
        {
            Assert.IsTrue(DistributionDeclarationPolicy.ListingCopyWithinLimits(
                "Jebby Jump", "A memory platformer", "Hop and remember the color sequence."));
            Assert.IsFalse(DistributionDeclarationPolicy.ListingCopyWithinLimits(new string('x', 31), "ok", "ok"));
            Assert.IsTrue(DistributionDeclarationPolicy.ListingCopyHasUnverifiedClaim("The final, certified best game"));
            Assert.IsFalse(DistributionDeclarationPolicy.ListingCopyHasUnverifiedClaim("A colorful memory platformer."));
        }
    }
}
