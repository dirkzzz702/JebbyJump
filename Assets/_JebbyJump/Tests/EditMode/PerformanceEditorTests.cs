using System;
using System.Collections.Generic;
using System.IO;
using JebbyJump.Performance;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    public class PerformanceEditorTests
    {
        [Serializable] private class AsmdefShape { public string name; public string[] includePlatforms; }

        [Test]
        public void PerformanceRegressionPolicy_RejectsLeak()
        {
            Assert.IsTrue(PerformanceRegressionPolicy.IsLeak(0, 1));
            Assert.IsFalse(PerformanceRegressionPolicy.IsLeak(5, 5));
            Assert.AreEqual(PerfRegression.Leak,
                PerformanceRegressionPolicy.Classify(leak: true, unexpectedGc: false, timingRegressed: false));
        }

        [Test]
        public void PerformanceRegressionPolicy_RejectsUnexpectedGcAlloc()
        {
            Assert.IsTrue(PerformanceRegressionPolicy.IsUnexpectedGcAlloc(true, true));
            Assert.IsFalse(PerformanceRegressionPolicy.IsUnexpectedGcAlloc(true, false));
            Assert.IsFalse(PerformanceRegressionPolicy.IsUnexpectedGcAlloc(false, true));
            Assert.AreEqual(PerfRegression.UnexpectedGcAlloc,
                PerformanceRegressionPolicy.Classify(false, true, false));
        }

        [Test]
        public void PerformanceRegressionPolicy_ClassifiesTimingRegression()
        {
            Assert.IsTrue(PerformanceRegressionPolicy.IsTimingRegression(10, 20, 10));
            Assert.IsFalse(PerformanceRegressionPolicy.IsTimingRegression(10, 10.5, 10));
            Assert.IsFalse(PerformanceRegressionPolicy.IsTimingRegression(0, 5, 10)); // no baseline
            Assert.AreEqual(PerfRegression.TimingRegression,
                PerformanceRegressionPolicy.Classify(false, false, true));
        }

        [Test]
        public void PerformanceRegressionPolicy_DoesNotClaimDeviceCertification()
        {
            Assert.IsFalse(PerformanceRegressionPolicy.ClaimsDeviceCertification);
        }

        [Test]
        public void PerfTooling_IsEditorOnly()
        {
            var text = File.ReadAllText(Path.GetFullPath(Path.Combine(Application.dataPath, "..",
                "Assets/_JebbyJump/Scripts/Performance/Editor/JebbyJump.Performance.Editor.asmdef")));
            var shape = JsonUtility.FromJson<AsmdefShape>(text);
            CollectionAssert.AreEqual(new[] { "Editor" }, shape.includePlatforms);
        }

        [Test]
        public void Reports_SerializeRequiredFields()
        {
            string perf = JsonUtility.ToJson(new PerfReport());
            foreach (var f in new[] { "Environment", "DeviceCertification", "TargetFrameRate", "Flows", "LeakChecks" })
                StringAssert.Contains(f, perf);
            string size = JsonUtility.ToJson(new BuildSizeAudit());
            foreach (var f in new[] { "CompressedAabBytes", "P24CompressedAabBytes", "AabDeltaBytes", "TotalPackedBytes", "LargestContributors" })
                StringAssert.Contains(f, size);
        }

        [Test]
        public void Reports_UseRelativePaths()
        {
            Assert.IsTrue(PerfReport.IsRelativePath("P24/abc/performance-baseline.json"));
            Assert.IsFalse(PerfReport.IsRelativePath("C:/Builds/P24/x.json"));
            Assert.IsFalse(PerfReport.IsRelativePath("/abs/x"));
        }

        [Test]
        public void BuildSizeMath_UnitsAndTopContributors()
        {
            Assert.AreEqual(113.625618, BuildSizeMath.Mb(113625618), 0.0001);
            Assert.AreEqual(108.36, BuildSizeMath.MiB(113625618), 0.01);

            var packed = new List<KeyValuePair<string, long>>
            {
                new KeyValuePair<string, long>("a", 10),
                new KeyValuePair<string, long>("b", 300),
                new KeyValuePair<string, long>("c", 50),
            };
            var top = BuildSizeMath.TopContributors(packed, 2);
            Assert.AreEqual(2, top.Length);
            Assert.AreEqual("b", top[0].Path);
            Assert.AreEqual(300, top[0].PackedBytes);
            Assert.AreEqual("c", top[1].Path);
        }
    }
}
