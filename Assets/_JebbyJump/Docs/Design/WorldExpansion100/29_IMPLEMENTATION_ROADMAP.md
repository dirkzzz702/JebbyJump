# 29 — Implementation Roadmap

Status: `PROPOSED`. Small, reviewable phases. Each has a paste-ready prompt in
`implementation_prompts/`. Nothing here is implemented in this planning task.

## Phase list

| Phase | Title | Gate to start |
|---|---|---|
| P34A | Canon & Architecture Approval | Approval Gate answered |
| P34B | World Data Foundation | P34A |
| P34C | Two-World Vertical Slice | P34B |
| P34D | World Level Select | P34B |
| P34E | 100-Level Data Generation | P34B |
| P34F | Story Cards | P34D |
| P34G | World Rewards (model C) | P34D + reward approval |
| P34H | World Art Technical Pipeline | P34C |
| P34I…P34R | World 01…10 Art Integration | P34H (+ art batch W01…W10) |
| P34S | Cross-World Polish | P34I–P34R |
| P34T | Balance Evidence (human playtest) | P34E + playtest owner |
| P34U | Device & Release Validation | P34S + P34T |

## Per-phase fields (in each prompt)

objective · scope · out-of-scope · files likely changed · migration impact · tests · manual QA ·
art dependencies · success criteria · rollback plan · commit strategy · next-phase gate.

## Sequencing notes

- **P34A first, always** — applies canon (doc 03) so no stale 50/3-world remains.
- P34B is the spine (world data); P34C proves the one-scene theming before any art volume.
- P34E (100-level data) can proceed in parallel with P34D/P34F but must not claim balance (P34T).
- Art phases P34I–P34R are one-per-world, each gated on its art batch (doc 21) and the pipeline P34H.
- **Staged launch option (Gate #8):** ship W1–W5 after their P34x complete, then W6–W10 — the
  contiguous mapping + additive saves make a staged release safe.

## Definition of done (whole expansion)

100 levels across 10 worlds live in one `Game` scene; save-compatible; world-tab level select;
story cards; reward model C; per-world art accepted; automated + human QA green; canon docs updated.
