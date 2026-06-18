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
        public void Game_SequenceCanvasConvertedToStandardScaler()
        {
            // P22: the memory-phase canvas is migrated off the legacy 800x600
            // Constant-Pixel scaler to the standard SWSS 1920x1080 (visual-only;
            // RenderMode + sorting are untouched). No 800x600 ref resolution
            // remains, and the canvas itself is still present.
            string t = Read("Game.unity");
            StringAssert.Contains("m_Name: SequenceCanvas", t);
            Assert.AreEqual(0, Count(t, @"x: 800, y: 600"),
                "SequenceCanvas must be converted off the legacy 800x600 scaler");
        }

        // P22: the gameplay modal input gate exists (blocks + clears gameplay
        // input under shell modals). Lives on the always-active HUDCanvas.
        [Test]
        public void Game_HasGameplayModalInputGate()
        {
            Assert.AreEqual(1,
                Count(Read("Game.unity"), @"GameplayModalInputGate"),
                "expected exactly one GameplayModalInputGate");
        }

        [Test]
        public void Game_HasSingleEventSystemAndInputModule()
        {
            string t = Read("Game.unity");
            Assert.AreEqual(1, Count(t, @"m_Name: EventSystem\b"));
            StringAssert.Contains("InputSystemUIInputModule", t);
        }

        // Check 1: GameShellCanvas renders + receives input ABOVE gameplay.
        // Only GameShellCanvas uses sortingOrder 500; its GraphicRaycaster is
        // created with the canvas (BuildGameShellCanvas), so input ordering
        // follows the render order.
        [Test]
        public void Game_GameShellCanvasSortsAboveGameplay()
        {
            Assert.GreaterOrEqual(
                Count(Read("Game.unity"), @"m_SortingOrder: 500"), 1,
                "GameShellCanvas should sort (and raycast) above gameplay");
        }

        // Check 4: every SafeAreaFitter target is wired (no fitter left
        // pointing at fileID 0) in both scenes.
        [Test]
        public void SafeAreaFitters_AllTargetsWired()
        {
            Assert.AreEqual(0,
                Count(Read("MainMenu.unity"), @"_target: \{fileID: 0\}"),
                "a MainMenu SafeAreaFitter target is unwired");
            Assert.AreEqual(0,
                Count(Read("Game.unity"), @"_target: \{fileID: 0\}"),
                "a Game SafeAreaFitter target is unwired");
        }

        // Check 4: idempotency guard - the scaffolds must not duplicate the
        // safe-area roots on re-run (3 panel SafeAreas + 1 MenuSafeArea + the
        // wardrobe's 1 CeremonySafeArea in Main Menu; pause + settings in Game).
        [Test]
        public void SafeAreaRoots_AreNotDuplicated()
        {
            string mm = Read("MainMenu.unity");
            Assert.AreEqual(3, Count(mm, @"m_Name: SafeArea\b"),
                "Main Menu: Level Select + Settings + Wardrobe SafeArea roots");
            Assert.AreEqual(1, Count(mm, @"m_Name: MenuSafeArea\b"));
            Assert.AreEqual(1, Count(mm, @"m_Name: CeremonySafeArea\b"));
            Assert.AreEqual(5, Count(Read("Game.unity"), @"m_Name: SafeArea\b"),
                "Game: Pause + Settings + HUD + MobileControls + Sequence "
                + "SafeArea roots (P22)");
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
