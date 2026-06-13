#if UNITY_EDITOR
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P16 scene-integrity: verifies the unlock-ceremony overlay is scaffolded
    // into MainMenu.unity exactly once and the WardrobePanelController's
    // ceremony refs are wired. Done by reading the scene YAML text (no
    // reference to the Assembly-CSharp panel type, no asmdef restructure -
    // per the approved guardrail).
    public class WardrobeCeremonySceneIntegrityTests
    {
        private static string SceneText()
        {
            string path = Path.Combine(
                Application.dataPath, "_JebbyJump/Scenes/MainMenu.unity");
            Assert.IsTrue(File.Exists(path), "MainMenu.unity not found at " + path);
            return File.ReadAllText(path);
        }

        [Test]
        public void UnlockCeremonyOverlay_ExistsExactlyOnce()
        {
            int count = Regex.Matches(
                SceneText(), @"m_Name: UnlockCeremonyOverlay\b").Count;
            Assert.AreEqual(1, count,
                "expected exactly one UnlockCeremonyOverlay in MainMenu.unity");
        }

        [Test]
        public void WardrobePanelController_CeremonyRefsAreWired()
        {
            string text = SceneText();
            string[] fields =
            {
                "_ceremonyOverlay", "_ceremonyTitle", "_ceremonyOutfitName",
                "_ceremonyMessage", "_ceremonyPreviewImage",
                "_ceremonyEquipButton", "_ceremonyContinueButton",
                "_previewLibrary",
            };
            foreach (var field in fields)
            {
                var m = Regex.Match(text, field + @": \{fileID: (-?\d+)");
                Assert.IsTrue(m.Success, field + " not present in scene");
                Assert.AreNotEqual("0", m.Groups[1].Value,
                    field + " is unwired (fileID 0)");
            }
        }
    }
}
#endif
