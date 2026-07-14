using JebbyJump.EditorTools;
using NUnit.Framework;

namespace JebbyJump.Tests.EditMode
{
    // Regression guard for the P-series UI overlap fix. The layout unit tests only
    // validate the abstract GameplayLayoutMetrics model; nothing previously checked the
    // real Game.unity scene, which is why 11 text overlaps went unnoticed. This test
    // runs the exact same read-only measurement the QA audit tool uses against the
    // actual scene, so any future edit that reintroduces a text/interactive overlap
    // (HUD or a modal result/pause/settings panel) fails CI instead of shipping.
    public class UiOverlapRegressionTests
    {
        [Test]
        public void Game_Scene_HasNoTextOverlaps()
        {
            int overlaps = UiOverlapMeasurement.CountTextOverlaps(out string report);
            Assert.AreEqual(0, overlaps, report);
        }

        [Test]
        public void MainMenu_Scene_HasNoTextOverlaps()
        {
            int overlaps = UiOverlapMeasurement.CountMainMenuTextOverlaps(out string report);
            Assert.AreEqual(0, overlaps, report);
        }

        // The always-active menu button stack must be an earlier sibling than every
        // modal panel, so an opened panel's full-screen backdrop renders above the
        // stack and pointer-blocks it (the P33 modality fix; rendered evidence showed
        // the stack drawing over Level Select / Settings / Wardrobe content).
        [Test]
        public void MainMenu_MenuStack_RendersBelowModalPanels()
        {
            bool ok = UiOverlapMeasurement.MenuStackRendersBelowPanels(out string detail);
            Assert.IsTrue(ok, "MenuSafeArea must precede all panels in sibling order: " + detail);
        }
    }
}
