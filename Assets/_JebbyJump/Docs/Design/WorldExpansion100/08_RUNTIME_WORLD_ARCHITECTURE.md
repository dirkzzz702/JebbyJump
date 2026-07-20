# 08 — Runtime World Architecture

Status: `PROPOSED`. Grounded in the audit (doc 01): one reusable `Game` scene, no per-level art
today, `LevelSessionController._levels` is authoritative order.

## Objective

Add a **data-driven world-theming layer** so ten visually unique worlds run in the single
`Game` scene, selected by data — never by scene duplication.

## Component set (names mirror existing conventions: `LevelConfig`/`LevelCatalog`)

```
WorldDefinition  (ScriptableObject)   — one per world; identity + visual set + reward refs
WorldCatalog     (ScriptableObject)   — ordered list of WorldDefinitions (mirrors LevelCatalog)
WorldVisualSet   (serializable/SO)    — bg, floor, 6 platform visuals, hazard visual, deco, VFX
WorldThemeApplier (MonoBehaviour)     — applies a world's visuals BEFORE level generation
WorldResolver    (static/helper)      — global level index → WorldDefinition (contiguous map)
WorldHazardVisualDefinition           — hazard art/prefab ref (behaviour stays CactusObstacle)
```

Do **not** add abstractions beyond these. No per-world scripts, no per-world scenes.

## Application timing (critical — avoids the "one-frame stale art" bug)

Order inside `Game.unity` bootstrap (extends the current `LevelSessionController.Awake` path):

```
1. Resolve CurrentLevelIndex (existing PendingLevelSelection path — unchanged).
2. WorldResolver.ForLevel(index) → WorldDefinition.
3. WorldThemeApplier.Apply(worldDef)   ← BEFORE PlatformSpawner runs and before first render:
     - set Background sprite (reuse WireGameBackground camera-lock approach)
     - set Floor/FloorVisual sprite
     - set the 6 platform visuals (material/sprite swap keyed to locked PlatformColor)
     - set hazard visual definition used by cactus spawns
     - set decoration/VFX
4. Memory sequence + PlatformSpawner run as today (rowIndex rule unchanged).
```

Rules:
- Default world = World 1 when data missing.
- Missing-art fallback = World 1 asset (never blank/magenta); log a warning; editor validator fails
  the build if any world art ref is null (doc 24).
- Applier must fully replace the previous world's visuals (no residual sprites) when a session
  starts — verified by the "no stale art across world switch" PlayMode test (doc 25).

## Platform visuals — preserve gameplay invariants

`PROPOSED`: shared platform prefab + **material/sprite swap keyed to locked `PlatformColor`**.
Never change collider, width (`PlatformWidth`), one-way behaviour, `rowIndex`, or Memory Cues.
`PlatformSpawner.SpawnPlatforms` stays as-is; only the *visual* applied to each colour changes per
world. Artwork must never alter collider or gameplay width (enforced by test, doc 25 R5).

## Hazard visuals

`CactusObstacle` behaviour is unchanged and shared. `WorldHazardVisualDefinition` supplies the
sprite/prefab. A distinct **prefab per world sharing the one behaviour component** is approved
(Gate #7). `PlatformSpawner.TrySpawnCactus` still targets **distractors only** — invariant kept.

## Backgrounds

Reuse the existing camera-locked single-background approach (`WireGameBackground.cs`, added
`a2daa75`): parent under Main Camera, scale to over-fill 16:9→20:9. **Parallax is OUT of launch
scope** (Gate #6) — optional later only if the perf baseline (doc 28) allows.

## What does NOT change

- Scene flow (`Boot→MainMenu→Game`), `SceneNames`, `SceneLoader`.
- Core loop, timer, rank, Stars, lives, restart.
- `LevelConfig` gameplay fields (world identity is derived from index, not stored on the level —
  see doc 09 for why, and the alternative).
