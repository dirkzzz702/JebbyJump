using JebbyJump.Rewards;
using JebbyJump.Wardrobe;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P18 wardrobe persistence migration. Ongoing equipped normalization runs
    // on every call (version-independent); the schema stamp is one-time.
    public class WardrobePersistenceMigrationTests
    {
        [SetUp]
        public void SetUp() => ResetState();

        [TearDown]
        public void TearDown() => ResetState();

        private static void ResetState()
        {
            PlayerPrefs.DeleteKey(WardrobePersistenceKeys.SchemaVersion);
            WardrobeStore.Reset();
            WardrobeUnlockAcknowledgementStore.ResetAll();
        }

        private static int Schema()
            => PlayerPrefs.GetInt(WardrobePersistenceKeys.SchemaVersion, 0);

        [Test]
        public void NoVersion_NormalizesLockedAndStoresCurrent()
        {
            WardrobeStore.SetEquippedOutfitId("silver_dreamer"); // 30 Stars
            var r = WardrobePersistenceMigrator.MigrateIfNeeded(0); // locked
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema());
            Assert.IsTrue(r.EquippedOutfitChanged);
            Assert.AreEqual(0, r.PreviousVersion);
        }

        [Test]
        public void UnknownEquipped_FallsBackToDefault()
        {
            PlayerPrefs.SetString(
                WardrobePersistenceKeys.EquippedOutfit, "does_not_exist");
            WardrobePersistenceMigrator.MigrateIfNeeded(100);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
        }

        [Test]
        public void EmptyEquipped_FallsBackToDefault()
        {
            PlayerPrefs.SetString(WardrobePersistenceKeys.EquippedOutfit, "");
            WardrobePersistenceMigrator.MigrateIfNeeded(100);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
        }

        [Test]
        public void ValidUnlockedEquipped_Preserved()
        {
            WardrobeStore.SetEquippedOutfitId("forest_cavalier");
            var r = WardrobePersistenceMigrator.MigrateIfNeeded(8);
            Assert.AreEqual("forest_cavalier", WardrobeStore.GetEquippedOutfitId());
            Assert.IsFalse(r.EquippedOutfitChanged);
        }

        // The key correction: a current schema version must NOT prevent a
        // now-locked equipped id from normalizing to default after a Stars drop.
        [Test]
        public void CurrentSchema_StillNormalizesLockedAfterStarReset()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            WardrobeStore.SetEquippedOutfitId("silver_dreamer");
            var r = WardrobePersistenceMigrator.MigrateIfNeeded(0);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
            Assert.IsTrue(r.EquippedOutfitChanged);
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion,
                r.PreviousVersion);
        }

        [Test]
        public void Idempotent_SecondCallNoChange()
        {
            WardrobeStore.SetEquippedOutfitId("silver_dreamer");
            WardrobePersistenceMigrator.MigrateIfNeeded(0);
            var r2 = WardrobePersistenceMigrator.MigrateIfNeeded(0);
            Assert.IsFalse(r2.EquippedOutfitChanged);
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion,
                r2.PreviousVersion);
        }

        [Test]
        public void FutureVersion_NotDowngraded_StillNormalizes()
        {
            PlayerPrefs.SetInt(WardrobePersistenceKeys.SchemaVersion, 99);
            WardrobeStore.SetEquippedOutfitId("silver_dreamer");
            WardrobePersistenceMigrator.MigrateIfNeeded(0);
            Assert.AreEqual(99, Schema(), "future version must not downgrade");
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
        }

        [Test]
        public void DoesNotChangeStars()
        {
            StarRewardStore.ResetAll(10);
            StarRewardStore.SetStarsIfHigher(0, 3);
            int before = StarRewardStore.GetTotalStars(10);
            WardrobePersistenceMigrator.MigrateIfNeeded(before);
            Assert.AreEqual(before, StarRewardStore.GetTotalStars(10));
            StarRewardStore.ResetAll(10);
        }

        [Test]
        public void DoesNotChangeAcknowledgements()
        {
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged("forest_cavalier");
            WardrobePersistenceMigrator.MigrateIfNeeded(8);
            Assert.IsTrue(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("forest_cavalier"));
        }

        [Test]
        public void StampCurrentVersion_SetsCurrent()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema());
        }

        [Test]
        public void Keys_PinExpectedLiterals()
        {
            Assert.AreEqual("jebby.wardrobe.equippedOutfit",
                WardrobePersistenceKeys.EquippedOutfit);
            Assert.AreEqual("jebby.wardrobe.unlockAcknowledged.",
                WardrobePersistenceKeys.UnlockAcknowledgementPrefix);
            Assert.AreEqual("jebby.wardrobe.schemaVersion",
                WardrobePersistenceKeys.SchemaVersion);
        }
    }
}
