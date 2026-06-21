using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace JebbyJump.Release
{
    // Editor preflight wrapper (fix #4: VALIDATE only, never fix; fix #12:
    // read-only). Gathers a snapshot from PlayerSettings/EditorBuildSettings/
    // manifest and runs the pure core, then adds read-only scene-existence,
    // required-asset, and TMP-digit checks. Never calls SaveAssets / scaffolds /
    // selection tools / mutates any asset.
    public static class ReleasePreflight
    {
        [MenuItem("Jebby Jump/Release/Run RC Preflight")]
        public static void RunAndLog()
        {
            var result = RunPreflight();
            foreach (var c in result.Checks)
                Debug.Log($"[Preflight][{c.Severity}] {c.CheckId}: {c.Message}");
            Debug.Log($"[Preflight] {(result.Passed ? "PASS" : "FAIL")} "
                + $"errors={result.ErrorCount} warnings={result.WarningCount}");
        }

        public static ReleasePreflightResult RunPreflight()
        {
            var snapshot = GatherSnapshot();
            var result = ReleasePreflightCore.Evaluate(snapshot, ReleaseSceneContract.Scenes);
            AddSceneExistenceChecks(result);
            ReleaseAssetChecks.Evaluate(GatherAssetPresence(), result);
            // Informational only (never fails the gate; never prints secrets): the
            // resolved signing intent for this environment.
            result.Add(ReleaseCheckResult.Info("signing.intent", SigningIntentLine()));
            return result;
        }

        // Reports signing INTENT + resolved mode from the environment using only the
        // PRESENCE of the keystore vars - never their values, the path, or passwords.
        private static string SigningIntentLine()
        {
            var r = JebbyJumpReleaseSigning.ResolveFromEnvironment();
            return $"intent={r.Intent}; resolved={r.Mode}; {r.Reason}";
        }

        public static ReleaseConfigSnapshot GatherSnapshot()
        {
            var s = new ReleaseConfigSnapshot
            {
                CompanyName = PlayerSettings.companyName,
                ProductName = PlayerSettings.productName,
                ApplicationIdentifier =
                    PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android),
                BundleVersion = PlayerSettings.bundleVersion,
                AndroidVersionCode = PlayerSettings.Android.bundleVersionCode,
                DevelopmentBuild = EditorUserBuildSettings.development,
                AndroidArchitectures = (int)PlayerSettings.Android.targetArchitectures,
                AndroidScriptingBackend =
                    PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android)
                        == ScriptingImplementation.IL2CPP ? 1 : 0,
                LandscapeOnly = ComputeLandscapeOnly(),
                PackageIds = ReadManifestPackageIds(),
            };

            var enabled = new List<string>();
            foreach (var sc in EditorBuildSettings.scenes)
                if (sc.enabled) enabled.Add(sc.path);
            s.EnabledScenePaths = enabled.ToArray();

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            s.ActiveInputHandler = 1;
#elif !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            s.ActiveInputHandler = 0;
#else
            s.ActiveInputHandler = 2;
#endif
            return s;
        }

        private static bool ComputeLandscapeOnly()
        {
            var d = PlayerSettings.defaultInterfaceOrientation;
            if (d == UIOrientation.LandscapeLeft || d == UIOrientation.LandscapeRight)
                return true;
            if (d == UIOrientation.AutoRotation)
                return !PlayerSettings.allowedAutorotateToPortrait
                    && !PlayerSettings.allowedAutorotateToPortraitUpsideDown
                    && (PlayerSettings.allowedAutorotateToLandscapeLeft
                        || PlayerSettings.allowedAutorotateToLandscapeRight);
            return false;
        }

        private static string[] ReadManifestPackageIds()
        {
            string path = Path.GetFullPath(
                Path.Combine(Application.dataPath, "../Packages/manifest.json"));
            if (!File.Exists(path)) return Array.Empty<string>();
            string text = File.ReadAllText(path);
            var ids = new List<string>();
            foreach (Match m in Regex.Matches(text, "\"([^\"]+)\"\\s*:\\s*\"[^\"]*\""))
                ids.Add(m.Groups[1].Value);
            return ids.ToArray();
        }

        private static void AddSceneExistenceChecks(ReleasePreflightResult r)
        {
            foreach (var rel in ReleaseSceneContract.Scenes)
            {
                string full = Path.GetFullPath(
                    Path.Combine(Application.dataPath, "..", rel));
                if (!File.Exists(full))
                    r.Add(ReleaseCheckResult.Error("scenes.exist", $"missing scene file: {rel}"));
            }
        }

        // Read-only presence gather (no AssetDatabase mutation, no SaveAssets, no
        // menu tools). The pure ReleaseAssetChecks evaluates the result.
        public static ReleaseAssetPresence GatherAssetPresence()
        {
            var p = new ReleaseAssetPresence
            {
                LevelCatalog = AssetDatabase.FindAssets("t:LevelCatalog").Length >= 1,
                OutfitVisualLibrary = AssetDatabase.FindAssets("t:OutfitVisualLibrary").Length >= 1,
                WardrobePreviewLibrary = AssetDatabase.FindAssets("t:WardrobePreviewLibrary").Length >= 1,
                InputActions = AssetDatabase.FindAssets("t:InputActionAsset").Length >= 1,
                OverrideControllers = AssetDatabase.FindAssets("t:AnimatorOverrideController").Length,
            };

            var jebby = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/_JebbyJump/Prefabs/Player/Jebby.prefab");
            p.JebbyPrefab = jebby != null;
            if (jebby != null)
            {
                var anim = jebby.GetComponentInChildren<Animator>(true);
                p.JebbyAnimator = anim != null && anim.runtimeAnimatorController != null;
            }

            p.PlatformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/_JebbyJump/Prefabs/Platforms/Platform.prefab") != null;

            var font = TMP_Settings.defaultFontAsset;
            p.TmpFont = font != null;
            if (font != null)
            {
                bool all = true;
                for (char c = '0'; c <= '9'; c++)
                    if (!font.HasCharacter(c)) { all = false; break; }
                p.TmpDigits = all;
            }
            return p;
        }
    }
}
