# Jebby Jump — Level Difficulty Analysis v0.1 (P26)

Data-driven, **read-only** analysis of the 10 LevelConfig + TimeRankConfig assets. Proposals are heuristic and **LOW-CONFIDENCE**; no balance values are changed by this audit (thresholds stay as authored until the P4B playtest).

- Timestamp: 2026-06-20T13:31:33.3411826Z  |  Git: 74f76d1
- Read-only proof: **READ-ONLY VERIFIED (config asset hash unchanged: d0510c5e96f1877c…)**
- Confidence: LOW — cross-level scoring is a heuristic pending the P4B human playtest. Only intra-level S<A<B threshold ordering is a hard invariant.
- Heuristic: `score = 1*seqLen + 0.5*colors + 0.3*max(0,10-memorySec) + 2*cactusChance + 0.1*platformsPerRow - 0.4*lives  (transparent heuristic; LOW-CONFIDENCE pending P4B playtest)`
- Hard-invariant (S<A<B) failures: **0**

## Difficulty curve

| Lvl | seq | colors | mem(s) | lives | cactus | per-row | S | A | B | heuristic |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | 3 | 3 | 5 | 3 | 0 | 2 | 8 | 12 | 18 | 5 |
| 2 | 4 | 3 | 5 | 3 | 0 | 2 | 10 | 14 | 20 | 6 |
| 3 | 4 | 4 | 5 | 3 | 0 | 2 | 11 | 15 | 22 | 6.5 |
| 4 | 5 | 4 | 5 | 3 | 0.2 | 2 | 13 | 18 | 25 | 7.9 |
| 5 | 5 | 4 | 5 | 3 | 0.15 | 2 | 14 | 19 | 27 | 7.8 |
| 6 | 5 | 5 | 4.5 | 3 | 0.25 | 3 | 16 | 22 | 30 | 8.75 |
| 7 | 5 | 5 | 4.5 | 3 | 0.2 | 3 | 18 | 25 | 33 | 8.65 |
| 8 | 6 | 6 | 5 | 3 | 0.15 | 2 | 20 | 27 | 36 | 9.8 |
| 9 | 6 | 5 | 4.5 | 3 | 0.4 | 3 | 22 | 30 | 40 | 10.05 |
| 10 | 6 | 6 | 5 | 3 | 0.3 | 3 | 25 | 33 | 45 | 10.2 |

## Findings
- **[OBSERVATION] curve.monotonicity** — heuristic difficulty decreases Level4Config -> Level5Config (7.9 -> 7.8); low-confidence, verify by playtest.
- **[OBSERVATION] curve.monotonicity** — heuristic difficulty decreases Level6Config -> Level7Config (8.75 -> 8.65); low-confidence, verify by playtest.

## Proposals (LOW-CONFIDENCE — for P4B playtest, not applied)
- Treat any `curve.monotonicity` / `threshold.trend` observation as a *question*, not a defect: confirm the intended pacing by playtesting before changing any value.
- Any `INVARIANT_FAIL` is a real ordering bug and should be fixed (S<A<B).
- Cross-level numeric tuning is DEFERRED to P4B with human playtest data + approval.

_No LevelConfig/TimeRankConfig asset is modified by this audit._
