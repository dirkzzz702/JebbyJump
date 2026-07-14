using System.Collections.Generic;
using System.Text.RegularExpressions;
using JebbyJump.Wardrobe;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // Pure logic for the P9 wardrobe foundation: catalog, equipped-id
    // store, and Stars-gated unlock service. No scene/UI. SetUp/TearDown
    // clear the equipped-outfit key so tests do not leak.
    public class WardrobeTests
    {
        private static readonly Regex SnakeCase =
            new Regex("^[a-z][a-z0-9_]*$");

        [SetUp]
        public void SetUp() => WardrobeStore.Reset();

        [TearDown]
        public void TearDown() => WardrobeStore.Reset();

        // ---- WardrobeCatalog ----

        [Test]
        public void Catalog_HasExactlySevenOutfits()
        {
            // Approved: 7 total = the default + six unlockable variants
            // (rookie_page was folded into the default - its art IS the
            // default look; see SetDefaultLook).
            Assert.AreEqual(7, WardrobeCatalog.Outfits.Count);
        }

        [Test]
        public void Catalog_IdsAreUniqueNonEmptySnakeCase()
        {
            var seen = new HashSet<string>();
            foreach (var o in WardrobeCatalog.Outfits)
            {
                Assert.IsFalse(string.IsNullOrEmpty(o.Id));
                Assert.IsTrue(SnakeCase.IsMatch(o.Id),
                    o.Id + " is not snake_case");
                Assert.IsTrue(seen.Add(o.Id), "duplicate id: " + o.Id);
            }
        }

        [Test]
        public void Catalog_DefaultExistsAndIsAlwaysUnlocked()
        {
            var def = WardrobeCatalog.GetById(WardrobeCatalog.DefaultOutfitId);
            Assert.IsNotNull(def);
            Assert.AreEqual("classic_color_knight", def.Id);
            Assert.IsTrue(def.AlwaysUnlocked);
            Assert.AreEqual(0, def.RequiredStars);
        }

        [Test]
        public void Catalog_ThresholdsMatchPlaceholders()
        {
            // P8 originals unchanged; P13 additions interleave. All values
            // remain PLACEHOLDERS pending P4B + balance review.
            Assert.AreEqual(0, WardrobeCatalog.GetById("classic_color_knight").RequiredStars);
            Assert.AreEqual(8, WardrobeCatalog.GetById("forest_cavalier").RequiredStars);
            Assert.AreEqual(12, WardrobeCatalog.GetById("crimson_hero").RequiredStars);
            Assert.AreEqual(15, WardrobeCatalog.GetById("sunshine_knight").RequiredStars);
            Assert.AreEqual(22, WardrobeCatalog.GetById("aqua_knight").RequiredStars);
            Assert.AreEqual(26, WardrobeCatalog.GetById("pastel_prince").RequiredStars);
            Assert.AreEqual(30, WardrobeCatalog.GetById("silver_dreamer").RequiredStars);
        }

        // Pins the approved runtime catalog ids AND their order (the panel
        // builds rows in catalog order; ascending Star threshold). Changing
        // the set or order requires explicit approval - update this test
        // alongside that approval. (Last approved: rookie_page folded into
        // the default, 8 -> 7.)
        [Test]
        public void Catalog_OrderAndIdsArePinned()
        {
            var expected = new[]
            {
                "classic_color_knight",
                "forest_cavalier",
                "crimson_hero",
                "sunshine_knight",
                "aqua_knight",
                "pastel_prince",
                "silver_dreamer",
            };
            Assert.AreEqual(expected.Length, WardrobeCatalog.Outfits.Count);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], WardrobeCatalog.Outfits[i].Id,
                    "catalog order/id changed at index " + i);
        }

        // The remaining P13 additions are runtime, Stars-gated (not always
        // unlocked), and cosmetic-only like the rest of the catalog.
        [Test]
        public void Catalog_P13AdditionsAreRuntimeAndGated()
        {
            foreach (var id in new[]
                { "crimson_hero", "pastel_prince" })
            {
                var def = WardrobeCatalog.GetById(id);
                Assert.IsNotNull(def, id);
                Assert.IsFalse(def.AlwaysUnlocked, id);
                Assert.Greater(def.RequiredStars, 0, id);
            }
        }

        [Test]
        public void Catalog_GetById_HandlesNullAndUnknown()
        {
            Assert.IsNull(WardrobeCatalog.GetById(null));
            Assert.IsNull(WardrobeCatalog.GetById(""));
            Assert.IsNull(WardrobeCatalog.GetById("does_not_exist"));
            Assert.IsFalse(WardrobeCatalog.Exists("does_not_exist"));
            Assert.IsTrue(WardrobeCatalog.Exists("forest_cavalier"));
        }

        // ---- WardrobeStore ----

        [Test]
        public void Store_DefaultEquippedIsClassic()
        {
            Assert.AreEqual("classic_color_knight",
                WardrobeStore.GetEquippedOutfitId());
        }

        [Test]
        public void Store_SetsKnownId()
        {
            Assert.AreEqual("forest_cavalier",
                WardrobeStore.SetEquippedOutfitId("forest_cavalier"));
            Assert.AreEqual("forest_cavalier",
                WardrobeStore.GetEquippedOutfitId());
        }

        [Test]
        public void Store_UnknownOrNullFallsBackToDefault()
        {
            WardrobeStore.SetEquippedOutfitId("nope");
            Assert.AreEqual("classic_color_knight",
                WardrobeStore.GetEquippedOutfitId());
            WardrobeStore.SetEquippedOutfitId(null);
            Assert.AreEqual("classic_color_knight",
                WardrobeStore.GetEquippedOutfitId());
        }

        [Test]
        public void Store_ResetReturnsToDefault()
        {
            WardrobeStore.SetEquippedOutfitId("aqua_knight");
            WardrobeStore.Reset();
            Assert.AreEqual("classic_color_knight",
                WardrobeStore.GetEquippedOutfitId());
        }

        // ---- WardrobeUnlockService ----

        [Test]
        public void Unlock_DefaultUnlockedAtZeroStars()
        {
            var def = WardrobeCatalog.GetById("classic_color_knight");
            Assert.IsTrue(WardrobeUnlockService.IsUnlocked(def, 0));
        }

        [Test]
        public void Unlock_ForestLockedBelow8_UnlockedAt8()
        {
            var def = WardrobeCatalog.GetById("forest_cavalier");
            Assert.IsFalse(WardrobeUnlockService.IsUnlocked(def, 7));
            Assert.IsTrue(WardrobeUnlockService.IsUnlocked(def, 8));
            Assert.IsTrue(WardrobeUnlockService.IsUnlocked(def, 9));
        }

        [Test]
        public void Unlock_SunshineLockedBelow15_UnlockedAt15()
        {
            var def = WardrobeCatalog.GetById("sunshine_knight");
            Assert.IsFalse(WardrobeUnlockService.IsUnlocked(def, 14));
            Assert.IsTrue(WardrobeUnlockService.IsUnlocked(def, 15));
        }

        [Test]
        public void Unlock_StateEquippedOnlyWhenUnlockedAndMatches()
        {
            var forest = WardrobeCatalog.GetById("forest_cavalier");
            // Unlocked + equipped id matches -> Equipped
            Assert.AreEqual(WardrobeItemState.Equipped,
                WardrobeUnlockService.GetState(forest, "forest_cavalier", 8));
            // Unlocked + not equipped -> Unlocked
            Assert.AreEqual(WardrobeItemState.Unlocked,
                WardrobeUnlockService.GetState(forest, "classic_color_knight", 8));
            // Locked even if id matches -> Locked (cannot be equipped-state)
            Assert.AreEqual(WardrobeItemState.Locked,
                WardrobeUnlockService.GetState(forest, "forest_cavalier", 0));
        }

        // Pins the display rule the wardrobe panel relies on (both on open
        // and after a row click): when the stored equip is known but locked
        // at the current Stars (e.g. after a dev Stars reset), the
        // normalized id makes the DEFAULT row read Equipped and the locked
        // outfit read Locked - no state where nothing is Equipped.
        [Test]
        public void Unlock_NormalizedLockedEquip_DefaultReadsEquipped()
        {
            const int totalStars = 0; // silver_dreamer (30) is locked
            string normalized = WardrobeUnlockService.NormalizeEquippedId(
                "silver_dreamer", totalStars);
            Assert.AreEqual("classic_color_knight", normalized);

            var classic = WardrobeCatalog.GetById("classic_color_knight");
            var silver = WardrobeCatalog.GetById("silver_dreamer");
            Assert.AreEqual(WardrobeItemState.Equipped,
                WardrobeUnlockService.GetState(classic, normalized, totalStars));
            Assert.AreEqual(WardrobeItemState.Locked,
                WardrobeUnlockService.GetState(silver, normalized, totalStars));
        }

        [Test]
        public void Unlock_NormalizeUnknownOrLockedFallsBackToDefault()
        {
            // unknown stored id -> default
            Assert.AreEqual("classic_color_knight",
                WardrobeUnlockService.NormalizeEquippedId("nope", 100));
            // locked stored id (Silver needs 30) at 0 stars -> default
            Assert.AreEqual("classic_color_knight",
                WardrobeUnlockService.NormalizeEquippedId("silver_dreamer", 0));
            // unlocked stored id stays
            Assert.AreEqual("forest_cavalier",
                WardrobeUnlockService.NormalizeEquippedId("forest_cavalier", 8));
        }
    }
}
