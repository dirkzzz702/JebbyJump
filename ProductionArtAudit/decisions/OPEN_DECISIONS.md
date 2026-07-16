# Open Decisions

| ID | Decision | Options | Default proposal | Depends |
|---|---|---|---|---|
| ~~D1~~ RESOLVED 2026-07-16 | Floor treatment | (a) thin strip; (b) taller cloud-meadow ledge 512×128, drawMode Tiled | **DECIDED: (b) taller ledge** (product owner) | ART-005 unblocked |
| D2 | UI corner-radius system | (a) 48px@256 soft; (b) 32px sharper; (c) full pill everywhere | (a) panels + true-pill buttons (briefs carry candidates) | ART-007/008 |
| D3 | Licence/provenance register | create `Docs/Release/Jebby_Jump_Art_Provenance_Register_v0.1.md` recording source/generator/rights for every shipped asset (incl. the 49 outfit sprites + 11 UI/world sprites + reference boards) | do it before public release; internal testing OK meanwhile | commercial acceptance |
| D4 | Cactus anticipation | (a) none (ship as-is); (b) pre-contact warn swap (needs small code) | (a) for internal testing; revisit with playtest data | ART-017 |
| D5 | Tablet screenshots | produce 7"/10" sets (min 4 each) for Play promotion eligibility now vs later | later (internal testing first) | ART-009 scope |
| D6 | Localisation of store assets | EN-only vs localised screenshots/graphics | EN-only for internal testing | store batch |
| D7 | Oversized-source re-export pass (1254² UI sprites ~1MB each, cactus PPU 1254 anomaly) | importer maxSize overrides vs re-exported assets vs SpriteAtlas | maxSize overrides (no art regeneration needed) — separate approved commit | build size |
