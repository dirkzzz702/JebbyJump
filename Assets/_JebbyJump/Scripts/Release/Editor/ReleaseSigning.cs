using System;

namespace JebbyJump.Release
{
    // Pure signing-resolution core (P26-Sign). Kept free of UnityEditor so it is
    // EditMode-testable and needs NO real secrets to classify an outcome.
    //
    // Correction #1: "upload key" is NOT the Play App Signing key. The signing MODE
    // never implies store-readiness; store-readiness is a separate status.
    // Correction #2: signing INTENT is explicit. Upload intent FAILS the build on any
    // missing/invalid input - it must never silently fall back to debug signing.

    public enum SigningIntent { Debug, Upload, Unknown }

    public enum SigningMode
    {
        DebugSigned,        // development; NOT production
        EnvUploadKeySigned, // signed with a custom UPLOAD key from env (NOT the Play app-signing key)
        EnvIncomplete,      // upload requested but config missing/invalid -> build must fail
    }

    [Serializable]
    public struct SigningResolutionResult
    {
        public string Intent;        // SigningIntent name
        public string Mode;          // SigningMode name
        public bool BuildShouldFail; // true when Upload intent but env incomplete/invalid
        public string Reason;
    }

    public static class SigningResolution
    {
        public static SigningIntent ParseIntent(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return SigningIntent.Debug;
            switch (raw.Trim().ToLowerInvariant())
            {
                case "debug":
                    return SigningIntent.Debug;   // explicit debug
                case "upload":
                case "custom":
                case "release":
                case "env-upload":                // P32 spec token
                case "env_upload":
                    return SigningIntent.Upload;
                default:
                    return SigningIntent.Unknown;  // fail-closed: an unrecognized mode never
                                                   // silently falls back to debug (P32 corr #2)
            }
        }

        // envComplete  = all four keystore env vars are non-empty.
        // keystoreFile = the referenced keystore file actually exists.
        public static SigningResolutionResult Resolve(
            SigningIntent intent, bool envComplete, bool keystoreFileExists)
        {
            if (intent == SigningIntent.Debug)
                return new SigningResolutionResult
                {
                    Intent = nameof(SigningIntent.Debug),
                    Mode = nameof(SigningMode.DebugSigned),
                    BuildShouldFail = false,
                    Reason = "No production signing requested; debug-signed (development; NOT production).",
                };

            // Fail-closed: an unknown/unrecognized signing mode refuses to build and never
            // falls back to debug (P32 correction #2).
            if (intent == SigningIntent.Unknown)
                return new SigningResolutionResult
                {
                    Intent = nameof(SigningIntent.Unknown),
                    Mode = nameof(SigningMode.EnvIncomplete),
                    BuildShouldFail = true,
                    Reason = "Unknown JJ_SIGNING_MODE; refusing to build (fail-closed; no debug fallback).",
                };

            // Upload intent: FAIL HARD on any missing/invalid input (correction #2).
            if (!envComplete)
                return new SigningResolutionResult
                {
                    Intent = nameof(SigningIntent.Upload),
                    Mode = nameof(SigningMode.EnvIncomplete),
                    BuildShouldFail = true,
                    Reason = "Upload signing requested but one or more of JJ_ANDROID_KEYSTORE/"
                        + "JJ_ANDROID_KEYSTORE_PASS/JJ_ANDROID_KEYALIAS/JJ_ANDROID_KEYALIAS_PASS is missing.",
                };
            if (!keystoreFileExists)
                return new SigningResolutionResult
                {
                    Intent = nameof(SigningIntent.Upload),
                    Mode = nameof(SigningMode.EnvIncomplete),
                    BuildShouldFail = true,
                    Reason = "Upload signing requested but the referenced keystore file does not exist.",
                };

            return new SigningResolutionResult
            {
                Intent = nameof(SigningIntent.Upload),
                Mode = nameof(SigningMode.EnvUploadKeySigned),
                BuildShouldFail = false,
                Reason = "Upload-key signing configured from environment "
                    + "(an UPLOAD key, NOT the Play App Signing key; store-readiness is separate).",
            };
        }

        // Human-readable signing status string for the report (never contains secrets).
        public static string StatusString(SigningResolutionResult r)
        {
            switch (r.Mode)
            {
                case nameof(SigningMode.EnvUploadKeySigned):
                    return "EnvUploadKeySigned (custom upload key; NOT Play App Signing; NOT store-certified)";
                case nameof(SigningMode.EnvIncomplete):
                    return r.Intent == nameof(SigningIntent.Unknown)
                        ? "EnvIncomplete (unknown JJ_SIGNING_MODE; build failed, fail-closed)"
                        : "EnvIncomplete (upload requested but config invalid; build failed)";
                default:
                    return "DebugSigned (development; NOT production)";
            }
        }
    }
}
