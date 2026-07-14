using JebbyJump.Wardrobe;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P19 save-migration compatibility matrix. Parameterized over schema state,
    // equipped-id state, Star thresholds, acknowledgement state, and
    // repeatability. Uses the actual CurrentVersion. Reads RAW PlayerPrefs to
    // assert exactly what lands on disk.
    public class WardrobeSaveCompatibilityMatrixTests
    {
        private const string Sentinel = "<none>";

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

        private static void SetRawEquipped(string value)
        {
            PlayerPrefs.SetString(WardrobePersistenceKeys.EquippedOutfit, value);
            PlayerPrefs.Save();
        }

        private static string RawEquipped()
            => PlayerPrefs.GetString(WardrobePersistenceKeys.EquippedOutfit, Sentinel);

        private static int Schema()
            => PlayerPrefs.GetInt(WardrobePersistenceKeys.SchemaVersion, 0);

        // ---- Equipped normalization (legacy + current schema) ----

        [TestCase("classic_color_knight", 0, "classic_color_knight")]
        [TestCase("forest_cavalier", 8, "forest_cavalier")]   // unlocked preserved
        [TestCase("forest_cavalier", 0, "classic_color_knight")] // locked -> default
        [TestCase("silver_dreamer", 30, "silver_dreamer")]    // unlocked at threshold
        [TestCase("does_not_exist", 100, "classic_color_knight")] // unknown -> default
        [TestCase("", 100, "classic_color_knight")]           // empty -> default
        public void LegacyMatrix_EquippedNormalization(
            string raw, int stars, string expected)
        {
            // Legacy = no schema key.
            SetRawEquipped(raw);
            WardrobePersistenceMigrator.MigrateIfNeeded(stars);
            Assert.AreEqual(expected, RawEquipped(), "stored equipped id");
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema(),
                "legacy must stamp current schema");
        }

        [TestCase("classic_color_knight", 0, "classic_color_knight")]
        [TestCase("forest_cavalier", 8, "forest_cavalier")]
        [TestCase("forest_cavalier", 0, "classic_color_knight")] // current still normalizes
        [TestCase("silver_dreamer", 30, "silver_dreamer")]
        [TestCase("does_not_exist", 100, "classic_color_knight")]
        [TestCase("", 100, "classic_color_knight")]
        public void CurrentMatrix_EquippedNormalization(
            string raw, int stars, string expected)
        {
            WardrobePersistenceMigrator.StampCurrentVersion(); // schema = current
            SetRawEquipped(raw);
            WardrobePersistenceMigrator.MigrateIfNeeded(stars);
            Assert.AreEqual(expected, RawEquipped(), "stored equipped id");
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema(),
                "current schema preserved");
        }

        // ---- Threshold boundaries (below / exact / above) ----

        [TestCase("forest_cavalier", 7, false)]
        [TestCase("forest_cavalier", 8, true)]
        [TestCase("forest_cavalier", 9, true)]
        [TestCase("silver_dreamer", 29, false)]
        [TestCase("silver_dreamer", 30, true)]
        [TestCase("silver_dreamer", 31, true)]
        public void ThresholdBoundaryMatrix(
            string outfitId, int stars, bool expectedUnlocked)
        {
            Assert.AreEqual(expectedUnlocked, WardrobeUnlockService.IsUnlocked(
                WardrobeCatalog.GetById(outfitId), stars),
                "unlock state at boundary");

            SetRawEquipped(outfitId);
            WardrobePersistenceMigrator.MigrateIfNeeded(stars);
            Assert.AreEqual(
                expectedUnlocked ? outfitId : WardrobeCatalog.DefaultOutfitId,
                RawEquipped(),
                "equipped preserved iff unlocked, else default");
        }

        // ---- Acknowledgement preservation ----

        [TestCase("forest_cavalier", 8)]   // still unlocked
        [TestCase("forest_cavalier", 0)]   // locked by Stars reset
        [TestCase("silver_dreamer", 0)]    // locked
        public void AcknowledgementPreservationMatrix(string ackId, int stars)
        {
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged(ackId);
            WardrobePersistenceMigrator.MigrateIfNeeded(stars);
            Assert.IsTrue(WardrobeUnlockAcknowledgementStore.IsAcknowledged(ackId),
                "migration/normalization must never clear acknowledgements");
        }

        // ---- Future version: no writes ----

        [TestCase(2, "silver_dreamer", 0)]
        [TestCase(99, "does_not_exist", 100)]
        [TestCase(2, "", 0)]
        public void FutureVersionMatrix_NoWrites(
            int futureVersion, string raw, int stars)
        {
            PlayerPrefs.SetInt(WardrobePersistenceKeys.SchemaVersion, futureVersion);
            SetRawEquipped(raw);

            var r = WardrobePersistenceMigrator.MigrateIfNeeded(stars);

            Assert.AreEqual(raw, RawEquipped(), "future equipped id unchanged");
            Assert.AreEqual(futureVersion, Schema(), "future schema not downgraded");
            Assert.AreEqual(WardrobeMigrationStatus.FutureVersionUnsupported, r.Status);
            Assert.IsFalse(r.DidWrite);
        }

        // ---- Repeatability ----

        [TestCase("forest_cavalier", 8)]
        [TestCase("silver_dreamer", 0)]
        public void RepeatedMigration_SecondCallIsNoChange(string raw, int stars)
        {
            SetRawEquipped(raw);
            WardrobePersistenceMigrator.MigrateIfNeeded(stars); // first
            string afterFirst = RawEquipped();

            var second = WardrobePersistenceMigrator.MigrateIfNeeded(stars);

            Assert.AreEqual(afterFirst, RawEquipped(), "stable after first migration");
            Assert.IsFalse(second.EquippedOutfitChanged);
            Assert.IsFalse(second.DidWrite);
            Assert.AreEqual(WardrobeMigrationStatus.NoChange, second.Status);
        }
    }
}
