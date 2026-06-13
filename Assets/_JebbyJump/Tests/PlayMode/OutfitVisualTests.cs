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
        private OutfitVisualLibrary _library;

        [SetUp]
        public void SetUp() => WardrobeStore.Reset();

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.DestroyImmediate(_go);
            _go = null;
            if (_aoc != null) Object.DestroyImmediate(_aoc);
            _aoc = null;
            if (_library != null) Object.DestroyImmediate(_library);
            _library = null;
            WardrobeStore.Reset();
        }

        private OutfitVisualLibrary NewLibrary()
        {
            _library = ScriptableObject.CreateInstance<OutfitVisualLibrary>();
            return _library;
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

        // The P13 catalog additions resolve to themselves; the STATIC layer
        // still carries no override (overrides come only from the serialized
        // OutfitVisualLibrary). Arbitrary unknown ids keep falling back to
        // default - the visual layer stays outfit-agnostic.
        [Test]
        public void Resolver_P13AdditionsResolveToThemselves_NoStaticOverride()
        {
            foreach (var id in new[]
                { "crimson_hero", "rookie_page", "pastel_prince" })
            {
                var def = OutfitVisualCatalog.GetVisualForOutfit(id);
                Assert.AreEqual(id, def.OutfitId);
                Assert.IsFalse(def.HasVisualOverride, id);
                Assert.IsNull(def.AnimatorControllerOverride, id);
            }
        }

        // ---- OutfitVisualLibrary (serialized override layer) ----

        [Test]
        public void Library_EntryProvidesOverrideForResolvedOutfit()
        {
            var lib = NewLibrary();
            _aoc = new AnimatorOverrideController();
            lib.AddEntry("forest_cavalier", _aoc);

            var def = OutfitVisualCatalog.GetVisualForOutfit(
                "forest_cavalier", lib);
            Assert.IsTrue(def.HasVisualOverride);
            Assert.AreSame(_aoc, def.AnimatorControllerOverride);
            Assert.AreEqual("forest_cavalier", def.OutfitId);
        }

        [Test]
        public void Library_OutfitWithoutEntryStaysNoOp()
        {
            var lib = NewLibrary();
            _aoc = new AnimatorOverrideController();
            lib.AddEntry("forest_cavalier", _aoc);

            var def = OutfitVisualCatalog.GetVisualForOutfit("aqua_knight", lib);
            Assert.IsFalse(def.HasVisualOverride);
            Assert.IsNull(def.AnimatorControllerOverride);
            Assert.AreEqual("aqua_knight", def.OutfitId);
        }

        [Test]
        public void Library_UnknownIdNormalizesBeforeLookup()
        {
            var lib = NewLibrary();
            _aoc = new AnimatorOverrideController();
            lib.AddEntry(WardrobeCatalog.DefaultOutfitId, _aoc);

            // Unknown id -> default id -> default's library entry applies.
            var def = OutfitVisualCatalog.GetVisualForOutfit("does_not_exist", lib);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId, def.OutfitId);
            Assert.IsTrue(def.HasVisualOverride);
            Assert.AreSame(_aoc, def.AnimatorControllerOverride);
        }

        [Test]
        public void Library_NullLibraryOrNullControllerStaysNoOp()
        {
            var defNoLib = OutfitVisualCatalog.GetVisualForOutfit(
                "forest_cavalier", null);
            Assert.IsFalse(defNoLib.HasVisualOverride);

            var lib = NewLibrary();
            lib.AddEntry("forest_cavalier", null);
            Assert.IsFalse(lib.TryGetOverride("forest_cavalier", out _));
            var def = OutfitVisualCatalog.GetVisualForOutfit(
                "forest_cavalier", lib);
            Assert.IsFalse(def.HasVisualOverride);
        }

        [Test]
        public void Library_AddEntryReplacesExistingForSameId()
        {
            var lib = NewLibrary();
            _aoc = new AnimatorOverrideController();
            lib.AddEntry("forest_cavalier", null);
            lib.AddEntry("forest_cavalier", _aoc);
            Assert.AreEqual(1, lib.Count);
            Assert.IsTrue(lib.TryGetOverride("forest_cavalier", out var c));
            Assert.AreSame(_aoc, c);
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

        // P17 3-arg applier: override-bearing def assigns the override; any
        // other case restores the captured default (NoOp when no default is
        // captured - never assigns null). Valid-controller assign/restore are
        // covered by the editor integration tests (real JebbyAnimator + AOCs);
        // these pure cases use an empty in-memory AnimatorOverrideController
        // (Unity logs an error when it is assigned, which proves the override
        // branch ran) and a null default.
        [Test]
        public void Applier_AppliesOverride_WhenDefinitionCarriesOne()
        {
            var animator = NewAnimator();
            _aoc = new AnimatorOverrideController();
            var def = new OutfitVisualDefinition(
                "forest_cavalier", "Forest Cavalier", true, _aoc);

            LogAssert.Expect(LogType.Error,
                new Regex("Could not set Runtime Animator Controller"));

            var result = OutfitVisualApplier.Apply(animator, null, def);

            Assert.AreEqual(OutfitVisualApplyResult.AppliedOverride, result);
        }

        [Test]
        public void Applier_NoOp_WhenNoOverrideFlagAndNoDefault()
        {
            var animator = NewAnimator();
            _aoc = new AnimatorOverrideController();
            // Override present but flag false -> restore path; null default
            // -> NoOp (must not assign, must not null the controller).
            var def = new OutfitVisualDefinition(
                "forest_cavalier", "Forest Cavalier", false, _aoc);

            var result = OutfitVisualApplier.Apply(animator, null, def);

            Assert.AreEqual(OutfitVisualApplyResult.NoOp, result);
            Assert.IsNull(animator.runtimeAnimatorController);
        }

        [Test]
        public void Applier_NoOp_WhenOverrideNullAndNoDefault()
        {
            var animator = NewAnimator();
            var def = new OutfitVisualDefinition(
                "forest_cavalier", "Forest Cavalier", true, null);

            var result = OutfitVisualApplier.Apply(animator, null, def);

            Assert.AreEqual(OutfitVisualApplyResult.NoOp, result);
            Assert.IsNull(animator.runtimeAnimatorController);
        }

        [Test]
        public void Applier_NullAnimator_ReturnsNoAnimator_NoThrow()
        {
            _aoc = new AnimatorOverrideController();
            var def = new OutfitVisualDefinition(
                "forest_cavalier", "Forest Cavalier", true, _aoc);

            var result = OutfitVisualApplyResult.AppliedOverride;
            Assert.DoesNotThrow(() =>
                result = OutfitVisualApplier.Apply(null, null, def));
            Assert.AreEqual(OutfitVisualApplyResult.NoAnimator, result);
        }
    }
}
