# 26 — Manual Playtest & Balance Plan

Status: `PROPOSED`. **No level is "balanced" without recorded human attempts** (P34T). Until then all
non-World-1 balance is `PLAYTEST-HYPOTHESIS`.

## Protocol (all 100 levels)

Capture per attempt:
```
tester_id · device · level_id · attempt_number · memory_mistakes · movement_mistakes ·
cactus_hits · skill_used · clear|fail · clear_time · perceived_difficulty(1-5) ·
frustration(1-5) · readability(1-5) · notes
```

## Acceptance bands (proposed targets — tune from data)

| Metric | Early worlds (1–3) | Mid (4–7) | Late (8–10) |
|---|---|---|---|
| completion rate (within a short session) | ≥ 90% | ≥ 75% | ≥ 55% |
| median attempts to first clear | 1–2 | 2–3 | 3–5 |
| clear duration | within band ±20% | within band ±20% | within band ±20% |
| S/A/B/C distribution | broad, S reachable | S reachable with skill | S hard but fair |
| rage-quit risk | very low | low | moderate-controlled |
| colour readability | ≥ 4/5 | ≥ 4/5 | ≥ 4/5 |
| mobile control comfort | ≥ 4/5 | ≥ 4/5 | ≥ 4/5 |

## Method

- Test on real devices (doc 27 matrix), touch controls, not just editor.
- Verify **every level clearable without skills** (hard requirement) — flag any level that seems to
  need a skill as a **blocker**, not a tuning note.
- Feed results back into `13_100_LEVEL_MASTER_TABLE.csv` → regenerate assets (doc 23); record each
  change + reason in doc 14 changelog.

## Authority

Final balance sign-off held by the person named at Approval Gate #10 (human playtest owner). Only
P34T promotes a band out of `PLAYTEST-HYPOTHESIS`.
