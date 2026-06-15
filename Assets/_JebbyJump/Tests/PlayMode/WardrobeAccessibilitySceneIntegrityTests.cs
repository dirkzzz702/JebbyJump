#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P20 scene-integrity: verifies the accessibility/mobile structure was
    // scaffolded into MainMenu.unity (safe-area roots + responsive regions
    // wired) and Game.unity/MainMenu Settings panels gained the Reduce Motion
    // toggle. YAML-text checks only (the panels are Assembly-CSharp; no asmdef
    // restructure - same approach as WardrobeCeremonySceneIntegrityTests).
    public class WardrobeAccessibilitySceneIntegrityTests
    {
        private static string Read(string scene)
        {
            string path = Path.Combine(
                Application.dataPath, "_JebbyJump/Scenes/" + scene);
            Assert.IsTrue(File.Exists(path), scene + " not found at " + path);
            return File.ReadAllText(path);
        }

        private static int Count(string text, string pattern)
            => Regex.Matches(text, pattern).Count;

        private static void AssertWired(string text, string field)
        {
            var m = Regex.Match(text, field + @": \{fileID: (-?\d+)");
            Assert.IsTrue(m.Success, field + " not present in scene");
            Assert.AreNotEqual("0", m.Groups[1].Value,
                field + " is unwired (fileID 0)");
        }

        [Test]
        public void MainMenu_HasSingleSafeAreaContentRoot()
        {
            // "SafeArea" (wardrobe) and "CeremonySafeArea" are distinct names.
            Assert.AreEqual(1, Count(Read("MainMenu.unity"), @"m_Name: SafeArea\b"),
                "expected exactly one wardrobe SafeArea root");
        }

        [Test]
        public void MainMenu_HasSingleCeremonySafeArea()
        {
            Assert.AreEqual(1,
                Count(Read("MainMenu.unity"), @"m_Name: CeremonySafeArea\b"),
                "expected exactly one CeremonySafeArea root");
        }

        [Test]
        public void MainMenu_WardrobeRegionRefsAreWired()
        {
            string text = Read("MainMenu.unity");
            foreach (var field in new[]
            {
                "_safeAreaRoot", "_headerRegion", "_listRegion",
                "_previewRegion", "_actionRegion", "_scrollRect",
            })
                AssertWired(text, field);
        }

        [Test]
        public void MainMenu_HasEventSystem()
        {
            Assert.GreaterOrEqual(
                Count(Read("MainMenu.unity"), @"m_Name: EventSystem\b"), 1,
                "MainMenu needs an EventSystem for keyboard/gamepad focus");
        }

        [Test]
        public void MainMenu_SettingsHasReduceMotionToggle()
        {
            string text = Read("MainMenu.unity");
            Assert.AreEqual(1, Count(text, @"m_Name: ReduceMotionToggle\b"),
                "expected one Reduce Motion toggle in MainMenu settings");
            AssertWired(text, "_reduceMotionToggle");
        }

        [Test]
        public void Game_SettingsHasReduceMotionToggle()
        {
            string text = Read("Game.unity");
            Assert.AreEqual(1, Count(text, @"m_Name: ReduceMotionToggle\b"),
                "expected one Reduce Motion toggle in Game (pause) settings");
            AssertWired(text, "_reduceMotionToggle");
        }
    }
}
#endif
