using UnityEditor;
using UnityEngine;

namespace JebbyJump.Release
{
    // Explicit, separate writer of tracked distribution config (version name/code + an
    // optional target-SDK pin). DRY-RUNS / refuses by default (correction #5): it writes
    // ProjectSettings ONLY when DistributionConfig.Approved is true AND the values pass the
    // gate. Preflight and the builder never call this. No secrets are handled here.
    public static class JebbyJumpDistributionConfig
    {
        // The approved tracked values. Edit these + set Approved = true (with reviewed
        // values) to enable an actual write; the default is the current baseline, so an
        // unedited run is a DRY-RUN no-op.
        public static DistributionConfig Approved = DistributionConfig.CurrentBaseline();

        // Highest version code already uploaded to Play (0 = first upload). External fact;
        // set when known so the gate can enforce a strict increase.
        public static int LastUploadedVersionCode = 0;

        [MenuItem("Jebby Jump/Release/Apply Approved Distribution Config")]
        public static void ApplyApprovedDistributionConfig()
        {
            var decision = DistributionConfigGate.ShouldApply(Approved, LastUploadedVersionCode);
            string targetSdk = Approved.TargetSdk == 0 ? "Automatic" : Approved.TargetSdk.ToString();

            if (!decision.Apply)
            {
                Debug.Log($"[Distribution] {decision.Reason} (would set version={Approved.VersionName} "
                    + $"code={Approved.VersionCode} targetSdk={targetSdk}) — NOTHING written.");
                return; // dry-run / refused
            }

            PlayerSettings.bundleVersion = Approved.VersionName;
            PlayerSettings.Android.bundleVersionCode = Approved.VersionCode;
            if (Approved.TargetSdk != 0)
                PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)Approved.TargetSdk;
            Debug.Log($"[Distribution] {decision.Reason} version={Approved.VersionName} "
                + $"code={Approved.VersionCode} targetSdk={targetSdk}");
        }
    }
}
