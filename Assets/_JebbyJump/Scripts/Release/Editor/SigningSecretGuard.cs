using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.Release
{
    // P32 hardening: detects an Android signing PASSWORD accidentally persisted in
    // ProjectSettings.asset. An env-upload build applies the password to PlayerSettings
    // transiently and restores it in a finally; this guard catches the rare case where an
    // interrupted build left a secret on disk before it could be committed. The detector is
    // pure (EditMode-testable); the menu item runs it against the real ProjectSettings.asset.
    public static class SigningSecretGuard
    {
        // True if the ProjectSettings YAML has a non-empty AndroidKeystorePass or
        // AndroidKeyaliasPass value on its own line.
        public static bool ProjectSettingsHasSigningSecret(string projectSettingsYaml)
        {
            if (string.IsNullOrEmpty(projectSettingsYaml)) return false;
            return HasNonEmptyValue(projectSettingsYaml, "AndroidKeystorePass")
                || HasNonEmptyValue(projectSettingsYaml, "AndroidKeyaliasPass");
        }

        // Matches "<key>:<horizontal-space>*<value?>" on a single line; value must contain a
        // non-space char. Uses [^\S\r\n] (not \s) so it can't span into the next line.
        private static bool HasNonEmptyValue(string yaml, string key)
        {
            var m = Regex.Match(yaml, key + @":[^\S\r\n]*(\S.*)?$", RegexOptions.Multiline);
            return m.Success && m.Groups[1].Value.Trim().Length > 0;
        }

        [MenuItem("Jebby Jump/Release/Scan ProjectSettings For Signing Secrets")]
        public static void ScanProjectSettings()
        {
            string path = Path.GetFullPath(Path.Combine(
                Application.dataPath, "..", "ProjectSettings/ProjectSettings.asset"));
            if (!File.Exists(path))
            {
                Debug.LogWarning("[SigningSecretGuard] ProjectSettings.asset not found.");
                return;
            }
            bool leak = ProjectSettingsHasSigningSecret(File.ReadAllText(path));
            if (leak)
                Debug.LogError("[SigningSecretGuard] ProjectSettings.asset contains a NON-EMPTY signing "
                    + "password — DO NOT COMMIT. Revert ProjectSettings.asset and re-run.");
            else
                Debug.Log("[SigningSecretGuard] OK — no signing password persisted in ProjectSettings.asset.");
        }
    }
}
