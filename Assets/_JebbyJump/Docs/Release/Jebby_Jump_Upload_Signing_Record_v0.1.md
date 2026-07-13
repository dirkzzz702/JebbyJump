# Jebby Jump — Upload Signing Record v0.1 (P32 — PREPARATION ONLY / BLOCKED)

> **STATUS: PREPARATION ONLY — BLOCKED. No upload-key signing was performed and no Play
> upload occurred.** No real upload keystore was provided; no secrets are committed or
> logged. This records the signing *mechanism* + a fail-hard proof, not a production-signed
> build.

## Signing status

- `UploadKeyStatus`: **NotProvided** (no keystore supplied via env).
- `UploadKeySignedArtifactStatus`: **NotBuilt** (cannot build an upload-signed AAB without a key).
- Expected upload-certificate fingerprint: **N/A** (no upload key).
- Only the debug-signed regression artifact exists (`UploadReady=false`, `PlayDistributionReady=false`).

## Env contract (secrets via env only — never chat/files/tracked config)

```
JJ_SIGNING_MODE=env-upload     # (env-upload | env_upload | upload) -> Upload intent
JJ_ANDROID_KEYSTORE=<path>     JJ_ANDROID_KEYSTORE_PASS=<secret>
JJ_ANDROID_KEYALIAS=<alias>    JJ_ANDROID_KEYALIAS_PASS=<secret>
JJ_BUILD_FORMAT=aab
```

Generate the keystore with `keytool` and store it outside the repo (see the Play Internal
Testing Runbook). Only present/missing status is ever recorded — never values.

## Fail-hard proof (verified this phase)

With `JJ_SIGNING_MODE=env-upload` and **no** keystore env vars set, the RC builder:

- resolves to **EnvIncomplete → build refused** (verdict Blocked, non-zero exit),
- produces **no AAB**,
- never falls back to debug signing,
- restores the signing configuration field-exact (all signing fields captured + restored + verified) (ProjectSettings untouched — the
  fail-hard path returns before any signing apply).

Additionally, an **unknown** `JJ_SIGNING_MODE` is **fail-closed** (refuses to build; never a
debug fallback). Both behaviors are covered by EditMode tests
(`SigningResolution` + `Unknown_FailsClosed_NeverDebug`).

**Pre-commit secret scan (hardening):** run `Jebby Jump/Release/Scan ProjectSettings For
Signing Secrets` (pure `SigningSecretGuard`, EditMode-tested) after any env-upload build to
confirm no non-empty keystore/alias password was persisted to `ProjectSettings.asset` — this
catches the rare case where an interrupted build left a secret on disk before it could be
reverted. If it reports a leak, revert `ProjectSettings.asset` and do not commit.

## Pre-submission cleanup recommendation (permissions)

The current artifact declares `INTERNET`, `ACCESS_NETWORK_STATE`, and
`com.android.vending.BILLING` (engine defaults + the **present-but-unused** IAP package).
**Recommended before a real upload:** remove the unused IAP/`com.unity.purchasing` package
(drops the BILLING permission) or explicitly justify it, and confirm the network permissions
are acceptable for the Data Safety declaration.

## Evidence / sources

aapt2 artifact audit (permissions), `SigningResolution` fail-hard behavior + EditMode tests,
the ignored `Builds/P32/<commit>/signed-aab-report.json`. No secrets, paths, or env values
recorded.
