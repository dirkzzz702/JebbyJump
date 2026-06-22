# Jebby Jump — Level Tuning Review-Candidate Log v0.1 (P28)

These are **review candidates / hypotheses to investigate with human data — NOT tuning
recommendations.** Per the P28 corrections: no exact replacement values are proposed without
real playtest data; every entry is `AuthorizedForApply = false` and
`DeferredPendingPlaytest = true`; none was applied. Deterministic doc (no dates/paths/names).
Confidence is LOW/Observation throughout, derived from the P26 heuristic baseline used as a
**hypothesis list, not authority**.

## Level-balance review candidates

| ID | Level | Category | Hypothesis | Suggested direction (qualitative) | Asset | Apply | Deferred |
|---|---|---|---|---|---|---|---|
| RC-RANK-01 | all | Rank threshold | S/A/B thresholds are placeholders; fair values need multiple human completion times per level | Derive from real median/best human times in P4B (not expert-only runs) | TimeRankConfig | false | true |
| RC-HAZ-01 | 9 (+curve) | Hazard pressure | Cactus chance is non-monotonic with an L9 spike (~0.40) and dips on L5/L7/L8; may read as an unfair spike | Review for a gentler rising hazard curve consistent with the family/casual assumption | LevelConfig | false | true |
| RC-MEM-01 | 6/7/9 | Memory load | Memory time drops to 4.5s only on L6/L7/L9; confirm intentional pulse vs. drift | Confirm intended memory pacing in real play | LevelConfig | false | true |
| RC-CURVE-01 | all | Learning/onboarding | Overall heuristic ramp (5.0→10.2) is reasonable; two tiny dips (L4→L5, L6→L7) are within noise | No change unless a playtest shows a real spike | — | false | true |

Each carries a risk note (e.g. RC-RANK-01: tuning to expert-only times would over-tighten
ranks; RC-HAZ-01: hazard is not the right lever for a rank issue; RC-MEM-01: memory changes
alter core difficulty). No numeric current→proposed mapping is asserted — that requires
human evidence first.

## Progression-economy observations (SEPARATE from level-balance tuning)

| ID | Topic | Observation | Apply | Deferred |
|---|---|---|---|---|
| PE-01 | Wardrobe star economy | Top outfit unlock needs all 30 stars (S/A on every one of the 10 levels) — steep for a gentle family/casual curve. (`StarReward` S/A=3, B=2, C=1 → max 30; wardrobe thresholds `0/4/8/12/15/22/26/30` are PLACEHOLDERS.) | false | true |

Wardrobe thresholds + StarReward semantics are **out of scope to edit** in P28 — this is an
observation for a future, separately-approved progression review, not a balance tuning
change.

## Disposition

All review candidates and observations are **DEFERRED pending real human playtest data**.
None accepted, none rejected, none applied. The next phase that may act on these is **P29D**
(balance retest after human feedback), and only via an explicitly-approved data-only tuning
pass mapped to these IDs.
