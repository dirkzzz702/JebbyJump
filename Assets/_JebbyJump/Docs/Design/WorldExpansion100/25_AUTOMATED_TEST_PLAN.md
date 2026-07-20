# 25 — Automated Test Plan

Status: `PROPOSED`. Extends the existing suite (baseline ~317 PlayMode per memory; verify).
Update `LevelBalanceAssetTests.cs:29` (hardcoded `10`) to `100` / data-driven (doc 03 §6, P34E).

## Catalog integrity (EditMode)

- exactly 10 worlds in `WorldCatalog`; exactly 100 levels in `LevelCatalog`.
- ten levels per world; world ranges contiguous (`(n-1)*10+1 … n*10`).
- no duplicate level ids; ids ordered; **existing L1–10 stable ids/asset names preserved**.
- exactly 10 finales (world_level_number 10).

## Level constraints (EditMode, data-driven from CSV/assets)

- sequence length in bounds and ≤ 10; choices/row ≤ 6.
- valid `PlatformColor` values; rank thresholds ordered S<A<B.
- no unsupported mechanic; **no skill-required level** (`skill_required=false` all 100).
- cactus only on permitted (distractor) platforms; every level has a valid world/background config.
- failure = full-restart (behavioural, PlayMode).

## World art integrity (EditMode, via doc 24 validators)

- every world references required art; all six platform visuals present; semantic-colour valid;
  bg/floor/hazard present; no missing refs; no wrong-world refs; import settings valid;
  **no collider change from visual swaps**.

## Runtime world switching (PlayMode)

- load World 1 then World 2 with **no stale art**; load all ten sequentially; missing-art falls back
  to World 1 safely; timer/sequence unaffected; Memory Cues correct; skills optional; cactus
  behaviour unchanged across worlds.

## Progression (PlayMode)

- existing L1–10 saves valid after expansion (unlock/stars/best-time); no mass-unlock; unlock
  crosses world boundaries at finale; Continue selects correct level+world; finale unlocks next
  world; World-10 ending idempotent; story cards ack once; world rewards don't duplicate; **Stars
  not consumed**; unknown/oob index handled.

## UI (PlayMode/EditMode)

- 10 world cards; 10 level cards per selected world; touch targets ≥ min; safe areas; deterministic
  focus/nav; scrolling; locked/finale states; localisation expansion tolerant; accessibility cues.

## Data round-trip (EditMode)

- generated `LevelConfig` assets match `13_100_LEVEL_MASTER_TABLE.csv` (no drift); generator
  idempotent (re-run zero diffs); World-1 assets untouched by generator.

## Notes

- Read PlayMode verdict from `TestResults.xml` (bridge job tracking dies on domain reload).
- Reuse `WardrobePersistenceMigrationTests` / `WardrobeSaveCompatibilityMatrixTests` shapes for the
  save-compat matrix.
