#if UNITY_EDITOR
using System.Collections.Generic;
using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P14 stabilization: validates the REAL P13B assets, not in-memory
    // stand-ins - the OutfitVisualLibrary asset's entries, each
    // AnimatorOverrideController's base + clip overrides, and the Jebby
    // prefab's wiring. Catches asset-side regressions (deleted library
    // entry, re-based controller, unwired prefab) that the pure logic
    // tests cannot see. Editor-only (AssetDatabase); the suite runs in
    // editor PlayMode. Read-only on assets; PlayerPrefs reset around the
    // end-to-end case.
    public class OutfitVisualAssetIntegrityTests
    {
        private const string LibraryPath =
            "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/OutfitVisualLibrary.asset";
        private const string JebbyPrefabPath =
            "Assets/_JebbyJump/Prefabs/Player/Jebby.prefab";
        private const string BaseControllerPath =
            "Assets/_JebbyJump/Art/Animations/JebbyAnimator.controller";

        private static readonly (string state, string defaultClip)[] States =
        {
            ("idle", "Jebby_Idle"),
            ("run", "Jebby_Run"),
            ("jump", "Jebby_Jump"),
            ("fall", "Jebby_Fall"),
            ("land", "Jebby_Land"),
            ("hurt", "Jebby_Hurt"),
            ("victory", "Jebby_Victory"),
        };

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

        private static OutfitVisualLibrary LoadLibrary()
        {
            var lib = AssetDatabase.LoadAssetAtPath<OutfitVisualLibrary>(LibraryPath);
            Assert.IsNotNull(lib, "library asset missing at " + LibraryPath);
            return lib;
        }

        [Test]
        public void LibraryAsset_HasEntriesForAllNonDefaultOutfits()
        {
            var lib = LoadLibrary();
            int nonDefault = 0;
            foreach (var o in WardrobeCatalog.Outfits)
            {
                if (o.Id == WardrobeCatalog.DefaultOutfitId) continue;
                nonDefault++;
                Assert.IsTrue(lib.TryGetOverride(o.Id, out var ctrl),
                    o.Id + " has no library entry / null controller");
                Assert.IsNotNull(ctrl, o.Id);
            }
            Assert.AreEqual(7, nonDefault);
            Assert.AreEqual(7, lib.Count, "unexpected extra library entries");
        }

        // The default outfit deliberately has NO library entry: applying it
        // is a no-op and the Animator keeps the serialized JebbyAnimator.
        // (Correct because outfits apply at spawn/scene load, where the
        // Animator starts from its prefab default - NOT a live-reset claim.)
        [Test]
        public void LibraryAsset_DefaultOutfitIntentionallyHasNoEntry()
        {
            var lib = LoadLibrary();
            Assert.IsFalse(
                lib.TryGetOverride(WardrobeCatalog.DefaultOutfitId, out _),
                "default outfit must stay no-op (base JebbyAnimator)");
        }

        // P19: no duplicate ids in the visual library (DuplicateVisualId guard).
        [Test]
        public void LibraryAsset_HasNoDuplicateIds()
        {
            var lib = LoadLibrary();
            var seen = new HashSet<string>();
            foreach (var id in lib.EntryIds())
                Assert.IsTrue(seen.Add(id), "duplicate visual library id: " + id);
        }

        [Test]
        public void LibraryAsset_ControllersAreOverridesOfJebbyAnimator()
        {
            var baseController =
                AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                    BaseControllerPath);
            Assert.IsNotNull(baseController, "JebbyAnimator asset missing");

            var lib = LoadLibrary();
            foreach (var o in WardrobeCatalog.Outfits)
            {
                if (o.Id == WardrobeCatalog.DefaultOutfitId) continue;
                Assert.IsTrue(lib.TryGetOverride(o.Id, out var ctrl), o.Id);

                var aoc = ctrl as AnimatorOverrideController;
                Assert.IsNotNull(aoc,
                    o.Id + " override is not an AnimatorOverrideController");
                Assert.AreSame(baseController, aoc.runtimeAnimatorController,
                    o.Id + " override is not based on JebbyAnimator");

                var overrides =
                    new List<KeyValuePair<AnimationClip, AnimationClip>>();
                aoc.GetOverrides(overrides);
                foreach (var (state, defaultClip) in States)
                {
                    AnimationClip found = null;
                    foreach (var pair in overrides)
                        if (pair.Key != null && pair.Key.name == defaultClip)
                            found = pair.Value;
                    Assert.IsNotNull(found,
                        o.Id + " does not override " + defaultClip);
                    Assert.AreEqual(
                        "anim_jebby_" + o.Id + "_" + state, found.name,
                        o.Id + "/" + state + " unexpected clip name");
                }
            }
        }

        [Test]
        public void JebbyPrefab_HasWiredOutfitVisualController()
        {
            var prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(JebbyPrefabPath);
            Assert.IsNotNull(prefab, "Jebby prefab missing");

            var ctrl = prefab.GetComponent<PlayerOutfitVisualController>();
            Assert.IsNotNull(ctrl,
                "Jebby prefab has no PlayerOutfitVisualController");

            var so = new SerializedObject(ctrl);
            Assert.IsNotNull(so.FindProperty("_animator").objectReferenceValue,
                "_animator not wired");
            Assert.IsNotNull(
                so.FindProperty("_spriteRenderer").objectReferenceValue,
                "_spriteRenderer not wired");
            Assert.AreSame(LoadLibrary(),
                so.FindProperty("_library").objectReferenceValue,
                "_library not wired to the OutfitVisualLibrary asset");
        }

        // End-to-end through the REAL library: the equipped outfit's actual
        // AnimatorOverrideController is assigned. Also documents the
        // spawn-only semantics: re-applying the default afterwards is a
        // no-op (the override remains until the next spawn/scene load
        // resets the Animator to its serialized controller) - live
        // mid-scene reset is intentionally NOT supported.
        [Test]
        public void EndToEnd_RealLibraryOverrideAppliesForEquippedOutfit()
        {
            var lib = LoadLibrary();
            Assert.IsTrue(lib.TryGetOverride("forest_cavalier", out var expected));

            _go = new GameObject("OutfitAssetIntegrityPlayer");
            var animator = _go.AddComponent<Animator>();
            var ctrl = _go.AddComponent<PlayerOutfitVisualController>();
            var so = new SerializedObject(ctrl);
            so.FindProperty("_animator").objectReferenceValue = animator;
            so.FindProperty("_library").objectReferenceValue = lib;
            so.ApplyModifiedPropertiesWithoutUndo();

            WardrobeStore.SetEquippedOutfitId("forest_cavalier");
            ctrl.ApplyEquippedOutfit();
            Assert.AreSame(expected, animator.runtimeAnimatorController);
            Assert.AreEqual("forest_cavalier", ctrl.CurrentOutfitId);

            // P17: applying the default RESTORES the captured default
            // controller - but this controller captured null at Awake (its
            // _animator was wired after AddComponent), so the restore is a
            // NoOp here and the override remains. Real default restoration
            // (with a captured JebbyAnimator) is covered by
            // OutfitVisualLiveResyncTests editor integration.
            WardrobeStore.SetEquippedOutfitId(WardrobeCatalog.DefaultOutfitId);
            ctrl.ApplyEquippedOutfit();
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId, ctrl.CurrentOutfitId);
            Assert.AreSame(expected, animator.runtimeAnimatorController,
                "no captured default -> restore is NoOp; override stays");
        }
    }
}
#endif
