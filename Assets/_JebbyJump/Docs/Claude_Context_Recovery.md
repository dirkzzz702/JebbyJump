# Jebby Jump — Claude Context Recovery

Generated 2026-05-16. Use this to restore full project context after a repo move or new conversation.

---

## Approved Decisions

| Decision | Choice | Notes |
|---|---|---|
| Unity version | Unity 6 (6000.x) | Not 2022.3 LTS |
| Render pipeline | URP 2D | Use "2D (URP)" template in Unity Hub |
| Cinemachine | Cinemachine 3 (CM3) | Ships with Unity 6; use `CinemachineCamera`, not `CinemachineVirtualCamera` |
| Input | Unity Input System (new) | Not legacy Input Manager |
| UI | UGUI | Not UI Toolkit |
| Assembly definition | None yet | No asmdef until explicitly approved in a future phase |
| Mobile controls | Unity Input System `OnScreenStick` / `OnScreenButton` | Phase 9 — not yet |
| Platform type | Individual prefabs | Not Tilemap for Phase 1–5; Tilemap can be layered in for art later |
| Hot update | None | No Lua, HybridCLR, ILRuntime, networking, IAP, analytics, leaderboard |
| Config strategy | ScriptableObjects for local authoring | JSON-ready structs for remote config later; no hardcoded values in gameplay systems |
| Assets root | `Assets/_JebbyJump/` | Not `Assets/_Project/` or flat Assets root |
| Namespace | `JebbyJump.Core` (core types) | Follow per-module as systems are added |

---

## Phase Plan

| Phase | Scope | Status |
|---|---|---|
| **1** | Project foundation — folder structure, core enums | **Done** |
| **2** | Packages + Input System setup | **Done** |
| **3** | Rigidbody2D Player Controller | **Done** |
| **4** | Cinemachine + test scene | **Done** |
| **5** | Platform identity + landing detection | **Done** |
| **6** | Memory sequence system | **Done** |
| **7** | LevelConfig + platform row generation | **Done** |
| **8** | Lives / score / respawn / level complete | **Done** |
| **9** | Mobile virtual controls polish | **Done** |
| **10** | Cactus obstacle + item-ready player stats | **Done** |

---

## Completed Work — Phase 1

### Folder structure created

```
Assets/_JebbyJump/
  Art/
    Sprites/          (.gitkeep)
    Animations/       (.gitkeep)
  Audio/
    Music/            (.gitkeep)
    SFX/              (.gitkeep)
  Docs/
    MVP_Backlog.md
    Claude_Context_Recovery.md   ← this file
  Prefabs/
    Platforms/        (.gitkeep)
    Player/           (.gitkeep)
    UI/               (.gitkeep)
  Scenes/             (.gitkeep — scenes created manually in Unity Editor)
  Scripts/
    Core/
      AppState.cs
      GamePhase.cs
      PlatformColor.cs
      SceneNames.cs
  Settings/
    Input/            (.gitkeep — reserved for Phase 2)
```

### Scripts created

**`Scripts/Core/AppState.cs`**
```csharp
namespace JebbyJump.Core
{
    public enum AppState { Boot, MainMenu, Gameplay, Paused }
}
```

**`Scripts/Core/GamePhase.cs`**
```csharp
namespace JebbyJump.Core
{
    public enum GamePhase { Idle, Memorizing, Playing, LevelComplete, GameOver }
}
```

**`Scripts/Core/PlatformColor.cs`**
```csharp
namespace JebbyJump.Core
{
    public enum PlatformColor { None, Red, Blue, Green, Yellow, Purple, Orange }
}
```

**`Scripts/Core/SceneNames.cs`**
```csharp
namespace JebbyJump.Core
{
    public static class SceneNames
    {
        public const string Boot     = "Boot";
        public const string MainMenu = "MainMenu";
        public const string Game     = "Game";
    }
}
```

---

## Completed Work — Phase 2

### Scripts and assets created

```
Assets/_JebbyJump/Settings/Input/JebbyInputActions.inputactions   ← action map source
Assets/_JebbyJump/Settings/Input/JebbyInputActions.cs             ← pre-generated; Unity will overwrite on regeneration
Assets/_JebbyJump/Scripts/Input/InputReader.cs                    ← ScriptableObject event bridge
```

### Action map: Player

| Action | Type | Keyboard | Gamepad |
|---|---|---|---|
| Move | Value / Vector2 | A/D, Left/Right Arrow | Left Stick |
| Jump | Button | Space | South |
| UseItem | Button | Left Shift, J | Right Shoulder, West |
| Pause | Button | Escape | Start |

### InputReader API

- `public Vector2 Move { get; }` — readable every frame/FixedUpdate
- `public bool IsJumpHeld { get; }` — true between JumpStarted and JumpCanceled
- `public event Action JumpStartedEvent`
- `public event Action JumpCanceledEvent`
- `public event Action UseItemStartedEvent`
- `public event Action PauseEvent`
- Namespace: `JebbyJump.Inputs`
- Create asset: right-click `Settings/Input/` → Create → JebbyJump → InputReader

---

## Completed Work — Phase 4

### Cinemachine installed
`Packages/manifest.json`: `"com.unity.cinemachine": "3.1.3"`

### Game scene camera setup
- `Main Camera`: `CinemachineBrain` component added
- `CM Camera` at (0, 0, -10): `CinemachineCamera` (Follow = Jebby, Lens.OrthographicSize = 7) + `CinemachinePositionComposer` (TargetOffset = (0,1,0), Damping = (1,1,0))
- Scene saved; SetCMLens.cs temp Editor script deleted after use

---

## Completed Work — Phase 5

### Scripts created
- `Scripts/Platforms/Platform.cs` — MonoBehaviour; stores `PlatformColor` + `RowIndex`; applies sprite tint in `Awake` and `OnValidate` via `ApplyVisualColor()`
- `Scripts/Platforms/PlatformColorPalette.cs` — static helper; converts `PlatformColor` enum to `UnityEngine.Color`
- `Scripts/Player/PlayerLandingDetector.cs` — `[RequireComponent(Collider2D)]`; detects top-only landings via `OnCollisionEnter2D` (normal.y > 0.5); uses `GetComponentInParent<Platform>()`; fires `public event Action<Platform> LandedOnPlatform` and logs once per landing

### Scene setup (Game.unity)
- `PlayerLandingDetector` added to Jebby root GameObject
- Three test platforms on Ground layer (7), each with SpriteRenderer + BoxCollider2D + Platform:

| Name | Row | Color | Position |
|---|---|---|---|
| Platform_Row0_Red | 0 | Red (1) | (−2, 2, 0) |
| Platform_Row1_Blue | 1 | Blue (2) | (0, 5.5, 0) |
| Platform_Row2_Yellow | 2 | Yellow (4) | (2, 9, 0) |

### Jump height update
`DefaultMovementConfig.asset`: `_jumpForce` 12 → 10 (max apex ≈ 5.1u; comfortably reaches one row, cannot skip a row at 3.5u spacing)

---

## Completed Work — Phase 6

### Scripts created
- `Scripts/Sequence/ColorSequenceConfig.cs` — ScriptableObject: sequenceLength, memoryTimeSeconds, availableColors
- `Scripts/Sequence/ColorSequenceManager.cs` — Generates sequence, tracks CurrentStepIndex, exposes ExpectedColor + IsComplete, fires SequenceComplete event
- `Scripts/Sequence/MemoryPhaseController.cs` — Coroutine: ShowingSequence → Playing → Completed; subscribes to PlayerLandingDetector; logs expected/got per landing; Phase 6 advances on every landing (validation in Phase 8)
- `Scripts/Sequence/SequenceDisplayUI.cs` — Dynamically creates colored Image swatches in a HorizontalLayoutGroup; Show/Hide driven by MemoryPhaseController

### Assets created
- `Settings/Sequence/DefaultSequenceConfig.asset` — length=4, memoryTime=5s, colors=[Red, Blue, Yellow]

### Scene setup (Game.unity)
- `SequenceSystem` GameObject: ColorSequenceManager (config wired) + MemoryPhaseController (all references wired)
- `SequenceCanvas` (Screen Space Overlay): SequenceDisplayUI + SequencePanel (HorizontalLayoutGroup, anchored top-center)

---

## Pending Manual Unity Editor Steps

These have NOT been done yet and require the Unity Editor:

1. Create the Unity project in Unity Hub — **"2D (URP)" template, Unity 6** — targeting this repo root.
2. Delete the template's sample content (SampleScene, demo assets).
3. Create three empty scenes in `Assets/_JebbyJump/Scenes/`: `Boot`, `MainMenu`, `Game`. Each with only a Main Camera.
4. Add all three to **File → Build Settings**: Boot index 0, MainMenu index 1, Game index 2.
5. Unity auto-generates `.meta` files on first import — no action needed.

---

## Pending Planned Files (future phases)

| File | Phase | Purpose |
|---|---|---|
| `Settings/Input/JebbyInputActions.inputactions` | 2 | Input action map (Move, Jump, Pause) |
| `Scripts/Input/InputReader.cs` | 2 | ScriptableObject event bridge wrapping the generated InputActions class |
| `Scripts/Player/PlayerMovementConfig.cs` | 3 | ScriptableObject: speeds, jump force, coyote time, gravity multipliers, etc. |
| `Scripts/Player/PlayerMotor.cs` | 3 | All Rigidbody2D physics — no input reading |
| `Scripts/Player/PlayerController.cs` | 3 | Thin: subscribes to InputReader, calls PlayerMotor |
| `Scripts/Player/PlayerAnimator.cs` | 3 | Drives Animator parameters from motor state |
| Cinemachine vcam in Game scene | 4 | Follow/LookAt Jebby; no custom script |
| `Scripts/Platform/Platform.cs` | 5 | Holds PlatformColor, triggers landing events |
| `Scripts/Platform/LandingValidator.cs` | 5 | Checks landed color against current sequence step |
| `Scripts/Sequence/ColorSequenceConfig.cs` | 6 | ScriptableObject: sequence length, color pool |
| `Scripts/Sequence/ColorSequenceManager.cs` | 6 | Generates and tracks the active color sequence |
| `Scripts/Level/LevelConfig.cs` | 7 | ScriptableObject: platform row rules, difficulty |
| `Scripts/Level/PlatformSpawner.cs` | 7 | Spawns platform rows from LevelConfig |
| `Scripts/Core/GameManager.cs` | 8 | Lives, score, respawn, win/lose state — NOT before Phase 8 |
| Mobile UI canvas with OnScreenStick/Button | 9 | Sends same Input System actions as keyboard |
| `Scripts/Obstacle/Cactus.cs` | 10 | Basic hazard |
| `Scripts/Player/PlayerStats.cs` | 10 | Item-ready stat container |

---

## Constraints and Hard Rules

### Architecture
- One responsibility per script. Keep scripts small and reviewable.
- No god classes.
- Use serialized private fields (`[SerializeField]`), not public fields.
- ScriptableObjects for all configs. No hardcoded values in gameplay systems.
- Input abstraction: gameplay code never checks `Keyboard.current` directly — always goes through `InputReader`.

### Naming and spelling
- American English throughout: `Memorizing` not `Memorise`, `Color` not `Colour`.
- App-level state enum is `AppState` (not `GameState`).
- Assets root is `Assets/_JebbyJump/` (not `_Project`).
- `GameConfig` does NOT own movement/gravity — those belong in `PlayerMovementConfig`.

### Phase discipline
- Never add systems from a later phase into the current phase, even as stubs.
- Each phase is proposed, reviewed, and approved separately before implementation begins.
- After each phase: list every changed file, confirm nothing outside scope was added, explain remaining manual Unity Editor steps.

### Explicitly excluded (MVP)
- No Lua, HybridCLR, ILRuntime, or downloaded-code hot update.
- No networking, IAP, analytics, or online leaderboard.
- No shop UI until Phase 8+ core loop is complete.
- No external packages beyond Unity Input System, Cinemachine, TextMeshPro, and URP — unless explicitly approved.
- No asmdef until explicitly approved.
- Do not change global Physics2D gravity in Project Settings — gravity multipliers live in `PlayerMovementConfig` SO and are applied per-Rigidbody2D in code.

### Player movement (Phase 3 requirements, for reference)
Must implement via Rigidbody2D: horizontal acceleration, ground deceleration, air control, jump impulse, variable jump height (cut on release), coyote time, jump buffering, fall gravity multiplier, low-jump gravity multiplier, max fall speed, ground detection via `Physics2D.OverlapCircle`. No transform-only movement.

---

## MVP Core Loop (reference)

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
