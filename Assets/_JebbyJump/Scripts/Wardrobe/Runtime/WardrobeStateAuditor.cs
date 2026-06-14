using System.Collections.Generic;
using UnityEngine;

namespace JebbyJump.Wardrobe
{
    // Issues a wardrobe save/registration audit can report. The save-state
    // subset (schema + equipped) is emitted by the read-only runtime
    // WardrobeStateAuditor; the asset-registration values are emitted by the
    // editor asset audit (the editor QA command) so the Runtime assembly never
    // depends on the Visual/asset layer.
    public enum WardrobeAuditIssue
    {
        None = 0,
        MissingSchemaVersion,
        LegacySchemaVersion,
        FutureSchemaVersion,
        EmptyEquippedId,
        UnknownEquippedId,
        LockedEquippedId,
        MissingVisualRegistration,
        MissingPreviewRegistration,
        MissingPreviewPose,
        DuplicatePreviewId,
        DuplicateVisualId,
    }

    // Read-only result of a persistence audit. Diagnostic only - describes what
    // a migration WOULD do; it performs no writes itself.
    public sealed class WardrobePersistenceAuditResult
    {
        public WardrobeStateSnapshot Snapshot { get; }
        public IReadOnlyList<WardrobeAuditIssue> Issues { get; }

        // schema <= CurrentVersion. A future (unsupported) schema is read-only:
        // it is intentionally non-repairing (RequiresMigration/Normalization
        // both false) so no caller mutates the save.
        public bool IsSupportedSchema { get; }
        public bool RequiresMigration { get; }     // schema behind -> would stamp
        public bool RequiresNormalization { get; } // equipped id would be repaired

        public WardrobePersistenceAuditResult(
            WardrobeStateSnapshot snapshot, IReadOnlyList<WardrobeAuditIssue> issues,
            bool isSupportedSchema, bool requiresMigration, bool requiresNormalization)
        {
            Snapshot = snapshot;
            Issues = issues;
            IsSupportedSchema = isSupportedSchema;
            RequiresMigration = requiresMigration;
            RequiresNormalization = requiresNormalization;
        }

        public bool HasIssues => Issues != null && Issues.Count > 0;
    }

    // Pure, READ-ONLY auditor for the persisted wardrobe save state. Reads RAW
    // PlayerPrefs values + key presence (never the sanitized WardrobeStore
    // output) so it can distinguish a missing equipped key (implicit Classic,
    // clean fresh state) from a present-but-empty value (invalid, repairable
    // under a supported schema). NEVER writes PlayerPrefs, Stars, or
    // acknowledgements; never raises events or analytics.
    public static class WardrobeStateAuditor
    {
        public static WardrobePersistenceAuditResult AuditPersistence(int totalStars)
        {
            bool hasSchemaKey = PlayerPrefs.HasKey(
                WardrobePersistenceKeys.SchemaVersion);
            int schema = PlayerPrefs.GetInt(
                WardrobePersistenceKeys.SchemaVersion, 0);

            bool hasEquippedKey = PlayerPrefs.HasKey(
                WardrobePersistenceKeys.EquippedOutfit);
            string rawEquipped = hasEquippedKey
                ? PlayerPrefs.GetString(WardrobePersistenceKeys.EquippedOutfit, "")
                : null;

            bool future = schema > WardrobePersistenceMigrator.CurrentVersion;
            bool supported = !future;
            bool requiresMigration =
                supported && schema < WardrobePersistenceMigrator.CurrentVersion;

            // Effective in-memory id (read-only): future -> Classic; supported ->
            // lock-normalized from the raw value (missing key -> Classic).
            string normalized = future
                ? WardrobeCatalog.DefaultOutfitId
                : WardrobeUnlockService.NormalizeEquippedId(
                    hasEquippedKey ? rawEquipped : WardrobeCatalog.DefaultOutfitId,
                    totalStars);

            var issues = new List<WardrobeAuditIssue>();

            // Schema issues.
            if (future) issues.Add(WardrobeAuditIssue.FutureSchemaVersion);
            else if (!hasSchemaKey) issues.Add(WardrobeAuditIssue.MissingSchemaVersion);
            else if (schema < WardrobePersistenceMigrator.CurrentVersion)
                issues.Add(WardrobeAuditIssue.LegacySchemaVersion);

            // Equipped-id issues - ONLY for supported schemas. A future save is
            // non-repairing, so its equipped state is intentionally not flagged.
            // A MISSING key is a clean implicit Classic (no issue).
            bool requiresNormalization = false;
            if (supported && hasEquippedKey)
            {
                if (string.IsNullOrWhiteSpace(rawEquipped))
                {
                    issues.Add(WardrobeAuditIssue.EmptyEquippedId);
                    requiresNormalization = true;
                }
                else if (!WardrobeCatalog.Exists(rawEquipped))
                {
                    issues.Add(WardrobeAuditIssue.UnknownEquippedId);
                    requiresNormalization = true;
                }
                else if (!WardrobeUnlockService.IsUnlocked(
                    WardrobeCatalog.GetById(rawEquipped), totalStars))
                {
                    issues.Add(WardrobeAuditIssue.LockedEquippedId);
                    requiresNormalization = true;
                }
            }

            var unlocked = new List<string>();
            var acknowledged = new List<string>();
            foreach (var o in WardrobeCatalog.Outfits)
            {
                if (WardrobeUnlockService.IsUnlocked(o, totalStars))
                    unlocked.Add(o.Id);
                if (WardrobeUnlockAcknowledgementStore.IsAcknowledged(o.Id))
                    acknowledged.Add(o.Id);
            }

            var snapshot = new WardrobeStateSnapshot(
                schema, hasSchemaKey, hasEquippedKey, rawEquipped, normalized,
                totalStars, unlocked, acknowledged);

            return new WardrobePersistenceAuditResult(
                snapshot, issues, supported, requiresMigration, requiresNormalization);
        }
    }
}
