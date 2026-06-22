# Jebby Jump — Level Balance Playtest Plan v0.1 (P28)

The **evidence-collection system** for the deferred P4B balance work, prepared for **all 10
levels**. P28 ran in **preparation only** mode: this plan + the data models + the manual
template are ready, but **no playtest evidence was collected** (see the Result doc;
manual balance playtest = NOT RUN). This document is deterministic — no dates, paths, run
IDs, or personal identities.

## Preparation assumptions (NOT final product decisions)

These shape the kit + the review candidates but are **assumptions pending confirmation**,
not approved policy:

- Difficulty curve: gentle-to-moderate family/casual (clear early success L1–3, rising
  memory + movement pressure by L10, never unfair).
- Rank meaning: S = clean/confident, A = minor hesitation, B = several mistakes/slow,
  C = completion below B.
- Lives/forgiveness: forgiving L1–3, mastery L4–7, fair-but-hard L8–10.
- Audience: mixed incl. children → Families sensitivity (avoid frustration spikes /
  unreadable memory speed / punishing early levels).

## Test plan (per band)

- L1 onboarding · L2–3 early learning · L4–6 mid ramp · L7–9 late ramp · L10 capstone.
- Plus every P26 candidate level (hazard/memory/curve focus — see the Review-Candidate Log).

Attempts (guidance): minimum 3 per level by the primary tester; 5 for L1–3 and every flagged
level. Rank proposals require **multiple** completion times, never a single run. Small
sample ⇒ confidence LOW.

## Tester profiles (category labels only — never personal names)

`developer` · `adult casual` · `new player` · `returning player` ·
`child (guardian-approved)`. Do not average incomparable tester types without labeling.

## Procedure

Before: reset to the agreed save state; record platform + input method + build commit;
disable unrelated debug overlays; confirm audio/input. Per level: start from Level Select
or the agreed route; watch the memory sequence normally; play without inspecting config
values; record each attempt immediately; don't restart early unless the tester naturally
would. Recommended: one full pass L1–10, then extra attempts on failed/flagged/rank-candidate
levels.

## Manual attempt template

Record one row per attempt (CSV header also emitted to `Builds/P28/<commit>/manual-playtest-template.csv`):

```
TesterProfile,Platform,InputMethod,BuildCommit,Level,Attempt,Completed,CompletionTimeSec,
Rank,LivesRemaining,Retries,MemoryMistakes,Falls,Difficulty1to5,Frustration1to5,Clarity1to5,Notes
```

Markdown form per attempt:

```
Tester profile:        (category label)
Platform / input:
Build commit:
Fresh/progressed save:
Level / Attempt:
Completed? / Time / Rank:
Lives remaining / Retries:
Memory mistakes / Falls:
Difficulty 1–5 / Frustration 1–5 / Clarity 1–5:
Notes:
```

## Child-tester safeguards

Guardian approval required; **no personal name stored** (use `child (guardian-approved)`);
short session length; observe frustration/fatigue; no pressure to continue. One adult test
never establishes Families/child readiness.

## Scope guardrails

Data-only and proposal-first. P28 changes no LevelConfig/TimeRankConfig and no protected
system (PlayerMotor/jump/skills/collision/camera/memory-validation/reward-semantics/
wardrobe-schema/analytics-names/input/quality-URP). Any future tuning is a separate,
explicitly-approved pass mapped to a review-candidate ID.
