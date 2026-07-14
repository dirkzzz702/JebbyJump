# Scene → Art Map (LIVE-INSPECTED 2026-07-14 review + STATIC YAML, commits 66eb4d0→355dba3)

## Boot.unity (LIVE-INSPECTED)
Main Camera + BootController only. **No art.** Instant transition to MainMenu
(RUNTIME-OBSERVED). No loading visual exists → acceptable (transition < 2s);
loading art = NOT_NEEDED for the slice.

## MainMenu.unity (LIVE-INSPECTED + RUNTIME-OBSERVED at 66eb4d0; layout fixed in P33 4240963)
| Element | Hierarchy | Art today | Status |
|---|---|---|---|
| Background | (camera clear colour only) | flat navy — NO background art | **GAP → ART-006** |
| Title | MainMenuCanvas/TitleText | plain TMP LiberationSans | **GAP → ART-010 wordmark** |
| 5 menu buttons | MainMenuCanvas/MenuSafeArea/* | Unity built-in UISprite, dark tint | **GAP → ART-008 button 9-slice** |
| Panels (LevelSelect/Settings/Wardrobe) | MainMenuCanvas/* | flat colour rects (scaffold colours) | **GAP → ART-007 panel 9-slice** |
| Level cards | runtime-instantiated `LevelSelectCard.prefab` | flat rects + TMP | covered by ART-007/008 kit |
| Wardrobe rows/preview | WardrobePanel | outfit idle sprites (prototype) + flat rects | sprites exist; chrome = kit |
| Ceremony overlay | UnlockCeremonyOverlay | flat rects + TMP | kit (post-launch polish acceptable) |

## Game.unity (LIVE-INSPECTED + RUNTIME-OBSERVED)
| Element | Hierarchy | Art today | Status |
|---|---|---|---|
| Sky | Background (SpriteRenderer) | `bg_sky_layer_01.png` | PRODUCTION_ACCEPTABLE |
| Floor | Floor (SpriteRenderer, tint #F2EBD9, scale 37.64×0.5) | **`Placeholder.png` 32×32** | **GAP → ART-005** |
| Platforms | PlatformSpawner instances | `spr_platform_base_01.png` runtime-tinted (PlatformColorPalette) | acceptable; polish optional |
| Cactus | spawner (distractor rows) | `spr_cactus_obstacle_01.png` (PPU 1254 anomaly) | acceptable; re-export note |
| Jebby | Jebby prefab | RookiePage set via base clips (355dba3) | prototype-acceptable |
| Memory swatches | SequenceCanvas/SequencePanel | `ui_icon_memory_base_01.png` tinted | acceptable |
| Hearts | HUDCanvas LivesIconContainer | `ui_icon_life_01.png` | acceptable |
| Move/Jump controls | MobileControlsCanvas | `ui_btn_left/right/jump.png` | GOOD (style anchor) |
| Skill 1 icon | Btn_Skill1/RocketBootsIcon | `spr_item_rocket_boots_01.png` | acceptable (oversized source) |
| Skill 2 icon | Btn_Skill2/Skill2Icon | **`ui_btn_bg.png` (generic circle)** | **GAP → ART-011 shield icon** |
| Skill 3 icon | Btn_Skill3/Skill3Icon | **`ui_btn_bg.png` (generic circle)** | **GAP → ART-012 potion icon** |
| Cooldown overlays | CooldownOverlay/Label | tinted fill + TMP | NOT_NEEDED (procedural) |
| Bubble shield VFX | BubbleShieldEffect | **`ui_btn_right.png` (an arrow sprite!)** | **GAP → ART-013** |
| Color echo VFX | ColorEchoEffect | `ui_btn_bg.png` tinted | **GAP → ART-014** |
| Pause/result/settings panels | GameShellCanvas | flat rects + TMP | kit (ART-007/008) |
| Pause button | MobileControlsCanvas/PauseButton | dark rect + "II" TMP | kit/icon (kit covers) |

## PlayerSettings (IMPORT-VERIFIED)
All icon slots empty (`m_Textures: []` for every kind/platform) → **GAP →
ART-001/002/003**. Splash: Unity default (Personal licence splash shows);
no custom splash logo → wordmark reuse (ART-010).
