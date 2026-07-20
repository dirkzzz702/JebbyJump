# 01 — Source of Truth & Repository Audit

All claims below are `REPO-VERIFIED` by direct inspection at the baseline commit unless
labelled otherwise. Exact paths / classes / members / assets are cited so a future session
does not re-derive them.

## 0. Baseline

| Item | Value | Source |
|---|---|---|
| Branch | `main` | `git branch --show-current` |
| HEAD | `a2daa75e4801c0347ed0044b895cdf91f52cc7f2` | `git rev-parse HEAD` |
| HEAD subject | `Warm kid-friendly UI palette + equal button labels` | `git log -1` |
| Working tree | clean except `.claude/settings.json` (pre-existing, unrelated) | `git status --short` |
| Unity | `6000.4.7f1` | UnitySkills `/health` |
| Bridge | UnitySkills v2.1.3 @ `localhost:8090`, `isCompiling:false` | `/health`, `/compile/status` |

## 1. Scene architecture — `REPO-VERIFIED`

- Scenes: `Assets/_JebbyJump/Scenes/{Boot,MainMenu,Game}.unity`.
- Flow: `BootController.Start()` → `SceneLoader.LoadMainMenu()`; `SceneLoader.LoadGame()` /
  `LoadMainMenu()` call `SceneManager.LoadScene(SceneNames.*)`.
- `JebbyJump.Core.SceneNames`: `Boot`, `MainMenu`, `Game` (string consts).
- **One reusable gameplay scene already exists.** The expansion does *not* require new
  gameplay scenes — it requires data-driven theming applied inside `Game.unity`.
- Files: `Scripts/Flow/BootController.cs`, `Scripts/Flow/SceneLoader.cs`, `Scripts/Core/SceneNames.cs`.

## 2. Level data model — `REPO-VERIFIED`

`Scripts/Platforms/LevelConfig.cs` (`JebbyJump.Level.LevelConfig : ScriptableObject`,
`menuName "JebbyJump/LevelConfig"`). Serialized fields (all the level currently carries):

| Field | Type | Default |
|---|---|---|
| `_sequenceLength` | int | 4 |
| `_memoryTimeSeconds` | float | 5 |
| `_memoryPhaseJumpMultiplier` | float | 0.35 |
| `_startingLives` | int | 3 |
| `_availableColors` | `PlatformColor[]` | Red,Blue,Yellow,Green |
| `_cactusSpawnChance` | float (0..1) | 0 |
| `_platformsPerRow` | int | 2 |
| `_rowVerticalSpacing` | float | 3.5 |
| `_rowStartY` | float | 2 |
| `_platformWidth` | float | 4 |
| `_platformHeight` | float | 0.5 |
| `_rowHorizontalSpread` | float | 8 |
| `_rowVerticalJitter` | float | 0 |
| `_rankConfig` | `TimeRankConfig` | per-level asset |

**Critical:** `LevelConfig` has **no** world / theme / background / floor / platform-art /
hazard-visual / level-id / world-id fields. `OnValidate()` clamps ranges.
→ *Theming and world identity are greenfield; nothing in the level asset selects art today.*

## 3. Current levels — `REPO-VERIFIED`

- 10 configs: `Assets/_JebbyJump/Settings/Level/Level{1..10}Config.asset`.
- 10 rank configs: `Settings/Level/TimeRanks/Level{01..10}_TimeRankConfig.asset` +
  `Settings/Level/DefaultTimeRankConfig.asset`.
- Menu-side catalog asset: `Settings/Level/LevelCatalog.asset`.
- Example (`Level1Config`): seq 3, memory 5s, jumpMult 0.35, lives 3, colors {R,G,B} (`010000000200000003000000` = Red,Green,Blue by `PlatformColor` order), cactus 0, perRow 2, vSpacing 3.3, startY 3, width 4, height 0.5, spread 7.5, jitter 0. → rows at y = 3, 6.3, 9.6 (3 rows, seq length 3).

## 4. Level ordering & session — `REPO-VERIFIED`

- `Scripts/Platforms/LevelSessionController.cs` holds `[SerializeField] LevelConfig[] _levels`
  in `Game.unity` = **authoritative order**. `CurrentLevelIndex` is the 0-based position.
- `PendingLevelSelection` (`Scripts/Progression/Runtime/`) passes the chosen index from menu → Game;
  consumed (`Reset()`) in `Awake`. Static `Index` / `Source` (`"continue"`/`"level_select"`/`"default"`).
- `AdvanceToNextLevel()` is linear (`CurrentLevelIndex++`), `IsFinalLevel` at last index.
- `LevelCatalog` (`Scripts/Progression/LevelCatalog.cs`) is the **menu-side mirror** of
  `_levels`; kept in sync via editor menu `Jebby Jump/Progression/Create Or Sync Level Catalog`.
  `GetLevelKey(i)` returns the `LevelConfig` **asset name** (e.g. `"Level1Config"`).

## 5. Save / progression keys — `REPO-VERIFIED` (the migration-critical section)

| Concern | Store | PlayerPrefs key | Keyed by |
|---|---|---|---|
| Unlock | `LevelProgressStore` | `jebby.level.highestUnlocked` (single int, default 0) | **index** (monotonic) |
| Best time | `BestTimeStore` | `JebbyJump.BestTime.<levelAssetName>` (float) | **asset name** |
| Stars | `StarRewardStore` | `jebby.rewards.levelStars.<levelIndex>` (int 0..3) | **index** |
| Rank | *not persisted* | — | recomputed from best time + `RankConfig` at display |

Files: `Scripts/Progression/Runtime/LevelProgressStore.cs`, `Scripts/Level/BestTimeStore.cs`,
`Scripts/Rewards/Runtime/StarRewardStore.cs`.

**Migration implication (`REPO-VERIFIED` → `PROPOSED` mapping):** because unlock and stars are
**index-keyed** and best-time is **asset-name-keyed**, existing local progress is preserved
**iff** the current 10 levels remain at array indices **0–9** with **unchanged asset names**.
Appending Levels 11–100 as indices 10–99 (new asset names) strands nothing. Any *reordering*
or *renaming* of Levels 1–10 would silently break unlock/stars (index) and/or best-time (name).
→ Recommend: **Levels 1–10 become World 1, unchanged IDs/indices/asset names.**
`StarRewardStore` comment already anticipates "a future 50 levels" — no aggregate key exists,
so totals are drift-free at 100.

## 6. Level Select UX — `REPO-VERIFIED` + risk

- `Scripts/UI/LevelSelectController.cs` builds a **single flat grid** of *all* catalog entries
  (`Rebuild()` loops `0.._catalog.Count`, one `LevelSelectCard` each, columns =
  `ShellLayoutMetrics.LevelSelectColumns`, one `ScrollRect`).
- `LevelSelectCard` bound with `(index, state, bestText, rankText, stars)`;
  `LevelCardClassifier.Classify(unlocked, hasBest)`.
- Continue target = `LevelProgressStore.GetContinueIndex(count)` (= highest unlocked, clamped).
- **RISK (`REPO-VERIFIED`):** at 100 levels this is exactly the "uncontrolled flat list" the
  expansion forbids. A world-tab / world-page restructure is required (roadmap phase **P34D**).
  Grid navigation (`ShellFocusUtil.BuildGridNavigation`) and scroll-to-focus already exist and
  can be reused per-world.

## 7. Gameplay art assignment — `REPO-VERIFIED`

- Background/floor/platform art are **scene-global objects in `Game.unity`**, not per-level:
  - `Background` SpriteRenderer (now `bg_menu_01`, camera-locked; see `WireGameBackground.cs`).
  - `Floor` + `FloorVisual` SpriteRenderers.
  - Platforms spawned from a single `_platformPrefab` by `PlatformSpawner` (below); color comes
    from `PlatformColor` + `PlatformColorPalette`, not per-world art.
- → **A world-visual layer + runtime applier is greenfield.** No per-level/per-world art
  selection exists anywhere today. This is the core architectural addition (phase **P34B/P34C**).

## 8. Platform generation & colours — `REPO-VERIFIED`

- `Scripts/Platforms/PlatformSpawner.cs`: `SpawnPlatforms(sequence)` builds rows from
  `LevelConfig`; row Y = `RowStartY + row * RowVerticalSpacing`; `BuildRowColors` guarantees the
  correct colour is present; `GetRowPositions` spreads across `RowHorizontalSpread` with jitter.
- Six semantic colours live in `PlatformColor` enum + `PlatformColorPalette.cs`
  (`#E63838/#3878E6/#38BF59/#FAD12E/#9938E6/#F28C26`). Sequence step is **`rowIndex`**, not Y —
  matches the confirmed row rule.
- `TrySpawnCactus(distractors)` only ever targets **distractor** platforms (never the correct one):
  guarded by `_cactusSpawnChance` and `distractors.Count > 0`. → cactus-on-correct-platform is
  structurally impossible today; the 100-level plan must preserve this invariant.

## 9. Hazard (cactus) — `REPO-VERIFIED`

- `Scripts/Obstacles/CactusObstacle.cs`; prefab `Prefabs/Obstacles/Cactus.prefab`;
  sprite `Art/Sprites/Obstacles/spr_cactus_obstacle_01.png`; grounding via
  `Editor/FixCactusGrounding.cs` (ink-base pivot, trigger box).
- Behaviour: touch during Playing → `PlayerHit` → `PlatformSpawner.CactusHit` → lose one life.
- → Themed hazards must be **visual reskins of this one behaviour**. A
  `WorldHazardVisualDefinition` (art/prefab) + shared `CactusObstacle` behaviour is the intended
  separation; do **not** author ten hazard scripts.

## 10. Rewards / wardrobe / migration precedent — `REPO-VERIFIED`

- Stars: `StarRewardCalculator`, `StarRewardFormatter`, `StarRewardStore` (`Scripts/Rewards/Runtime/`).
  Stars are a **mastery record**, capped 0..3, best-only, non-spendable, do not affect progression.
- Wardrobe: `WardrobeCatalog`, `WardrobeUnlockService`, `WardrobePersistenceKeys`,
  `WardrobePersistenceMigrator`, `WardrobeUnlockAcknowledgementStore` (`Scripts/Wardrobe/Runtime/`).
  → **Reusable precedent** for a versioned save migration + acknowledgement pattern (story-card
  "seen" flags, world-reward idempotency should follow this shape).

## 11. Tests relevant to expansion — `REPO-VERIFIED`

| Test | Path | Note for expansion |
|---|---|---|
| `LevelBalanceAssetTests` | `Tests/EditMode/LevelBalanceAssetTests.cs` | **Hardcodes `Assert.AreEqual(10, LevelConfigPaths().Length)` (line 29)** → must become 100 or data-driven. Also asserts `_sequenceLength >= 1`, ≥1 `LevelCatalog`. |
| `ProgressionPlayModeTests` | `Tests/PlayMode/` | unlock/continue behaviour — extend for world boundaries. |
| `AnalyticsCatalogTests` | `Tests/PlayMode/` | analytics params (LevelIndex/LevelNumber). |
| `WardrobePersistenceMigrationTests` | `Tests/PlayMode/` | migration precedent to mirror. |
| `WardrobeSaveCompatibilityMatrixTests` | `Tests/PlayMode/` | save-compat matrix precedent. |

Baseline suite size per memory: ~317 PlayMode tests (verify before asserting).

## 12. Documentation to update (stale 50-level / 3-world) — `REPO-VERIFIED`

*(Patch details in `03_GDD_ROADMAP_PATCH_PROPOSAL.md`; do not edit these in this task.)*

| File | Lines | Stale text |
|---|---|---|
| `CLAUDE.md` | 12, 130 | "50-level launch target", "Build scalable 50-level structure" |
| `Docs/Design/Jebby_Jump_GDD.md` | 140–141, 289 | "50 levels", "3 worlds", "Build scalable 50-level structure" |
| `Docs/Design/Jebby_Jump_Roadmap.md` | 8, 80–81, 103, 141 | "50 levels", "3 worlds", "P3 — 50-Level Data Foundation" |
| `Docs/Production/Jebby_Jump_Full_Production_Plan.md` | 213, 359–360 | "P3 — 50-Level", "50 levels", "3 worlds" |
| `Docs/Production/Jebby_Jump_Full_Production_Plan_v1.0.md` | 152, 271–272 | "P3 — 50-Level", "50 levels", "3 worlds" |
| `Tests/EditMode/LevelBalanceAssetTests.cs` | 29 | `Assert.AreEqual(10, …)` |

## 13. Architectural risks (summary)

| ID | Risk | Severity | Mitigation phase |
|---|---|---|---|
| R1 | Flat Level Select cannot scale to 100 | High | P34D world navigation |
| R2 | No per-level/world art layer exists | High | P34B/P34C world config + applier |
| R3 | Index-keyed unlock+stars break if L1–10 reordered/renamed | High | P34A decision: L1–10 unchanged; P34B tests |
| R4 | `LevelBalanceAssetTests` hardcodes count 10 | Med | P34E data-driven test |
| R5 | Themed hazards could drift from cactus behaviour | Med | P34C shared behaviour + visual def |
| R6 | Camera-locked background (`WireGameBackground`) is single-layer; parallax not present | Low | P34H optional, only if budget allows |
| R7 | No measured perf baseline for 100-level texture load | Med | P34U measure before budgets |
| R8 | Balance for a 100-level curve has no human playtest evidence | High | all balance = PLAYTEST-HYPOTHESIS until P34T |
