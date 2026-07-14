# Art Production Roadmap

## Phase A — decisions (0.5 day, product owner)
Resolve D1 (floor), confirm D2 candidates (radius), open the provenance
register (D3). D4–D7 can wait.

## Phase B — generation batches (ChatGPT sessions; order + gates in
`generation/GENERATION_BATCH_PLAN.md`)
1. **001 store identity** (icons ART-001/002/003/015) — unblocks the Play
   listing alongside the existing P31 doc package.
2. **002 wordmark** (ART-010) → human brand/legal review.
3. **003 floor** (ART-005, after D1) + **004 menu bg** (ART-006).
4. **005 UI kit** (ART-007/008) — biggest visible quality jump.
5. **006 skill icons + VFX** (ART-011..014).
6. **007 key art + feature graphic** (ART-016/004).

## Phase C — Unity integration (one approved commit per group)
Follow `integration/post_import_wiring.csv` order 1→7; every step gated by the
existing test suites + the two overlap audit commands. Icons wiring also
re-runs the release preflight/AAB build.

## Phase D — captures + store (after C)
ART-009 screenshots (focused editor or device) → StoreAssets/ complete →
P31 store package unblocks its graphics rows; privacy URL/console items remain
the separate P32 externals.

## Phase E — before PUBLIC release (not internal testing)
Provenance/licence register complete (D3); outfit final-art certification
pass; D7 texture-size hygiene commit; tablet screenshots (D5) if promoting.

Dependencies: B2→B6(007 composite optional), B(003..005)→D(captures),
A(D1)→B3, A(D2 note)→B5. Everything in B is parallelizable across sessions
except those edges.
