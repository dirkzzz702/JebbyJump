# Jebby Jump — Wardrobe Save Compatibility Matrix v0.1 (P19)

Release-hardening reference for the wardrobe save/migration subsystem. P19 adds
NO new player-facing feature: it proves persistence, migration, normalization,
acknowledgement, equip, and registration stay safe across legacy/current/future
saves, Stars resets, and repeated initialization.

## Scope

- Automated wardrobe save/migration/audit hardening only.
- No gameplay, economy, threshold, balance, or art changes.
- **No schema bump** (still v1 — P19 adds no new persisted data shape).
- Manual rendered UI / art / gameplay / on-device migration QA remains
  **DEFERRED / NOT VERIFIED**. Prototype outfit art is **not** final-certified.

## Persistence key registry

All centralized in `WardrobePersistenceKeys` (stable wire keys — never rename):

| Key | Type | Owner | Meaning |
|---|---|---|---|
| `jebby.wardrobe.equippedOutfit` | string | WardrobeStore | equipped outfit id (only) |
| `jebby.wardrobe.schemaVersion` | int | WardrobePersistenceMigrator | save schema version |
| `jebby.wardrobe.unlockAcknowledged.<id>` | int 0/1 | WardrobeUnlockAcknowledgementStore | ceremony seen (NOT ownership) |

Related, non-wardrobe: `jebby.rewards.levelStars.<i>` (Stars), `JebbyJump.BestTime.<key>` (best times).

## Schema policy

- `CurrentVersion = 1`. Absent key reads as `0` (legacy).
- Legacy (`< current`) → migrate forward (stamp current), then normalize.
- Current (`== current`) → still revalidate/normalize the equipped id every init.
- **Future (`> current`) → READ-ONLY**: never downgrade, normalize, or write;
  return `FutureVersionUnsupported`; display falls back to Classic in memory.

## Normalization rules (equipped id)

Read-only resolver `WardrobePersistenceMigrator.GetEffectiveOutfitId([stars])`:

- **Missing key** → implicit Classic (clean fresh state; never materialized).
- **Present empty / whitespace** → invalid → Classic (repairable under support).
- **Unknown id** → Classic.
- **Known but locked** (Stars below threshold) → Classic.
- **Known + unlocked** → preserved.
- **Future schema** → Classic in memory; the on-disk save is left untouched.

Migration writes the repaired equipped id via the low-level `WardrobeStore`
(no appearance event, no analytics). Stars/acknowledgements/thresholds untouched.

## Supported migration paths

1. Legacy clean (valid unlocked equipped) → stamp current. `MigratedLegacy`.
2. Legacy + invalid equipped → repair + stamp. `MigratedAndNormalized`.
3. Current + invalid equipped → repair only. `NormalizedCurrent`.
4. Current clean → no-op. `NoChange`.
5. Future → no writes. `FutureVersionUnsupported`.

Write order: equipped id first, **schema stamp last**, then a single
`PlayerPrefs.Save`. An interruption before the stamp re-runs safely next startup
(idempotent recovery).

## Future-version policy (non-destructive)

- No persistence writes; no normalization writes; no schema downgrade.
- No acknowledgement or Star changes; no appearance event; no analytics.
- Safe in-memory Classic fallback via `GetEffectiveOutfitId` (player + panel).
- MainMenu logs a warning in editor/development builds only.
- Audit marks it `IsSupportedSchema=false`, `RequiresMigration=false`,
  `RequiresNormalization=false` (clearly non-repairing).

## Reset matrix

| Reset | Equipped | Acks | Schema | Stars |
|---|---|---|---|---|
| Reset Wardrobe | → Classic | cleared | stamped current | unchanged |
| Reset Stars | unchanged (next init normalizes if now locked) | preserved | preserved | cleared |
| Reset Everything | → Classic | cleared | current | cleared |
| Reset Best Times | unchanged | unchanged | unchanged | unchanged |
| Reset Local Progress | unchanged | unchanged | unchanged | unchanged |

## Acknowledgement rules

- Per-outfit key; default is implicitly acknowledged (no key).
- Migration/normalization never marks or clears acknowledgements.
- Only `WardrobeCatalog` ids are read/reset (no wildcard key enumeration).
- Unknown legacy acknowledgement keys are ignored.

## Equip / event / analytics invariants

- All user equips route through `WardrobeEquipService` (`WardrobeStore` internals,
  migration, reset, and test setup are the only other writers).
- Success → one appearance event + one `cosmetic_equipped` (`source=wardrobe` or
  `source=unlock_ceremony`). AlreadyEquipped/Locked/Unknown → none.
- Migration/audit/normalization → zero appearance events, zero analytics
  (guaranteed: `JebbyJump.Wardrobe.Runtime` references no analytics assembly).
- Preview pose changes → zero analytics.

## Asset-registration invariants

- 7/7 non-default outfits have visual override entries; default has none.
- Each override is an `AnimatorOverrideController` based on `JebbyAnimator` with
  all 7 clips overridden.
- 8/8 outfits have unique preview entries with all 7 poses present.
- No duplicate ids in either library.

## Automated test matrix (every case → a test)

| Area | Test(s) |
|---|---|
| Audit clean / legacy / unknown / locked / empty / missing-key / raw-read / future non-repairing / no-mutate | `WardrobeStateAuditorTests` |
| Legacy + current equipped normalization | `WardrobeSaveCompatibilityMatrixTests.LegacyMatrix_*`, `CurrentMatrix_*` |
| Threshold below/exact/above | `ThresholdBoundaryMatrix` |
| Acknowledgement preservation (incl. Stars-reset lock) | `AcknowledgementPreservationMatrix`, `DoesNotChangeAcknowledgements` |
| Future version: no writes | `FutureVersionMatrix_NoWrites`, `FutureVersion_NoWritesNoNormalize` |
| Idempotent / repeated | `RepeatedMigration_SecondCallIsNoChange`, `Idempotent_SecondCallNoChange` |
| Status + DidWrite | `Status_LegacyClean_*`, `Status_LegacyNormalize_*`, `Status_CurrentNormalize_*` |
| Current-schema still normalizes after Stars reset | `CurrentSchema_StillNormalizesLockedAfterStarReset`, `AfterResetStars_NextInitNormalizes*` |
| Schema-stamped-last recovery | `SchemaLastRecovery_RemigratesCleanly` |
| Missing vs empty equipped | `MissingEquippedKey_StaysCleanImplicitDefault`, auditor `MissingEquippedKey_IsClean*` / `EmptyEquippedValue_IsRepairableIssue` |
| Effective read (future→Classic, supported→stored/normalized) | `EffectiveOutfitId_*` |
| Migration emits no appearance event | `DoesNotPublishAppearanceEvent` |
| Migration emits no analytics | asmdef isolation (no analytics reference) |
| Migration does not change Stars | `DoesNotChangeStars` |
| Reset boundaries | `WardrobeResetBoundaryTests` |
| Asset registration / duplicate ids | `OutfitVisualAssetIntegrityTests`, `WardrobePreviewAssetIntegrityTests` |
| Key literals pinned | `Keys_PinExpectedLiterals` |

## Manual / deferred checks

- On-device upgrade/downgrade migration — **DEFERRED / NOT VERIFIED**.
- Rendered wardrobe UI / outfit visuals / ceremony / carousel — **DEFERRED**.
- All prior P4B–P18 manual deferrals preserved.

## Release decision

**Automated wardrobe release-hardening complete** when all automated checks are
green. NOT a claim of production-ready, art-final, balanced thresholds, or
device-certified migration.
