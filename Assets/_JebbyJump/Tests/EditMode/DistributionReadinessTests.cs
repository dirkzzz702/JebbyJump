using JebbyJump.Release;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    public class DistributionReadinessTests
    {
        private static DistributionReport PrepReport() => new DistributionReport
        {
            TreeClean = true,
            AutomatedTestStatus = DistStatus.Passed,
            PreflightStatus = DistStatus.Passed,
            PageSize16KbStatus = DistStatus.Passed,
            MissingExternalItems = new[] { "Play Console account", "upload keystore" },
        };

        [Test]
        public void PreparationMode_ReturnsPreparationComplete()
        {
            Assert.AreEqual(DistributionReadinessPolicy.PreparationComplete,
                DistributionReadinessPolicy.DecidePreparation(PrepReport()));
        }

        [Test]
        public void PreparationComplete_RequiresEnumeratedExternals() // corr #7
        {
            var r = PrepReport();
            r.MissingExternalItems = new string[0];
            Assert.AreEqual(DistributionReadinessPolicy.Blocked,
                DistributionReadinessPolicy.DecidePreparation(r));
        }

        [Test]
        public void DirtyTree_Blocks() // corr #1,#10
        {
            var r = PrepReport(); r.TreeClean = false;
            Assert.AreEqual(DistributionReadinessPolicy.Blocked,
                DistributionReadinessPolicy.DecidePreparation(r));
        }

        [Test]
        public void PageSize16KbFailure_Blocks()
        {
            var r = PrepReport(); r.PageSize16KbStatus = DistStatus.Failed;
            Assert.AreEqual(DistributionReadinessPolicy.Blocked,
                DistributionReadinessPolicy.DecidePreparation(r));
        }

        [Test]
        public void PreparationMode_NeverClaimsConsoleOrDevice() // corr #8
        {
            var r = PrepReport(); r.InternalTrackUploadStatus = DistStatus.Passed;
            Assert.IsTrue(DistributionReadinessPolicy.ClaimsConsoleOrDevice(r));
            Assert.AreEqual(DistributionReadinessPolicy.Blocked,
                DistributionReadinessPolicy.DecidePreparation(r));

            var r2 = PrepReport(); r2.PhysicalInstallStatus = DistStatus.Passed;
            Assert.AreEqual(DistributionReadinessPolicy.Blocked,
                DistributionReadinessPolicy.DecidePreparation(r2));
        }

        [Test]
        public void FreshReport_DefaultsDoNotClaimConsoleOrDevice()
        {
            Assert.IsFalse(DistributionReadinessPolicy.ClaimsConsoleOrDevice(new DistributionReport()));
        }

        [Test]
        public void Report_UsesIndependentStatuses_InclPlayConsoleActionStatus() // corr #8
        {
            string json = JsonUtility.ToJson(new DistributionReport());
            foreach (var f in new[]
            {
                "PlayConsoleActionStatus", "InternalTrackUploadStatus", "PhysicalInstallStatus",
                "UploadSigningStatus", "MissingExternalItems", "PolicySnapshots",
                "ReadinessDecision", "ArtifactPurpose",
            })
                StringAssert.Contains(f, json);
        }

        [Test]
        public void Report_GuardsSecretsAndTesterEmails() // corr #4
        {
            Assert.IsTrue(DistributionReport.ContainsTesterEmail("contact tester@example.com please"));
            Assert.IsFalse(DistributionReport.ContainsTesterEmail("no addresses here"));
            Assert.IsTrue(DistributionReport.ContainsSecretLike("keystorePass=hunter2"));
            Assert.IsTrue(DistributionReport.IsRelativePath("P27/abc/distribution-report.json"));
            Assert.IsFalse(DistributionReport.IsRelativePath("C:/Builds/x.json"));
        }
    }
}
