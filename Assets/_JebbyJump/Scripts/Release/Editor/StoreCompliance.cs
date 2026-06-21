using System;
using System.Collections.Generic;

namespace JebbyJump.Release
{
    // Pure Play store-compliance policy (P26-Store). UnityEditor-free so it is
    // EditMode-testable. Corrections:
    //  #4  configured target SDK (may be Automatic=0) is reported SEPARATELY from the
    //      resolved artifact target SDK; Automatic is a REPRODUCIBILITY flag, not
    //      automatic non-compliance (compliance is judged on the resolved value).
    //  #5  every Play-policy assumption carries source + effective-date + as-of metadata
    //      so stale assumptions are visible (the assistant's knowledge can age).
    //  #8  launcher/adaptive icon (shipped IN the artifact) is distinct from the Play
    //      listing graphics (icon 512², feature 1024×500, screenshots) uploaded to the
    //      Console.

    [Serializable]
    public struct PlayPolicyAssumption
    {
        public string Name;
        public string AssumedValue;
        public string Source;
        public string EffectiveDate;
        public string AsOf;
    }

    [Serializable]
    public struct StoreComplianceSnapshot
    {
        public int ConfiguredTargetSdk;   // 0 = Automatic (highest installed)
        public int ResolvedTargetSdk;     // highest installed platform / read artifact (0 = unknown)
        public int MinSdk;
        public bool LandscapeOnly;
        public int AndroidVersionCode;
        public string ApplicationId;
        public bool HasAdaptiveIcon;      // launcher/adaptive icon (in-artifact)
        public bool HasLegacyIcon;
    }

    [Serializable]
    public struct StoreComplianceFinding
    {
        public string CheckId;
        public string Status;       // "PASS" | "FLAG" | "INFO"
        public string Message;
        public string Recommendation;

        public static StoreComplianceFinding Pass(string id, string m)
            => new StoreComplianceFinding { CheckId = id, Status = "PASS", Message = m };
        public static StoreComplianceFinding Flag(string id, string m, string rec)
            => new StoreComplianceFinding { CheckId = id, Status = "FLAG", Message = m, Recommendation = rec };
        public static StoreComplianceFinding Info(string id, string m)
            => new StoreComplianceFinding { CheckId = id, Status = "INFO", Message = m };
    }

    public static class StoreCompliancePolicy
    {
        // ASSUMED Play target API minimum. VERIFY against current policy (it raises
        // roughly annually); this constant is dated in DefaultAssumptions().
        public const int AssumedPlayMinTargetSdk = 35; // Android 15

        public static PlayPolicyAssumption[] DefaultAssumptions() => new[]
        {
            new PlayPolicyAssumption
            {
                Name = "Play target API level (new apps)",
                AssumedValue = ">= 35 (Android 15)",
                Source = "Google Play target API level requirements",
                EffectiveDate = "new apps 2024-08-31; updates 2025-08-31 (raises ~annually)",
                AsOf = "2026-01 (assistant knowledge cutoff) — VERIFY current requirement",
            },
            new PlayPolicyAssumption
            {
                Name = "16 KB memory page support",
                AssumedValue = "required for apps targeting Android 15+",
                Source = "Android 15 16 KB page size / Google Play guidance",
                EffectiveDate = "~2025-11 (Play, apps targeting Android 15+)",
                AsOf = "2026-01 — VERIFY current requirement + device scope",
            },
            new PlayPolicyAssumption
            {
                Name = "App distribution + signing",
                AssumedValue = "new apps ship AAB + use Play App Signing",
                Source = "Google Play (AAB requirement) / Play App Signing",
                EffectiveDate = "2021-08 (new apps require AAB)",
                AsOf = "2026-01 — VERIFY",
            },
        };

        public static StoreComplianceFinding[] Evaluate(
            StoreComplianceSnapshot s, int assumedMinTargetSdk = AssumedPlayMinTargetSdk)
        {
            var f = new List<StoreComplianceFinding>();

            // --- target SDK: configured vs resolved (correction #4) ---
            if (s.ConfiguredTargetSdk == 0)
                f.Add(StoreComplianceFinding.Flag("target-sdk.reproducibility",
                    "Configured target SDK is Automatic (highest installed). The artifact's "
                    + "target API then depends on the build machine.",
                    "Pin PlayerSettings.Android.targetSdkVersion to a fixed API for deterministic, "
                    + "policy-locked builds."));
            else
                f.Add(StoreComplianceFinding.Info("target-sdk.configured",
                    $"Configured target SDK = API {s.ConfiguredTargetSdk} (pinned)."));

            if (s.ResolvedTargetSdk <= 0)
                f.Add(StoreComplianceFinding.Info("target-sdk.compliance",
                    "Resolved artifact target SDK not read in this context. Build an APK "
                    + "(JJ_BUILD_FORMAT=apk) to confirm the actual value via aapt2."));
            else if (s.ResolvedTargetSdk >= assumedMinTargetSdk)
                f.Add(StoreComplianceFinding.Pass("target-sdk.compliance",
                    $"Resolved artifact target SDK = API {s.ResolvedTargetSdk} "
                    + $"(>= assumed Play minimum {assumedMinTargetSdk})."));
            else
                f.Add(StoreComplianceFinding.Flag("target-sdk.compliance",
                    $"Resolved artifact target SDK = API {s.ResolvedTargetSdk}, below the assumed "
                    + $"Play minimum {assumedMinTargetSdk}.",
                    "Raise the resolved target API (install/pin a newer platform) and re-verify "
                    + "against the CURRENT Play target-API requirement."));

            // --- 16 KB pages (correction #5): cannot be judged from settings alone ---
            f.Add(StoreComplianceFinding.Info("pages-16kb",
                "16 KB page-size support must be verified on the GENERATED artifact "
                + "(zipalign -c -P 16). See the release report PageSize16kStatus for the built APK."));

            // --- min SDK / orientation (informational) ---
            f.Add(StoreComplianceFinding.Info("min-sdk", $"minSdkVersion = API {s.MinSdk}."));
            f.Add(s.LandscapeOnly
                ? StoreComplianceFinding.Info("orientation", "Landscape-only (matches design).")
                : StoreComplianceFinding.Flag("orientation",
                    "Orientation is not landscape-only.", "Confirm intended orientation before submission."));

            // --- version code ---
            f.Add(s.AndroidVersionCode >= 1
                ? StoreComplianceFinding.Pass("version-code", $"versionCode = {s.AndroidVersionCode}.")
                : StoreComplianceFinding.Flag("version-code",
                    $"versionCode = {s.AndroidVersionCode} (< 1).", "Set a positive, increasing versionCode."));

            // --- application id ---
            bool placeholderId = string.IsNullOrEmpty(s.ApplicationId)
                || s.ApplicationId.StartsWith("com.DefaultCompany")
                || s.ApplicationId.StartsWith("com.unity3d")
                || s.ApplicationId.StartsWith("com.Company");
            f.Add(placeholderId
                ? StoreComplianceFinding.Flag("application-id",
                    $"Application id looks like a placeholder: '{s.ApplicationId}'.",
                    "Set a final, owned application id before submission.")
                : StoreComplianceFinding.Pass("application-id", $"Application id = '{s.ApplicationId}'."));

            // --- icons (correction #8): launcher/adaptive (in-artifact) ---
            f.Add(s.HasAdaptiveIcon
                ? StoreComplianceFinding.Pass("icon.launcher.adaptive",
                    "Adaptive launcher icon is configured (ships in the artifact).")
                : StoreComplianceFinding.Flag("icon.launcher.adaptive",
                    s.HasLegacyIcon
                        ? "Only a legacy launcher icon is configured; no adaptive icon."
                        : "No launcher icon configured.",
                    "Configure an adaptive launcher icon (foreground + background layers) in "
                    + "Player Settings > Android > Icon."));
            f.Add(StoreComplianceFinding.Info("icon.listing",
                "Play LISTING graphics (512×512 hi-res icon, 1024×500 feature graphic, "
                + "screenshots) are SEPARATE Console uploads — NOT shipped in the artifact."));

            return f.ToArray();
        }

        // Number of hard FLAGs (non-INFO) — used for the report's honest summary line.
        public static int FlagCount(StoreComplianceFinding[] findings)
        {
            int n = 0;
            foreach (var x in findings) if (x.Status == "FLAG") n++;
            return n;
        }
    }
}
