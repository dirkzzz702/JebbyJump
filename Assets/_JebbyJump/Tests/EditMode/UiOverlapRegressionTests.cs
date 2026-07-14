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
    }
}
