# Paste-Ready Kickoff Prompt — Batch 003 (Gameplay Floor Ledge)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001–002 accepted & integrated). This
session executes **batch 003 only**: the gameplay floor, ART-005. Precise
EXECUTION, not redesign; the specs win over instinct.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch
via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/ProductionArtAudit/asset_briefs/ART-005.md`

## Step 1 — load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` — binding rules.
2. `ProductionArtAudit/generation/batches/003_gameplay_floor.md` — this batch.
3. `ProductionArtAudit/asset_briefs/ART-005.md` — full spec (canvas, palette, prompt, §20 post-processing).
4. `ProductionArtAudit/zip_blueprint/zip_manifest.json` + `ProductionArtAudit/tools/validate_generated_art.py` + `ProductionArtAudit/tools/build_final_art_zip.py`.

## Step 2 — references (HARD GATE)
Upload before generating:
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_sky_layer_01.png` — the world sky palette anchor.
- `ProductionArtAudit/evidence/screenshots/Game__PlayingHUD__1686x948__none__level1-timer-running.png` — in-game context (the current cream floor band Jebby stands on).
(These are Git-LFS/binary: follow raw-URL redirects and confirm real images, not pointer text. If unavailable, STOP and ask me to upload.)

## Step 3 — the decision is made
Design decision **D1 is RESOLVED: taller cloud-meadow LEDGE** (not the thin
strip). Generate exactly to the brief: a soft storybook ground ledge, warm
cream (#F2EBD9 family) top surface with tiny pastel grass tufts/pebbles and a
solid warm body below, muted saturation so the six platform colours pop above
it.

## Step 4 — execute ART-005
- Output exactly `Assets/_JebbyJump/Art/Sprites/Platforms/spr_floor_strip_01.png`,
  **512×128 RGBA**.
- It MUST tile seamlessly on the X axis (left and right edge columns
  near-identical — the validator enforces mean diff < 2/255).
- Transparent only above the ledge silhouette; solid warm fill below.
- Use image generation for the look; use deterministic Pillow for the exact
  512×128 canvas, the seam check, and top-corner transparency (brief §20).
  Never eyeball dimensions or the seam.

## Step 5 — validate + deliver
- `python ProductionArtAudit/tools/validate_generated_art.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --ids ART-005`
- On PASS: `python ProductionArtAudit/tools/build_final_art_zip.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --out jebby_art_batch003.zip --ids ART-005`
- Return: ZIP + validation_report.md + a tiled preview (paste the sprite 3× side
  by side so I can eyeball the seam) + the brief §22 acceptance checkboxes.

## Non-negotiables
Exact filename/path/size; RGBA; seamless X tile; no characters/objects; no
saturated colours that clash with platform hues; no visible seam; no hard black
outline; no .meta/Unity-native files; report the validator's real output. This
ZIP is image delivery only — Unity floor wiring (assign sprite, drawMode Tiled,
reset tint, delete the old Placeholder.png) happens later in the project.

When delivered, STOP. Batch 004 (menu background) runs in a new session.
