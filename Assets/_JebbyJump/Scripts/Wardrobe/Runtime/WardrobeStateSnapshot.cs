using System.Collections.Generic;

namespace JebbyJump.Wardrobe
{
    // Read-only point-in-time view of the persisted wardrobe save state, built
    // from RAW PlayerPrefs (not the sanitized WardrobeStore output) so callers
    // can see exactly what is on disk - including a missing equipped key vs a
    // present-but-empty value. Pure data: no Unity asset references, no writes.
    // Produced by WardrobeStateAuditor.AuditPersistence.
    public readonly struct WardrobeStateSnapshot
    {
        public readonly int SchemaVersion;        // raw stored value (0 if absent)
        public readonly bool HasSchemaKey;        // distinguishes "absent" from 0
        public readonly bool HasEquippedKey;      // distinguishes missing from empty
        public readonly string RawEquippedOutfitId;      // raw value; null if absent
        public readonly string NormalizedEquippedOutfitId; // safe in-memory effective id
        public readonly int TotalStars;
        public readonly IReadOnlyList<string> UnlockedOutfitIds;
        public readonly IReadOnlyList<string> AcknowledgedOutfitIds;

        public WardrobeStateSnapshot(
            int schemaVersion, bool hasSchemaKey, bool hasEquippedKey,
            string rawEquippedOutfitId, string normalizedEquippedOutfitId,
            int totalStars, IReadOnlyList<string> unlockedOutfitIds,
            IReadOnlyList<string> acknowledgedOutfitIds)
        {
            SchemaVersion = schemaVersion;
            HasSchemaKey = hasSchemaKey;
            HasEquippedKey = hasEquippedKey;
            RawEquippedOutfitId = rawEquippedOutfitId;
            NormalizedEquippedOutfitId = normalizedEquippedOutfitId;
            TotalStars = totalStars;
            UnlockedOutfitIds = unlockedOutfitIds;
            AcknowledgedOutfitIds = acknowledgedOutfitIds;
        }
    }
}
