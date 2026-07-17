# Paste-Ready Kickoff Prompt — Batch 007 (Key Art + Feature Graphic)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001–006 accepted & integrated). This
session executes **batch 007 only**: ART-016 (marketing key art) and ART-004
(Google Play feature graphic, derived from the key art). Generate the key art
FIRST; the feature graphic is a composition/crop from it — dependent outputs,
one hero-scene style. Precise EXECUTION, not redesign.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch
via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/ProductionArtAudit/asset_briefs/ART-016.md`

## Step 1 — load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` — binding rules.
2. `ProductionArtAudit/generation/batches/007_feature_keyart.md` — this batch.
3. `ProductionArtAudit/asset_briefs/ART-016.md` + `ART-004.md` — full specs.
4. `ProductionArtAudit/zip_blueprint/zip_manifest.json` + `ProductionArtAudit/tools/validate_generated_art.py` + `ProductionArtAudit/tools/build_final_art_zip.py`.

## Step 2 — references (HARD GATE)
Upload before generating (Git-LFS: follow raw-URL redirects, confirm real
PNGs, not pointer text; if unavailable STOP and ask me to upload):
- `Assets/_JebbyJump/Docs/Art/References/jebby_color_knight_character_sheet_v01.png` — LOCKED identity (mandatory).
- `Assets/_JebbyJump/Docs/Art/References/jebby_outfit_variations_board_v01.png` — palette board.
- `ProductionArtAudit/evidence/screenshots/Game__PlayingHUD__1686x948__none__level1-timer-running.png` — in-game world context.

## Step 3 — ART-016 key art
- Output exactly `Marketing/KeyArt/jebby_jump_keyart_3840x2160.png`,
  **3840×2160, fully opaque**.
- Scene: Jebby (chibi humanoid knight per the LOCKED sheet — warm brown eyes,
  brown/ivory ear-feather hair, blue cape, rainbow gem badge; NO dog anatomy)
  mid-leap in his jump pose, arms up, joyful, on a rising spiral of rounded
  platform pills coloured EXACTLY #E63838, #3878E6, #38BF59, #FAD12E,
  #9938E6, #F28C26, ascending toward a distant glowing rainbow-gem tower
  among pastel clouds; warm light from upper right; restrained sparkles.
- Jebby left-of-centre at ~35% width, face clearly visible and identity-exact.
- Soft storybook fantasy, premium indie quality. NO text anywhere.

## Step 4 — ART-004 feature graphic (derived)
- Output exactly `StoreAssets/Feature/jebby_jump_feature_1024x500.png`,
  **1024×500, 24-bit, NO alpha**.
- Derive from the key art: recompose/crop so Jebby occupies the left-centre
  third with his face intact, colourful platform trail rising right, and the
  RIGHT third calmer (reserved for a future logo overlay). Deterministic
  Pillow for the exact canvas + alpha strip.
- Ship TEXT-FREE. (The game's wordmark exists but is still pending brand/legal
  sign-off for store use — do not composite it unless I explicitly say
  "wordmark approved" and upload it.)
- Nothing critical in the outer 10% (Play may edge-crop).

## Step 5 — validate + deliver
- `python ProductionArtAudit/tools/validate_generated_art.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --ids ART-016,ART-004`
- On PASS: `python ProductionArtAudit/tools/build_final_art_zip.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --out jebby_art_batch007.zip --ids ART-016,ART-004`
- Return: ZIP + validation_report.md + a 30%-zoom preview of the feature
  graphic (Play renders it small — I judge legibility there) + the briefs'
  §22 acceptance checkboxes.

## Non-negotiables
Exact filenames/paths/sizes; key art opaque 3840×2160; feature graphic
1024×500 with NO alpha; identity LOCKED to the character sheet; platform
colours exact; no text/UI/fake screenshots; no other characters; no AI
artefacts (extra limbs, melted shapes, wrong anatomy); no .meta/Unity-native
files; report the validator's real output. These are store/marketing files —
no Unity wiring at all.

When delivered, STOP. Batch 008 (store screenshots) is a CAPTURE task, not a
generation session.
