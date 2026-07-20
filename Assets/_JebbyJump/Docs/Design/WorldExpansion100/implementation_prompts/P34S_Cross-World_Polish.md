# P34S — Cross-World Polish

**Phase objective:** Transitions, level-select polish, story/reward polish, memory/perf pass.

## Standing constraints (restate every phase)
- 100 levels / 10 worlds / 10 per world; ONE reusable `Game` scene (no scene duplication).
- Core loop unchanged: memory → hide → timer → platformer → land correct colour by `rowIndex` → clear time/rank/Stars.
- Every level completable WITHOUT skills; skills never bypass sequence validation.
- Max sequence length 10; max choices/row 6; failure = full-level restart.
- Only hazard = cactus-equivalent (touch during Playing → lose 1 life); never on the correct platform. No new mechanics.
- Six locked semantic colours; no 7th. Memory Cues preserved.
- Levels 1–10 stay as World 1 UNCHANGED (indices 0–9, same asset names) — never reorder/rename them.
- No currency/economy write, no new SDK/package, no ProjectSettings change without explicit approval.

## Method
1. Start read-only: `git status --short`, `git rev-parse HEAD`, confirm branch, UnitySkills connected, Unity not compiling.
2. Inspect current state before changing anything (cite file:line).
3. Keep edits inside the declared file scope; idempotent editor tools where the repo already uses them.
4. Add/extend tests; run EditMode + PlayMode; read `TestResults.xml` for the verdict.
5. Git integrity: only the declared scope changes; commit with a descriptive message.
6. Manual-QA note: agent cannot prove human-felt balance — mark balance PLAYTEST-HYPOTHESIS.
7. STOP at this phase's boundary; report a commit summary + next-phase gate.

## Scope (this phase only)
- **Files likely changed:** UI/World polish, tests
- **Out of scope:** anything not listed above; any other world/phase; ProjectSettings; packages.
- **Migration impact:** must preserve existing local progress (L1–10 indices/asset-names).
- **Art dependencies:** see doc 18 (manifest) / doc 20 (briefs) where art is involved.

## Success criteria / gate
- No stale art; smooth transitions; perf within budget
- Tests added/extended and green (EditMode + PlayMode via TestResults.xml).
- Git: only declared scope changed; descriptive commit.
- Rollback: revert this phase's commit; no cross-phase coupling.

## Next-phase gate
Do not start the next phase until this one's tests are green and committed, and any
balance values remain labelled PLAYTEST-HYPOTHESIS until P34T.
