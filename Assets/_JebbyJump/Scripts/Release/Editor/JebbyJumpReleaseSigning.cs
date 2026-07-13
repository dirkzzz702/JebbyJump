using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.Release
{
    // Editor glue for env-driven Android signing (P26-Sign, corrections #2/#3).
    //
    // - Resolves intent + validity from environment via the pure SigningResolution.
    // - Applies an UPLOAD key to PlayerSettings ONLY for the build, then the caller
    //   restores the captured snapshot BYTE-FOR-BYTE in a finally (nothing secret is
    //   ever persisted to ProjectSettings.asset).
    // - Never logs/serializes passwords or the keystore path; only the resolved MODE
    //   (and, separately, the public signer fingerprint from the artifact) is reported.
    public static class JebbyJumpReleaseSigning
    {
        public const string EnvMode      = "JJ_SIGNING_MODE";        // "debug" (default) | "upload"
        public const string EnvKeystore  = "JJ_ANDROID_KEYSTORE";    // path to .keystore/.jks
        public const string EnvKsPass    = "JJ_ANDROID_KEYSTORE_PASS";
        public const string EnvAlias     = "JJ_ANDROID_KEYALIAS";
        public const string EnvAliasPass = "JJ_ANDROID_KEYALIAS_PASS";

        public struct SigningSnapshot
        {
            public bool UseCustomKeystore;
            public string KeystoreName, KeystorePass, KeyaliasName, KeyaliasPass;
        }

        public static SigningSnapshot Capture() => new SigningSnapshot
        {
            UseCustomKeystore = PlayerSettings.Android.useCustomKeystore,
            KeystoreName = PlayerSettings.Android.keystoreName,
            KeystorePass = PlayerSettings.Android.keystorePass,
            KeyaliasName = PlayerSettings.Android.keyaliasName,
            KeyaliasPass = PlayerSettings.Android.keyaliasPass,
        };

        public static void Restore(SigningSnapshot s)
        {
            PlayerSettings.Android.useCustomKeystore = s.UseCustomKeystore;
            PlayerSettings.Android.keystoreName = s.KeystoreName;
            PlayerSettings.Android.keystorePass = s.KeystorePass;
            PlayerSettings.Android.keyaliasName = s.KeyaliasName;
            PlayerSettings.Android.keyaliasPass = s.KeyaliasPass;
        }

        // True iff the current PlayerSettings signing fields match the snapshot exactly
        // (field-exact restore proof, correction #3).
        public static bool VerifyRestored(SigningSnapshot s)
        {
            var now = Capture();
            return now.UseCustomKeystore == s.UseCustomKeystore
                && now.KeystoreName == s.KeystoreName
                && now.KeystorePass == s.KeystorePass
                && now.KeyaliasName == s.KeyaliasName
                && now.KeyaliasPass == s.KeyaliasPass;
        }

        public static SigningResolutionResult ResolveFromEnvironment()
        {
            var intent = SigningResolution.ParseIntent(Environment.GetEnvironmentVariable(EnvMode));
            string ks = Environment.GetEnvironmentVariable(EnvKeystore);
            bool envComplete = !string.IsNullOrEmpty(ks)
                && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvKsPass))
                && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvAlias))
                && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvAliasPass));
            bool fileExists = !string.IsNullOrEmpty(ks) && File.Exists(ks);
            return SigningResolution.Resolve(intent, envComplete, fileExists);
        }

        // Applies the resolved signing config for THIS build. Caller must already have
        // captured a snapshot and must restore it in finally. Returns the resolution.
        public static SigningResolutionResult ApplyResolved(SigningResolutionResult r)
        {
            if (r.Mode == nameof(SigningMode.EnvUploadKeySigned))
            {
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = Environment.GetEnvironmentVariable(EnvKeystore);
                PlayerSettings.Android.keystorePass = Environment.GetEnvironmentVariable(EnvKsPass);
                PlayerSettings.Android.keyaliasName = Environment.GetEnvironmentVariable(EnvAlias);
                PlayerSettings.Android.keyaliasPass = Environment.GetEnvironmentVariable(EnvAliasPass);
            }
            else
            {
                // Explicit debug: do not use a custom keystore for this build.
                PlayerSettings.Android.useCustomKeystore = false;
            }
            return r;
        }
    }
}
