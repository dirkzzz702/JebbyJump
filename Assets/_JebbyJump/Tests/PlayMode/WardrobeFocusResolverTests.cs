using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // P20 deterministic keyboard/gamepad focus map (pure logic; the panel maps
    // these targets to actual Selectables).
    public class WardrobeFocusResolverTests
    {
        [Test]
        public void PanelFocus_NoRows_IsNone()
        {
            Assert.AreEqual(WardrobeFocusTarget.None,
                WardrobeFocusResolver.ResolvePanelFocus(0, -1));
        }

        [Test]
        public void PanelFocus_ValidEquipped_IsEquippedRow()
        {
            Assert.AreEqual(WardrobeFocusTarget.EquippedRow,
                WardrobeFocusResolver.ResolvePanelFocus(8, 2));
        }

        [Test]
        public void PanelFocus_NoOrOutOfRangeEquipped_IsFirstRow()
        {
            Assert.AreEqual(WardrobeFocusTarget.FirstRow,
                WardrobeFocusResolver.ResolvePanelFocus(8, -1));
            Assert.AreEqual(WardrobeFocusTarget.FirstRow,
                WardrobeFocusResolver.ResolvePanelFocus(8, 99));
        }

        [Test]
        public void CeremonyFocus_PrefersEquipWhenEnabled()
        {
            Assert.AreEqual(WardrobeFocusTarget.CeremonyEquip,
                WardrobeFocusResolver.ResolveCeremonyFocus(true));
            Assert.AreEqual(WardrobeFocusTarget.CeremonyContinue,
                WardrobeFocusResolver.ResolveCeremonyFocus(false));
        }

        [Test]
        public void CloseFocus_IsOpener()
        {
            Assert.AreEqual(WardrobeFocusTarget.Opener,
                WardrobeFocusResolver.ResolveCloseFocus());
        }
    }
}
