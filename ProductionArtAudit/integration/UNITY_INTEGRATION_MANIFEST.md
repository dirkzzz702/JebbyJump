# Unity Integration Manifest

Image generation ≠ integration. After extracting a validated art ZIP into the
repository root, the following Unity work remains, per asset. **DO NOT DELETE
OR REPLACE ANY EXISTING `.meta` FILE.** New files get fresh metas on import —
commit those metas with the wiring change.

## Store-only files (no Unity work at all)
`StoreAssets/**`, `Marketing/**` — never imported; used at Play Console /
press time. (ART-001, ART-004, ART-009, ART-016, wordmark SVG master.)

## Per-asset wiring
| Asset | After import | Verification |
|---|---|---|
| ART-002/003/015 icons | Import under `Assets/_JebbyJump/Art/Icons/` (Texture Type: Default, no sprite needed); assign in PlayerSettings > Android > Adaptive Icon fg/bg (+ monochrome where exposed); fill legacy icon slot from the 512 master | Build AAB; check launcher icon on device/emulator; release preflight still green |
| ART-005 floor | Sprite (2D), PPU 100, pivot Top-Centre, wrap Repeat; Game.unity `Floor` SpriteRenderer: sprite=new, drawMode=Tiled, size≈(37.6, height per decision D1), m_Color=white; delete `Art/Sprites/Placeholder.png`+meta afterwards | PlayMode scene-integrity + EditMode overlap tests; visual check at 16:9/20:9 |
| ART-006 menu bg | Sprite (2D UI); MainMenu.unity: new full-screen Image as FIRST sibling under MainMenuCanvas; envelope fit | MainMenu overlap regression tests; contrast check |
| ART-007 panel | Sprite; **Sprite Editor border 64/64/64/64**; swap Image sprite + type=Sliced + color=white on: LevelSelectPanel, SettingsPanel, WardrobePanel (MainMenu), PausePanel, SettingsPanel, LevelCompletePanel/Card, GameOverPanel/Card (Game), wardrobe rows/ceremony card, LevelSelectCard.prefab | overlap tests both scenes; wardrobe/shell PlayMode suites |
| ART-008 button | Sprite; **border 48/48/40/40**; swap Button target-graphic sprites across shell (menu stack 5, panel Back/Reset/Equip, result/pause buttons); keep tint transitions | ShellLayoutMetrics ≥90u tests; focus-visibility check |
| ART-010 wordmark | Sprite; MainMenu: Image (~700×233 units) replacing/above TitleText at the P33 slot (y=380); optional Unity Splash logo | MainMenu overlap tests; splash preview |
| ART-011/012 icons | Sprite; assign to `Btn_Skill2/Skill2Icon` and `Btn_Skill3/Skill3Icon` Image components in Game.unity | HUD visual check; ActiveSkillHUD alpha behaviour unchanged |
| ART-013/014 VFX | Sprite (PPU 100); assign to `BubbleShieldEffect` / `ColorEchoEffect` SpriteRenderers | activate skills in Play Mode; tint check across 6 colours |
| ART-017 cactus warn | GATED on design decision D4 + code change (anticipation swap) — do not wire before approval | new PlayMode test for the swap |

## Recommended import hygiene for THIS batch (approval-gated, separate commit)
The audit measured every existing UI/control sprite at 1254² with no platform
overrides (~1MB each). When touching these areas, consider maxTextureSize
overrides (Android 256/512) or a SpriteAtlas — build-size win; needs its own
approved pass (CLAUDE.md scope rule).

## Post-integration QA sequence
1. Assets/Refresh + compile clean.
2. `Jebby Jump/QA/Audit UI Overlaps` + `Audit Main Menu UI Overlaps` → 0.
3. EditMode + PlayMode suites green.
4. Play-mode visual pass (menu, wardrobe, gameplay, pause, result).
5. THEN capture ART-009 store screenshots.
