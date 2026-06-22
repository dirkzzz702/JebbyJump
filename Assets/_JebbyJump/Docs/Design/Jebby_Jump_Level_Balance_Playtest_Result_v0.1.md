# Jebby Jump — Level Balance Playtest Result v0.1 (P28)

Outcome of **P28 (preparation only)**. The evidence system is prepared for all 10 levels,
but **no human playtest occurred**, so no evidence was collected and no balance claim is
made. Deterministic doc — no dates, paths, run IDs, or personal identities. (Timestamps +
raw output live only in the ignored `Builds/P28/<commit>/` report.)

## Decision

> **P28 preparation complete — manual balance playtest NOT RUN.**

Balance remains **provisional**; current LevelConfig/TimeRankConfig values are placeholders
awaiting real completion-time data.

## Read-only / no-hidden-tuning proof

The baseline tool hashes all LevelConfig + TimeRankConfig + LevelCatalog assets **and** the
wardrobe/star threshold sources (`WardrobeCatalog.cs`, `StarRewardCalculator.cs`) before and
after; the run is verified to change none of them. EditMode tests independently re-assert
the 10-level invariants and that inspection mutates no asset. No tuning/protected asset was
modified.

## Per-level baseline (EvidenceStatus = NotRun for all 10)

| Lvl | status | seq | colors | mem(s) | lives | cactus | /row | S | A | B |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | NotRun | 3 | 3 | 5.0 | 3 | 0.00 | 2 | 8 | 12 | 18 |
| 2 | NotRun | 4 | 3 | 5.0 | 3 | 0.00 | 2 | 10 | 14 | 20 |
| 3 | NotRun | 4 | 4 | 5.0 | 3 | 0.00 | 2 | 11 | 15 | 22 |
| 4 | NotRun | 5 | 4 | 5.0 | 3 | 0.20 | 2 | 13 | 18 | 25 |
| 5 | NotRun | 5 | 4 | 5.0 | 3 | 0.15 | 2 | 14 | 19 | 27 |
| 6 | NotRun | 5 | 5 | 4.5 | 3 | 0.25 | 3 | 16 | 22 | 30 |
| 7 | NotRun | 5 | 5 | 4.5 | 3 | 0.20 | 3 | 18 | 25 | 33 |
| 8 | NotRun | 6 | 6 | 5.0 | 3 | 0.15 | 2 | 20 | 27 | 36 |
| 9 | NotRun | 6 | 5 | 4.5 | 3 | 0.40 | 3 | 22 | 30 | 40 |
| 10 | NotRun | 6 | 6 | 5.0 | 3 | 0.30 | 3 | 25 | 33 | 45 |

Objective assumption: gentle-to-moderate family/casual (preparation assumption, not a
final product decision). Intra-level S<A<B holds for all 10 (test-verified).

## Levels attempted / not attempted

Attempted: **none** (no human tester this phase). Not attempted: **all 10** — the system is
ready to collect their evidence when a tester is available.

## Review candidates & observations

See `Jebby_Jump_Level_Tuning_Proposal_Log_v0.1.md`. All are LOW-confidence hypotheses,
`AuthorizedForApply = false`, `DeferredPendingPlaytest = true`; none applied.

## Limitations

No human play; a scripted editor bot would play the memory phase perfectly and is not
representative, so it was not used. No device/touch validation. Balance, rank thresholds,
and the star economy are unvalidated. Not store/device/art/accessibility certified.

## Next

Run the kit with a real tester (P29D) → analyze real data → then, only with explicit
approval, a data-only tuning pass.
