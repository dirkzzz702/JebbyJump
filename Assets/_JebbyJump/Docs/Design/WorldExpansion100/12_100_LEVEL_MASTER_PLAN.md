# 12 — 100-Level Master Plan

Status: World 1 = `EXISTING-PRESERVED`; Worlds 2–10 = `PROPOSED` / all balance `PLAYTEST-HYPOTHESIS`.
Authoritative data: `13_100_LEVEL_MASTER_TABLE.csv` (100 rows, machine-validated).

## Structure

- 100 levels, global ids `001`–`100`; World n = ids `(n-1)*10+1 … n*10`; contiguous.
- Every world uses the same **ten-level rhythm**:

| Lvl | Role | Intent |
|---|---|---|
| 1 | intro | theme intro, low pressure |
| 2 | reinforce | core pattern |
| 3 | first-complexity | first modest step up |
| 4 | route-reading | read the route |
| 5 | mid-checkpoint | consolidation |
| 6 | denser-distractors | more decoys |
| 7 | precision | tighter landings |
| 8 | synthesis | memory + movement |
| 9 | pre-finale-mastery | mastery test |
| 10 | **finale** | special set-piece (art/composition, not new mechanic) |

## Difficulty sources (existing parameters only — no new mechanics)

sequence length · colour count · choices/row (`platformsPerRow`) · distractor density · cactus
count/placement (distractor-only) · platform width · horizontal gap/spread · same-row Y stagger ·
memory display duration · time-rank thresholds. (Per doc 14 curve.)

## Hard constraints (validated in the CSV generator + doc 25 tests)

- exactly 100 levels, 10 worlds, 10/world, 10 finales
- sequence length ≤ 10; choices/row ≤ 6
- `skill_required = false` on all 100
- cactus never on the correct platform (distractor-only, structural today)
- failure = full-level restart
- six locked colours; no 7th

## World-by-world summary (from CSV)

| World | Band | Finale seq | Finale choices | Finale clear(s) | Balance |
|---|---|---|---|---|---|
| W01 Cloud Meadow | W1-2 | 6 | 3 | 14 | EXISTING-PRESERVED |
| W02 Enchanted Forest | W1-2 | 5 | 3 | 14 | HYPOTHESIS |
| W03 Crystal Caves | W3-4 | 6 | 4 | 18 | HYPOTHESIS |
| W04 Sunshine Desert | W3-4 | 6 | 4 | 18 | HYPOTHESIS |
| W05 Ocean Sky | W5-6 | 7 | 5 | 22 | HYPOTHESIS |
| W06 Candy Cloud Kingdom | W5-6 | 7 | 5 | 22 | HYPOTHESIS |
| W07 Clockwork Heights | W7-8 | 9 | 6 | 26 | HYPOTHESIS |
| W08 Moonlit Dreamscape | W7-8 | 9 | 6 | 26 | HYPOTHESIS |
| W09 Stormfire Volcano | W9-10 | 10 | 6 | 30 | HYPOTHESIS |
| W10 Rainbow Tower Castle | W9-10 | 10 | 6 | 30 | HYPOTHESIS |

## Notes on World 1 (preserved)

World 1 rows mirror the shipped `Level1Config`…`Level10Config` values (seq 3→6, choices 2→3,
colours 3→6, cactus 0→0.4). The generator (doc 23) must **not** overwrite these. Their titles in the
CSV are cosmetic labels for the plan; the assets themselves are untouched (Gate #4).

## Physical-plausibility caveat

Target clear times assume the current movement/jump feel and screen height (camera ortho 7).
If P34C/P34E reveal a proposed band is physically implausible (e.g. seq 10 in 22 s at late-world
gaps), adjust and record the reason (doc 14). No band is validated until P34T human playtest.
