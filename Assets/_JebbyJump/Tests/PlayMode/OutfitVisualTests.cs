using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P11 outfit visual resolver + controller. Pure resolver checks plus a
    // few controller checks on a bare GameObject (no scene, no assets).
    // SetUp/TearDown reset the equipped key and destroy created objects so
    // tests do not leak. Every outfit is a safe no-op in P11 (no art).
    public class OutfitVisualTests
    {
        private GameObject _go;

        [SetUp]
        public void SetUp() => WardrobeStore.Reset();

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.DestroyImmediate(_go);
            _go = null;
            WardrobeStore.Reset();
        }

        private PlayerOutfitVisualController NewController()
        {
            // No Animator/SpriteRenderer wired - exercises the null-safe path.
            _go = new GameObject("OutfitVisualTestPlayer");
            return _go.AddComponent<PlayerOutfitVisualController>();
        }

        // ---- OutfitVisualCatalog / resolver ----

        [Test]
        public void Resolver_AllCatalogOutfitsResolve()
        {
            foreach (var o in WardrobeCatalog.Outfits)
            {
                var def = OutfitVisualCatalog.GetVisualForOutfit(o.Id);
                Assert.IsNotNull(def);
                Assert.AreEqual(o.Id, def.OutfitId);
                Assert.AreEqual(o.DisplayName, def.DisplayName);
            }
        }

        [Test]
        public void Resolver_NullEmptyUnknownResolveToDefault()
        {
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                OutfitVisualCatalog.GetVisualForOutfit(null).OutfitId);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                OutfitVisualCatalog.GetVisualForOutfit("").OutfitId);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                OutfitVisualCatalog.GetVisualForOutfit("does_not_exist").OutfitId);
        }

        [Test]
        public void Resolver_DefaultHasNoOverride()
        {
            var def = OutfitVisualCatalog.GetVisualForOutfit(
                WardrobeCatalog.DefaultOutfitId);
            Assert.AreEqual("classic_color_knight", def.OutfitId);
            Assert.IsFalse(def.HasVisualOverride);
            Assert.IsNull(def.AnimatorControllerOverride);
            Assert.IsFalse(OutfitVisualCatalog.HasVisual(
                WardrobeCatalog.DefaultOutfitId));
        }

        [Test]
        public void Resolver_NonDefaultOutfitsHaveNoOverrideAndDoNotCrash()
        {
            foreach (var o in WardrobeCatalog.Outfits)
            {
                if (o.Id == WardrobeCatalog.DefaultOutfitId) continue;
                var def = OutfitVisualCatalog.GetVisualForOutfit(o.Id);
                Assert.IsFalse(def.HasVisualOverride,
                    o.Id + " should have no override");
                Assert.IsNull(def.AnimatorControllerOverride, o.Id);
            }
        }

        // ---- PlayerOutfitVisualController ----

        [Test]
        public void Controller_ApplyNullFallsBackToDefault()
        {
            var c = NewController();
            c.ApplyOutfit(null);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId, c.CurrentOutfitId);
        }

        [Test]
        public void Controller_ApplyUnknownFallsBackToDefault()
        {
            var c = NewController();
            c.ApplyOutfit("does_not_exist");
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId, c.CurrentOutfitId);
        }

        [Test]
        public void Controller_ApplyDefaultRecordsDefault_NoAnimator_NoThrow()
        {
            var c = NewController();
            Assert.IsFalse(c.HasSpriteRenderer);
            Assert.DoesNotThrow(() =>
                c.ApplyOutfit(WardrobeCatalog.DefaultOutfitId));
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId, c.CurrentOutfitId);
        }

        [Test]
        public void Controller_ApplyNonDefaultNoOp_DoesNotThrow_RecordsId()
        {
            var c = NewController();
            Assert.DoesNotThrow(() => c.ApplyOutfit("forest_cavalier"));
            Assert.AreEqual("forest_cavalier", c.CurrentOutfitId);
        }

        [Test]
        public void Controller_ApplyEquippedReadsStore()
        {
            WardrobeStore.SetEquippedOutfitId("forest_cavalier");
            var c = NewController();
            c.ApplyEquippedOutfit();
            Assert.AreEqual("forest_cavalier", c.CurrentOutfitId);
        }
    }
}
