using System.Text.RegularExpressions;
using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace JebbyJump.Tests
{
    // P11 outfit visual resolver + controller. Pure resolver checks plus a
    // few controller checks on a bare GameObject (no scene, no assets).
    // SetUp/TearDown reset the equipped key and destroy created objects so
    // tests do not leak. Every outfit is a safe no-op in P11 (no art).
    public class OutfitVisualTests
    {
        private GameObject _go;
        private AnimatorOverrideController _aoc;

        [SetUp]
        public void SetUp() => WardrobeStore.Reset();

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.DestroyImmediate(_go);
            _go = null;
            if (_aoc != null) Object.DestroyImmediate(_aoc);
            _aoc = null;
            WardrobeStore.Reset();
        }

        private PlayerOutfitVisualController NewController()
        {
            // No Animator/SpriteRenderer wired - exercises the null-safe path.
            _go = new GameObject("OutfitVisualTestPlayer");
            return _go.AddComponent<PlayerOutfitVisualController>();
        }

        // Bare GameObject with an Animator, for OutfitVisualApplier tests.
        private Animator NewAnimator()
        {
            _go = new GameObject("OutfitVisualTestAnimator");
            return _go.AddComponent<Animator>();
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

        // ---- OutfitVisualApplier (override-assignment rule) ----
        // P12 seam: production catalog never returns an override (no art yet);
        // these use an in-memory AnimatorOverrideController (no committed asset)
        // to prove the future override path assigns correctly.

        // Proves the applier ATTEMPTS the override assignment for an
        // override-bearing definition. An empty in-memory
        // AnimatorOverrideController is rejected by Unity (it logs an error),
        // and that error is emitted ONLY if the assignment was actually
        // attempted - so consuming the expected error proves the positive
        // branch ran, without needing a committed/editor controller asset.
        // (Asserting the assigned value would require a valid controller, i.e.
        // editor/asset APIs - out of P12 scope. The cases below prove the
        // applier does NOT assign when it should not.)
        [Test]
        public void Applier_AttemptsAssign_WhenDefinitionCarriesOverride()
        {
            var animator = NewAnimator();
            _aoc = new AnimatorOverrideController();
            var def = new OutfitVisualDefinition(
                "forest_cavalier", "Forest Cavalier", true, _aoc);

            LogAssert.Expect(LogType.Error,
                new Regex("Could not set Runtime Animator Controller"));

            var applied = OutfitVisualApplier.Apply(animator, def);

            Assert.AreEqual("forest_cavalier", applied);
        }

        [Test]
        public void Applier_DoesNotAssign_WhenHasVisualOverrideFalse()
        {
            var animator = NewAnimator();
            _aoc = new AnimatorOverrideController();
            // Override present but flag false -> must NOT assign.
            var def = new OutfitVisualDefinition(
                "forest_cavalier", "Forest Cavalier", false, _aoc);

            var applied = OutfitVisualApplier.Apply(animator, def);

            Assert.AreEqual("forest_cavalier", applied);
            Assert.IsNull(animator.runtimeAnimatorController);
        }

        [Test]
        public void Applier_DoesNotAssign_WhenOverrideNull()
        {
            var animator = NewAnimator();
            var def = new OutfitVisualDefinition(
                "forest_cavalier", "Forest Cavalier", true, null);

            var applied = OutfitVisualApplier.Apply(animator, def);

            Assert.AreEqual("forest_cavalier", applied);
            Assert.IsNull(animator.runtimeAnimatorController);
        }

        [Test]
        public void Applier_NullAnimator_DoesNotThrow()
        {
            _aoc = new AnimatorOverrideController();
            var def = new OutfitVisualDefinition(
                "forest_cavalier", "Forest Cavalier", true, _aoc);

            string applied = null;
            Assert.DoesNotThrow(() => applied = OutfitVisualApplier.Apply(null, def));
            Assert.AreEqual("forest_cavalier", applied);
        }
    }
}
