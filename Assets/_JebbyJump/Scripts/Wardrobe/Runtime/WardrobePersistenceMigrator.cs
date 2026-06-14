using UnityEngine;

namespace JebbyJump.Wardrobe
{
    // Outcome of a MigrateIfNeeded call. Lets callers (and tests) distinguish a
    // legacy schema upgrade, an in-place equipped-id repair, a no-op, and an
    // unsupported future save without re-reading PlayerPrefs.
    public enum WardrobeMigrationStatus
    {
        NoChange = 0,                 // supported schema, nothing to write
        MigratedLegacy = 1,           // stamped schema forward (equipped unchanged)
        NormalizedCurrent = 2,        // current schema, repaired the equipped id
        MigratedAndNormalized = 3,    // stamped forward AND repaired the equipped id
        FutureVersionUnsupported = 4, // stored schema ahead of this build: READ-ONLY
    }

    public readonly struct WardrobeMigrationResult
    {
        public readonly int PreviousVersion;
        public readonly int CurrentVersion;
        public readonly bool EquippedOutfitChanged;
        public readonly string PreviousEquippedId;
        public readonly string CurrentEquippedId;
        public readonly WardrobeMigrationStatus Status;
        public readonly bool DidWrite;

        public WardrobeMigrationResult(
            int previousVersion, int currentVersion, bool equippedChanged,
            string previousEquippedId, string currentEquippedId,
            WardrobeMigrationStatus status, bool didWrite)
        {
            PreviousVersion = previousVersion;
            CurrentVersion = currentVersion;
            EquippedOutfitChanged = equippedChanged;
            PreviousEquippedId = previousEquippedId;
            CurrentEquippedId = currentEquippedId;
            Status = status;
            DidWrite = didWrite;
        }

        public bool IsFutureVersionUnsupported =>
            Status == WardrobeMigrationStatus.FutureVersionUnsupported;
    }

    // Local wardrobe save hardening. TWO independent jobs (for SUPPORTED saves):
    //
    //   1. ONGOING equipped-outfit normalization - runs on EVERY call,
    //      regardless of schema version. An unknown/empty/now-locked equipped
    //      id always recovers to the default (e.g. after a Stars change leaves
    //      a previously-unlocked outfit locked). This must NOT be gated by the
    //      schema version. A MISSING equipped key reads as the default, so it is
    //      already clean (implicit Classic) and is never rewritten.
    //
    //   2. ONE-TIME schema migration - stamps the schema version once when the
    //      stored version is behind; future per-version data transforms slot in
    //      here. Written LAST (equipped first) so an interrupted run re-runs
    //      safely on the next startup.
    //
    // FUTURE saves (storedVersion > CurrentVersion, e.g. a downgrade after a
    // later build) are treated as READ-ONLY: no normalization, no schema write,
    // no downgrade, no Stars/acknowledgement change, no appearance event, no
    // analytics. The on-disk save is left exactly as found; callers display a
    // safe in-memory Classic via GetEffectiveOutfitId.
    //
    // Idempotent, local-only, deterministic. A supported-path correction writes
    // the equipped id via WardrobeStore.SetEquippedOutfitId (the low-level
    // primitive), so it raises NO appearance event and NO analytics (the Runtime
    // assembly references neither). Never touches Stars, thresholds, or unlock
    // acknowledgements.
    public static class WardrobePersistenceMigrator
    {
        public const int CurrentVersion = 1;

        public static WardrobeMigrationResult MigrateIfNeeded(int totalStars)
        {
            int previousVersion = PlayerPrefs.GetInt(
                WardrobePersistenceKeys.SchemaVersion, 0);

            // FUTURE schema: READ-ONLY. Leave every wardrobe PlayerPrefs value
            // untouched - no normalization, no stamp, no downgrade. The equipped
            // id reported here is the raw stored value (unchanged); the safe
            // in-memory fallback for display/apply is GetEffectiveOutfitId.
            if (previousVersion > CurrentVersion)
            {
                string futureRaw = PlayerPrefs.GetString(
                    WardrobePersistenceKeys.EquippedOutfit,
                    WardrobeCatalog.DefaultOutfitId);
                return new WardrobeMigrationResult(
                    previousVersion, previousVersion, false, futureRaw, futureRaw,
                    WardrobeMigrationStatus.FutureVersionUnsupported,
                    didWrite: false);
            }

            // (1) Ongoing normalization - ALWAYS for supported schema, version-
            // independent. Read the raw stored value (not GetEquippedOutfitId,
            // which already hides unknowns) so a garbage/empty/locked id is
            // actually rewritten. A MISSING key falls back to the default, so
            // normalized == raw -> no write (implicit Classic, clean state).
            string rawEquipped = PlayerPrefs.GetString(
                WardrobePersistenceKeys.EquippedOutfit,
                WardrobeCatalog.DefaultOutfitId);
            string normalized = WardrobeUnlockService.NormalizeEquippedId(
                rawEquipped, totalStars);
            bool equippedChanged = normalized != rawEquipped;
            if (equippedChanged)
                WardrobeStore.SetEquippedOutfitId(normalized); // sanitized write + Save; no event

            // (2) One-time schema stamp - only when behind (never downgrade a
            // future version). Written LAST so an interruption between the
            // equipped write and here simply re-runs migration next startup.
            bool stamped = previousVersion < CurrentVersion;
            if (stamped)
            {
                PlayerPrefs.SetInt(
                    WardrobePersistenceKeys.SchemaVersion, CurrentVersion);
                PlayerPrefs.Save();
            }

            var status = stamped
                ? (equippedChanged
                    ? WardrobeMigrationStatus.MigratedAndNormalized
                    : WardrobeMigrationStatus.MigratedLegacy)
                : (equippedChanged
                    ? WardrobeMigrationStatus.NormalizedCurrent
                    : WardrobeMigrationStatus.NoChange);
            return new WardrobeMigrationResult(
                previousVersion, CurrentVersion, equippedChanged,
                rawEquipped, normalized, status,
                didWrite: equippedChanged || stamped);
        }

        // Read-only "effective" equipped outfit for DISPLAY / APPLY. NEVER
        // writes. FUTURE schema -> Classic in memory (the on-disk save is left
        // untouched). Supported schema -> the sanitized stored id (already
        // lock-normalized at menu init). Stars-free, so the gameplay player can
        // use it with no Stars dependency.
        public static string GetEffectiveOutfitId()
        {
            if (IsFutureSchema()) return WardrobeCatalog.DefaultOutfitId;
            return WardrobeStore.GetEquippedOutfitId();
        }

        // Stars-aware effective read for UI that knows the Star total: FUTURE ->
        // Classic; supported -> lock-normalized to the default if the stored
        // outfit is now locked/unknown. NEVER writes.
        public static string GetEffectiveOutfitId(int totalStars)
        {
            if (IsFutureSchema()) return WardrobeCatalog.DefaultOutfitId;
            return WardrobeUnlockService.NormalizeEquippedId(
                WardrobeStore.GetEquippedOutfitId(), totalStars);
        }

        // True when the stored schema is ahead of this build (read-only save).
        public static bool IsFutureSchema()
            => PlayerPrefs.GetInt(WardrobePersistenceKeys.SchemaVersion, 0)
               > CurrentVersion;

        // Sets the schema version to current WITHOUT running migrations - used
        // after a wardrobe reset so a clean state does not replay history.
        public static void StampCurrentVersion()
        {
            PlayerPrefs.SetInt(
                WardrobePersistenceKeys.SchemaVersion, CurrentVersion);
            PlayerPrefs.Save();
        }
    }
}
