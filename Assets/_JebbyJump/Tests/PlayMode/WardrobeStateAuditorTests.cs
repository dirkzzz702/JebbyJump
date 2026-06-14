using JebbyJump.Wardrobe;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P19 read-only persistence auditor. Reads RAW PlayerPrefs + key presence
    // (never sanitized WardrobeStore output); future schema is non-repairing;
    // never mutates PlayerPrefs.
    public class WardrobeStateAuditorTests
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

        // Writes a raw equipped value WITHOUT WardrobeStore sanitization.
        private static void SetRawEquipped(string value)
        {
            PlayerPrefs.SetString(WardrobePersistenceKeys.EquippedOutfit, value);
            PlayerPrefs.Save();
        }

        private static bool HasIssue(
            WardrobePersistenceAuditResult r, WardrobeAuditIssue issue)
        {
            foreach (var i in r.Issues) if (i == issue) return true;
            return false;
        }

        [Test]
        public void CleanCurrentSave_NoIssues()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            WardrobeStore.SetEquippedOutfitId("forest_cavalier");
            var r = WardrobeStateAuditor.AuditPersistence(8); // forest unlocked
            Assert.IsFalse(r.HasIssues, "expected no issues");
            Assert.IsTrue(r.IsSupportedSchema);
            Assert.IsFalse(r.RequiresMigration);
            Assert.IsFalse(r.RequiresNormalization);
            Assert.AreEqual("forest_cavalier", r.Snapshot.NormalizedEquippedOutfitId);
        }

        [Test]
        public void MissingSchemaKey_IsLegacyAndRequiresMigration()
        {
            var r = WardrobeStateAuditor.AuditPersistence(0);
            Assert.IsFalse(r.Snapshot.HasSchemaKey);
            Assert.IsTrue(HasIssue(r, WardrobeAuditIssue.MissingSchemaVersion));
            Assert.IsTrue(r.RequiresMigration);
            Assert.IsTrue(r.IsSupportedSchema);
        }

        [Test]
        public void PresentBehindSchema_IsLegacy()
        {
            PlayerPrefs.SetInt(WardrobePersistenceKeys.SchemaVersion, 0);
            var r = WardrobeStateAuditor.AuditPersistence(0);
            Assert.IsTrue(r.Snapshot.HasSchemaKey);
            Assert.IsTrue(HasIssue(r, WardrobeAuditIssue.LegacySchemaVersion));
            Assert.IsTrue(r.RequiresMigration);
        }

        // Required: missing equipped key = clean implicit Classic, NOT an issue.
        [Test]
        public void MissingEquippedKey_IsCleanNoIssue()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            // No equipped key set.
            var r = WardrobeStateAuditor.AuditPersistence(100);
            Assert.IsFalse(r.Snapshot.HasEquippedKey);
            Assert.IsNull(r.Snapshot.RawEquippedOutfitId);
            Assert.IsFalse(r.RequiresNormalization);
            Assert.IsFalse(HasIssue(r, WardrobeAuditIssue.EmptyEquippedId));
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                r.Snapshot.NormalizedEquippedOutfitId);
        }

        // Required: present-but-empty value = invalid + repairable under support.
        [Test]
        public void EmptyEquippedValue_IsRepairableIssue()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            SetRawEquipped("");
            var r = WardrobeStateAuditor.AuditPersistence(100);
            Assert.IsTrue(r.Snapshot.HasEquippedKey);
            Assert.IsTrue(HasIssue(r, WardrobeAuditIssue.EmptyEquippedId));
            Assert.IsTrue(r.RequiresNormalization);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                r.Snapshot.NormalizedEquippedOutfitId);
        }

        [Test]
        public void UnknownEquipped_FlagsIssue()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            SetRawEquipped("does_not_exist");
            var r = WardrobeStateAuditor.AuditPersistence(100);
            Assert.IsTrue(HasIssue(r, WardrobeAuditIssue.UnknownEquippedId));
            Assert.IsTrue(r.RequiresNormalization);
        }

        [Test]
        public void LockedEquipped_FlagsIssue()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            WardrobeStore.SetEquippedOutfitId("silver_dreamer"); // 30 Stars
            var r = WardrobeStateAuditor.AuditPersistence(0);    // locked
            Assert.IsTrue(HasIssue(r, WardrobeAuditIssue.LockedEquippedId));
            Assert.IsTrue(r.RequiresNormalization);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                r.Snapshot.NormalizedEquippedOutfitId);
        }

        // Required: auditor reads RAW values, not sanitized WardrobeStore output.
        [Test]
        public void ReadsRawEquippedValue_NotSanitized()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            SetRawEquipped("does_not_exist");
            var r = WardrobeStateAuditor.AuditPersistence(100);
            Assert.AreEqual("does_not_exist", r.Snapshot.RawEquippedOutfitId,
                "snapshot must expose the raw stored id");
            Assert.AreNotEqual(WardrobeStore.GetEquippedOutfitId(),
                r.Snapshot.RawEquippedOutfitId,
                "raw must differ from the sanitized store read");
        }

        // Required: future schema is non-repairing - no equipped issue flagged,
        // unsupported, no migration/normalization action.
        [Test]
        public void FutureVersion_IsUnsupportedAndNonRepairing()
        {
            PlayerPrefs.SetInt(WardrobePersistenceKeys.SchemaVersion, 99);
            WardrobeStore.SetEquippedOutfitId("silver_dreamer"); // would be locked
            var r = WardrobeStateAuditor.AuditPersistence(0);
            Assert.IsTrue(HasIssue(r, WardrobeAuditIssue.FutureSchemaVersion));
            Assert.IsFalse(HasIssue(r, WardrobeAuditIssue.LockedEquippedId),
                "future save must not flag a repairable equipped issue");
            Assert.IsFalse(r.IsSupportedSchema);
            Assert.IsFalse(r.RequiresMigration);
            Assert.IsFalse(r.RequiresNormalization);
            Assert.AreEqual(WardrobeCatalog.DefaultOutfitId,
                r.Snapshot.NormalizedEquippedOutfitId);
        }

        [Test]
        public void DoesNotMutatePlayerPrefs()
        {
            PlayerPrefs.SetInt(WardrobePersistenceKeys.SchemaVersion, 99);
            WardrobeStore.SetEquippedOutfitId("silver_dreamer");
            int schemaBefore = PlayerPrefs.GetInt(
                WardrobePersistenceKeys.SchemaVersion, -1);
            string equippedBefore = PlayerPrefs.GetString(
                WardrobePersistenceKeys.EquippedOutfit, "<none>");

            WardrobeStateAuditor.AuditPersistence(0);

            Assert.AreEqual(schemaBefore, PlayerPrefs.GetInt(
                WardrobePersistenceKeys.SchemaVersion, -1));
            Assert.AreEqual(equippedBefore, PlayerPrefs.GetString(
                WardrobePersistenceKeys.EquippedOutfit, "<none>"));
        }

        [Test]
        public void Snapshot_ReportsUnlockedAndStars()
        {
            WardrobePersistenceMigrator.StampCurrentVersion();
            var r = WardrobeStateAuditor.AuditPersistence(8);
            Assert.AreEqual(8, r.Snapshot.TotalStars);
            CollectionAssert.Contains(r.Snapshot.UnlockedOutfitIds,
                WardrobeCatalog.DefaultOutfitId);
            CollectionAssert.Contains(r.Snapshot.UnlockedOutfitIds, "forest_cavalier");
            CollectionAssert.DoesNotContain(r.Snapshot.UnlockedOutfitIds,
                "silver_dreamer");
        }
    }
}
