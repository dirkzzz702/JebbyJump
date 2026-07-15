# Paste-Ready Kickoff Prompt — Batch 002 (Wordmark)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batch 001 is accepted and integrated). This
session executes **batch 002 only**: the game wordmark, ART-010. Precise
EXECUTION, not redesign; the specs win over instinct.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch
via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/ProductionArtAudit/generation/batches/002_wordmark_brand.md`

## Step 1 — load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` — binding rules.
2. `ProductionArtAudit/generation/batches/002_wordmark_brand.md` — this batch.
3. `ProductionArtAudit/asset_briefs/ART-010.md` — the full spec (canvas,
   palette, prompt, negative prompt, §20 post-processing).
4. `ProductionArtAudit/zip_blueprint/zip_manifest.json` + `ProductionArtAudit/tools/validate_generated_art.py` + `tools/build_final_art_zip.py`.

## Step 2 — reference (HARD GATE)
Required upload: `Assets/_JebbyJump/Docs/Art/References/jebby_color_knight_character_sheet_v01.png`
(Git-LFS: follow the raw-URL redirect to media.githubusercontent.com and
confirm a real PNG, not an LFS pointer text file). If unavailable, STOP and
ask me to upload it.

## Step 3 — execute ART-010
Wordmark reading exactly **"Jebby Jump"** — plump rounded storybook
letterforms, warm ivory fill, soft gold outline, gentle top-light, exactly ONE
small rainbow-gem accent dotting a letter, transparent background, playful arc
<= 8 degrees, readable at 200px wide. ORIGINAL lettering only — do NOT trace
or reproduce any existing commercial font.

Deliverables (both files):
- `Assets/_JebbyJump/Art/Sprites/UI/ui_wordmark_jebbyjump.png` — exactly
  1200x400 RGBA (render from your master; trim to content + ~4% padding
  before fitting the canvas; deterministic Pillow resize, never eyeballed).
- `Marketing/Brand/jebby_jump_wordmark_master.svg` — a real vector master
  (true paths, not an embedded bitmap). If you cannot produce genuine vector
  lettering, say so explicitly and deliver the PNG only with the SVG marked
  FAILED — do not fake an SVG wrapper around a raster image.

Also render two PREVIEW composites (not part of the ZIP): the wordmark over
(a) a pastel sky swatch and (b) a dark #262633 panel swatch, so I can judge
contrast in both contexts.

## Step 4 — validate + deliver
- `python validate_generated_art.py --manifest zip_manifest.json --dir <out> --ids ART-010`
- On PASS: `python build_final_art_zip.py --manifest zip_manifest.json --dir <out> --out jebby_art_batch002.zip --ids ART-010`
- Return: ZIP + validation_report.md + the two previews + an honest note on
  how the lettering was constructed (so originality can be reviewed).

## Non-negotiables
Exact spelling/case "Jebby Jump"; no taglines; no more than one gem accent;
no baked shadow; no .meta/Unity-native files; no existing-font tracing; report
the validator's real output. This asset is a **CANDIDATE pending human
brand/legal review** — say so in your delivery message.

When delivered, STOP. Batch 003+ run in separate sessions.
