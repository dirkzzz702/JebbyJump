# 16 — LevelConfig Proposed Values

Status: mapping from the design table (doc 13 CSV) to `LevelConfig` serialized fields (doc 01 §2).
World 1 = `EXISTING-PRESERVED` (do not overwrite). Worlds 2–10 = `PROPOSED` / `PLAYTEST-HYPOTHESIS`.

## CSV column → LevelConfig field map

| CSV column | LevelConfig field | Notes |
|---|---|---|
| `sequence_length` | `_sequenceLength` | ≤ 10 (validated) |
| `memory_display_duration` | `_memoryTimeSeconds` | — |
| (fixed 0.35) | `_memoryPhaseJumpMultiplier` | keep current default unless tuned |
| `lives` | `_startingLives` | 3 |
| `unique_colours_used` | `_availableColors` | first N of the six locked colours (enum order) |
| `cactus_hazard_count`→chance | `_cactusSpawnChance` | see conversion below |
| `maximum_choices_per_row` | `_platformsPerRow` | ≤ 6 (validated) |
| `platform_width_profile` | `_platformWidth` | — |
| (derived) | `_rowVerticalSpacing` | keep world-appropriate (~3.3–3.9) |
| (fixed 3) | `_rowStartY` | current default |
| `horizontal_gap_profile` | `_rowHorizontalSpread` | — |
| `same_row_y_stagger` | `_rowVerticalJitter` | 0–0.35 |
| (per-level asset) | `_rankConfig` | `Level{NN}_TimeRankConfig` (S/A/B from doc 14) |

## Cactus count → chance conversion

`LevelConfig` stores `_cactusSpawnChance` (0..1), not a count. The generator converts the design
`cactus_hazard_count` intent into a spawn chance per row such that expected hazards ≈ target,
staying **distractor-only** (spawner invariant). World-1 chances are preserved as shipped
(0, 0.15, 0.2, 0.25, 0.3, 0.4).

## Rank config assets

New `Settings/Level/TimeRanks/Level{11..100}_TimeRankConfig.asset` (author S/A/B per doc 14).
Existing `Level01..10` rank configs preserved.

## Generator guarantees (doc 23)

- Idempotent; dry-run; refuses duplicate ids; preserves World-1 hand values; validates
  seq≤10 / choices≤6 / rank order; emits a report; never silently overwrites tuned values.
