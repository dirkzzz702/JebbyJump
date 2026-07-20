# 31 — Approval Gate

Decisions the user must make **before** runtime implementation begins. Each has a
`PROPOSED` recommendation; nothing here is implemented. Answering these unlocks the
downstream docs (04/12/13/18) and phase P34A.

| # | Decision | Recommendation (PROPOSED) | Why it gates |
|---|---|---|---|
| 1 | **World roster & order** | Prompt roster: 01 Cloud Meadow · 02 Enchanted Forest · 03 Crystal Caves · 04 Sunshine Desert · 05 Ocean Sky · 06 Candy Cloud Kingdom · 07 Clockwork Heights · 08 Moonlit Dreamscape · 09 Stormfire Volcano · 10 Rainbow Tower Castle | Creative bible, 100-level table, art manifest are all keyed to it — changing it later = full rework |
| 2 | **Reward model** | **C — combined**: finale first-clear → non-spendable *World Gem* trophy; world mastery → themed cosmetic. Stars never consumed | Determines save keys, UI, analytics (doc 07/10/11) |
| 3 | **Cosmetic unlock condition** | World mastery = all 10 levels cleared (≥ C rank). No Star spend, no perfection required | Wardrobe threshold + reward wiring |
| 4 | **Levels 1–10** | **World 1, unchanged** IDs/indices/asset names (only save-safe path) | Index-keyed unlock+stars break if reordered/renamed |
| 5 | **Story cards** | One **before** each world + opening + World-10 ending; first-view persistent, replayable | Presenter + persistence design |
| 6 | **Parallax** | **Out** of launch scope (single camera-locked bg/world) | Art asset count + perf budget |
| 7 | **Themed hazard prefab** | Allow a **distinct prefab per world** sharing one `CactusObstacle` behaviour component | Art pipeline + validation |
| 8 | **Launch shape** | Staged content releases (W1–W5, then W6–W10) *or* single 100-level launch | Roadmap sequencing |
| 9 | **Art-production order** | W00 shared templates → W01…W10 → UI batch → QA batch | Batch plan |
| 10 | **Human playtest & tuning authority** | Name who runs playtests & owns final balance sign-off (P34T) | All balance is PLAYTEST-HYPOTHESIS until then |

**Do not begin runtime implementation until 1–4 (at minimum) are approved.** Items 5–10 can be
confirmed in parallel but block their specific phases.
