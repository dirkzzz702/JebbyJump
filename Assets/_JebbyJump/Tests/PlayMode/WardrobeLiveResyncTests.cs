using System.Collections.Generic;
using JebbyJump.Rewards;
using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P17 validated equip service + change event (publish only on success).
    public class WardrobeEquipServiceTests
    {
        private readonly List<WardrobeEquippedOutfitChanged> _events =
            new List<WardrobeEquippedOutfitChanged>();

        [SetUp]
        public void SetUp()
        {
            WardrobeStore.Reset();
            WardrobeAppearanceEvents.ResetForTests();
            _events.Clear();
            WardrobeAppearanceEvents.EquippedOutfitChanged += _events.Add;
        }

        [TearDown]
        public void TearDown()
        {
            WardrobeAppearanceEvents.ResetForTests();
            WardrobeStore.Reset();
        }

        [Test]
        public void KnownUnlocked_StoresAndReturnsSuccess()
        {
            Assert.AreEqual(WardrobeEquipResult.Success,
                WardrobeEquipService.TryEquip("forest_cavalier", 8));
            Assert.AreEqual("forest_cavalier", WardrobeStore.GetEquippedOutfitId());
        }

        [Test]
        public void Locked_ReturnsLockedAndDoesNotWriteOrPublish()
        {
            Assert.AreEqual(WardrobeEquipResult.Locked,
                WardrobeEquipService.TryEquip("silver_dreamer", 0));
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
            Assert.AreEqual(0, _events.Count);
        }

        [Test]
        public void Unknown_ReturnsUnknownAndDoesNotWriteOrPublish()
        {
            Assert.AreEqual(WardrobeEquipResult.UnknownOutfit,
                WardrobeEquipService.TryEquip("does_not_exist", 100));
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
            Assert.AreEqual(0, _events.Count);
        }

        [Test]
        public void AlreadyEquipped_ReturnsAlreadyAndDoesNotPublishAgain()
        {
            Assert.AreEqual(WardrobeEquipResult.Success,
                WardrobeEquipService.TryEquip("forest_cavalier", 8));
            _events.Clear();
            Assert.AreEqual(WardrobeEquipResult.AlreadyEquipped,
                WardrobeEquipService.TryEquip("forest_cavalier", 8));
            Assert.AreEqual(0, _events.Count);
        }

        [Test]
        public void Success_PublishesPreviousAndCurrentIds()
        {
            WardrobeEquipService.TryEquip("forest_cavalier", 8);
            Assert.AreEqual(1, _events.Count);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId, _events[0].PreviousOutfitId);
            Assert.AreEqual("forest_cavalier", _events[0].CurrentOutfitId);
        }

        [Test]
        public void Unsubscribe_StopsCallbacks()
        {
            WardrobeAppearanceEvents.EquippedOutfitChanged -= _events.Add;
            WardrobeEquipService.TryEquip("forest_cavalier", 8);
            Assert.AreEqual(0, _events.Count);
        }

        [Test]
        public void DoesNotModifyStars()
        {
            StarRewardStore.ResetAll(10);
            StarRewardStore.SetStarsIfHigher(0, 3);
            int before = StarRewardStore.GetTotalStars(10);
            WardrobeEquipService.TryEquip("forest_cavalier", 8);
            Assert.AreEqual(before, StarRewardStore.GetTotalStars(10));
            StarRewardStore.ResetAll(10);
        }
    }

    // P17 live re-sync on the player visual controller (no scene assets).
    public class PlayerOutfitLiveResyncTests
    {
        private GameObject _go;

        [SetUp]
        public void SetUp()
        {
            WardrobeStore.Reset();
            WardrobeAppearanceEvents.ResetForTests();
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.DestroyImmediate(_go);
            _go = null;
            WardrobeAppearanceEvents.ResetForTests();
            WardrobeStore.Reset();
        }

        private PlayerOutfitVisualController NewController()
        {
            _go = new GameObject("LiveResyncTestPlayer");
            return _go.AddComponent<PlayerOutfitVisualController>();
        }

        [Test]
        public void Event_UpdatesCurrentOutfitId()
        {
            var c = NewController(); // OnEnable subscribed
            WardrobeEquipService.TryEquip("forest_cavalier", 8); // publishes
            Assert.AreEqual("forest_cavalier", c.CurrentOutfitId);
        }

        [Test]
        public void Disable_Unsubscribes()
        {
            var c = NewController();
            _go.SetActive(false); // OnDisable unsubscribes
            WardrobeEquipService.TryEquip("aqua_knight", 22);
            Assert.AreNotEqual("aqua_knight", c.CurrentOutfitId);
        }

        [Test]
        public void Reenable_Resubscribes()
        {
            var c = NewController();
            _go.SetActive(false);
            _go.SetActive(true); // OnEnable subscribes again
            WardrobeEquipService.TryEquip("forest_cavalier", 8);
            Assert.AreEqual("forest_cavalier", c.CurrentOutfitId);
        }
    }

#if UNITY_EDITOR
    // P17 editor integration against the REAL OutfitVisualLibrary + JebbyAnimator:
    // proves override->override switching and override->default restoration with
    // valid controllers (the pure tests use an empty in-memory AOC and a null
    // default, so they only cover the no-op/attempt branches).
    public class OutfitVisualLiveResyncTests
    {
        private const string LibraryPath =
            "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/OutfitVisualLibrary.asset";
        private const string JebbyAnimatorPath =
            "Assets/_JebbyJump/Art/Animations/JebbyAnimator.controller";

        private GameObject _go;
        private OutfitVisualLibrary _lib;
        private RuntimeAnimatorController _default;

        [SetUp]
        public void SetUp()
        {
            _lib = UnityEditor.AssetDatabase
                .LoadAssetAtPath<OutfitVisualLibrary>(LibraryPath);
            _default = UnityEditor.AssetDatabase
                .LoadAssetAtPath<RuntimeAnimatorController>(JebbyAnimatorPath);
            Assert.IsNotNull(_lib, "OutfitVisualLibrary asset missing");
            Assert.IsNotNull(_default, "JebbyAnimator asset missing");
            _go = new GameObject("OutfitLiveResyncEditorPlayer");
            _go.AddComponent<Animator>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null) Object.DestroyImmediate(_go);
            _go = null;
        }

        private Animator Anim => _go.GetComponent<Animator>();

        private OutfitVisualDefinition Def(string id)
            => OutfitVisualCatalog.GetVisualForOutfit(id, _lib);

        [Test]
        public void ForestToAqua_AppliesAquaController()
        {
            _lib.TryGetOverride("aqua_knight", out var aqua);
            OutfitVisualApplier.Apply(Anim, _default, Def("forest_cavalier"));
            var r = OutfitVisualApplier.Apply(Anim, _default, Def("aqua_knight"));
            Assert.AreEqual(OutfitVisualApplyResult.AppliedOverride, r);
            Assert.AreSame(aqua, Anim.runtimeAnimatorController);
        }

        [Test]
        public void AquaToDefault_RestoresJebbyAnimator()
        {
            OutfitVisualApplier.Apply(Anim, _default, Def("aqua_knight"));
            var r = OutfitVisualApplier.Apply(
                Anim, _default, Def(WardrobeCatalog.DefaultOutfitId));
            Assert.AreEqual(OutfitVisualApplyResult.RestoredDefault, r);
            Assert.AreSame(_default, Anim.runtimeAnimatorController);
        }

        [Test]
        public void AllSevenOverrides_ApplySequentially()
        {
            foreach (var o in WardrobeCatalog.Outfits)
            {
                if (o.AlwaysUnlocked) continue;
                Assert.IsTrue(_lib.TryGetOverride(o.Id, out var expected), o.Id);
                var r = OutfitVisualApplier.Apply(Anim, _default, Def(o.Id));
                Assert.AreEqual(OutfitVisualApplyResult.AppliedOverride, r, o.Id);
                Assert.AreSame(expected, Anim.runtimeAnimatorController, o.Id);
            }
        }
    }
#endif
}
