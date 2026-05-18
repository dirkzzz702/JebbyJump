# Jebby Jump｜Product Roadmap v0.4

## 1. Roadmap Purpose

This roadmap defines the planned development path for **Jebby Jump**.

It separates:

- MVP work
- MVP polish
- Future systems
- Long-term product expansion

The goal is to avoid scope creep while preserving future ideas such as advanced platform layouts, obstacles, equipment, active skills, wardrobe, and content updates.

---

## 2. Current Development Status

Completed / implemented through the current repo direction:

```text
Phase 1: Project foundation
Phase 2: Packages + Input System setup
Phase 3: Rigidbody2D Player Controller
Phase 4: Cinemachine + test scene
Phase 5: Platform identity + landing detection
Phase 6: Memory sequence system
Phase 7: LevelConfig + platform row generation
Phase 8: Lives / score / respawn / level complete
Phase 9: Mobile virtual controls
Phase 10: Cactus obstacle + item-ready player stats
Phase 11: HUD + result panels
Phase 12: Retry / restart level flow
Phase 13: Boot / MainMenu / Game scene flow
Phase 14: Basic audio feedback
Phase 15: MVP level set + session progression
Phase 16: Playability tuning
Phase 17: Visual readability + UX feedback
Phase 18: Basic tutorial / onboarding
Phase 19: Advanced platform layout foundation
```

Current / Next:

```text
Phase 20: Rocket Boots Equipped Active Skill Prototype
```

Important design update:

```text
Items should be equipment or active skills, not random scene pickups by default.
Rocket Boots must not be a row-skipping power-up.
```

---

## 3. MVP Development Phases

### Phase 1 — Project Foundation ✅

Create the clean Unity project base.

### Phase 2 — Input System Setup ✅

Create unified input abstraction.

### Phase 3 — Rigidbody2D Player Controller ✅

Create production-quality 2D platformer movement.

### Phase 4 — Cinemachine + Test Scene ✅

Add smooth camera follow.

### Phase 5 — Platform Identity + Landing Detection ✅

Detect landing platform row/color and support one-way platforms.

### Phase 6 — Memory Sequence System ✅

Generate, display, hide, and progress through the sequence.

### Phase 7 — LevelConfig + Platform Row Generation ✅

Generate playable rows from level data.

### Phase 8 — Lives / Score / Respawn / Level Complete ✅

Complete the core gameplay loop.

### Phase 9 — Mobile Virtual Controls ✅

Support mobile-friendly left/right/jump controls.

### Phase 10 — Cactus Obstacle + Item-ready Stats ✅

Add cactus hazard and player stats foundation.

### Phase 11 — In-game HUD + Result Screens ✅

Add lives, score, game over, and level complete UI.

### Phase 12 — Retry / Restart Level Flow ✅

Add retry buttons and clean current-level restart.

### Phase 13 — Menu / Scene Flow Foundation ✅

Add Boot → MainMenu → Game flow.

### Phase 14 — Basic Audio Feedback ✅

Add event-driven SFX.

### Phase 15 — MVP Level Set + Session Progression ✅

Add Level 1–3, Next Level, and MVP Complete state.

### Phase 16 — Playability Tuning ✅

Tune MVP difficulty/readability.

### Phase 17 — Visual Readability + UX Feedback ✅

Add lightweight moment-to-moment UI feedback.

### Phase 18 — Basic Tutorial / Onboarding ✅

Add simple tutorial hints without save data.

### Phase 19 — Advanced Platform Layout Foundation ✅

Add optional same-row vertical jitter to create future same-row mobility challenges.

---

## 4. Next Approved Direction

### Phase 20 — Rocket Boots Equipped Active Skill Prototype

Goal:

Add Rocket Boots as the first character-equipped active skill prototype.

Purpose:

```text
Test the active-skill model without building inventory, shop, save data, or equipment UI.
```

Core rule:

```text
Rocket Boots help with same-row mobility, recovery, and cactus avoidance.
Rocket Boots must not allow intentional sequence-row skipping.
Row > CurrentStepIndex remains wrong / lose life.
```

Expected design:

```text
Rocket Boots equipped by default for prototype
Use Item input activates it
one use per level
short duration
small jump boost
small movement / air-control assist
no pickup object
no shop
no inventory
no save data
```

Done when:

```text
Player can activate Rocket Boots during Playing.
Boost is useful but controlled.
Effect cancels/resets on life loss, retry, and next level as designed.
Future-row landing while boosted still loses life.
No inventory/shop/save systems are added.
```

---

## 5. Later MVP / Post-MVP Phases

### Phase 21 — Basic Active Skill HUD / Charges

Only after Phase 20 works.

Possible additions:

```text
small active skill indicator
charge count
ready/unavailable feedback
mobile active skill button
```

No inventory or shop yet.

### Phase 22 — MVP Art Replacement Pass

Replace placeholder visuals with first-pass production-style assets:

```text
default Jebby sprite
platform sprites
cactus sprite
skill/equipment icons
simple background
app/menu visuals
```

Follow the Art Bible.

### Phase 23 — Basic Level Select / Local Progress

Only after the 3-level MVP flow is stable.

Possible additions:

```text
local current-session level select
optional local save unlocks
simple best score
```

No cloud save.

### Phase 24 — Basic Settings

Possible additions:

```text
sound on/off
music on/off if music exists
control visibility options
```

---

## 6. V2 Roadmap

### V2.0 — Advanced Obstacles and Landing Zones

Add:

- Landing zones: left / center / right
- Cactus can mark specific zones unsafe
- Ice platform
- Cracked platform
- Cloud platform
- Moving platform

Rules:

```text
Default colored platforms remain one-way.
Obstacles are separate hazard objects.
Correct color + safe zone succeeds.
Correct color + unsafe zone damages.
```

### V2.1 — Equipment and Active Skill System

Expand the Phase 20 prototype into a small real system.

Add:

```text
equipment slots
3 active skill slots
pre-level loadout
basic local unlock state later
```

Potential active skills:

- Rocket Boots
- Bullet Time
- Bubble Shield
- Color Echo

Rules:

```text
Equipment and active skills help but do not replace memory.
They must not bypass sequence validation.
They should support harder layouts, not trivialize them.
```

### V2.2 — Simple Rewards

Add:

- Coins or stars
- Level completion reward
- Simple local progress
- Best score / highest level

No IAP yet.

---

## 7. V3 Roadmap

### V3.0 — Jebby’s Wardrobe

Approved as future product direction.

Add:

- Wardrobe screen
- Outfit preview
- Outfit unlock state
- Fragment collection system
- Cosmetic-only outfits first

Initial outfit candidates:

- Forest Cavalier
- Sunshine Knight
- Aqua Knight
- Silver Dreamer

Rules:

- Default Jebby remains Classic Cavalier / Jebby the Color Knight.
- Outfits must preserve face identity, proportions, warm eyes, ear-feather silhouette, and rainbow gem motif.
- No wardrobe implementation before explicit approval.

### V3.1 — Endless Tower

Add:

- Endless climbing mode
- Procedural difficulty scaling
- Local high score
- Reward milestones
- Optional shop every few rows

### V3.2 — More Worlds

Add:

- Rainbow Garden
- Cloud Valley
- Cactus Desert
- Ice Mountain
- Star Space

---

## 8. V4 Roadmap

### V4.0 — Product Polish

Add:

- App icon
- Splash screen
- Store screenshots
- Trailer/key art
- Settings
- Audio settings
- Accessibility settings
- Better tutorial

### V4.1 — Content Update Foundation

Add later if needed:

- Addressables
- Remote content catalog
- Config manifest
- CDN / hosting
- Local fallback configs

No code hot update unless explicitly re-evaluated.

---

## 9. Long-term Ideas

Possible future directions:

- Daily challenge
- Weekly tower
- Seasonal outfits
- Limited event levels
- Parent/kid friendly mode
- Accessibility mode with color symbols
- Local challenge mode
- Story chapters
- Boss-like memory challenges
- Player-created level seeds

These are not part of MVP.

---

## 10. Scope Control Rules

Claude / AI must not implement future roadmap items unless explicitly approved.

Specifically do not add early:

```text
Wardrobe
Outfit fragments
Shop
Inventory / equipment UI
Skill slot UI
IAP
Analytics
Cloud save
Online leaderboard
Networking
Addressables
Lua / HybridCLR / ILRuntime
Seasonal events
```

Specific equipment / skill guardrail:

```text
Do not implement random scene pickups as the default item model.
Items should be equipment or active skills.
Do not design Rocket Boots as a row-skipping power-up.
```

Each phase must be planned, reviewed, approved, implemented, verified, and committed before moving to the next phase.

---

## 11. Current Next Action

Next approved focus:

```text
Phase 20: Rocket Boots Equipped Active Skill Prototype
```

Expected Phase 20 output:

```text
RocketBootsEffect on Jebby
basic active-skill activation through Use Item input
one use per level
controlled mobility boost
no pickup object
no inventory/shop/save systems
```
