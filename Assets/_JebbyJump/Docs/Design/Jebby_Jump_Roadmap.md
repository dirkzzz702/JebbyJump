# Jebby Jump｜Roadmap Update v0.3

## Purpose

This file updates the roadmap after the Rocket Boots design decision.

Copy these sections into:

```text
Assets/_JebbyJump/Docs/Design/Jebby_Jump_Roadmap.md
```

---

## Updated Current / Next

Replace the current next action with:

```md
## Current Next Action

Next approved focus:

```text
Phase 19: Advanced Platform Layout Foundation
```

Expected Phase 19 output:

```text
same-row platform layout variation
optional Y jitter within logical rows
safe reachability rules
LevelConfig layout parameters
no item system
no Rocket Boots yet
```
```

---

## Add Phase 19

```md
### Phase 19 — Advanced Platform Layout Foundation

Goal:

Create layout variation that makes movement choices more meaningful without breaking the memory-row rule.

Purpose:

```text
Make future mobility items, especially Rocket Boots, meaningful.
Create same-row reach challenges.
Preserve row-by-row sequence progression.
```

Possible additions:

```text
same-row staggered Y positions
larger horizontal gaps within a row
per-level layout difficulty parameters
safe reachability constraints
layout readability rules
```

Important rule:

```text
RowIndex defines the sequence step.
Y position does not define the sequence step.
```

Do not add Rocket Boots yet.

Do not add:

```text
shop
inventory
save data
wardrobe
new item economy
new obstacle types
level selection
persistent progression
```

Done when:

```text
A row can optionally contain platforms with small Y offsets.
Platforms in the same logical row still share the same RowIndex.
Jebby must still progress through rows in order.
No row-skip bypass is introduced.
Layouts remain reachable and fair.
```
```

---

## Move Rocket Boots to Later

Replace the old “First Items / Rocket Boots” roadmap ordering with:

```md
### Phase 20 — Rocket Boots Prototype

Goal:

Add Rocket Boots as a mobility assist for same-row layout challenges.

Design rule:

```text
Rocket Boots help with reach, recovery, and cactus avoidance.
Rocket Boots must not allow intentional sequence-row skipping.
Row > CurrentStepIndex remains wrong / lose life.
```

Possible effect:

```text
small jump assist
small horizontal / air-control assist
short duration
```

Do not add shop/inventory/economy in this phase.
```

---

## Scope Control Update

Add this guardrail:

```md
Specific item guardrail:

```text
Do not implement Rocket Boots or other items until the approved phase.
Do not design Rocket Boots as a row-skipping power-up.
```
```
