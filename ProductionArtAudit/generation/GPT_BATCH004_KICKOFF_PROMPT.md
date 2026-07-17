# Paste-Ready Kickoff Prompt — Batch 004 (Main-Menu Background)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001–003 accepted & integrated). This
session executes **batch 004 only**: the main-menu background, ART-006.
Precise EXECUTION, not redesign; the specs win over instinct.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch
via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/ProductionArtAudit/asset_briefs/ART-006.md`

## Step 1 — load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` — binding rules.
2. `ProductionArtAudit/generation/batches/004_menu_background.md` — this batch.
3. `ProductionArtAudit/asset_briefs/ART-006.md` — full spec (canvas, palette, prompt, §20 post-processing).
4. `ProductionArtAudit/zip_blueprint/zip_manifest.json` + `ProductionArtAudit/tools/validate_generated_art.py` + `ProductionArtAudit/tools/build_final_art_zip.py`.

## Step 2 — references (HARD GATE)
Upload before generating (Git-LFS/binary: follow raw-URL redirects, confirm
real images, not pointer text; if unavailable STOP and ask me to upload):
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_sky_layer_01.png` — the
  gameplay sky. The menu background must read as the SAME world.
- `Assets/_JebbyJump/Docs/Art/References/jebby_color_knight_character_sheet_v01.png` — style/tone anchor.
- `ProductionArtAudit/evidence/screenshots/Game__PlayingHUD__1686x948__none__level1-timer-running.png` — in-game context.

## Step 3 — execute ART-006
- Output exactly `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png`,
  **2400×1080, fully opaque** (24-bit; the wide canvas lets 16:9–20:9 crop
  safely).
- Scene: soft pastel blue-pink sky continuing the uploaded gameplay sky's
  exact palette; fluffy clouds; a few small floating grassy islands near the
  left/right edges; a subtle distant tower of tiny colourful platform pills
  rising on the right horizon with a faint rainbow glint (the game's theme).
- The CENTRE band (middle ~40% width) must stay low-detail and slightly
  darker/calmer — the menu wordmark + five buttons sit there, and white UI
  text must remain readable over it (the validator checks centre-band
  luminance).
- NO characters. NO text. Nothing critical in the outer 10% (aspect crop
  zone).
- Use image generation for the look; deterministic Pillow for the exact
  2400×1080 canvas and alpha strip (brief §20). Never eyeball dimensions.

## Step 4 — validate + deliver
- `python ProductionArtAudit/tools/validate_generated_art.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --ids ART-006`
- On PASS: `python ProductionArtAudit/tools/build_final_art_zip.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --out jebby_art_batch004.zip --ids ART-006`
- Return: ZIP + validation_report.md + TWO crop previews (a centred 1920×1080
  crop and a centred 2400×1080 full view, each with a mock white "Jebby Jump"
  text block over the centre band so I can judge readability) + the brief §22
  acceptance checkboxes.

## Non-negotiables
Exact filename/path/size; fully opaque; same-world palette as the uploaded
sky; calm centre band; no characters/text/UI; no style drift; no
.meta/Unity-native files; report the validator's real output. Image delivery
only — Unity wiring (full-screen Image behind the title) happens later in the
project.

When delivered, STOP. Batch 005 (UI kit) runs in a new session.
