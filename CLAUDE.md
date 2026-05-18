# Jebby Jump Development Guide

## Project

Jebby Jump is a production-ready Unity 2D memory platformer.

The player controls Jebby, a Cavalier King Charles Spaniel-inspired humanoid fantasy color knight, using real platformer controls. Jebby is not a literal dog.

The player must memorize a color sequence, then physically jump upward through colored platforms in the correct order.

## Development Standard

This is not a throwaway prototype. Build a production-quality MVP foundation from the beginning.

MVP means:

- Small feature scope
- Clean architecture
- Real movement
- Real physics
- Desktop and mobile controls
- Expandable systems

Do not build fake click-to-jump gameplay.

## Engine Direction

Use:

- Unity 2D
- Rigidbody2D physics
- Unity Input System
- Cinemachine 2D
- ScriptableObjects for configs
- UGUI / TMP for MVP UI
- Config-driven architecture for future live updates

## MVP Core Loop

1. Player starts a level.
2. Game shows a color sequence.
3. Player memorizes the sequence.
4. Sequence is hidden.
5. Player controls Jebby with keyboard or mobile controls.
6. Jebby jumps upward through colored platforms.
7. Landing on the correct color at the current expected row advances the sequence.
8. Landing on the wrong color at the current expected row costs a life.
9. Landing on a future row before the expected step costs a life.
10. Landing on a completed / lower row is ignored.
11. Completing the full sequence clears the level.
12. Losing all lives triggers game over.

## Required Input

Desktop:

- A / D or Left / Right = move
- Space = jump
- Shift or J = use equipped active skill
- Esc = pause

Mobile:

- Left virtual button
- Right virtual button
- Jump virtual button
- Active skill button later

Use one input abstraction so gameplay code does not care whether input comes from keyboard or mobile UI.

## Player Movement Requirements

Implement proper Rigidbody2D movement with:

- Horizontal acceleration
- Ground deceleration
- Air control
- Jump impulse
- Variable jump height
- Coyote time
- Jump buffering
- Fall gravity multiplier
- Low-jump gravity multiplier
- Max fall speed
- Ground detection
- Landing detection

Do not use fake transform-only jumping for the player controller.

## Architecture Rules

- Keep scripts small and reviewable.
- One responsibility per script.
- Use serialized private fields.
- Avoid god classes.
- Do not add networking.
- Do not add IAP.
- Do not add analytics.
- Do not add online leaderboard.
- Do not add shop UI until explicitly approved.
- Do not add inventory/equipment UI until explicitly approved.
- Do not add external packages beyond Unity Input System and Cinemachine unless approved.

## Live Update Strategy

For MVP, do not implement Lua, HybridCLR, ILRuntime, or downloaded-code hot update.

Use config-driven architecture.

Content that should be updateable later without an app release:

- Level configs
- Difficulty settings
- Equipment / active skill values later
- Obstacle spawn rules
- Shop offers later
- Tutorial text
- Localization text
- Addressable sprites/audio/backgrounds later

Prefer ScriptableObjects for local authoring and serializable DTO/JSON-ready structures for remote config later.

Do not hardcode level data inside gameplay systems.  
Do not hardcode equipment/skill values inside movement code.  
Do not hardcode color/platform layouts in PlayerMotor or a god manager.

## Design Documents

Current game design is defined in:

```text
Assets/_JebbyJump/Docs/Design/Jebby_Jump_GDD.md
```

Current product roadmap is defined in:

```text
Assets/_JebbyJump/Docs/Design/Jebby_Jump_Roadmap.md
```

Claude must follow these documents together with this `CLAUDE.md`.

Implementation priority:

1. `CLAUDE.md` defines working rules, constraints, and phase discipline.
2. `Jebby_Jump_GDD.md` defines gameplay design and system intent.
3. `Jebby_Jump_Roadmap.md` defines planned future scope and what is not part of MVP.
4. `Jebby_Jump_Art_Bible.md` defines visual direction and locked character references.

Claude must not implement future roadmap systems unless explicitly approved.

Future roadmap systems that are not part of current approved work include:

- Wardrobe / outfit fragments
- Shop
- Monetization / IAP
- Online leaderboard
- Cloud save
- Analytics
- Addressables remote content
- Seasonal events
- Code hot update
- Inventory / equipment UI
- Skill slot UI
- Save data / persistent unlocks unless approved

## Art Bible and Locked Visual References

Current art direction is defined in:

```text
Assets/_JebbyJump/Docs/Art/Jebby_Jump_Art_Bible.md
```

Locked visual references are stored in:

```text
Assets/_JebbyJump/Docs/Art/References/
```

Required reference files:

- `jebby_color_knight_character_sheet_v01.png`
- `jebby_outfit_variations_board_v01.png`

Jebby’s default character design is locked.

Default Jebby must remain:

- Jebby the Color Knight
- Classic Cavalier as default identity
- Cavalier-inspired humanoid fantasy color knight
- Not a literal dog
- Brown-and-ivory flowing ear-like hair
- Large warm eyes
- Small blue cape
- Little boots
- Rainbow gem badge
- Brave, gentle, magical, child-friendly
- Rainbow tower / color memory adventure theme

Claude must not redesign Jebby’s default appearance unless explicitly approved.

## Wardrobe / Outfit System Roadmap

The wardrobe system is approved as a future product direction, but it is not part of MVP.

Planned direction:

- MVP: Default Jebby only.
- V2: Basic outfit preview or 1–2 unlockable cosmetic outfits.
- V3: Full `Jebby’s Wardrobe` system with outfit fragments.

Outfits should be cosmetic first. Do not add gameplay advantages, wardrobe UI, skins, outfit fragments, cosmetic inventory, outfit economy, or outfit-related save data until explicitly approved.

## Platform Layout and Equipment / Active Skill Guardrails

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

Items should not be implemented as random scene pickups by default.

Long-term item model:

```text
Jebby can equip gear and active skills before a level.
Active skills may be placed into limited active skill slots, e.g. 3 slots.
Active skills may be one-use-per-level, limited charges, or cooldown-based.
```

Rocket Boots should be an equipped active skill / gear prototype, not a random scene pickup.

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

## First Production MVP Features

- Boot scene
- Main menu scene
- Game scene
- Player controller
- Camera follow
- Platform generation
- Color sequence system
- Memory countdown UI
- Landing validation
- Lives and score
- Game over / level complete
- Retry / menu flow
- 3 MVP levels
- HUD and result panels
- Basic audio feedback
- Basic tutorial / UX feedback
- Basic cactus obstacle
- Item-ready player stats architecture
- Same-row platform layout variation
- Rocket Boots equipped active skill prototype, when explicitly approved

## Claude Working Rules

- Work one approved phase at a time.
- Always propose a short implementation plan before editing files.
- Wait for approval before implementing a phase or making broad structural changes.
- Do not implement features outside the approved phase.
- Keep project-owned files under `Assets/_JebbyJump/` unless explicitly approved.
- Do not modify scenes, prefabs, ProjectSettings, generated files, packages, or build settings unless explicitly approved.
- Do not create a god `GameManager`.
- Do not add packages unless explicitly approved.
- Prefer small, reviewable changes over large multi-system changes.
- After each implementation, list every changed file and explain any Unity Editor manual steps.
- If a required Unity Editor step cannot be completed safely by code or MCP, document the exact manual step instead of guessing.

## Phase Discipline

Current approved phase order:

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
20. Rocket Boots equipped active skill prototype

Do not skip phases or pull work from later phases into the current one without approval.

Current next recommended phase:

```text
Phase 20: Rocket Boots Equipped Active Skill Prototype
```

Do not create RocketBootsPickup or scene pickup objects for Phase 20 unless explicitly approved.

## Coplay MCP / Unity Editor Automation Rules

Coplay MCP may be used only for controlled Unity Editor setup tasks.

Allowed MCP tasks:

- Create ScriptableObject assets.
- Create simple prefabs.
- Add Unity components.
- Wire serialized references.
- Create simple temporary test scene objects.
- Help validate scene and prefab setup.
- Create folders or assets under `Assets/_JebbyJump/`.

Not allowed without explicit approval:

- Change ProjectSettings broadly.
- Import packages.
- Modify generated InputActions code directly.
- Rewrite scenes or prefabs at large scale.
- Create gameplay architecture.
- Add monetization, analytics, networking, cloud save, online leaderboard, or IAP.
- Modify files outside `Assets/_JebbyJump/`, except clearly required Unity project files that were explicitly approved.

## Unity Editor Setup Safety Rules

- Prefer Unity Editor actions or generated Editor scripts over manual YAML editing for `.unity`, `.prefab`, `.asset`, and ProjectSettings files.
- Do not manually edit generated `JebbyInputActions.cs`; regenerate it from `JebbyInputActions.inputactions`.
- Do not manually edit scene or prefab YAML unless explicitly approved.
- Commit Unity `.meta` files.
- Do not commit `Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, `Logs/`, or `UserSettings/`.

## Code Review Expectations

After implementation, report:

1. Every file changed or created.
2. Any Unity Editor manual setup still required.
3. How to verify compilation.
4. How to manually test the implemented phase.
5. Any assumptions made.
6. Any known risks or TODOs.

For player movement code, ensure:

- Input is read through `InputReader`.
- Rigidbody2D velocity/forces are applied only from physics-safe code paths.
- `PlayerMotor` does not know gameplay rules, colors, score, lives, or UI.
- `PlayerController` stays as a thin input bridge.
- Missing serialized references fail clearly with validation/logging.

## Key Principle

Playable with real controls first.  
Polish second.  
Equipment and active skills later.  
Shop / inventory / wardrobe much later.

Memory tells the player where to go.  
Skill decides whether they can get there.  
Equipment and active skills help the player survive or recover, but must not bypass the memory sequence.
