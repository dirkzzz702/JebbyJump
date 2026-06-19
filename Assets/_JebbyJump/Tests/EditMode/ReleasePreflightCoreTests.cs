using System.Linq;
using JebbyJump.Core;
using JebbyJump.Release;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    public class ReleasePreflightCoreTests
    {
        private static ReleaseConfigSnapshot Clean() => new ReleaseConfigSnapshot
        {
            CompanyName = ReleaseIdentity.CompanyName,
            ProductName = ReleaseIdentity.ProductName,
            ApplicationIdentifier = ReleaseIdentity.ApplicationIdentifier,
            BundleVersion = "1.0",
            AndroidVersionCode = 1,
            EnabledScenePaths = ReleaseSceneContract.Copy(),
            ActiveInputHandler = ReleaseTargetInvariants.NewInputSystemOnly,
            LandscapeOnly = true,
            AndroidScriptingBackend = ReleaseTargetInvariants.IL2CPP,
            AndroidArchitectures = ReleaseTargetInvariants.ARM64,
            DevelopmentBuild = false,
            PackageIds = new[] { "com.unity.inputsystem", "com.unity.modules.ui" },
        };

        private static bool HasError(ReleasePreflightResult r, string checkId)
            => r.Checks.Any(c => c.CheckId == checkId && c.Severity == ReleaseCheckSeverity.Error);

        [Test]
        public void Preflight_PassesCleanSupportedConfiguration()
        {
            var r = ReleasePreflightCore.Evaluate(Clean(), ReleaseSceneContract.Scenes);
            Assert.IsTrue(r.Passed, "clean config should pass; errors: "
                + string.Join("; ", r.Checks.Where(c => c.Severity == ReleaseCheckSeverity.Error).Select(c => c.Message)));
        }

        [Test]
        public void Preflight_FailsMissingRequiredScene()
        {
            var s = Clean();
            s.EnabledScenePaths = new[] { ReleaseSceneContract.Scenes[0], ReleaseSceneContract.Scenes[2] };
            var r = ReleasePreflightCore.Evaluate(s, ReleaseSceneContract.Scenes);
            Assert.IsFalse(r.Passed);
            Assert.IsTrue(HasError(r, ReleasePreflightChecks.Scenes));
        }

        [Test]
        public void Preflight_FailsDuplicateScene()
        {
            var s = Clean();
            s.EnabledScenePaths = new[]
            {
                ReleaseSceneContract.Scenes[0], ReleaseSceneContract.Scenes[0],
                ReleaseSceneContract.Scenes[1], ReleaseSceneContract.Scenes[2],
            };
            var r = ReleasePreflightCore.Evaluate(s, ReleaseSceneContract.Scenes);
            Assert.IsFalse(r.Passed);
            Assert.IsTrue(HasError(r, ReleasePreflightChecks.SceneNoDup));
        }

        [Test]
        public void Preflight_FailsTestSceneEnabled()
        {
            var s = Clean();
            s.EnabledScenePaths = new[] { "Assets/Scenes/SampleScene.unity" };
            var r = ReleasePreflightCore.Evaluate(s, ReleaseSceneContract.Scenes);
            Assert.IsFalse(r.Passed);
            Assert.IsTrue(HasError(r, ReleasePreflightChecks.SceneNoTest));
        }

        [Test]
        public void Preflight_FailsUnexpectedNetworkSdk()
        {
            var s = Clean();
            s.PackageIds = new[] { "com.unity.inputsystem", "com.evil.adnetwork" };
            var r = ReleasePreflightCore.Evaluate(s, ReleaseSceneContract.Scenes);
            Assert.IsFalse(r.Passed);
            Assert.IsTrue(HasError(r, ReleasePreflightChecks.Packages));
        }

        [Test]
        public void Preflight_FailsWrongOrientation()
        {
            var s = Clean();
            s.LandscapeOnly = false;
            var r = ReleasePreflightCore.Evaluate(s, ReleaseSceneContract.Scenes);
            Assert.IsTrue(HasError(r, ReleasePreflightChecks.Orientation));
        }

        [Test]
        public void Preflight_FailsWrongInputHandling()
        {
            var s = Clean();
            s.ActiveInputHandler = 2; // both
            var r = ReleasePreflightCore.Evaluate(s, ReleaseSceneContract.Scenes);
            Assert.IsTrue(HasError(r, ReleasePreflightChecks.InputHandler));
        }

        [Test]
        public void Preflight_FailsInvalidBuildIdentity()
        {
            var s = Clean();
            s.CompanyName = "DefaultCompany";
            s.ApplicationIdentifier = "com.DefaultCompany.JebbyJump";
            var r = ReleasePreflightCore.Evaluate(s, ReleaseSceneContract.Scenes);
            Assert.IsTrue(HasError(r, ReleasePreflightChecks.Identity));
        }

        [Test]
        public void Preflight_FailsMissingRequiredAsset()
        {
            var r = new ReleasePreflightResult();
            ReleaseAssetChecks.Evaluate(new ReleaseAssetPresence
            {
                LevelCatalog = false, // missing
                JebbyPrefab = true, JebbyAnimator = true, OutfitVisualLibrary = true,
                WardrobePreviewLibrary = true, InputActions = true, PlatformPrefab = true,
                TmpFont = true, TmpDigits = true, OverrideControllers = 0,
            }, r);
            Assert.IsFalse(r.Passed);
            Assert.IsTrue(HasError(r, "asset.level_catalog"));
        }

        [Test]
        public void Preflight_RequiredAssetsPresent_NoError()
        {
            var r = new ReleasePreflightResult();
            ReleaseAssetChecks.Evaluate(new ReleaseAssetPresence
            {
                LevelCatalog = true, JebbyPrefab = true, JebbyAnimator = true,
                OutfitVisualLibrary = true, WardrobePreviewLibrary = true, InputActions = true,
                PlatformPrefab = true, TmpFont = true, TmpDigits = true, OverrideControllers = 3,
            }, r);
            Assert.IsTrue(r.Passed);
        }

        [Test]
        public void ReleaseScenes_MatchSceneNamesContract()
        {
            Assert.AreEqual(3, ReleaseSceneContract.Scenes.Length);
            StringAssert.Contains(SceneNames.Boot + ".unity", ReleaseSceneContract.Scenes[0]);
            StringAssert.Contains(SceneNames.MainMenu + ".unity", ReleaseSceneContract.Scenes[1]);
            StringAssert.Contains(SceneNames.Game + ".unity", ReleaseSceneContract.Scenes[2]);
        }

        [Test]
        public void ReleaseScenes_AllPathsExist()
        {
            foreach (var rel in ReleaseSceneContract.Scenes)
            {
                string full = System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(Application.dataPath, "..", rel));
                Assert.IsTrue(System.IO.File.Exists(full), "missing scene: " + rel);
            }
        }

        [Test]
        public void ApprovedPackages_AreClassifiedExplicitly()
        {
            var ids = ReleasePreflight.GatherSnapshot().PackageIds;
            Assert.Greater(ids.Length, 0, "expected packages in manifest");
            foreach (var id in ids)
                Assert.AreNotEqual(ReleasePackageCategory.Unknown,
                    ReleasePackageClassification.Classify(id).Category,
                    "unclassified package: " + id);
        }

        [Test]
        public void ReleaseBuild_DoesNotMutateApprovedConfig()
        {
            string company = PlayerSettings.companyName;
            var before = EditorBuildSettings.scenes.Select(s => s.path + ":" + s.enabled).ToArray();
            ReleasePreflight.RunPreflight(); // read-only
            Assert.AreEqual(company, PlayerSettings.companyName);
            var after = EditorBuildSettings.scenes.Select(s => s.path + ":" + s.enabled).ToArray();
            CollectionAssert.AreEqual(before, after);
        }
    }
}
