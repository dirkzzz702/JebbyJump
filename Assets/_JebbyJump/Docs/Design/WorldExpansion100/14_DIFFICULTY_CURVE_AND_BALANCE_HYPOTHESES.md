# 14 — Difficulty Curve & Balance Hypotheses

Status: all values `PLAYTEST-HYPOTHESIS` (World 1 `EXISTING-PRESERVED`). Not validated balance.

## Proposed bands (starting constraints — adjust if physically implausible)

| Worlds | Clear range | Seq length | Choices/row |
|---|---|---|---|
| 01–02 | ~10–14 s | 3–5 | 2–3 |
| 03–04 | ~12–18 s | 4–6 | 3–4 |
| 05–06 | ~15–22 s | 5–7 | 3–5 |
| 07–08 | ~18–26 s | 6–9 | 4–6 |
| 09–10 | ~22–30 s | 8–10 | 5–6 |

Within a world, difficulty ramps intro→finale (CSV interpolates each parameter across levels 1→10).

## Rank thresholds (proposed derivation)

For target clear time `T`: `S = 0.6·T`, `A = 0.8·T`, `B = 1.0·T`, `C = any completion within lives`.
Guaranteed ordered `S < A < B` (asserted in generator + doc 25). Actual per-world
`TimeRankConfig` assets are authored/tuned in P34E/P34T; rank is never persisted (recomputed at
display, doc 01 §5) so tuning never strands a stale rank.

## Difficulty levers (existing only)

| Lever | Low world | High world |
|---|---|---|
| sequence length | 3 | 10 |
| colours used | 3 | 6 |
| choices/row | 2 | 6 |
| cactus chance | 0 | ~0.45 (distractor-only) |
| platform width | ~4.0 | ~3.3 |
| horizontal spread | ~7.5 | ~10.0 |
| Y stagger (jitter) | 0 | ~0.35 (W5+) |
| memory display | 5.0 s | 4.0 s |

Never make cactus unavoidable; never place it on the correct platform; never require a skill.

## Adjustment protocol

If P34C/P34E measurement shows a band is physically implausible (jump arc can't cross the proposed
gap, or seq length can't fit the screen height at the proposed spacing), the generator input CSV is
edited and the change **explained** in this doc's changelog. No silent balance edits.

## Acceptance (P34T only)

A band is "validated" only after recorded human attempts meet the bands in doc 26
(completion rate, median attempts, clear-time distribution, S/A/B/C spread). Until then every value
here is a hypothesis.
