# Jebby Jump — Production Signing & Internal Testing Track v0.1 (P26)

Env-driven Android signing **scaffolding** + the path to a Play internal testing track.
P26 adds the *mechanism* (and proves it round-trips safely); it does **not** create a
production-signed build or upload anything. Keystores, passwords, and certificates are
**NEVER** committed (`*.keystore` / `*.jks` / `*.p12` are gitignored) and are **never**
persisted to `ProjectSettings.asset` — the builder applies them transiently and restores
the prior signing config byte-for-byte after every build.

## Signing modes (correction #1 — names matter)

| Mode | What it is | Store-ready? |
| --- | --- | --- |
| `DebugSigned` | Android debug key (development; NOT production) | No |
| `EnvUploadKeySigned` | a custom **UPLOAD** key supplied via env | **No, not by itself** |

> An **upload key ≠ the Play App Signing key.** With Play App Signing, you sign the
> upload artifact with *your* upload key; Google **re-signs** the delivered app with the
> Google-managed **app signing key**. So `EnvUploadKeySigned` means "correctly signed for
> *upload*," never "store-certified." Store-readiness is tracked separately and is
> **DEFERRED** in P26.

## Explicit signing intent (correction #2 — fail-hard)

Signing intent is explicit via `JJ_SIGNING_MODE`:

- unset / `debug` → **DebugSigned** (the explicit default).
- `upload` → **EnvUploadKeySigned**, but **only** if all four keystore variables are set
  *and* the keystore file exists. If anything is missing or invalid the build **FAILS**
  (verdict `Release candidate blocked`). It **never** silently falls back to debug.

### Environment contract

```
JJ_SIGNING_MODE=upload                 # omit/"debug" for debug signing
JJ_ANDROID_KEYSTORE=/abs/path/upload.keystore
JJ_ANDROID_KEYSTORE_PASS=********
JJ_ANDROID_KEYALIAS=upload
JJ_ANDROID_KEYALIAS_PASS=********
JJ_BUILD_FORMAT=aab                    # "aab" (default) | "apk"
JJ_TESTS_PASSED=1 JJ_TESTS_TOTAL=<n>   # set after the test gate passes (honest verdict)
```

## Generate an upload keystore (once, kept OUTSIDE the repo)

```
keytool -genkeypair -v \
  -keystore /secure/location/jebbyjump-upload.keystore \
  -alias upload -keyalg RSA -keysize 2048 -validity 9125 \
  -storetype PKCS12
```

Store it and its passwords in a secret manager / CI secret store. Never commit it; never
paste it into `ProjectSettings.asset`.

## Build (AAB for Play, APK for sideload) — correction #11

The AAB and APK are **separate artifacts with different purposes** and land in separate
output dirs + reports:

- **AAB** (`Builds/P23/Android/<ver>/JebbyJump.aab`) → **Play distribution**. Play
  re-signs via App Signing and generates per-device APKs. Not directly installable.
- **APK** (`Builds/P23/AndroidApk/<ver>/JebbyJump.apk`) → **direct install** (sideload /
  `adb install` / internal sharing). Signed as-is; not processed by Play App Signing.

```
# Headless RC build (AAB, debug-signed by default)
Unity -batchmode -nographics -projectPath . \
  -executeMethod JebbyJump.Release.JebbyJumpReleaseBuilder.BuildReleaseCandidateFromCommandLine

# Installable APK
JJ_BUILD_FORMAT=apk Unity -batchmode -nographics -projectPath . \
  -executeMethod JebbyJump.Release.JebbyJumpReleaseBuilder.BuildReleaseCandidateFromCommandLine
```

Menu equivalent: `Jebby Jump/Release/Build Release Candidate` (never exits the editor).

## Signature verification + safe restore (correction #3)

After every build the pipeline:

1. **Verifies the actual artifact signature** — `apksigner verify --print-certs` for the
   APK (records the public signer SHA-256 cert fingerprint), `jarsigner -verify` for the
   AAB. Result is recorded as `Verified` / `Failed` / `Skipped` (Skipped only if the tool
   genuinely can't be located — never a false pass).
2. **Restores the signing config byte-for-byte** (`useCustomKeystore`, keystore/alias
   names, and passwords) to the pre-build snapshot and asserts no drift
   (`SigningConfigRestored = Restored | DRIFT`). No secret is ever written to disk.

## Upload to a Play internal testing track (external / manual)

1. Create the app in Play Console; enrol in **Play App Signing** (recommended — let Google
   manage the app signing key; you keep the upload key).
2. Complete the required declarations first (data safety, content rating questionnaire,
   target audience & Families, privacy policy, ads) — see
   `Jebby_Jump_Store_Submission_Readiness_v0.1.md`.
3. Build the **AAB** with `JJ_SIGNING_MODE=upload`.
4. **Internal testing → Create new release →** upload the AAB → add testers → roll out.
5. Use the generated **APK** for fast local sideload checks in parallel.

## Status (P26)

- Mechanism: **ready** (env-driven, fail-hard, verified, byte-for-byte restored).
- Production signing, Play App Signing enrolment, and store upload: **DEFERRED / external**.
- No keystore/password/certificate is committed or persisted by this repo.

## P27 note

P27 exercised this pipeline in **Preparation only** mode. No real upload keystore was
supplied, so the env-upload path was **NOT RUN** and the rebuilt debug-signed AAB is a
**regression-gate artifact (not upload-ready)**. The full operator step list (keytool
keystore generation, env-upload build, signature verification, Play App Signing, internal
track, testers, rollout) now lives in `Jebby_Jump_Play_Internal_Testing_Runbook_v0.1.md`,
and the Play declarations are drafted in `Jebby_Jump_Play_Declaration_Record_v0.1.md`.
A `Distribution Readiness Audit` records the independent statuses + enumerated external
blockers. Decision: **P27 preparation complete — upload requires authorized external
action.**
