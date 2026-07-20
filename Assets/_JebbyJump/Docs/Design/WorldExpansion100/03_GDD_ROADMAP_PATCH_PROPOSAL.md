# 03 — GDD / Roadmap / CLAUDE Patch Proposal

> **STATUS: APPLIED in P34A** (commit follows this doc's §1–§5, excluding the archived
> `_v1.0` snapshot — see §5 decision). Verified by re-running the stale scan in §7:
> no `50-level` / `3 worlds` text remains in CLAUDE.md, the GDD, the Roadmap, or the living
> Production Plan. Test canon (§6) is deferred to **P34E** as planned.

This documents exactly how the canon docs should later change from
*50 levels / 3 worlds* → *100 levels / 10 worlds*. Apply only in phase **P34A** after the
roster is approved (Approval Gate). Each entry: file · line(s) · current · proposed.

## 1. `CLAUDE.md`

| Line | Current | Proposed |
|---|---|---|
| 12 | `50-level launch target` | `100-level launch target (10 themed worlds × 10 levels)` |
| 130 | `3. Build scalable 50-level structure.` | `3. Build scalable 100-level / 10-world structure.` |

Also review the "Do Not Add Without Approval" list (currently `10+ levels before screen polish`):
add a note that the World-Expansion-100 plan is the approved planning artefact and that
per-world art/level batches remain individually gated.

## 2. `Docs/Design/Jebby_Jump_GDD.md`

| Line | Current | Proposed |
|---|---|---|
| 140 | `50 levels` | `100 levels` |
| 141 | `3 worlds` | `10 themed worlds (10 levels each)` |
| ~142+ | "Suggested split" (3-world split) | Replace with the 10-world roster (doc 04) once approved |
| 289 | `3. Build scalable 50-level structure.` | `3. Build scalable 100-level / 10-world structure.` |

Add a new subsection under §10 Level Design pointing to this package as the authoritative
100-level design source, and to `13_100_LEVEL_MASTER_TABLE.csv` for per-level values.

## 3. `Docs/Design/Jebby_Jump_Roadmap.md`

| Line | Current | Proposed |
|---|---|---|
| 8 | `50 levels` | `100 levels` |
| 80 | `50 levels` | `100 levels` |
| 81 | `3 worlds` | `10 themed worlds` |
| 103 | `### P3 — 50-Level Data Foundation` | `### P3 — 100-Level / 10-World Data Foundation` (or supersede P3 with the P34x roadmap, doc 29) |
| 141 | `50 levels, monetization, wardrobe, services, mobile polish.` | `100 levels, …` |

Recommend adding a pointer: "Level/world scope is superseded by
`Docs/Design/WorldExpansion100/29_IMPLEMENTATION_ROADMAP.md` (phases P34A–P34U)."

## 4. `Docs/Production/Jebby_Jump_Full_Production_Plan.md`

| Line | Current | Proposed |
|---|---|---|
| 213 | `### P3 — 50-Level Data Foundation` | `### P3 — 100-Level / 10-World Data Foundation` |
| 359 | `50 levels` | `100 levels` |
| 360 | `3 worlds` | `10 themed worlds` |

## 5. `Docs/Production/Jebby_Jump_Full_Production_Plan_v1.0.md`

| Line | Current | Proposed |
|---|---|---|
| 152 | `### P3 — 50-Level Data Foundation` | `### P3 — 100-Level / 10-World Data Foundation` |
| 271 | `- 50 levels` | `- 100 levels` |
| 272 | `- 3 worlds` | `- 10 themed worlds` |

**DECISION (P34A): LEFT UNPATCHED — treated as a frozen historical snapshot.** The `_v1.0`
suffix marks a versioned record of what the plan said at v1.0; rewriting it would falsify
history, and the living `Jebby_Jump_Full_Production_Plan.md` carries the current canon. The
three stale lines above are therefore expected to persist in this file only.
*Reverse this decision if the team prefers versioned docs to be kept current — it is a one-line
patch per row.*

## 6. Test canon — `Tests/EditMode/LevelBalanceAssetTests.cs`

| Line | Current | Proposed |
|---|---|---|
| 29 | `Assert.AreEqual(10, LevelConfigPaths().Length);` | `Assert.AreEqual(100, LevelConfigPaths().Length);` **or** make data-driven against `WorldCatalog` (preferred — see doc 25) |

This is code, **not** a doc — it changes in phase **P34E** (data generation), guarded by tests,
never in this planning task.

## 7. Other sweeps to run at P34A

Before applying, re-run the stale-scan (new stale statements may appear as docs evolve):

```
grep -rinE "50[ -]?level|50 levels|three world|3 world|3-world|three-world" \
  CLAUDE.md Assets/_JebbyJump/Docs Assets/_JebbyJump/Tests
```

Check comments/editor tooling for "50" or "3 world" assumptions
(e.g. `Jebby Jump/Progression/Create Or Sync Level Catalog` should have no count cap).
