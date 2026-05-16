# Jebby Jump Development Guide

## Project

Jebby Jump is a production-ready Unity 2D memory platformer.

The player controls Jebby, a cute Cavalier King Charles Spaniel dog, using real platformer controls. The player must memorize a color sequence, then physically jump upward through colored platforms in the correct order.

## Development Standard

This is not a throwaway prototype.

Build a production-quality MVP foundation from the beginning.

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
- UGUI for MVP UI
- Config-driven architecture for future live updates

## MVP Core Loop

1. Player starts a level.
2. Game shows a color sequence.
3. Player memorizes the sequence.
4. Sequence is hidden.
5. Player controls Jebby with keyboard or mobile controls.
6. Jebby jumps upward through colored platforms.
7. Landing on the correct color advances the sequence.
8. Landing on the wrong color costs a life.
9. Completing the full sequence clears the level.
10. Losing all lives triggers game over.

## Required Input

Desktop:
- A / D or Left / Right = move
- Space = jump
- Shift or J = use item later
- Esc = pause

Mobile:
- Left virtual button
- Right virtual button
- Jump virtual button
- Item virtual button later

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
- Do not add shop UI until the core loop is complete.
- Do not add external packages beyond Unity Input System and Cinemachine unless approved.

## Live Update Strategy

For MVP, do not implement Lua, HybridCLR, ILRuntime, or downloaded-code hot update.

Use config-driven architecture.

Core gameplay systems should be stable C# code shipped with the app.

Content that should be updateable later without an app release:
- Level configs
- Difficulty settings
- Item values
- Obstacle spawn rules
- Shop offers later
- Tutorial text
- Localization text
- Addressable sprites/audio/backgrounds later

Prefer ScriptableObjects for local authoring and serializable DTO/JSON-ready structures for remote config later.

Do not hardcode level data inside gameplay systems.
Do not hardcode item values inside item effects.
Do not hardcode color/platform layouts in PlayerMotor or LevelManager.

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
- 3 MVP levels
- Basic cactus obstacle
- Item-ready player stats architecture

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

Do not skip phases or pull work from later phases into the current one without approval.

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

For Phase 3:
- Implement C# scripts first without MCP.
- After code review, MCP may be used only to create/wire the Jebby prefab, movement config asset, GroundCheck child, Player/Ground layers, and temporary test floor.
- Stop after Phase 3 setup. Do not proceed to Phase 4.

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
Items and shop third.

Memory tells the player where to go.
Skill decides whether they can get there.
Items give clever ways to survive.