# Jebby Jump — Play Declaration Record v0.1 (P27)

**WORKSHEETS / DRAFTS — none of these are submitted Play Console results.** Every section
below is an input to be reviewed and explicitly approved by the operator before it is
entered into the Console. No timestamps, account IDs, tester emails, or secrets appear in
this committed document.

## Identity (approved)

- Developer/brand: SparkLibrary
- Application id: com.sparklibrary.jebbyjump
- Version name: 1.0 — **version code: 1** (first internal upload). Any later upload
  **must** use a strictly greater, explicitly approved code (`VersionCodePolicy`).

## Target audience & Families (DECIDED)

Mixed audience — **children + older users → Google Play Families** applies to the child
portion (certified ad SDKs only, no personalized ads to children, COPPA/GDPR-K, parental
considerations). The app currently ships **no ad SDK / no IAP / no backend**; any future
addition must re-open these declarations.

## Data Safety — DRAFT (status: Draft, not finalizable yet)

Working assumption: **no data collected, no data shared.** This stays a DRAFT until (1)
the generated artifact's permissions are audited, (2) transitive SDK behavior is reviewed,
(3) the Console form is reviewed, and (4) the operator approves. `DataSafetyCanFinalize`
returns false until audit + approval. Do **not** submit a "no data" answer merely because
no first-party analytics code is called.

## Content rating (IARC) — WORKSHEET (rating NOT predicted)

Answer each in the Console questionnaire; this worksheet does not guess the result:

- violence · fear/horror · sexual content · language · drugs/alcohol/tobacco · gambling ·
  user interaction · user-generated content · location sharing · purchases · ads

## Privacy policy & contact — REQUIRED (status: Blocked)

- Privacy-policy URL: **MISSING** — required (child audience). Must be a public `https://`
  URL that loads without auth, matches the developer, and describes actual behavior.
- Support/contact email, developer display name, website: **to be provided** (not invented
  here; not committed).

## Regions / pricing / monetization — DECISION REQUIRED

- Countries/regions: to decide.
- Free or paid: to decide (assumed free for internal testing).
- Contains ads: **No** (no ad SDK wired) — declare actual runtime behavior.
- Contains IAP: **No** (no billing wired) — IAP package may be present but is unused.

## Store listing — DRAFT (operator approves final wording)

- Title (≤30): `Jebby Jump`
- Short description (≤80): `Memory platformer — watch the color sequence, then hop the matching tiles.`
- Full description (≤4000): `Jebby Jump is a colorful memory platformer. Each level shows
  a short color sequence; remember it, then jump across the matching platforms before the
  clock runs out. Chase faster clear times and time ranks, and dress Jebby up with
  cosmetic outfits. Designed mobile-first for landscape play.`
- Category: Games → (Arcade or Puzzle — choose). Tags: memory, platformer, casual.
- Release notes (internal): `Internal testing build — core memory/platforming loop,
  10 levels, time ranks, cosmetic wardrobe. Prototype art; balance not final.`

No "final/best/certified/guaranteed/accessibility/performance" claims (checked by
`ListingCopyHasUnverifiedClaim`).

## Graphics — launcher icon vs listing assets (distinct)

- **In-artifact:** Android launcher / adaptive icon (Player Settings). The P26 compliance
  audit flags this if no adaptive icon is configured.
- **Console listing (separate uploads, currently MISSING):** 512×512 hi-res icon,
  1024×500 feature graphic, ≥2 landscape screenshots. Prototype screenshots are acceptable
  for **internal testing only** and must match the current build (no debug overlays / dev
  text / account info).

## Play-policy snapshot (verify — these age; PolicyVerificationStatus shown)

| Policy | Required | Resolved | Result | Verification | Source |
| --- | --- | --- | --- | --- | --- |
| Target API (new apps) | ≥ 35 | 36 (resolved) | Pass | NotVerifiedThisRun | Play target API requirements |
| 16 KB page support | supported | Aligned16k (APK) | Pass | NotVerifiedThisRun | Android 15 16 KB / Play |
| AAB + Play App Signing | required | not yet configured | Flag | CarriedForward | Play App Signing |

Configured target SDK is **Automatic (0)**; resolved is what Play evaluates. Re-verify
every value against the current official source before a real upload.
