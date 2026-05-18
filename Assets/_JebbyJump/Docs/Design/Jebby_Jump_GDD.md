# Jebby Jump｜GDD Update v0.3

## Purpose

This file updates the current GDD after the Rocket Boots design discussion.

Main decision:

```text
Rocket Boots should not be implemented before platform layouts create meaningful same-row mobility challenges.
Rocket Boots must not be a row-skipping power-up.
```

Copy these sections into:

```text
Assets/_JebbyJump/Docs/Design/Jebby_Jump_GDD.md
```

---

## Update 1 — Core Design Statement

Replace the current item sentence with:

```text
Memory tells the player where to go.
Skill decides whether they can get there.
Items help the player recover, reposition, or solve harder layouts.
Items must not bypass the memory sequence.
```

---

## Update 2 — Row Progression Rule

Add under Core Mechanics / Correctness Validation:

```md
### Row Progression Rule

Current gameplay rule:

```text
Row < CurrentStepIndex  → ignore
Row == CurrentStepIndex → validate color
Row > CurrentStepIndex  → wrong landing / lose life
```

This rule must be preserved unless explicitly redesigned.

Landing on completed/lower rows is ignored so falling down is not over-punished.

Landing on future rows is treated as a mistake because it skips the memory sequence order.
```

---

## Update 3 — Advanced Platform Layouts

Add under Platform System:

```md
### Future Advanced Platform Layouts

Future levels may support staggered platforms within the same logical row.

Example:

```text
RowIndex 3:
  Red platform:   y = 11.2
  Blue platform:  y = 11.7
  Green platform: y = 10.9
```

Important rule:

```text
RowIndex defines the sequence step.
Y position does not define the sequence step.
```

This enables same-row mobility challenges without allowing sequence-row skipping.

Possible future `LevelConfig` fields:

```text
allowStaggeredRows
rowVerticalJitter
maxPlatformYDifferenceWithinRow
minPlatformHorizontalGap
maxHorizontalGap
```

Do not implement advanced staggered layouts until explicitly approved.
```

---

## Update 4 — Rocket Boots Design Direction

Replace the current Rocket Boots item text with:

```md
### Rocket Boots Design Direction

Rocket Boots are **deferred until advanced platform layout variation exists**.

Rocket Boots are not intended as a row-skipping power-up.

Rocket Boots should eventually help with:

- Staggered platforms within the same RowIndex
- Larger horizontal gaps within the same row
- Reaching higher/farther same-step platforms
- Avoiding cactus side hazards
- Recovery from bad positioning

Rocket Boots should not allow intentional row skipping.

Validation remains:

```text
Row > CurrentStepIndex = wrong landing / lose life
```

Recommended future Rocket Boots behavior:

```text
small jump assist
horizontal / air-control assist
short duration
no row-skip bypass
```

Suggested future tuning direction:

```text
jumpMultiplier: about 1.10–1.20
moveSpeed or air-control assist: about 1.10–1.25
duration: about 5 seconds
```

Do not implement Rocket Boots before platform layouts include meaningful same-row mobility challenges.
```

---

## Update 5 — Current Phase Direction

Add or replace the current phase note with:

```md
## Current Phase Direction

The next recommended gameplay phase is:

```text
Phase 19: Advanced Platform Layout Foundation
```

Purpose:

```text
Add layout variation that creates meaningful same-row mobility challenges.
Do this before implementing Rocket Boots.
```

Rocket Boots should move to a later phase after staggered/gapped same-row platform layouts exist.
```
