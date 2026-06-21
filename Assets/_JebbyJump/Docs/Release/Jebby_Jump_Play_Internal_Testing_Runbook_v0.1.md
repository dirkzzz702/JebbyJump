# Jebby Jump — Play Internal Testing Runbook v0.1 (P27)

Ordered steps for an authorized human operator to execute a Google Play **internal
testing** upload once the external prerequisites exist. P27 itself ran in **Preparation
only** mode: no Play Console action, no real upload keystore, no device. This runbook is
instructions — it is **not** a record that any of these steps were performed.

> Security: keystores, passwords, Play credentials, and tester emails are **never**
> committed, pasted into chat, or written into tracked files / reports. Supply secrets
> only through environment variables or a secret manager.

## 1. Generate the upload keystore (once, stored OUTSIDE the repo)

```
keytool -genkeypair -v \
  -keystore <secure-location>/jebbyjump-upload.keystore \
  -alias upload -keyalg RSA -keysize 2048 -validity 9125 -storetype PKCS12
```

Record the **upload certificate SHA-256 fingerprint** (public) for later verification.
Back it up securely; losing it (before Play App Signing) blocks future updates.

## 2. Environment contract (secrets via env only)

```
JJ_SIGNING_MODE=env-upload
JJ_ANDROID_KEYSTORE=<absolute path to the keystore>
JJ_ANDROID_KEYSTORE_PASS=<store password>
JJ_ANDROID_KEYALIAS=upload
JJ_ANDROID_KEYALIAS_PASS=<key password>
JJ_BUILD_FORMAT=aab
JJ_TESTS_PASSED=1  JJ_TESTS_TOTAL=<n>  JJ_TESTS_FAILED=0
```

Upload mode **fails the build** on any missing/invalid value — it never falls back to
debug signing.

## 3. Apply approved tracked config, then build the upload-key-signed AAB

1. Set the approved version name, **version code** (greater than every previously
   uploaded code), and target SDK in `JebbyJumpDistributionConfig.Approved` and set
   `Approved = true`; run `Jebby Jump/Release/Apply Approved Distribution Config`
   (refuses unless approved + valid). Commit the tracked config; confirm a clean tree.
2. Build from that clean commit:

```
Unity -batchmode -nographics -projectPath . \
  -executeMethod JebbyJump.Release.JebbyJumpReleaseBuilder.BuildReleaseCandidateFromCommandLine
```

3. Confirm the report: `AndroidAabBuilt`, warning gate Passed, `SignatureVerifyStatus`
   = Verified, `SigningConfigRestored` = Restored. Compare the AAB signer fingerprint to
   the recorded **upload** certificate fingerprint.

> A debug-signed AAB/APK is only a **regression-gate** artifact — never upload it as a
> distribution candidate.

## 4. Play Console (authorized operator; each write read-then-confirm-then-verify)

1. Create / select the app: package `com.sparklibrary.jebbyjump`, default language,
   game classification, free/paid decision (changing free→paid later is restricted).
2. Opt into **Play App Signing**; record the **app-signing** fingerprint AND the
   **upload** fingerprint separately (never conflate them).
3. Complete declarations from the Declaration Record worksheet (Data Safety, content
   rating questionnaire, target audience & Families, privacy policy, ads, app access).
4. **Internal testing → Create release →** upload the exact AAB (match the recorded
   SHA-256). Review Play's processing: version code, target SDK, supported devices,
   warnings, 16 KB result.
5. Add the approved tester list / Google Group (store tester emails only in the private
   system — never in the repo).
6. Final confirm (artifact hash, version code, declarations saved, track = internal),
   then **start internal rollout only**. Do not create open/closed/production tracks.

## 5. Device smoke (use the P25 plan subset)

Tester joins the track → Play offers the version → install → launch → landscape → Main
Menu → start a level → move/jump → Pause/Resume → audio → no crash/ANR. Record device
model, Android version, tested version code, result. If no device, mark **NOT RUN**.

## 6. Record afterward (public, non-secret values only)

Version/version code, public certificate fingerprints, artifact SHA-256, track name,
tester count (not emails), declaration decisions, Play warnings, result — into the
Distribution Readiness Result + Declaration Record. Keep raw Console screenshots/logs in
the ignored `Builds/P27/` evidence area, redacted.
