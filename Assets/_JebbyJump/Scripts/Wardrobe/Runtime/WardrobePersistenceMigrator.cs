using UnityEngine;

namespace JebbyJump.Wardrobe
{
    public readonly struct WardrobeMigrationResult
    {
        public readonly int PreviousVersion;
        public readonly int CurrentVersion;
        public readonly bool EquippedOutfitChanged;
        public readonly string PreviousEquippedId;
        public readonly string CurrentEquippedId;

        public WardrobeMigrationResult(
            int previousVersion, int currentVersion, bool equippedChanged,
            string previousEquippedId, string currentEquippedId)
        {
            PreviousVersion = previousVersion;
            CurrentVersion = currentVersion;
            EquippedOutfitChanged = equippedChanged;
            PreviousEquippedId = previousEquippedId;
            CurrentEquippedId = currentEquippedId;
        }
    }

    // Local wardrobe save hardening. TWO independent jobs:
    //
    //   1. ONGOING equipped-outfit normalization - runs on EVERY call,
    //      regardless of schema version. An unknown/empty/now-locked equipped
    //      id always recovers to the default (e.g. after a Stars change leaves
    //      a previously-unlocked outfit locked). This must NOT be gated by the
    //      schema version.
    //
    //   2. ONE-TIME schema migration - stamps the schema version once when the
    //      stored version is behind; future per-version data transforms slot
    //      in here. A future (greater) stored version is never downgraded.
    //
    // Idempotent, local-only, deterministic. Writes the equipped id via
    // WardrobeStore.SetEquippedOutfitId (the low-level primitive), so a
    // correction raises NO appearance event and NO analytics. Never touches
    // Stars, thresholds, or unlock acknowledgements.
    public static class WardrobePersistenceMigrator
    {
        public const int CurrentVersion = 1;

        public static WardrobeMigrationResult MigrateIfNeeded(int totalStars)
        {
            int previousVersion = PlayerPrefs.GetInt(
                WardrobePersistenceKeys.SchemaVersion, 0);

            // (1) Ongoing normalization - ALWAYS, version-independent. Read the
            // raw stored value (not GetEquippedOutfitId, which already hides
            // unknowns) so a garbage/locked id is actually rewritten.
            string rawEquipped = PlayerPrefs.GetString(
                WardrobePersistenceKeys.EquippedOutfit,
                WardrobeCatalog.DefaultOutfitId);
            string normalized = WardrobeUnlockService.NormalizeEquippedId(
                rawEquipped, totalStars);
            bool equippedChanged = normalized != rawEquipped;
            if (equippedChanged)
                WardrobeStore.SetEquippedOutfitId(normalized); // sanitized write + Save; no event

            // (2) One-time schema stamp - only when behind (never downgrade a
            // future version). Per-version one-time transforms would go here.
            if (previousVersion < CurrentVersion)
            {
                PlayerPrefs.SetInt(
                    WardrobePersistenceKeys.SchemaVersion, CurrentVersion);
                PlayerPrefs.Save();
            }

            int currentVersion = previousVersion < CurrentVersion
                ? CurrentVersion : previousVersion;
            return new WardrobeMigrationResult(
                previousVersion, currentVersion, equippedChanged,
                rawEquipped, normalized);
        }

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
