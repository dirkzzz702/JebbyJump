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
            WardrobeAppearanceEvents.ResetForTests();
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

        // P19 (changed from P18): a FUTURE schema is READ-ONLY. The save is left
        // exactly as found - no downgrade AND no equipped normalization/write.
        [Test]
        public void FutureVersion_NoWritesNoNormalize()
        {
            PlayerPrefs.SetInt(WardrobePersistenceKeys.SchemaVersion, 99);
            PlayerPrefs.SetString(
                WardrobePersistenceKeys.EquippedOutfit, "silver_dreamer");
            var r = WardrobePersistenceMigrator.MigrateIfNeeded(0); // would be locked
            Assert.AreEqual(99, Schema(), "future version must not downgrade");
            Assert.AreEqual("silver_dreamer", PlayerPrefs.GetString(
                WardrobePersistenceKeys.EquippedOutfit, ""),
                "future save equipped id must not be rewritten");
            Assert.AreEqual(WardrobeMigrationStatus.FutureVersionUnsupported, r.Status);
            Assert.IsFalse(r.DidWrite);
            Assert.IsTrue(r.IsFutureVersionUnsupported);
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

        // A migration repair writes through the low-level store, so it must NOT
        // raise the appearance change event (that is reserved for user equips).
        [Test]
        public void DoesNotPublishAppearanceEvent()
        {
            int events = 0;
            WardrobeAppearanceEvents.EquippedOutfitChanged += _ => events++;
            WardrobeStore.SetEquippedOutfitId("silver_dreamer"); // will normalize
            WardrobePersistenceMigrator.MigrateIfNeeded(0);
            Assert.AreEqual(0, events,
                "migration normalization must not publish an appearance event");
        }

        [Test]
        public void Status_LegacyClean_IsMigratedLegacy()
        {
            WardrobeStore.SetEquippedOutfitId("forest_cavalier");
            var r = WardrobePersistenceMigrator.MigrateIfNeeded(8); // unlocked
            Assert.AreEqual(WardrobeMigrationStatus.MigratedLegacy, r.Status);
            Assert.IsTrue(r.DidWrite); // schema stamped
            Assert.IsFalse(r.EquippedOutfitChanged);
        }

        [Test]
        public void Status_LegacyNormalize_IsMigratedAndNormalized()
        {
            WardrobeStore.SetEquippedOutfitId("silver_dreamer");
            var r = WardrobePersistenceMigrator.MigrateIfNeeded(0); // locked
            Assert.AreEqual(WardrobeMigrationStatus.MigratedAndNormalized, r.Status);
            Assert.IsTrue(r.DidWrite);
        }

        [Test]
        public void Status_CurrentNormalize_IsNormalizedCurrent()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            WardrobeStore.SetEquippedOutfitId("silver_dreamer");
            var r = WardrobePersistenceMigrator.MigrateIfNeeded(0); // locked
            Assert.AreEqual(WardrobeMigrationStatus.NormalizedCurrent, r.Status);
            Assert.IsTrue(r.DidWrite);
        }

        // Required: a MISSING equipped key is a clean implicit Classic; migration
        // must not materialize it.
        [Test]
        public void MissingEquippedKey_StaysCleanImplicitDefault()
        {
            var r = WardrobePersistenceMigrator.MigrateIfNeeded(100);
            Assert.IsFalse(r.EquippedOutfitChanged);
            Assert.IsFalse(
                PlayerPrefs.HasKey(WardrobePersistenceKeys.EquippedOutfit),
                "missing equipped key must not be created by migration");
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
        }

        // Schema is stamped LAST: if a run is interrupted after the equipped
        // write but before the stamp, the next run completes safely + idempotent.
        [Test]
        public void SchemaLastRecovery_RemigratesCleanly()
        {
            PlayerPrefs.SetString(WardrobePersistenceKeys.EquippedOutfit,
                WardrobeCatalog.DefaultOutfitId); // equipped already repaired
            // schema key intentionally absent (stamp interrupted)
            var r1 = WardrobePersistenceMigrator.MigrateIfNeeded(0);
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema());
            Assert.IsFalse(r1.EquippedOutfitChanged);
            Assert.AreEqual(WardrobeMigrationStatus.MigratedLegacy, r1.Status);

            var r2 = WardrobePersistenceMigrator.MigrateIfNeeded(0);
            Assert.IsFalse(r2.DidWrite);
            Assert.AreEqual(WardrobeMigrationStatus.NoChange, r2.Status);
        }

        // Required: read-only safe effective id. FUTURE -> Classic in memory,
        // save untouched.
        [Test]
        public void EffectiveOutfitId_FutureSchema_ReturnsDefaultNoWrite()
        {
            PlayerPrefs.SetInt(WardrobePersistenceKeys.SchemaVersion, 99);
            PlayerPrefs.SetString(
                WardrobePersistenceKeys.EquippedOutfit, "silver_dreamer");
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobePersistenceMigrator.GetEffectiveOutfitId());
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobePersistenceMigrator.GetEffectiveOutfitId(0));
            Assert.AreEqual("silver_dreamer", PlayerPrefs.GetString(
                WardrobePersistenceKeys.EquippedOutfit, ""), "save unchanged");
            Assert.AreEqual(99, Schema());
        }

        [Test]
        public void EffectiveOutfitId_Supported_ReturnsStoredAndLockNormalized()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            WardrobeStore.SetEquippedOutfitId("forest_cavalier");
            Assert.AreEqual("forest_cavalier",
                WardrobePersistenceMigrator.GetEffectiveOutfitId());   // stars-free
            Assert.AreEqual("forest_cavalier",
                WardrobePersistenceMigrator.GetEffectiveOutfitId(8));  // unlocked
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobePersistenceMigrator.GetEffectiveOutfitId(0));  // locked
        }
    }
}
