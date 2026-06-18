using JebbyJump.Shell;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // P22 correction #1: the input-block decision is pure + tested. (InputReader
    // itself is an Assembly-CSharp ScriptableObject, not reachable from the test
    // asmdef, so the decision logic is tested here and the gate wiring via the
    // scene-integrity tests.)
    public class GameplayInputBlockPolicyTests
    {
        [Test]
        public void NoModalOpen_DoesNotBlock()
        {
            Assert.IsFalse(GameplayInputBlockPolicy.ShouldBlock(false, false, false, false));
        }

        [Test]
        public void Paused_Blocks()
        {
            Assert.IsTrue(GameplayInputBlockPolicy.ShouldBlock(true, false, false, false));
        }

        [Test]
        public void SettingsOpen_Blocks()
        {
            Assert.IsTrue(GameplayInputBlockPolicy.ShouldBlock(false, true, false, false));
        }

        [Test]
        public void ResultOpen_Blocks()
        {
            Assert.IsTrue(GameplayInputBlockPolicy.ShouldBlock(false, false, true, false));
        }

        [Test]
        public void GameOverOpen_Blocks()
        {
            Assert.IsTrue(GameplayInputBlockPolicy.ShouldBlock(false, false, false, true));
        }

        [Test]
        public void MultipleOpen_Blocks()
        {
            Assert.IsTrue(GameplayInputBlockPolicy.ShouldBlock(true, true, false, true));
        }
    }
}
