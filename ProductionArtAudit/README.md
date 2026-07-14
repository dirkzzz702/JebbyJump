# ProductionArtAudit — Jebby Jump

Complete production art gap audit + pixel-exact ChatGPT generation handoff,
performed 2026-07-15 against local commit `355dba3` (read-only on the Unity
project; only this folder was created).

Start here:
1. `AUDIT_SUMMARY.md` — verdict, counts, blockers.
2. `ART_PRODUCTION_ROADMAP.md` — recommended batch order.
3. `decisions/OPEN_DECISIONS.md` — 7 decisions to make (2 gate generation).
4. To generate art: give a future ChatGPT session
   `generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` + ONE
   `generation/batches/NNN_*.md` + the referenced briefs + the uploads in
   `generation/REFERENCE_UPLOAD_INDEX.md` + `zip_blueprint/zip_manifest.json`
   + `tools/validate_generated_art.py` + `tools/build_final_art_zip.py`.
5. After receiving a ZIP: validate again locally, extract at repo root, then
   follow `integration/UNITY_INTEGRATION_MANIFEST.md`.

Folder map matches the audit spec (source_of_truth/ evidence/ manifests/
asset_briefs/ generation/ zip_blueprint/ integration/ qa/ tools/ decisions/).
Regeneration scripts for every manifest/brief live in `tools/` (single-source:
`build_missing_art_manifest.py`).
