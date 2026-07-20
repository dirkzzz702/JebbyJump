# 15 — Finale Level Design (Level 10 of each world)

Status: `PROPOSED` / balance `PLAYTEST-HYPOTHESIS`. Finales feel special through **composition,
scale, art, route shape, and reward presentation — never a new mechanic** (C7/C9).

## Shared finale spec

- `finale_flag = true` (CSV col); world_level_number 10; global id `n*10`.
- Sequence/choices at the **top of the world's band** (see CSV).
- Cactus placement: distractor-only, slightly higher count than level 9 (still avoidable, never on
  correct platform).
- Tower landmark featured prominently (doc 05 stage for that world).
- On **first clear**: World Gem trophy (doc 07) + world-mastery cosmetic check + story transition
  toward the next world.
- Failure/retry: full-level restart (unchanged).

## Per-world finale set-piece (art/composition only)

| World | Finale name | Set-piece | Route silhouette | Tower treatment |
|---|---|---|---|---|
| W01 | Cloud Meadow Crown | grand cloud arch | gentle rising S | faint glimmer, framed |
| W02 | Enchanted Forest Crown | great glowing tree | weaving canopy climb | spire through boughs |
| W03 | Crystal Caves Crown | cathedral of crystal | branching shafts | gleam down a shaft |
| W04 | Sunshine Desert Crown | sunlit ruin arch | dune-crest ridge | beyond the dunes |
| W05 | Ocean Sky Crown | floating reef spire | rising bubbles path | far sky-island |
| W06 | Candy Cloud Crown | frosting palace gate | swirl ascent | candy-bright ahead |
| W07 | Clockwork Heights Crown | giant gear face | interlocking cogs climb | ticking just ahead |
| W08 | Moonlit Dreamscape Crown | moon bridge | star-strung arc | shining near |
| W09 | Stormfire Volcano Crown | ember summit | steep heroic climb | bold against firelit sky |
| W10 | Rainbow Tower Castle Crown | the tower's grand hall + summit | radiant final ascent | **the stage itself** |

## World 10 ending (special)

After clearing level 100:
- Ending story card `story.ending` (doc 06) — Jebby reaches the top, sky lights up.
- Celebrates completing all 100 levels; acknowledges time ranks + Stars **without requiring
  perfection**; not locked behind spending; **replayable**.
- No cinematic package required — a story card + finale art + celebratory VFX (existing systems).

## Tests (doc 25)

- exactly 10 finales; each is world_level_number 10.
- finale first-clear grants World Gem once (idempotent).
- finale cactus still distractor-only; no skill required.
