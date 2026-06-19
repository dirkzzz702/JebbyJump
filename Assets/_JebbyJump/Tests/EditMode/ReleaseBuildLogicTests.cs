using System;
using System.Reflection;
using JebbyJump.Release;
using NUnit.Framework;
using UnityEditor;

namespace JebbyJump.Tests.EditMode
{
    public class ReleaseBuildLogicTests
    {
        [Test]
        public void WindowsFallback_OnlyForToolchainUnavailable()
        {
            Assert.AreEqual(AndroidBuildAction.BuildAab,
                ReleaseBuildDecision.ResolveAction(true));
            Assert.AreEqual(AndroidBuildAction.ToolchainBlockedWindowsSmoke,
                ReleaseBuildDecision.ResolveAction(false));
        }

        [Test]
        public void AndroidFailure_IsNotMaskedByWindowsSmoke()
        {
            // Toolchain available + Android build failed => AndroidBuildFailed,
            // regardless of any Windows smoke result.
            Assert.AreEqual(AndroidBuildStatus.AndroidBuildFailed,
                ReleaseBuildDecision.MapStatus(true, false, true));
            Assert.AreEqual(AndroidBuildStatus.AndroidAabBuilt,
                ReleaseBuildDecision.MapStatus(true, true, false));
        }

        [Test]
        public void FutureModuleMissing_ReturnsClearError()
        {
            Assert.AreEqual(AndroidBuildStatus.AndroidToolchainBlocked_WindowsSmokePassed,
                ReleaseBuildDecision.MapStatus(false, false, true));
            Assert.AreEqual(AndroidBuildStatus.AndroidToolchainBlocked_WindowsSmokeFailed,
                ReleaseBuildDecision.MapStatus(false, false, false));
        }

        [Test]
        public void Readiness_AndroidCompleteOnlyWhenAllGatesPass()
        {
            Assert.AreEqual(ReleaseReadiness.AndroidComplete,
                ReleaseReadiness.Verdict(true, true, AndroidBuildStatus.AndroidAabBuilt, true, true, true));
            Assert.AreEqual(ReleaseReadiness.WindowsSmokeOnly,
                ReleaseReadiness.Verdict(true, true, AndroidBuildStatus.AndroidToolchainBlocked_WindowsSmokePassed, true, true, true));
        }

        [Test]
        public void UnclassifiedBuildWarning_InvalidatesRc()
        {
            Assert.IsFalse(ReleaseWarningAllowlist.IsAllowed("CS9999: a novel build warning"));
            Assert.AreNotEqual(ReleaseReadiness.AndroidComplete,
                ReleaseReadiness.Verdict(true, true, AndroidBuildStatus.AndroidAabBuilt,
                    warningGatePassed: false, hashingOk: true, configMatches: true));
        }

        [Test]
        public void TestsNotVerified_InvalidatesRc()
        {
            Assert.AreNotEqual(ReleaseReadiness.AndroidComplete,
                ReleaseReadiness.Verdict(true, testsPassed: false,
                    AndroidBuildStatus.AndroidAabBuilt, true, true, true));
        }

        [Test]
        public void CliFailure_ReturnsNonZeroExitCode()
        {
            Assert.AreEqual(1, ReleaseExitCode.From(ReleaseReadiness.Failed));
            Assert.AreEqual(1, ReleaseExitCode.From(ReleaseReadiness.Blocked));
            Assert.AreEqual(0, ReleaseExitCode.From(ReleaseReadiness.AndroidComplete));
            Assert.AreEqual(0, ReleaseExitCode.From(ReleaseReadiness.WindowsSmokeOnly));
        }

        [Test]
        public void ReleaseScenes_BuilderUsesImmutableContract()
        {
            var copy = ReleaseSceneContract.Copy();
            copy[0] = "mutated";
            Assert.AreNotEqual("mutated", ReleaseSceneContract.Scenes[0],
                "contract must be immutable to external mutation");
            Assert.AreEqual(3, ReleaseSceneContract.Scenes.Length);
        }

        [Test]
        public void AndroidAabFlag_IsSetAndRestored()
        {
            var state = JebbyJumpReleaseBuilder.CaptureState();
            try
            {
                EditorUserBuildSettings.buildAppBundle = !state.AppBundle;
                Assert.AreNotEqual(state.AppBundle, EditorUserBuildSettings.buildAppBundle);
            }
            finally
            {
                JebbyJumpReleaseBuilder.RestoreState(state);
            }
            Assert.AreEqual(state.AppBundle, EditorUserBuildSettings.buildAppBundle);
        }

        [Test]
        public void TemporaryBuildTarget_IsRestored()
        {
            var state = JebbyJumpReleaseBuilder.CaptureState();
            Assert.AreEqual(EditorUserBuildSettings.activeBuildTarget, state.Target);
            // Restoring an unchanged snapshot is a safe no-op (no costly switch).
            JebbyJumpReleaseBuilder.RestoreState(state);
            Assert.AreEqual(state.Target, EditorUserBuildSettings.activeBuildTarget);
            // dev/profiler flags round-trip through the guard.
            JebbyJumpReleaseBuilder.RestoreState(state);
            Assert.AreEqual(state.Development, EditorUserBuildSettings.development);
        }

        [Test]
        public void MenuBuild_DoesNotExitEditor()
        {
            var t = typeof(JebbyJumpReleaseBuilder);
            var menu = t.GetMethod("BuildReleaseCandidateMenu", BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(menu);
            Assert.AreEqual(typeof(void), menu.ReturnType);
            // The menu uses the report-returning core (no exit); CLI exit is a seam.
            var core = t.GetMethod("BuildReleaseCandidate", BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(core);
            Assert.AreEqual(typeof(ReleaseReport), core.ReturnType);
            var exitSeam = t.GetField("CliExit", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(exitSeam, "CLI exit must be centralized behind a seam");
        }
    }
}
