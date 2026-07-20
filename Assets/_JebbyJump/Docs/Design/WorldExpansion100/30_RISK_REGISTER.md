# 30 — Risk Register

Status: `PROPOSED`. Severity × likelihood; each with a mitigation phase.

| ID | Risk | Sev | Likelihood | Mitigation | Phase |
|---|---|---|---|---|---|
| R1 | Flat Level Select can't scale to 100 | High | Certain | Two-tier world→level nav (doc 11) | P34D |
| R2 | No per-level/world art layer exists | High | Certain | WorldDefinition + applier (doc 08/09) | P34B/P34C |
| R3 | Index-keyed unlock+stars break if L1–10 reordered/renamed | High | Low (if rule kept) | L1–10 unchanged (Gate #4); tests | P34A/P34B |
| R4 | `LevelBalanceAssetTests` hardcodes 10 | Med | Certain | data-driven test (doc 03 §6) | P34E |
| R5 | Themed platform/hazard art alters collider/width | High | Med | visual-swap only; collider-invariance validator | P34H/P34I+ |
| R6 | Hazard art mistakable for a safe platform | Med | Med | doc 22 silhouette check; distractor-only spawn | P34H+ |
| R7 | One-frame stale/fallback art on world switch | Med | Med | apply-before-render order (doc 08); PlayMode test | P34C |
| R8 | Camera-lock bg leaves margin at wide aspect ("faint grey" recurrence) | Med | Low | 1.5× over-fill verified 20:9 (doc 27) | P34U |
| R9 | Balance implausible at late worlds (seq 10 in gaps) | Med | Med | adjust bands + explain (doc 14); P34C measure | P34C/P34T |
| R10 | Memory budget unknown for 100-level art | Med | Med | measure baseline first (doc 28) | P34U |
| R11 | Reward accidentally becomes currency semantics | High | Low | model C markers only; no spend; tests | P34G |
| R12 | Save corruption / partial progress mishandled | High | Low | additive keys; defensive clamps exist; matrix tests | P34B |
| R13 | Art volume (170 assets) slips schedule | Med | Med | staged launch (Gate #8); per-world batches | P34I–P34R |
| R14 | Colour-blind readability drops with themed materials | Med | Med | Memory Cues kept; 70px hue-separation check | P34H/P34U |
| R15 | Scope creep into new mechanics | Med | Low | constraints restated in every phase prompt | all |

## Top-3 to watch

R2 (greenfield art layer), R3 (save safety — mitigated by Gate #4), R5 (collider invariance under
themed art). All three have explicit tests/validators before any world art ships.
