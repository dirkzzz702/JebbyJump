using System;
using System.IO;
using JebbyJump.Release;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    public class ReleaseReportHashTests
    {
        [Serializable]
        private class AsmdefShape
        {
            public string name;
            public string[] includePlatforms;
        }

        private static string AsmdefText(string rel)
            => File.ReadAllText(Path.GetFullPath(Path.Combine(Application.dataPath, "..", rel)));

        [Test]
        public void ReleaseAssembly_IsEditorOnly()
        {
            var shape = JsonUtility.FromJson<AsmdefShape>(AsmdefText(
                "Assets/_JebbyJump/Scripts/Release/Editor/JebbyJump.Release.Editor.asmdef"));
            Assert.AreEqual("JebbyJump.Release.Editor", shape.name);
            Assert.AreEqual(1, shape.includePlatforms.Length);
            Assert.AreEqual("Editor", shape.includePlatforms[0]);
        }

        [Test]
        public void ReleaseTooling_IsExcludedFromPlayer()
        {
            // Editor-only includePlatforms means the whole release asmdef (preflight,
            // builder, report, hashing, DTOs) is compiled out of player builds.
            var shape = JsonUtility.FromJson<AsmdefShape>(AsmdefText(
                "Assets/_JebbyJump/Scripts/Release/Editor/JebbyJump.Release.Editor.asmdef"));
            CollectionAssert.AreEqual(new[] { "Editor" }, shape.includePlatforms);
        }

        [Test]
        public void Report_SeparatesBuildAndSigningStatus()
        {
            var r = new ReleaseReport
            {
                AndroidBuildStatus = "AndroidBuildFailed",
                SigningStatus = "DebugSigned (development; NOT production)",
            };
            Assert.AreNotEqual(r.AndroidBuildStatus, r.SigningStatus);
            // No single broad boolean "Passed" that could hide a blocker.
            Assert.IsNull(typeof(ReleaseReport).GetField("Passed"));
        }

        [Test]
        public void Report_UsesRelativeArtifactPaths()
        {
            Assert.IsTrue(ReleaseReport.IsRelativePath("Android/1.0/JebbyJump.aab"));
            Assert.IsFalse(ReleaseReport.IsRelativePath("C:/Builds/P23/x.aab"));
            Assert.IsFalse(ReleaseReport.IsRelativePath("/abs/x"));
            Assert.AreEqual("a/b.txt",
                ArtifactHasher.Relative("D:/proj/Builds/P23", "D:/proj/Builds/P23/a/b.txt"));
        }

        [Test]
        public void BuildReport_SerializesRequiredFields()
        {
            var json = JsonUtility.ToJson(new ReleaseReport());
            foreach (var f in new[]
            {
                "ReadinessVerdict", "PreflightStatus", "TestsStatus", "AndroidBuildStatus",
                "WindowsSmokeStatus", "SigningStatus", "WarningGateStatus",
                "ArtifactHashingStatus", "ManualQaStatus", "PrimaryArtifactSha256",
            })
                StringAssert.Contains(f, json);
        }

        [Test]
        public void BuildReport_DoesNotContainSecrets()
        {
            Assert.IsTrue(ReleaseReport.ContainsSecretLike("storePassword=hunter2"));
            Assert.IsTrue(ReleaseReport.ContainsSecretLike("path/to/release.keystore"));
            Assert.IsFalse(ReleaseReport.ContainsSecretLike("JebbyJump 1.0 Android AAB ok"));
        }

        [Test]
        public void BuildReport_WrittenOnPreflightFailure()
        {
            RunWriteCase(new ReleaseReport
            {
                PreflightStatus = "Failed",
                ReadinessVerdict = ReleaseReadiness.Blocked,
            });
        }

        [Test]
        public void BuildReport_WrittenOnBuildFailure()
        {
            RunWriteCase(new ReleaseReport
            {
                PreflightStatus = "Passed",
                AndroidBuildStatus = AndroidBuildStatus.AndroidBuildFailed.ToString(),
                ReadinessVerdict = ReleaseReadiness.Failed,
            });
        }

        private static void RunWriteCase(ReleaseReport report)
        {
            string dir = Path.Combine(Path.GetTempPath(), "jj_rc_" + Guid.NewGuid().ToString("N"));
            try
            {
                ReleaseReportWriter.Write(report, dir);
                Assert.IsTrue(File.Exists(Path.Combine(dir, "release-report.json")));
                Assert.IsTrue(File.Exists(Path.Combine(dir, "release-report.md")));
                StringAssert.Contains(report.ReadinessVerdict,
                    File.ReadAllText(Path.Combine(dir, "release-report.json")));
            }
            finally
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
            }
        }

        [Test]
        public void ArtifactHasher_ProducesStableSha256()
        {
            string f = Path.Combine(Path.GetTempPath(), "jj_hash_" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                File.WriteAllText(f, "hello");
                string h1 = ArtifactHasher.Sha256File(f);
                string h2 = ArtifactHasher.Sha256File(f);
                Assert.AreEqual(h1, h2);
                Assert.AreEqual(64, h1.Length);
                Assert.AreEqual("2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824", h1);
            }
            finally
            {
                if (File.Exists(f)) File.Delete(f);
            }
        }

        [Test]
        public void WindowsArtifactHash_CoversCompletePackage()
        {
            string dir = Path.Combine(Path.GetTempPath(), "jj_pkg_" + Guid.NewGuid().ToString("N"));
            try
            {
                Directory.CreateDirectory(Path.Combine(dir, "sub"));
                File.WriteAllText(Path.Combine(dir, "a.exe"), "aaa");
                File.WriteAllText(Path.Combine(dir, "sub", "b.dat"), "bbbb");
                var manifest = ArtifactHasher.Sha256Directory(dir, dir);
                Assert.AreEqual(2, manifest.Length, "manifest must cover every file, not just the exe");
                foreach (var fh in manifest)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(fh.Sha256));
                    Assert.IsTrue(ReleaseReport.IsRelativePath(fh.RelativePath), fh.RelativePath);
                }
            }
            finally
            {
                if (Directory.Exists(dir)) Directory.Delete(dir, true);
            }
        }

        [Test]
        public void WarningClassifier_OnlyAllowsNarrowPatterns()
        {
            foreach (var e in ReleaseWarningAllowlist.Entries)
                Assert.IsTrue(ReleaseWarningAllowlist.IsNarrow(e.Pattern),
                    "allowlist pattern too broad: " + e.Pattern);
            Assert.IsFalse(ReleaseWarningAllowlist.IsNarrow("*"));
            Assert.IsFalse(ReleaseWarningAllowlist.IsNarrow("short"));
            Assert.IsTrue(ReleaseWarningAllowlist.IsNarrow("a-specific-and-long-warning-token"));
            Assert.IsFalse(ReleaseWarningAllowlist.IsAllowed("anything at all"));
        }
    }
}
