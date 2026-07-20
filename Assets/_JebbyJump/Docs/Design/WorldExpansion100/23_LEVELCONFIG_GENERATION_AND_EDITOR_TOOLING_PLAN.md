# 23 — LevelConfig Generation & Editor Tooling Plan

Status: `PROPOSED`. Do **not** implement in this planning task (built in P34E).

## Authoritative source

`13_100_LEVEL_MASTER_TABLE.csv` (CSV chosen — reviewable, diff-friendly, version-controlled).
The generator is the only writer of Levels 11–100 asset values.

## Editor generator (menu e.g. `Jebby Jump/Progression/Generate Levels From Table`)

Behaviour:
- **Consumes** the CSV; maps columns → `LevelConfig` fields (doc 16).
- **Creates/updates** `Settings/Level/Level{011..100}Config.asset` deterministically.
- **Idempotent:** re-run with unchanged CSV = zero asset diffs.
- **Preserves stable ids:** never renumbers/renames existing assets; World-1 (`Level1..10Config`)
  is **read-only** to the generator (skip / assert unchanged).
- **Refuses duplicate** global level ids; validates world assignment (contiguous), sequence ≤ 10,
  choices ≤ 6, rank ordering S<A<B, cactus distractor-only intent.
- **Never silently overwrites** hand-tuned values: if an existing asset's values differ from the
  CSV for a non-World-1 level, require an explicit `--force`/confirm and log the diff.
- **Dry-run** mode: report intended changes without writing.
- **Report:** emits a summary (created/updated/skipped, validation results) to `Builds/` or console.
- Also creates/updates `Level{11..100}_TimeRankConfig` assets from S/A/B (doc 14) and syncs the
  `LevelCatalog` + `LevelSessionController._levels` ordering (reuse the existing
  `Create Or Sync Level Catalog` menu path).

## Guardrails

- No writes to World-1 assets. No writes to ProjectSettings/packages. No PlayerPrefs.
- Prove generated assets match the source table (round-trip test, doc 25).
- Runs only in the editor; deterministic (fixed field order → stable YAML → no churn).
