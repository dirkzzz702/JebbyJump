# Document Conflicts / Drift Affecting Art (2026-07-14)

1. **Wardrobe catalog counts** — Cosmetic Wardrobe Spec v0.1 and several QA
   docs describe an 8-outfit catalog with `rookie_page` at 4 Stars. Superseded
   2026-07-14 (commit `355dba3`, Roadmap note): catalog is 7; the default
   displays as "Rookie Page" and points at the RookiePage sprite set.
   → Art impact: no separate rookie_page outfit art is required; outfit board
   still lists Rookie Page as a variation (now the default's look).
2. **First Outfit Art Request Pack** frames Forest Cavalier as the only
   outfit being produced; all 7 variant sets were imported in P13B. The
   pack's IMPORT CONTRACT (PPU 100, bottom-centre pivot, 7 states, transparent
   corners) remains the live contract (IMPORT-VERIFIED against .meta files);
   its production framing is historical.
3. **Stale "portrait" lines** in `Jebby_Jump_Reward_UI_Visual_QA_Checklist_v0.1.md`
   (lines 58/141) and `Jebby_Jump_Manual_Test_Plan_v0.1.md` (line 43): the game
   is landscape-only (ProjectSettings locked). Screenshot/marketing specs in
   this audit are landscape.
4. **Art Bible §1 "Recommended repo location"** names
   `Jebby_Jump_Art_Bible_v0.2.md`; the actual file is `Jebby_Jump_Art_Bible.md`.
   Cosmetic only.
5. **MVP_Backlog / Claude_Context_Recovery** carry no historical banner but are
   superseded by the Roadmap. Ignored for scope.
6. **Store graphics checklist (P31)** predates the P29B "StoreAssets/ repo-root"
   plan. This audit follows the approved P29B convention: store-only graphics
   live under repo-root `StoreAssets/` (never imported by Unity); in-Unity
   launcher icons live under `Assets/_JebbyJump/Art/Icons/`.

No conflict blocks the audit; none required guessing.
