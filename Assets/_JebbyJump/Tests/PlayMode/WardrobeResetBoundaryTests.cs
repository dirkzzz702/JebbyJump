using JebbyJump.Progression;
using JebbyJump.Rewards;
using JebbyJump.Wardrobe;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P19 reset boundaries. ResetProgressTool lives in the editor assembly (not
    // test-reachable), so these compose the SAME store primitives it composes
    // and pin the wardrobe-facing outcome. The tool's composition is verified by
    // source inspection; these guard the primitives the tool relies on.
    public class WardrobeResetBoundaryTests
    {
        private const int LevelCount = 10;
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
            StarRewardStore.ResetAll(LevelCount);
            LevelProgressStore.ResetLocalProgress();
        }

        private static int Schema()
            => PlayerPrefs.GetInt(WardrobePersistenceKeys.SchemaVersion, 0);

        private static string RawEquipped()
            => PlayerPrefs.GetString(WardrobePersistenceKeys.EquippedOutfit, Sentinel);

        // Reset Wardrobe = WardrobeStore.Reset + ack ResetAll + StampCurrentVersion.
        [Test]
        public void ResetWardrobe_ClearsKnownAcksAndStampsCurrent()
        {
            WardrobeStore.SetEquippedOutfitId("forest_cavalier");
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged("forest_cavalier");

            WardrobeStore.Reset();
            WardrobeUnlockAcknowledgementStore.ResetAll();
            WardrobePersistenceMigrator.StampCurrentVersion();

            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
            Assert.IsFalse(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("forest_cavalier"));
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema());
        }

        // Reset Stars = StarRewardStore.ResetAll only. Acks + schema preserved;
        // equipped id may temporarily remain non-default (next init normalizes).
        [Test]
        public void ResetStars_PreservesAcksSchemaAndDoesNotNormalizeEquipped()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            WardrobeStore.SetEquippedOutfitId("silver_dreamer");
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged("forest_cavalier");
            StarRewardStore.SetStarsIfHigher(0, 3);

            StarRewardStore.ResetAll(LevelCount); // Reset Stars

            Assert.AreEqual(0, StarRewardStore.GetTotalStars(LevelCount));
            Assert.IsTrue(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("forest_cavalier"));
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema());
            Assert.AreEqual("silver_dreamer", RawEquipped(),
                "Reset Stars does not itself normalize the equipped id");
        }

        // Reset Stars leaves a now-locked equipped id that the NEXT init repairs.
        [Test]
        public void AfterResetStars_NextInitNormalizesLockedToDefault()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            WardrobeStore.SetEquippedOutfitId("silver_dreamer");
            StarRewardStore.SetStarsIfHigher(0, 3);

            StarRewardStore.ResetAll(LevelCount);
            WardrobePersistenceMigrator.MigrateIfNeeded(
                StarRewardStore.GetTotalStars(LevelCount)); // next startup

            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
        }

        // Reset Everything (wardrobe portion) ends clean at the current schema.
        [Test]
        public void ResetEverything_LeavesCleanCurrentSchema()
        {
            WardrobeStore.SetEquippedOutfitId("silver_dreamer");
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged("forest_cavalier");
            StarRewardStore.SetStarsIfHigher(0, 3);

            // Tool order: ResetLocalProgress + ResetBestTimes + ResetStars + ResetWardrobe.
            LevelProgressStore.ResetLocalProgress();
            StarRewardStore.ResetAll(LevelCount);
            WardrobeStore.Reset();
            WardrobeUnlockAcknowledgementStore.ResetAll();
            WardrobePersistenceMigrator.StampCurrentVersion();

            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                WardrobeStore.GetEquippedOutfitId());
            Assert.IsFalse(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("forest_cavalier"));
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema());
            Assert.AreEqual(0, StarRewardStore.GetTotalStars(LevelCount));
        }

        // Reset Local Progress targets only its own keys - wardrobe untouched.
        [Test]
        public void ResetLocalProgress_DoesNotTouchWardrobe()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            WardrobeStore.SetEquippedOutfitId("forest_cavalier");
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged("forest_cavalier");

            LevelProgressStore.ResetLocalProgress();

            Assert.AreEqual("forest_cavalier", RawEquipped());
            Assert.IsTrue(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("forest_cavalier"));
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema());
        }

        // Reset Best Times targets only "JebbyJump.BestTime.*" keys - wardrobe
        // untouched (the tool deletes those keys inline).
        [Test]
        public void ResetBestTimeKey_DoesNotTouchWardrobe()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            WardrobeStore.SetEquippedOutfitId("forest_cavalier");
            WardrobeUnlockAcknowledgementStore.MarkAcknowledged("forest_cavalier");
            PlayerPrefs.SetFloat("JebbyJump.BestTime.level_01", 12.3f);

            PlayerPrefs.DeleteKey("JebbyJump.BestTime.level_01"); // Reset Best Times
            PlayerPrefs.Save();

            Assert.AreEqual("forest_cavalier", RawEquipped());
            Assert.IsTrue(
                WardrobeUnlockAcknowledgementStore.IsAcknowledged("forest_cavalier"));
            Assert.AreEqual(WardrobePersistenceMigrator.CurrentVersion, Schema());
        }
    }
}
