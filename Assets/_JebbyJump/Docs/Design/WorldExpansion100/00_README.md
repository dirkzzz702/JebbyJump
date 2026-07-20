# World Expansion 100 — Planning Package

**Status:** PLANNING / SPECIFICATION ONLY — not permission to implement.
**Scope target:** replace the old *50-level / 3-world* direction with **100 levels across 10 themed worlds** (10 levels/world, one reusable `Game` scene).

## What this folder is

A design + specification + implementation-handoff package so a future Claude Code
session can implement the expansion phase-by-phase without re-deriving the
architecture or inventing requirements. It contains **no runtime changes**: no C#,
scenes, prefabs, ScriptableObjects, art, PlayerPrefs, packages, or ProjectSettings
were modified in producing it.

## Repository baseline reviewed

| Item | Value |
|---|---|
| Branch | `main` |
| HEAD | `a2daa75` (Warm kid-friendly UI palette + equal button labels) |
| Unity | `6000.4.7f1` |
| Scene flow | `Boot → MainMenu → Game` (one reusable `Game` scene) |
| Levels today | **10** (`Level1Config`…`Level10Config`) |
| UnitySkills bridge | connected (`localhost:8090`, v2.1.3) |

## Document index

| # | File | Purpose | State |
|---|---|---|---|
| 00 | `00_README.md` | This overview | Draft |
| 01 | `01_SOURCE_OF_TRUTH_AND_REPO_AUDIT.md` | Verified current architecture, save keys, risks | Draft |
| 02 | `02_DECISION_REGISTER.md` | Confirmed / repo-verified / proposed / open / blocked | Draft |
| 03 | `03_GDD_ROADMAP_PATCH_PROPOSAL.md` | How to later update GDD/Roadmap/CLAUDE (not applied) | Draft |
| 04 | `04_TEN_WORLD_CREATIVE_BIBLE.md` | Per-world art/creative briefs | **Pending roster approval** |
| 05 | `05_WORLD_PROGRESSION_AND_LANDMARK_PLAN.md` | Rainbow Tower landmark progression | Pending |
| 06 | `06_WORLD_STORY_CARDS.md` | Postcard cards between worlds | Pending |
| 07 | `07_WORLD_REWARD_AND_COSMETIC_OPTIONS.md` | Reward model comparison | Pending |
| 08 | `08_RUNTIME_WORLD_ARCHITECTURE.md` | Data-driven world theming architecture | Pending |
| 09 | `09_DATA_MODEL_AND_CONFIG_SCHEMA.md` | Config/schema proposals | Pending |
| 10 | `10_SAVE_AND_PROGRESSION_MIGRATION_PLAN.md` | Preserve existing local progress | Pending |
| 11 | `11_LEVEL_SELECT_AND_WORLD_MAP_UX.md` | 100-level navigation | Pending |
| 12 | `12_100_LEVEL_MASTER_PLAN.md` | Level design narrative | **Pending roster approval** |
| 13 | `13_100_LEVEL_MASTER_TABLE.csv` | 100 level rows (data) | **Pending roster approval** |
| 14 | `14_DIFFICULTY_CURVE_AND_BALANCE_HYPOTHESES.md` | Difficulty curve | Pending |
| 15 | `15_FINALE_LEVEL_DESIGN.md` | 10 finales | Pending |
| 16 | `16_LEVELCONFIG_PROPOSED_VALUES.md` | Per-level config values | Pending |
| 17 | `17_WORLD_ART_SYSTEM_AND_TECHNICAL_SPECS.md` | Import/technical contracts | Pending |
| 18 | `18_WORLD_ART_ASSET_MANIFEST.csv` | Every art asset row | **Pending roster approval** |
| 19 | `19_WORLD_ART_FOLDER_TREE.txt` | Folder/filename manifest | Pending |
| 20 | `20_WORLD_ART_GENERATION_BRIEFS.md` | ChatGPT art briefs | Pending |
| 21 | `21_WORLD_ART_GENERATION_BATCH_PLAN.md` | Batch order + gates | Pending |
| 22 | `22_WORLD_ART_VALIDATION_PLAN.md` | Art validation | Pending |
| 23 | `23_LEVELCONFIG_GENERATION_AND_EDITOR_TOOLING_PLAN.md` | Deterministic level-gen tool | Pending |
| 24 | `24_WORLD_ASSET_VALIDATION_TOOLING_PLAN.md` | World-asset validators | Pending |
| 25 | `25_AUTOMATED_TEST_PLAN.md` | EditMode/PlayMode tests | Pending |
| 26 | `26_MANUAL_PLAYTEST_AND_BALANCE_PLAN.md` | Human playtest protocol | Pending |
| 27 | `27_ART_AND_DEVICE_QA_PLAN.md` | Aspect/device QA | Pending |
| 28 | `28_PERFORMANCE_AND_MEMORY_BUDGET.md` | Budgets (needs measured baseline) | Pending |
| 29 | `29_IMPLEMENTATION_ROADMAP.md` | P34A…P34U phases | Pending |
| 30 | `30_RISK_REGISTER.md` | Risks + mitigations | Pending |
| 31 | `31_APPROVAL_GATE.md` | Decisions the user must make first | Draft |
| 32 | `32_SELF_VALIDATION.md` | Package self-checks | Draft (finalised last) |
| — | `implementation_prompts/` | One paste-ready prompt per phase | Pending |

## How to read this package

1. Start with **01** (what actually exists) and **02** (what's decided vs open).
2. Make the calls in **31_APPROVAL_GATE.md** (roster, reward model, Levels 1–10 handling
   are the big three).
3. The **Pending roster approval** docs (04/12/13/18) are intentionally deferred: they are
   the most voluminous and would need full rework if the world roster or the Levels 1–10
   decision changes. They are produced once the gate is answered.

## Anti-guessing labels used throughout

`CONFIRMED` · `REPO-VERIFIED` · `PROPOSED` · `PLAYTEST-HYPOTHESIS` · `OPEN DECISION` ·
`BLOCKED` · `NOT APPLICABLE`.
