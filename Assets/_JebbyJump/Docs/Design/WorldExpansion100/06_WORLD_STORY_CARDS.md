# 06 — World Story Cards

Status: `PROPOSED`. Lightweight "magical travel postcard" tone. Short, self-contained cards —
**no** serialised plot, **no** dialogue system. One card **before** each world (Gate #5) + an
opening card + a World-10 ending. First view is persistent; all replayable.

## Card fields (every card)

```
card_id            e.g. story.open, story.W01 … story.W10, story.ending
trigger            opening: first launch; per-world: on entering the world (before level 1);
                   ending: after clearing level 100
headline           short warm title
body               <= 30 words, child-safe, gentle
illustration       story_card_<worldslug>_01 (doc 18) / opening + ending bespoke
tower_stage        matches doc 05 stage for that world
skip_back          Skip + Back always available; never blocks play
first_view         shown once automatically (persist jebby.story.seen.<card_id>)
replay             re-viewable from World Map (world card → story)
localisation       body kept short for expansion; no baked text in art
accessibility      readable contrast; dismissible; no timed-only text
```

## Card set

| Card | Trigger | Headline | Body (≤30 words) |
|---|---|---|---|
| story.open | first launch | A Rainbow Calls | Far away, a Rainbow Tower shines. Jebby packs a tiny bag and hops into the sky to find it. Ready to jump? |
| story.W01 | enter W01 | Cloud Meadow | Soft clouds drift like pillows. Somewhere past them, a glimmer waits. Hop carefully — remember the colours! |
| story.W02 | enter W02 | Whispering Woods | The forest hums with light. The tower peeks through the leaves. Follow the glow. |
| story.W03 | enter W03 | Singing Crystals | Caves sparkle and chime. A bright shaft points the way down and onward. |
| story.W04 | enter W04 | Golden Dunes | Warm sand, friendly sun. Over the next dune, the tower rises taller. |
| story.W05 | enter W05 | Where Sea Floats Up | Reefs drift in the sky. The tower stands on a far floating island. |
| story.W06 | enter W06 | Candy Kingdom | Everything smells sweet! The tower glows candy-bright ahead. |
| story.W07 | enter W07 | Clockwork Heights | Gears turn and steam puffs. The tower ticks, so close now. |
| story.W08 | enter W08 | Moonlit Dreams | Stars twinkle softly. Under the moon, the tower shines near. |
| story.W09 | enter W09 | Emberpeaks | Warm embers glow. The tower stands bold against the firelit sky. |
| story.W10 | enter W10 | The Rainbow Tower | You made it to the tower itself! One last radiant climb to the top. |
| story.ending | clear L100 | Home of the Rainbow | Jebby reaches the top — the whole sky lights up! Every colour, every jump, brought you here. |

## Persistence & replay

- `jebby.story.seen.<card_id>` set on first auto-view (mirrors the Wardrobe acknowledgement store
  pattern; idempotent). Replay never sets/needs the flag.
- Ending card does not gate anything; replayable from World 10 card.

## Phase

Presenter + persistence built in **P34F**; hooks the World-Detail enter transition (doc 11).
