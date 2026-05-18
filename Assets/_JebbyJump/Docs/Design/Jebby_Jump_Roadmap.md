# Jebby Jump｜Product Roadmap v0.3

## 1. Roadmap Purpose

This roadmap defines the planned development path for **Jebby Jump**.

It separates:

- MVP work
- Post-MVP polish
- Future systems
- Long-term product expansion

The goal is to avoid scope creep while preserving important future ideas such as items, obstacles, wardrobe, and live content.

---

## 2. Current Development Status

Completed:

```text
Phase 1:  Project foundation
Phase 2:  Packages + Input System setup
Phase 3:  Rigidbody2D Player Controller
Phase 4:  Cinemachine + test scene
Phase 5:  Platform identity + landing detection
Phase 6:  Memory sequence system
Phase 7:  LevelConfig + platform row generation
Phase 8:  Lives / score / respawn / level complete
Phase 9:  Mobile virtual controls polish
Phase 10: Cactus obstacle + item-ready player stats
Phase 11: In-game HUD + game over / level complete panels
Phase 12: Retry / restart level flow
Phase 13: Boot + Main Menu + scene flow foundation
Phase 14: Basic audio feedback (SFX)
Phase 15: MVP level set + session level progression
Phase 16: Playability tuning / MVP balance pass
Phase 17: MVP polish — visual readability + UX feedback
Phase 18: Basic tutorial / first-time onboarding
Phase 19: Advanced platform layout foundation (_rowVerticalJitter)
```

Current / Next:

```text
Phase 20: Rocket Boots Prototype
  Only after Phase 19 advanced platform layouts create a meaningful
  same-row mobility challenge.
```

---

## 3. MVP Development Phases

### Phase 1 — Project Foundation ✅

Goal:

Create the clean Unity project base.

Includes:

- Unity 6 LTS
- URP 2D
- `Assets/_JebbyJump/` structure
- Boot / MainMenu / Game scenes
- Core enums
- `CLAUDE.md`
- Basic docs

Done when:

- Project opens with no compile errors.
- Folder structure is clean.
- Scenes exist and are in build order.

---

### Phase 2 — Input System Setup ✅

Goal:

Create a unified input abstraction.

Includes:

- `JebbyInputActions.inputactions`
- Generated `JebbyInputActions.cs`
- `InputReader` ScriptableObject
- Move, Jump, UseItem, Pause actions

Done when:

- InputReader asset exists.
- Unity compiles.
- Gameplay code can read input without touching `Keyboard.current`.

---

### Phase 3 — Rigidbody2D Player Controller ✅

Goal:

Create production-quality 2D platformer movement.

Includes:

- `PlayerMovementConfig`
- `PlayerMotor`
- `PlayerController`
- `PlayerAnimator`
- Jebby prefab
- GroundCheck child
- Test floor
- Player/Ground layers

Done when:

- Jebby falls and lands.
- A/D or arrows move.
- Space jumps.
- Tap/hold jump height differs.
- No missing scripts or console errors.

---

### Phase 4 — Cinemachine + Test Scene ✅

Goal:

Add smooth camera follow.

Includes:

- Cinemachine package
- CinemachineBrain on Main Camera
- CinemachineCamera following Jebby
- Orthographic size 7
- Follow damping
- Slight upward target offset

Done when:

- Camera smoothly follows Jebby.
- Test scene remains playable.
- No custom camera script required.

---

### Phase 5 — Platform Identity + Landing Detection ✅

Goal:

Let the game know which platform Jebby landed on.

Includes:

- `Platform.cs`
- `PlatformColorPalette.cs`
- `PlayerLandingDetector.cs`
- Test colored platforms
- One-way platform collision for default colored platforms
- Landing event or debug log

Rules:

- Default colored test platforms should be one-way.
- Use `PlatformEffector2D` with `BoxCollider2D.usedByEffector = true`.
- Jebby should be able to jump through platforms from below and land on top.
- Detect landing from above only.
- Ignore side/head collisions.
- Avoid repeated spam while standing still.
- No memory sequence yet.
- No correctness validation yet.

Done when:

```text
Landing on top of a platform logs:
Row X, Color Y
```

---

### Phase 6 — Memory Sequence System ✅

Goal:

Create and display the color memory challenge.

Includes:

- `ColorSequenceGenerator`
- `MemoryPhaseController`
- Simple memory UI
- Countdown
- Hide sequence after memory phase

Rules:

- No platform row generation yet unless explicitly approved.
- No score/lives yet unless approved.
- No UI polish beyond functional display.

Done when:

- Game generates a color sequence.
- Player sees it for a configurable duration.
- Sequence hides.
- Game transitions to playing phase.

---

### Phase 7 — LevelConfig + Platform Row Generation ✅

Goal:

Generate playable platform rows from level data.

Includes:

- `LevelConfig` ScriptableObject
- Platform row generator
- Available colors
- Sequence length
- Row spacing
- Platform count per row
- Conservative reachable layouts

Rules:

- Each row must contain the required sequence color.
- Other platforms act as decoys.
- Do not generate impossible jumps.
- No complex procedural difficulty yet.

Done when:

- A level can generate rows based on a sequence.
- Jebby can physically climb through generated rows.

---

### Phase 8 — Lives / Score / Respawn / Level Complete ✅

Goal:

Complete the MVP gameplay loop.

Includes:

- Correct/wrong landing validation
- Lives
- Score
- Respawn
- Level complete
- Game over
- Basic result screens

Done when:

- Correct platform advances the sequence.
- Wrong platform costs a life.
- Lives reaching zero triggers game over.
- Finishing the sequence clears the level.

---

### Phase 9 — Mobile Virtual Controls Polish ✅

Goal:

Support mobile-friendly touch controls.

Includes:

- Left/right virtual buttons
- Jump button
- UseItem placeholder button
- Safe area consideration
- Same InputReader path

Rules:

- Do not duplicate movement logic.
- Mobile input must feed into same input system.

Done when:

- Player can control Jebby on mobile/touch simulation.
- Keyboard controls still work.

---

### Phase 10 — Cactus Obstacle + Item-ready Stats ✅

Goal:

Add first obstacle and prepare item architecture.

Includes:

- Cactus obstacle
- Damage/hazard handling
- Runtime stat modifier foundation
- Rocket Boots-ready stats
- Bullet Time-ready ability path

Rules:

- Cactus should be a separate child obstacle object with its own collider.
- Do not make cactus part of the platform's one-way collider.
- First cactus version may use a trigger hazard collider.
- Later versions may add left / center / right landing zones.
- No full shop yet.
- No monetization.
- No complex inventory.
- Keep item architecture minimal and extensible.

Done when:

- Cactus can harm or block Jebby.
- Player stats can be modified cleanly later.

---

### Phase 11 — In-game HUD + Game Over / Level Complete Panels ✅

Goal: Add lives display, score display, and result panels.

---

### Phase 12 — Retry / Restart Level Flow ✅

Goal: Retry button restarts current level with score and lives reset.

---

### Phase 13 — Boot + Main Menu + Scene Flow Foundation ✅

Goal: Boot scene → Main Menu → Game → back to Main Menu flow.

---

### Phase 14 — Basic Audio Feedback (SFX) ✅

Goal: Jump, land, correct, wrong, cactus, level complete, game over sounds.

---

### Phase 15 — MVP Level Set + Session Level Progression ✅

Goal: Three playable MVP levels with Next Level flow and MVP Complete state.

---

### Phase 16 — Playability Tuning / MVP Balance Pass ✅

Goal: Tune level configs, platform spacing, memory timing for fair feel.

---

### Phase 17 — MVP Polish — Visual Readability + UX Feedback ✅

Goal: Transient feedback text (Correct / Wrong / Go / Cactus), result panel layout fix.

---

### Phase 18 — Basic Tutorial / First-time Onboarding ✅

Goal: Session-only tutorial hints for Level 1 (memory phase, wrong color, cactus).

---

### Phase 19 — Advanced Platform Layout Foundation ✅

Goal: Add same-row vertical jitter as layout variation foundation.

Includes:

- `LevelConfig._rowVerticalJitter` (float, default 0, clamped 0–1)
- `PlatformSpawner` applies per-platform Random Y offset within a row
- Level 3 set to `_rowVerticalJitter = 0.3`

Rules:

- RowIndex is assigned from the row loop counter, not from Y position.
- Y position does not define the sequence step.
- All platforms in the same logical row share the same RowIndex.
- Cactus placement reads actual platform transform position — no changes needed.
- Row validation logic unchanged.

Done when:

- Level 1/2 remain flat (jitter = 0).
- Level 3 shows small same-row stagger.
- Row validation and cactus still work correctly.

---

### Phase 20 — Rocket Boots Prototype (next, pending Phase 19 review)

Goal: First item prototype — temporary same-row mobility boost.

**Prerequisite:** Phase 19 must create a meaningful same-row layout challenge before Rocket Boots has a clear purpose.

Rules:

- Rocket Boots must NOT be a row-skipping power-up.
- Rocket Boots must NOT bypass row validation.
- Rocket Boots may assist same-row mobility, wider gaps, cactus avoidance.
- Do not add shop, inventory, economy, or wardrobe.

---

## 4. MVP Release Target

MVP should include:

```text
Real movement
Memory sequence
Colored platforms
Correct/wrong validation
Lives
Score
Respawn
3 playable levels
Basic UI
Basic test visuals
```

MVP should not include:

```text
Wardrobe
Shop
IAP
Online leaderboard
Cloud save
Analytics
Full art production
Seasonal events
Remote content update
```

---

## 5. Post-MVP Roadmap

### V1.1 — Playability Polish

Focus:

- Better jump tuning
- Better camera feel
- Basic sound effects
- Correct/wrong feedback
- Small landing effects
- Simple background
- Better platform visuals

### V1.2 — MVP Art Pass

Focus:

- Default Jebby sprite
- Basic Jebby animation
- Colored platform sprites
- Simple sky background
- UI buttons
- Life/score icons
- Memory sequence icons

### V1.3 — Mobile Build Readiness

Focus:

- Android build
- Touch control polish
- Resolution/safe area checks
- Basic performance check
- Build profile setup

---

## 6. V2 Roadmap

### V2.0 — Obstacles

Add:

- Cactus
- Ice platform
- Cracked platform
- Cloud platform
- Moving platform

Obstacle + one-way platform compatibility:

Default colored platforms remain one-way. Obstacles such as cactus are separate child objects with their own hazard colliders.

First cactus version:

- Cactus is a trigger hazard.
- Touching cactus causes damage.
- Platform one-way behavior remains unchanged.

Later advanced version:

- Add platform landing zones: left, center, right.
- Obstacles can mark specific landing zones unsafe.
- Correct color plus safe zone succeeds.
- Correct color plus unsafe zone causes damage.

### V2.1 — First Items

**Prerequisite for Rocket Boots:** Advanced same-row platform layouts (Phase 19+) must create a meaningful horizontal mobility challenge before Rocket Boots is implemented.

Add:

- Rocket Boots — same-row mobility boost only, must NOT bypass row validation
- Bullet Time
- Bubble Shield
- Color Echo

Rules:

- Items help but do not replace memory.
- Keep item usage limited.
- Balance around fun, not monetization.

### V2.2 — Simple Rewards

Add:

- Coins or stars
- Level completion reward
- Simple local progress
- Best score / highest level

No IAP yet.

---

## 7. V3 Roadmap

### V3.0 — Jebby's Wardrobe

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
Item inventory
IAP
Analytics
Cloud save
Online leaderboard
Networking
Addressables
Lua / HybridCLR / ILRuntime
Seasonal events
Rocket Boots before Phase 19 creates a meaningful same-row mobility challenge
```

Each phase must be planned, reviewed, approved, implemented, verified, and committed before moving to the next phase.

---

## 11. Current Next Action

Next approved focus:

```text
Phase 20: Rocket Boots Prototype
```

Prerequisite:

```text
Phase 19 (Advanced Platform Layout Foundation) must create a meaningful
same-row mobility challenge before Rocket Boots is implemented.

Rocket Boots must NOT be designed as a row-skipping power-up.
Rocket Boots must NOT bypass row validation or allow sequence shortcuts.
```

Expected Phase 20 output:

```text
RocketBootsEffect.cs — temporary jump boost via PlayerStats
RocketBootsPickup.cs — trigger pickup in scene
LevelConfig integration if pickup spawning becomes config-driven
Effect expires after duration; cancels on life loss / retry / next level
GameFeedbackUI: "Rocket Boots!" on activation
No shop, inventory, save data, or economy
```
