# Jebby Jump — Play Console Declaration Record v0.2 (P32 — PREPARATION ONLY)

> **STATUS: PREPARED-ENTRY WORKSHEET — `SubmittedInConsole = false`, `VerifiedInConsole =
> false`.** Nothing entered, submitted, or verified in the Play Console. Consolidates the
> P31 declarations for eventual Console entry. Deterministic — no secrets/timestamps/paths/
> tester identities.

## Identity

Developer/brand: SparkLibrary · app id `com.sparklibrary.jebbyjump` · version 1.0 /
versionCode 1 (`VersionCodeStatus = NotVerified` — no Console evidence that the code is
unused).

## Declaration entries (each prepared, NOT submitted)

| Declaration | Prepared value / status | Submitted | Verified |
| --- | --- | --- | --- |
| App/game classification | Game | false | false |
| Free / paid | Free | false | false |
| Ads | No (no ad SDK) | false | false |
| In-app purchases | No (BILLING permission present but unused) | false | false |
| Target audience / Families | Mixed (children + older) → Families | false | false |
| Privacy policy | **Blocked — no hosted URL** | false | false |
| Data Safety | Candidate "no data collected/shared" — needs artifact audit + approval | false | false |
| Content rating (IARC) | Worksheet answers; **rating not predicted** | false | false |
| App access | No login/gated content | false | false |
| Store listing text | Draft (see Listing Copy doc) | false | false |
| Graphics / screenshots | **Missing** (icon / feature / screenshots) | false | false |
| Contact details | `<PROVIDE: support email>` | false | false |

## Permissions note (pre-submission cleanup)

`INTERNET` / `ACCESS_NETWORK_STATE` / `BILLING` are present from engine defaults + the unused
IAP package. Recommend removing/justifying the unused billing package before submission and
aligning the Data Safety answers accordingly.

## Evidence / sources

P31 store package docs (listing, privacy, data-safety, content-rating, graphics, Families),
the aapt2 artifact audit, and the P32 upload-signing record. All items remain external/Blocked
or candidate pending user approval + Console action.
