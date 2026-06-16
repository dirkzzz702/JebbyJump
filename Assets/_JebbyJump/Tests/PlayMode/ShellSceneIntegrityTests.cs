#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P21 wider-shell scene integrity (YAML-text checks; the panels are
    // Assembly-CSharp). Verifies the GameShellCanvas isolation, the result/
    // game-over modal cards, the Main Menu safe-area, one EventSystem +
    // InputSystemUIInputModule per scene, and that the gameplay SequenceCanvas
    // was left untouched.
    public class ShellSceneIntegrityTests
    {
        private static string Read(string scene)
        {
            string path = Path.Combine(
                Application.dataPath, "_JebbyJump/Scenes/" + scene);
            Assert.IsTrue(File.Exists(path), scene + " not found");
            return File.ReadAllText(path);
        }

        private static int Count(string t, string p) => Regex.Matches(t, p).Count;

        [Test]
        public void Game_HasSingleGameShellCanvas()
        {
            Assert.AreEqual(1,
                Count(Read("Game.unity"), @"m_Name: GameShellCanvas\b"),
                "expected exactly one GameShellCanvas");
        }

        [Test]
        public void Game_ResultAndGameOverAreModalCards()
        {
            // MakeModalCard wraps each centered card content in a "Card" child.
            Assert.AreEqual(2, Count(Read("Game.unity"), @"m_Name: Card\b"),
                "expected Level Complete + Game Over modal Cards");
        }

        [Test]
        public void Game_SequenceCanvasLeftUntouched()
        {
            // The memory-phase gameplay canvas (out of scope) keeps its scaler.
            StringAssert.Contains("m_Name: SequenceCanvas", Read("Game.unity"));
            Assert.GreaterOrEqual(
                Count(Read("Game.unity"), @"x: 800, y: 600"), 1,
                "SequenceCanvas 800x600 scaler must be preserved (gameplay)");
        }

        [Test]
        public void Game_HasSingleEventSystemAndInputModule()
        {
            string t = Read("Game.unity");
            Assert.AreEqual(1, Count(t, @"m_Name: EventSystem\b"));
            StringAssert.Contains("InputSystemUIInputModule", t);
        }

        [Test]
        public void MainMenu_HasMenuSafeAreaAndEventSystem()
        {
            string t = Read("MainMenu.unity");
            Assert.AreEqual(1, Count(t, @"m_Name: MenuSafeArea\b"),
                "expected one Main Menu button-stack safe area");
            Assert.AreEqual(1, Count(t, @"m_Name: EventSystem\b"));
            StringAssert.Contains("InputSystemUIInputModule", t);
        }
    }
}
#endif
