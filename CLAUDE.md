# CLAUDE.md Update — Platform Layout + Rocket Boots Guardrails

## Purpose

Update `CLAUDE.md` so Claude does not implement Rocket Boots too early and does not treat it as a row-skipping power-up.

Copy this section into:

```text
CLAUDE.md
```

Recommended placement:

```text
After Wardrobe / Outfit System Roadmap
Before First Production MVP Features
```

---

## Platform Layout and Item Guardrails

Current row progression rule:

```text
Row < CurrentStepIndex  → ignore
Row == CurrentStepIndex → validate color
Row > CurrentStepIndex  → wrong landing / lose life
```

Do not undo this rule unless explicitly approved.

Future advanced layouts may support staggered platform Y positions within the same logical row.

Important rule:

```text
RowIndex defines the sequence step.
Y position does not define the sequence step.
```

Do not implement Rocket Boots yet.

Rocket Boots must not be designed as a row-skipping power-up.

Rocket Boots may only assist:

- same-row mobility
- recovery from poor positioning
- reaching wider horizontal gaps
- reaching slightly staggered same-row platforms
- cactus avoidance

Rocket Boots must not:

- bypass row validation
- allow intentional sequence-row skipping
- ignore wrong-color validation
- function as a shortcut around the memory sequence

Before implementing Rocket Boots, platform layouts should include meaningful same-row mobility challenges, such as staggered rows or larger same-row gaps.

---

## Phase Discipline Update

Replace / extend the current approved phase order with:

```text
1. Project foundation
2. Packages + Input System setup
3. Rigidbody2D Player Controller
4. Cinemachine + test scene
5. Platform identity + landing detection
6. Memory sequence system
7. LevelConfig + platform row generation
8. Lives / score / respawn / level complete
9. Mobile virtual controls polish
10. Cactus obstacle + item-ready player stats
11. HUD + result panels
12. Retry / restart level flow
13. Menu / scene flow foundation
14. Basic audio feedback
15. MVP level set + session progression
16. Playability tuning
17. Visual readability + UX feedback
18. Basic tutorial / onboarding
19. Advanced platform layout foundation
20. Rocket Boots prototype, only after Phase 19 creates a meaningful need
```

Current next recommended phase:

```text
Phase 19: Advanced Platform Layout Foundation
```

Do not proceed to Rocket Boots until Phase 19 is complete and approved.
