# Launch Scope for Art (derived 2026-07-14, commit 355dba3)

## What "launch" means right now
The implemented, test-gated product is the **10-level vertical slice**
(Boot → MainMenu → Game; Level Select; Settings; Wardrobe with 7 outfits;
3 equipped skills; time ranking + Stars), targeted at **Android internal
testing first** (P23–P32 release pipeline; AAB debug-signed; Play upload
blocked on externals). The Roadmap's 50-level / 3-world / ads / gems /
endless-tower content is approved DIRECTION but not implemented — art for it
is NOT launch scope (CLAUDE.md: "Do Not Add Without Approval… 10+ levels
before screen polish").

## Scope classes used in this audit
- `LAUNCH_REQUIRED` — internal-testing/store submission of the current slice
  is blocked or visibly placeholder without it.
- `LAUNCH_RECOMMENDED` — needed to meet the Art Bible "premium indie" bar or
  obvious polish/accessibility for the current slice.
- `POST_LAUNCH` — real, documented, but after the slice ships to testers.
- `FUTURE_UNAPPROVED` — Roadmap ideas gated behind explicit approval.
- `NOT_NEEDED` — no artwork required (procedural/TMP/tint suffices).
- `OPEN_SCOPE_DECISION` — needs a product decision first.

## Classification of the major surfaces
| Surface | Class | Basis |
|---|---|---|
| App icon (Play 512) + Android adaptive fg/bg | LAUNCH_REQUIRED | Play listing mandatory; ProjectSettings icons empty (IMPORT-VERIFIED) |
| Feature graphic 1024×500 | LAUNCH_REQUIRED | Play listing mandatory (EXTERNAL-SPEC-VERIFIED) |
| Store screenshots (min 2, rec. 4+) | LAUNCH_REQUIRED | Play listing mandatory; must be real captures |
| Gameplay floor strip | LAUNCH_REQUIRED | Floor is literally `Placeholder.png` tinted (DEPENDENCY-VERIFIED + RUNTIME-OBSERVED) |
| Main-menu background | LAUNCH_REQUIRED | flat navy camera clear colour today; first screen players see |
| Shell UI kit (9-slice panel + pill button) | LAUNCH_REQUIRED | shell uses Unity built-in flat rects (scaffold colours), below Art Bible bar |
| Wordmark / logo | LAUNCH_REQUIRED | title is plain LiberationSans TMP; needed for store + splash + menu |
| Skill icons (Bubble Shield, Health Potion) | LAUNCH_RECOMMENDED | both currently reuse `ui_btn_bg.png` (DEPENDENCY-VERIFIED) |
| Shield/echo VFX sprites | LAUNCH_RECOMMENDED | shield effect renders `ui_btn_right.png` (an arrow!) |
| Adaptive monochrome icon | LAUNCH_RECOMMENDED | optional per Android docs; cheap alongside fg |
| Key art | LAUNCH_RECOMMENDED | store/press reuse; not blocking internal testing |
| Cactus warning/contact variant | LAUNCH_RECOMMENDED | single static cactus works; anticipation improves fairness read |
| Outfit final-art certification | LAUNCH_RECOMMENDED | prototypes ship to internal testing; certify before public |
| Parallax layers / world themes / new platform types / rank medals / ceremony art / trailer | POST_LAUNCH or FUTURE_UNAPPROVED | not in the slice |
| iOS icons/splash set | FUTURE_UNAPPROVED | Android-first pipeline; no iOS build tooling exercised |
| Memory-cue symbols | NOT_NEEDED | TMP digits already implement non-colour cues (P22) |
| Platform state variants (highlight/completed) | OPEN_SCOPE_DECISION | game logic has no such states today; adding them is a design change |
