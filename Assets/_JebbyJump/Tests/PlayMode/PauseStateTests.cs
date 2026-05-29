using JebbyJump.Session;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // Pure logic for the pause flag that gameplay input handlers read.
    // The PauseMenuController itself (timeScale, panels, scene loads) is a
    // MonoBehaviour verified via compile + scaffold + manual play-test; only
    // the testable state lives here.
    public class PauseStateTests
    {
        [SetUp]
        public void SetUp() => PauseState.SetPaused(false);

        [TearDown]
        public void TearDown() => PauseState.SetPaused(false);

        [Test]
        public void Default_IsNotPaused()
        {
            Assert.IsFalse(PauseState.IsPaused);
            Assert.IsFalse(PauseState.BlocksGameplayInput);
        }

        [Test]
        public void SetPausedTrue_BlocksInput()
        {
            PauseState.SetPaused(true);
            Assert.IsTrue(PauseState.IsPaused);
            Assert.IsTrue(PauseState.BlocksGameplayInput);
        }

        [Test]
        public void SetPausedFalse_UnblocksInput()
        {
            PauseState.SetPaused(true);
            PauseState.SetPaused(false);
            Assert.IsFalse(PauseState.IsPaused);
            Assert.IsFalse(PauseState.BlocksGameplayInput);
        }

        [Test]
        public void Toggle_RoundTrips()
        {
            PauseState.SetPaused(true);
            Assert.IsTrue(PauseState.IsPaused);
            PauseState.SetPaused(false);
            Assert.IsFalse(PauseState.IsPaused);
            PauseState.SetPaused(true);
            Assert.IsTrue(PauseState.IsPaused);
        }
    }
}
