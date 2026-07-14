# Canonical Art Direction (verified from local Art Bible v0.2 + GDD, 2026-07-14)

## Identity (locked — Art Bible §2)
- **Jebby the Color Knight**: Cavalier-King-Charles-inspired **humanoid** fantasy
  knight; NOT a literal dog. Chibi proportions, warm large eyes, brown-and-ivory
  ear-feather hair, small blue cape, little boots, **rainbow gem badge**.
- Locked references (must be uploaded to any generator):
  - `Assets/_JebbyJump/Docs/Art/References/jebby_color_knight_character_sheet_v01.png` (1402×1122)
  - `Assets/_JebbyJump/Docs/Art/References/jebby_outfit_variations_board_v01.png` (1536×1024)
- Design Lock Rule: outfits may change cape colour/clothing/boots/accessories;
  never the face identity, chibi proportions, eyes, hair silhouette, rainbow gem
  motif, or friendly-brave personality.
- Since commit `355dba3` the DEFAULT outfit displays as "Rookie Page" and points
  at the RookiePage sprite set; there is no separate default art.

## Game visual identity (Art Bible §3)
Bright · magical · friendly · child-safe · warm/adventurous · colourful but not
messy · soft fantasy (not hard action) · **premium indie, not generic mobile**.

Forbidden: aggressive/gritty armour, scary fantasy, hyper-casual ad-like UI,
sharp cyber styling, dense text, uncontrolled AI visual noise.

## UI shape language
Rounded panels, pill-style buttons, soft shadows, light borders, restrained
sparkle accents. (RUNTIME-OBSERVED: the ornate gold/blue mobile control buttons
already meet this bar; the flat dark shell rectangles do not.)

## Locked / semantic colours (code-derived, exact)
| Role | Source | Value |
|---|---|---|
| Platform Red | `PlatformColorPalette.cs:10` | (0.90,0.22,0.22) = **#E63838** |
| Platform Blue | `:11` | (0.22,0.47,0.90) = **#3878E6** |
| Platform Green | `:12` | (0.22,0.75,0.35) = **#38BF59** |
| Platform Yellow | `:13` | (0.98,0.82,0.18) = **#FAD12E** |
| Platform Purple | `:14` | (0.60,0.22,0.90) = **#9938E6** |
| Platform Orange | `:15` | (0.95,0.55,0.15) = **#F28C26** |
| Rank S gold | `HUDController.cs:50` | #FFD61A |
| Rank A silver | `:53` | #D9D9E6 |
| Rank B bronze | `:56` | #CC8033 |
| Rank C gray | `:59` | #999999 |
| Shell panel dark | scaffolds | (0.15,0.15,0.20 @ 0.98) ≈ #262633 — placeholder neutral, replaceable |
| Shell button dark | scaffolds | (0.20,0.20,0.25) ≈ #333340 — placeholder neutral |
| Floor cream tint | Game.unity Floor SpriteRenderer | (0.95,0.92,0.85) = #F2EBD9 |

Platform colours are GAMEPLAY-SEMANTIC (memory correctness) — art must not
shift their hue perceptibly; accessibility numbers (Memory Cues, TMP digits)
are the non-colour companion cue and need no bitmap.

## Existing style anchors (FILE-MEASURED / RUNTIME-OBSERVED)
- `bg_sky_layer_01.png` (941×1672 opaque) — the pastel-sky look is the world
  style anchor.
- `ui_btn_left/right/jump/bg` (1254² RGBA) — ornate gold-rimmed blue-glass
  orbs; the control style anchor.
- 49 outfit sprites (1122×1402 RGBA, PPU 100, bottom-centre pivot, transparent
  corners PASS) — the character style anchor (prototype palette transfers).
