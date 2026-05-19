# CLAUDE.md Production Update v1.0

## Production Mode

Jebby Jump is now a production game, not just MVP.

Direction:

```text
Mobile-first
Desktop-supported
50-level launch target
Time ranking instead of score
Equipped skills
Cosmetic wardrobe
Rewarded ads + Rainbow Gems
Backend + analytics + ads
```

## Core Product Rules

### Platform Priority

```text
Mobile is primary.
Desktop is secondary.
```

Every UI and control decision must consider mobile first.

### Performance Metric

Score should be removed or de-emphasized.

Use:

```text
clear time
best time
time rank
```

Recommended:

```text
Timer starts when Playing phase begins after memory sequence.
```

Do not implement score-based progression unless explicitly approved.

### Monetization

Production direction:

```text
Free-to-play
Rewarded ads
Rainbow Gems
Cosmetics
Optional convenience purchases
```

Avoid:

```text
forced gameplay ads
hard stamina wall at launch
pay-to-win wardrobe
```

### Equipped Skills

Items should not be random scene pickups by default.

Two skill types:

```text
1. Equipment Skill
   - granted by gear
   - cooldown-based
   - reusable
   - example: Rocket Boots

2. Consumable Skill Item
   - equipped into skill slot
   - limited use / one-time use
   - cooldown protected
   - example: Health Potion
```

Start with:

```text
1 active skill slot
```

Expand later:

```text
3 active skill slots
```

Do not build full skill-slot UI until approved.

### Wardrobe

Wardrobe is cosmetic only.

Do not attach gameplay stats to outfits unless explicitly redesigned.

### Backend / Analytics / Ads

Production direction includes:

```text
backend
cloud/account capability
analytics
ads
```

Use service abstractions.

Do not add vendor SDKs without explicit approval.

Gameplay code must not directly depend on ads/analytics/backend SDKs.

## Current Production Priorities

1. Polish current game screen/UI to production quality.
2. Replace score with time ranking.
3. Build scalable 50-level structure.
4. Build production mobile UI shell.
5. Add skill/economy/wardrobe/backend in planned phases.

## Do Not Add Without Approval

```text
10+ levels before screen polish
shop
inventory
save data
backend SDK
ads SDK
analytics SDK
full wardrobe
full skill slot UI
new game mode
hard stamina system
forced ads
```

Each production feature must be planned and reviewed before implementation.
