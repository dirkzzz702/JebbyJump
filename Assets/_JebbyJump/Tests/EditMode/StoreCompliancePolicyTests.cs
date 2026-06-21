using JebbyJump.Release;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    public class StoreCompliancePolicyTests
    {
        private static StoreComplianceSnapshot Base() => new StoreComplianceSnapshot
        {
            ConfiguredTargetSdk = 35,
            ResolvedTargetSdk = 35,
            MinSdk = 25,
            LandscapeOnly = true,
            AndroidVersionCode = 1,
            ApplicationId = "com.sparklibrary.jebbyjump",
            HasAdaptiveIcon = true,
            HasLegacyIcon = true,
        };

        private static StoreComplianceFinding Find(StoreComplianceFinding[] f, string id)
        {
            foreach (var x in f) if (x.CheckId == id) return x;
            return default;
        }

        [Test]
        public void AutomaticTargetSdk_IsReproducibilityFlag_NotComplianceFailure()
        {
            var s = Base(); s.ConfiguredTargetSdk = 0; s.ResolvedTargetSdk = 35; // correction #4
            var f = StoreCompliancePolicy.Evaluate(s);
            Assert.AreEqual("FLAG", Find(f, "target-sdk.reproducibility").Status);
            Assert.AreEqual("PASS", Find(f, "target-sdk.compliance").Status); // resolved meets minimum
        }

        [Test]
        public void ResolvedBelowMinimum_FlagsCompliance()
        {
            var s = Base(); s.ResolvedTargetSdk = 30;
            Assert.AreEqual("FLAG", Find(StoreCompliancePolicy.Evaluate(s), "target-sdk.compliance").Status);
        }

        [Test]
        public void ResolvedUnknown_IsInfoNotFlag()
        {
            var s = Base(); s.ResolvedTargetSdk = 0;
            Assert.AreEqual("INFO", Find(StoreCompliancePolicy.Evaluate(s), "target-sdk.compliance").Status);
        }

        [Test]
        public void PlaceholderApplicationId_Flagged()
        {
            var s = Base(); s.ApplicationId = "com.DefaultCompany.JebbyJump";
            Assert.AreEqual("FLAG", Find(StoreCompliancePolicy.Evaluate(s), "application-id").Status);
        }

        [Test]
        public void RealApplicationId_Passes()
        {
            Assert.AreEqual("PASS", Find(StoreCompliancePolicy.Evaluate(Base()), "application-id").Status);
        }

        [Test]
        public void MissingAdaptiveIcon_Flagged_AndListingGraphicsAreDistinctInfo()
        {
            var s = Base(); s.HasAdaptiveIcon = false; s.HasLegacyIcon = false;
            var f = StoreCompliancePolicy.Evaluate(s);
            Assert.AreEqual("FLAG", Find(f, "icon.launcher.adaptive").Status);
            Assert.AreEqual("INFO", Find(f, "icon.listing").Status); // correction #8 distinction
        }

        [Test]
        public void Orientation_LandscapeInfo_PortraitFlag()
        {
            Assert.AreEqual("INFO", Find(StoreCompliancePolicy.Evaluate(Base()), "orientation").Status);
            var s = Base(); s.LandscapeOnly = false;
            Assert.AreEqual("FLAG", Find(StoreCompliancePolicy.Evaluate(s), "orientation").Status);
        }

        [Test]
        public void PolicyAssumptions_AreDated_AndSourced()
        {
            var assumptions = StoreCompliancePolicy.DefaultAssumptions(); // correction #5
            Assert.Greater(assumptions.Length, 0);
            foreach (var a in assumptions)
            {
                Assert.IsFalse(string.IsNullOrEmpty(a.Source), "source");
                Assert.IsFalse(string.IsNullOrEmpty(a.EffectiveDate), "effectiveDate");
                Assert.IsFalse(string.IsNullOrEmpty(a.AsOf), "asOf");
            }
        }

        [Test]
        public void Report_SerializesRequiredFields()
        {
            var report = new StoreComplianceReport
            {
                Snapshot = Base(),
                Findings = StoreCompliancePolicy.Evaluate(Base()),
                PolicyAssumptions = StoreCompliancePolicy.DefaultAssumptions(),
            };
            string json = JsonUtility.ToJson(report);
            foreach (var fld in new[] { "Snapshot", "Findings", "PolicyAssumptions", "ConfiguredTargetSdk", "ResolvedTargetSdk" })
                StringAssert.Contains(fld, json);
        }
    }
}
