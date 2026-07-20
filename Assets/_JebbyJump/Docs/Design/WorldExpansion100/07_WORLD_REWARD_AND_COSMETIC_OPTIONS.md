# 07 — World Reward & Cosmetic Options

Status: **model C CONFIRMED** at Approval Gate. This doc records the comparison (as required) and
the confirmed design. **No reward is implemented in this planning task.** No currency, no Star
consumption, no wardrobe-threshold change.

## Models compared

### Option A — Non-spendable World Gems
One themed World Gem trophy for first completion of each world finale. Collection markers only.

### Option B — World cosmetic unlock
Each world's finale/mastery unlocks one themed cosmetic; Stars not consumed.

### Option C — Combined ✅ CONFIRMED
Finale first-clear → World Gem trophy; world **mastery** → themed cosmetic.

## Comparison

| Criterion | A | B | C (chosen) |
|---|---|---|---|
| Compatible with current Stars | yes | yes | yes (Stars untouched) |
| Wardrobe threshold impact | none | adds unlock | adds unlock (no threshold change) |
| Save migration | +1 gem key | reuse wardrobe unlock | +1 gem key + wardrobe unlock |
| UI complexity | low | low-med | med |
| Analytics | gem event | unlock event | both (reuse existing) |
| Player motivation | collection | dress-up | collection + dress-up (strongest) |
| Commercial | trophy only | cosmetic funnel | trophy + cosmetic funnel |
| Currency-semantics risk | none (marker) | none | none (marker + cosmetic) |

## Confirmed design (C)

- **World Gem trophy:** granted once on **first clear of the world finale** (level `n*10`).
  Non-spendable, never decreases, purely a collection/progress marker. Key
  `jebby.rewards.worldGem.<worldId>` (doc 10), idempotent.
- **Cosmetic unlock:** on **world mastery = all 10 levels of the world cleared** (≥ C rank, i.e.
  any completion — no perfection, no Star spend). Uses the **existing Wardrobe unlock service**;
  cosmetic ids are per-world (`08_..cosmetic`, doc 04). Wardrobe thresholds are **not** modified.
- **Stars:** remain a mastery record only; **never consumed** by rewards.

## Guardrails (must hold in P34G)

- No spendable currency / Rainbow Gems write without separate explicit approval (C12).
- Ending (World 10) reward not locked behind spending; replayable.
- Reward writes idempotent; double-clear/duplicate grants once (test, doc 25).

## Open sub-decision

- Exact cosmetic art per world is `PROPOSED` in doc 04 (e.g. Cloudpuff cape…Radiant heirloom set);
  final cosmetic identities confirmed during each world's art phase (P34I–P34R) + wardrobe review.
