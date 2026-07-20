# 02 — Decision Register

Legend: `CONFIRMED` (user-approved / proven) · `REPO-VERIFIED` (inspected) ·
`PROPOSED` (needs approval) · `OPEN DECISION` (blocks implementation) ·
`BLOCKED` (missing evidence) · `PLAYTEST-HYPOTHESIS` (needs human playtest).

## A. Confirmed decisions (from the master prompt / product rules)

| ID | Decision | Status |
|---|---|---|
| C1 | Replace 50-level/3-world with **100 levels / 10 worlds / 10 per world** | CONFIRMED |
| C2 | **One reusable `Game` scene**; worlds are data + assets, not scenes | CONFIRMED |
| C3 | Preserve core loop: memory → hide → timer → platformer → land correct colour per row → clear time/rank/Stars | CONFIRMED |
| C4 | Row rule: sequence step = `rowIndex`, not Y | CONFIRMED |
| C5 | Every level completable **without skills**; skills never bypass sequence validation | CONFIRMED |
| C6 | Max sequence length **10**; max choices per row **6**; failure = full-level restart | CONFIRMED |
| C7 | Only approved hazard = **cactus-equivalent** (touch during Playing → lose 1 life). No new mechanics | CONFIRMED |
| C8 | Six semantic colours retained & identity-locked (`#E63838/#3878E6/#38BF59/#FAD12E/#9938E6/#F28C26`); no 7th | CONFIRMED |
| C9 | Level 10 of each world is a **finale** (special via art/composition, not new mechanic) | CONFIRMED |
| C10 | Short self-contained story cards; recurring Rainbow Tower landmark; special World-10 ending; no lore-heavy cutscenes | CONFIRMED |
| C11 | Cactus never spawns on the correct platform | CONFIRMED (also REPO-VERIFIED invariant) |
| C12 | No currency/economy write, no new SDK/package, without explicit approval | CONFIRMED |

## B. Repository-derived facts (see doc 01)

| ID | Fact |
|---|---|
| V1 | Scene flow `Boot→MainMenu→Game`; one Game scene already reusable |
| V2 | `LevelConfig` has **no** world/theme/art/id fields |
| V3 | 10 levels today; order = `LevelSessionController._levels`; menu mirror = `LevelCatalog` |
| V4 | Unlock (`jebby.level.highestUnlocked`) & Stars (`jebby.rewards.levelStars.<index>`) are **index-keyed**; Best time (`JebbyJump.BestTime.<assetName>`) is **name-keyed** |
| V5 | Level Select is a single flat grid (won't scale to 100) |
| V6 | Cactus behaviour = one script; only spawns on distractors |
| V7 | Wardrobe has a versioned migration + acknowledgement precedent to reuse |
| V8 | `LevelBalanceAssetTests` hardcodes count 10 |

## C0. User-confirmed at Approval Gate (2026-07-18)

| Gate | Decision | Status |
|---|---|---|
| Roster | Recommended 10-world roster (Cloud Meadow → Rainbow Tower Castle) | **CONFIRMED** |
| Reward model | **Option C — combined** (World Gem trophy on finale first-clear + themed cosmetic on world mastery; Stars never consumed) | **CONFIRMED** |
| Levels 1–10 | **World 1, unchanged** (indices 0–9 + asset names preserved) | **CONFIRMED** |
| Scope | Build full planning package now | **CONFIRMED** |

## C. Proposed decisions (recommended — need approval)

| ID | Decision | Recommendation | Alternatives |
|---|---|---|---|
| P1 | **World roster** | ✅ CONFIRMED — prompt's 10-world roster (Cloud Meadow → Rainbow Tower Castle) | — |
| P2 | **Levels 1–10 handling** | ✅ CONFIRMED — **World 1 unchanged** (preserve indices 0–9 + asset names) | — |
| P3 | **Level-to-world mapping** | Contiguous: W_n = levels `(n-1)*10+1 … n*10`; keep existing asset names for 1–10, add `Level011Config…Level100Config` | Non-contiguous (rejected — breaks index unlock) |
| P4 | **World config type** | One `WorldDefinition` ScriptableObject per world + a `WorldCatalog` (mirrors existing `LevelConfig`/`LevelCatalog` pattern) | Struct-in-catalog (less inspectable) |
| P5 | **Theme application timing** | Apply world visuals **before** level generation & before gameplay visible; default World 1; safe fallback art | — |
| P6 | **Platform theming** | Shared shape/collider + themed **material/sprite swap** keyed to locked colour (collider/width never change) | Per-world sprite set (more art, same rule) |
| P7 | **Hazard theming** | `CactusObstacle` behaviour + `WorldHazardVisualDefinition` (art/prefab only) | — |
| P8 | **Reward model** | ✅ CONFIRMED — **Option C (combined)**: finale first-clear → non-spendable World Gem trophy; world mastery → themed cosmetic (details in doc 07) | — |
| P9 | **Story-card timing** | One card **before** each world + opening + World-10 ending; first-view persistent, replayable | after-each-world |
| P10 | **Parallax** | **Out** of launch scope; single camera-locked background per world (matches current `WireGameBackground`) | in-scope only if perf budget proven |
| P11 | **Level authoring source** | **CSV** (`13_100_LEVEL_MASTER_TABLE.csv`) → deterministic editor generator → `LevelConfig` assets (reviewable, diff-friendly) | JSON / SO-only |
| P12 | **Launch shape** | Single 100-level launch **or** staged (W1–W5 then W6–W10) — recommend staged content releases | single launch |
| P13 | **Docs location** | `Assets/_JebbyJump/Docs/Design/WorldExpansion100/` (matches existing Design docs convention; `.meta` auto-generated by Unity) | repo-root `Docs/` (rejected — repo keeps design docs under Assets) |

## D. Open decisions (implementation blockers until answered)

These map 1:1 to `31_APPROVAL_GATE.md`. Implementation of the affected phase must not start
until resolved:

1. Final world roster & order (P1).
2. Reward model A/B/C (P8) + exact cosmetic-unlock condition.
3. Levels 1–10: unchanged vs redesigned-with-preserved-IDs (P2).
4. Story cards before vs after each world (P9).
5. Parallax in launch scope? (P10).
6. Themed hazard may use a distinct **prefab** sharing one behaviour component? (P7).
7. Single launch vs staged releases (P12).
8. Art-production order (default: W00 templates → W01…W10 → UI → QA).
9. Human playtest availability + who holds tuning authority (gates P34T balance sign-off).

## E. Blocked

| ID | Item | Reason | Unblock |
|---|---|---|---|
| B1 | Final performance/memory budgets | No measured baseline captured yet | Measure in P34U before numeric budgets (doc 28) |
| B2 | Exact art import contracts for *new* world families | Derive from accepted art importers; any un-measurable value = `BLOCKED — TECHNICAL ART DECISION REQUIRED` | doc 17 during P34H |
| B3 | Final balance values | No human playtest for 100-level curve | All balance stays `PLAYTEST-HYPOTHESIS` until P34T |
