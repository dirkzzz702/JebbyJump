# Jebby Jump — Play Store Submission Readiness v0.1 (P26)

Submission **preparation** package: listing templates, the compliance surface, and a
readiness checklist. This is **not** a submission and **not** a store-readiness
certification. Play policies change — every dated assumption (see the
`Store Compliance Audit` report and the `PolicyAssumptions` block) must be re-verified
against current policy before submission.

## Identity (from the approved build config)

- Developer/brand: **SparkLibrary**
- Application id: **com.sparklibrary.jebbyjump**
- Version: **1.0** (versionCode **1**)
- Orientation: **landscape-only**

## Target audience & Families (DECIDED: mixed — children + older users)

Jebby Jump targets a **mixed audience including children under 13 and older users**, so
the **Google Play Families Policy applies to the child portion**:

- Enrol in the **Designed for Families / Families** program; complete the **target
  audience & content** section selecting the relevant age bands (including an under-13
  band).
- **Ads/monetization to children:** only **Families-certified ad SDKs**; **no
  personalized/behavioral ads** to children; ad content rated appropriately. (Jebby Jump
  ships **no ad SDK** today — any future ad integration must meet this before release.)
- **Data:** COPPA / GDPR-K obligations for child users; minimize data; no child data sold
  or used for personalized ads.
- Consider a **neutral age screen** if behavior differs by age band.

> The **IARC content rating is NOT predicted here.** It is assigned only by completing
> the IARC questionnaire in the Console. Answer it truthfully (memory/platformer; declare
> any ads/IAP/UGC accurately).

## Data safety — DRAFT (correction #6: pending verification)

**Working assumption: "no data collected, no data shared."** This is a **DRAFT**, not a
conclusion, and must be confirmed by all of:

1. **Generated-artifact permissions** — inspect the built AAB/APK `AndroidManifest`
   (`aapt2 dump permissions`) for any requested permissions.
2. **Transitive SDKs** — review what Unity itself and any included packages (e.g.
   Unity services, IAP) collect, even if unused at runtime.
3. **Runtime behavior** — confirm no analytics/ads/backend is actually wired (current
   abstractions are local/no-op).
4. **Play Console review** — the data-safety form is declared and reviewed in Console.

A **privacy policy is REQUIRED regardless** (apps with a child audience always require a
privacy policy URL). Host one and record its URL in the listing before submission.

## Listing field templates (fill with final values)

| Field | Constraint | Value |
| --- | --- | --- |
| App title | ≤ 30 chars | `Jebby Jump` |
| Short description | ≤ 80 chars | _TODO_ |
| Full description | ≤ 4000 chars | _TODO_ |
| Category | — | Games → (Arcade / Puzzle — choose) |
| Contact email | required | _TODO (developer contact)_ |
| Website | optional | _TODO_ |
| Privacy policy URL | **required (child audience)** | _TODO — must be hosted_ |

## Graphics — launcher/adaptive icon vs listing graphics (correction #8)

Two **different** things, do not conflate:

- **Launcher / adaptive icon** — ships **inside the artifact** (Player Settings → Android
  → Icon: adaptive foreground + background layers). Audited by `Store Compliance Audit`
  (`icon.launcher.adaptive`).
- **Play listing graphics** — uploaded to the **Console**, **not** in the artifact:
  - Hi-res icon **512×512** PNG
  - Feature graphic **1024×500**
  - **≥ 2 screenshots** (landscape, 16:9 recommended for this game)

## Technical compliance (see `Store Compliance Audit` report)

- **Target API:** configured = Automatic (highest installed); the **resolved** artifact
  value is what Play evaluates. Pin it for reproducible, policy-locked builds and verify
  it meets the **current** Play target-API minimum (assumed ≥ API 35 — verify).
- **16 KB page support:** verify on the generated artifact (`zipalign -c -P 16`); see the
  release report `PageSize16kStatus`.
- **App signing:** ship the **AAB** and enrol in **Play App Signing** (see the signing
  doc). The build is upload-signed at most; Play holds the app signing key.
- **64-bit:** ARM64 (IL2CPP) — satisfied.

## Submission-readiness checklist

- [ ] Final listing copy (short/full description, category)
- [ ] Privacy policy hosted + URL set (**required**)
- [ ] Data-safety form completed (after the DRAFT verification above)
- [ ] IARC content-rating questionnaire completed (not predicted)
- [ ] Target audience & content (Families) completed; ads/data settings consistent
- [ ] Adaptive launcher icon configured (in-artifact)
- [ ] Listing graphics uploaded (512² icon, 1024×500 feature, ≥2 screenshots)
- [ ] Target API pinned + verified against current Play minimum
- [ ] 16 KB alignment verified on the artifact
- [ ] Play App Signing enrolled; AAB upload-signed
- [ ] Internal testing track validated (see signing/internal-testing doc)

## DEFERRED / external

Actual upload, real metadata values, privacy-policy hosting, production signing, the
pre-launch report, and store acceptance are all **external / DEFERRED**. Device/visual QA
remains **DEFERRED / NOT VERIFIED**.

## P27 note

P27 (Preparation only) turned this readiness material into actionable worksheets: the
final Play declarations + listing copy draft + content-rating questionnaire worksheet are
in `Jebby_Jump_Play_Declaration_Record_v0.1.md` (all **DRAFT/WORKSHEET**, none submitted),
the operator steps are in `Jebby_Jump_Play_Internal_Testing_Runbook_v0.1.md`, and the
`Distribution Readiness Audit` enumerates the missing external items. Data Safety stays a
DRAFT pending artifact audit + Console review + approval; a privacy-policy URL is still
**REQUIRED/MISSING**; the IARC rating is **not predicted**. Target audience remains
**mixed (children + older) → Families**.

## P31 note

These readiness items were finalized as dedicated DRAFT docs in P31 — `Store_Listing_Final_
Copy`, `Privacy_Policy_Draft`, `Data_Safety_Worksheet`, `Content_Rating_Worksheet`,
`Store_Graphics_Checklist`, `Families_Readiness_Worksheet`, `Internal_Testing_Release_Notes`,
and `Store_External_Readiness_Checklist`. All remain **NOT SUBMITTED**; decision: **Store
listing package blocked — missing external content** (privacy URL, support email, listing
graphics, adaptive icon external/missing).
