# Paste-Ready Kickoff Prompt for the ChatGPT Art Session

Copy everything below the line into a fresh ChatGPT session (one batch per
session). Swap the batch number/asset IDs when moving to the next batch per
`GENERATION_BATCH_PLAN.md`.

---

You are producing FINAL production art for the Unity game **Jebby Jump**. A
completed audit has already specified everything — your job is precise
EXECUTION, not redesign. Where any instinct conflicts with the specs, the
specs win.

## Repository access
Public repo: `https://github.com/dirkzzz702/JebbyJump` (branch `main`,
commit `b0f790f` or later). The handoff package lives in `ProductionArtAudit/`.
Fetch files via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md`

## Step 1 — load your instructions (read ALL before generating anything)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` — the standing rules; they bind you.
2. `ProductionArtAudit/generation/batches/001_store_identity.md` — THIS session's batch.
3. The briefs it lists: `ProductionArtAudit/asset_briefs/ART-001.md`, `ART-002.md`, `ART-003.md`, `ART-015.md`.
4. `ProductionArtAudit/zip_blueprint/zip_manifest.json` — exact paths/sizes contract.
5. `ProductionArtAudit/tools/validate_generated_art.py` and `tools/build_final_art_zip.py` — you must actually run these (Python + Pillow).

## Step 2 — obtain the locked references (HARD GATE)
Required for this batch:
- `Assets/_JebbyJump/Docs/Art/References/jebby_color_knight_character_sheet_v01.png` — the LOCKED Jebby identity
- `Assets/_JebbyJump/Docs/Art/References/jebby_outfit_variations_board_v01.png`
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_sky_layer_01.png`

These are Git-LFS files: raw URLs redirect to `media.githubusercontent.com` —
follow the redirect and confirm you received a real PNG (not a small text
pointer file starting with `version https://git-lfs...`). If you cannot obtain
any required reference as a real image, STOP and ask me to upload it. You are
FORBIDDEN from generating character art without the identity sheet in hand.

## Step 3 — execute batch 001
Follow the batch file exactly: generate ART-001 first, derive ART-002's
subject from it, ART-003 is background-only, ART-015 is a DETERMINISTIC
derivation of ART-002 (no fresh generation). Use image generation for visuals
and Python/Pillow for every exact-canvas resize, alpha rule, safe-zone and
size check named in each brief's §20 — never eyeball them, never invent
dimensions, never rename files.

## Step 4 — validate, package, deliver
1. Place outputs at their exact repo-root-relative `archive_path`s.
2. Run: `python validate_generated_art.py --manifest zip_manifest.json --dir <outdir> --ids ART-001,ART-002,ART-003,ART-015`
3. Only on PASS: `python build_final_art_zip.py --manifest zip_manifest.json --dir <outdir> --out jebby_art_batch001.zip --ids ART-001,ART-002,ART-003,ART-015`
4. Return: the ZIP + `validation_report.md` + a per-asset table of each
   brief's §22 acceptance checkboxes (honestly filled) + anything that failed
   or was skipped and why.

## Non-negotiables (recap; the master prompt has the full list)
- Generate ONLY the four listed asset IDs. Exact filenames, paths, dimensions.
- Never include `.meta` or any Unity-native file in the ZIP.
- No text/lettering in any of these four assets. No checkerboard, no white
  halo, no watermarks.
- Jebby's identity is LOCKED to the character sheet: humanoid chibi knight
  (not a literal dog), warm brown eyes, brown/ivory ear-feather hair, blue
  cape, rainbow gem badge. No redesign, no style drift, no AI artefacts.
- Report the validator's REAL output; never claim pixel compliance unchecked.
- This ZIP is image delivery only — state clearly that Unity integration
  (icon slot wiring etc.) happens later in the project per
  `ProductionArtAudit/integration/`.

When batch 001 is delivered and accepted, STOP. Batch 002 runs in a new
session.
