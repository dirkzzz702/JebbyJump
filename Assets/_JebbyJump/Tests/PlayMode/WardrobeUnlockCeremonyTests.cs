using System.Collections.Generic;
using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // P16 acknowledgement store (local PlayerPrefs; not ownership).
    public class WardrobeUnlockAcknowledgementTests
    {
        [SetUp]
        public void SetUp() => WardrobeUnlockAcknowledgementStore.ResetAll();

        [TearDown]
        public void TearDown() => WardrobeUnlockAcknowledgementStore.ResetAll();

        [Test]
        public void DefaultFalseForNonDefault()
        {
            Assert.IsFalse(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("forest_cavalier"));
        }

        [Test]
        public void MarkAndRead()
        {
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged("forest_cavalier");
            Assert.IsTrue(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("forest_cavalier"));
            Assert.IsFalse(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("aqua_knight"));
        }

        [Test]
        public void NullEmptyUnknownSafe()
        {
            Assert.DoesNotThrow(() =>
            {
                WardrobeUnlockAcknowledgementStore.MarkAcknowledged(null);
                WardrobeUnlockAcknowledgementStore.MarkAcknowledged("");
                WardrobeUnlockAcknowledgementStore.MarkAcknowledged("does_not_exist");
                WardrobeUnlockAcknowledgementStore.ClearAcknowledged(null);
            });
            Assert.IsFalse(WardrobeUnlockAcknowledgementStore.IsAcknowledged(null));
            Assert.IsFalse(WardrobeUnlockAcknowledgementStore.IsAcknowledged(""));
            Assert.IsFalse(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("does_not_exist"));
        }

        [Test]
        public void ResetAllClears()
        {
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged("forest_cavalier");
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged("aqua_knight");
            WardrobeUnlockAcknowledgementStore.ResetAll();
            Assert.IsFalse(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("forest_cavalier"));
            Assert.IsFalse(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("aqua_knight"));
        }

        // Default/AlwaysUnlocked is treated as already acknowledged and never
        // gets a key (Mark is a no-op for it).
        [Test]
        public void DefaultOutfitIsTreatedAsAcknowledged()
        {
            Assert.IsTrue(WardrobeUnlockAcknowledgementStore.IsAcknowledged(
                WardrobeCatalog.DefaultOutfitId));
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged(
                WardrobeCatalog.DefaultOutfitId);
            Assert.IsTrue(WardrobeUnlockAcknowledgementStore.IsAcknowledged(
                WardrobeCatalog.DefaultOutfitId));
        }
    }

    // P16 new-unlock classifier (pure).
    public class WardrobeNewUnlockServiceTests
    {
        private static List<string> Ids(int totalStars, System.Func<string, bool> ack)
        {
            var result = new List<string>();
            foreach (var d in WardrobeNewUnlockService.GetNewlyUnlocked(totalStars, ack))
                result.Add(d.Id);
            return result;
        }

        [Test]
        public void ZeroStarsReturnsNone()
        {
            Assert.AreEqual(0, Ids(0, _ => false).Count);
        }

        [Test]
        public void FourStarsReturnsRookieOnly()
        {
            CollectionAssert.AreEqual(new[] { "rookie_page" }, Ids(4, _ => false));
        }

        [Test]
        public void FifteenStarsReturnsEligibleInCatalogOrder()
        {
            CollectionAssert.AreEqual(
                new[] { "rookie_page", "forest_cavalier", "crimson_hero", "sunshine_knight" },
                Ids(15, _ => false));
        }

        [Test]
        public void ExcludesAcknowledged()
        {
            CollectionAssert.AreEqual(
                new[] { "rookie_page", "crimson_hero", "sunshine_knight" },
                Ids(15, id => id == "forest_cavalier"));
        }

        [Test]
        public void ExcludesLockedAndDefault()
        {
            var all = Ids(100, _ => false);
            Assert.IsFalse(all.Contains(WardrobeCatalog.DefaultOutfitId));
            // every non-default outfit, none locked at 100 stars.
            int nonDefault = 0;
            foreach (var o in WardrobeCatalog.Outfits)
                if (!o.AlwaysUnlocked) nonDefault++;
            Assert.AreEqual(nonDefault, all.Count);
        }

        [Test]
        public void DoesNotMutateStoreOrStars()
        {
            string before = WardrobeStore.GetEquippedOutfitId();
            WardrobeNewUnlockService.GetNewlyUnlocked(15, _ => false);
            Assert.AreEqual(before, WardrobeStore.GetEquippedOutfitId());
        }
    }

    // P16 ceremony presenter (pure; ack only on Continue/EquipNow).
    public class WardrobeCeremonyPresenterTests
    {
        private static List<CosmeticItemDefinition> Items(params string[] ids)
        {
            var list = new List<CosmeticItemDefinition>();
            foreach (var id in ids) list.Add(WardrobeCatalog.GetById(id));
            return list;
        }

        [Test]
        public void ShowsFirst()
        {
            var p = new WardrobeCeremonyPresenter(
                Items("forest_cavalier", "crimson_hero"), _ => { }, _ => true);
            Assert.IsTrue(p.IsActive);
            Assert.AreEqual("forest_cavalier", p.Current.Id);
            Assert.AreEqual(1, p.Position);
            Assert.AreEqual(2, p.Count);
        }

        [Test]
        public void DoesNotAcknowledgeUntilAction()
        {
            var acked = new List<string>();
            var p = new WardrobeCeremonyPresenter(
                Items("forest_cavalier"), acked.Add, _ => true);
            Assert.AreEqual(0, acked.Count);
        }

        [Test]
        public void ContinueAcknowledgesAndAdvances()
        {
            var acked = new List<string>();
            var p = new WardrobeCeremonyPresenter(
                Items("forest_cavalier", "crimson_hero"), acked.Add, _ => true);
            p.Continue();
            CollectionAssert.AreEqual(new[] { "forest_cavalier" }, acked);
            Assert.AreEqual("crimson_hero", p.Current.Id);
        }

        [Test]
        public void EquipNowEquipsAcknowledgesAndAdvances()
        {
            var acked = new List<string>();
            var equipped = new List<string>();
            var p = new WardrobeCeremonyPresenter(
                Items("forest_cavalier", "crimson_hero"), acked.Add,
                id => { equipped.Add(id); return true; });
            Assert.IsTrue(p.EquipNow());
            CollectionAssert.AreEqual(new[] { "forest_cavalier" }, equipped);
            CollectionAssert.AreEqual(new[] { "forest_cavalier" }, acked);
            Assert.AreEqual("crimson_hero", p.Current.Id);
        }

        [Test]
        public void FailedEquipDoesNotAcknowledgeOrAdvance()
        {
            var acked = new List<string>();
            var p = new WardrobeCeremonyPresenter(
                Items("forest_cavalier"), acked.Add, _ => false);
            Assert.IsFalse(p.EquipNow());
            Assert.AreEqual(0, acked.Count);
            Assert.IsTrue(p.IsActive);
            Assert.AreEqual("forest_cavalier", p.Current.Id);
        }

        [Test]
        public void EndsCleanly()
        {
            var p = new WardrobeCeremonyPresenter(
                Items("forest_cavalier", "crimson_hero"), _ => { }, _ => true);
            p.Continue();
            Assert.IsFalse(p.Continue()); // acknowledges last, no more remain
            Assert.IsFalse(p.IsActive);
            Assert.IsNull(p.Current);
        }

        [Test]
        public void EmptyQueueIsInactive()
        {
            var p = new WardrobeCeremonyPresenter(
                new List<CosmeticItemDefinition>(), _ => { }, _ => true);
            Assert.IsFalse(p.IsActive);
            Assert.IsFalse(p.Continue());
            Assert.IsFalse(p.EquipNow());
        }
    }
}
