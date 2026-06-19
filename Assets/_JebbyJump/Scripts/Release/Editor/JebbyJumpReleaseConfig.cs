using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace JebbyJump.Release
{
    // The ONLY writer of tracked build config (fix #4): sets the approved identity
    // + the build scene list from the immutable contract. Explicit, separate
    // command - preflight and the builder never call this; they only validate and
    // fail on drift. Idempotent.
    public static class JebbyJumpReleaseConfig
    {
        [MenuItem("Jebby Jump/Release/Apply Approved Build Config")]
        public static void ApplyApprovedBuildConfig()
        {
            PlayerSettings.companyName = ReleaseIdentity.CompanyName;
            PlayerSettings.productName = ReleaseIdentity.ProductName;
            PlayerSettings.SetApplicationIdentifier(
                NamedBuildTarget.Android, ReleaseIdentity.ApplicationIdentifier);
            PlayerSettings.SetApplicationIdentifier(
                NamedBuildTarget.Standalone, ReleaseIdentity.ApplicationIdentifier);
            PlayerSettings.SetApplicationIdentifier(
                NamedBuildTarget.iOS, ReleaseIdentity.ApplicationIdentifier);

            var paths = ReleaseSceneContract.Scenes;
            var scenes = new EditorBuildSettingsScene[paths.Length];
            for (int i = 0; i < paths.Length; i++)
                scenes[i] = new EditorBuildSettingsScene(paths[i], true);
            EditorBuildSettings.scenes = scenes; // in-process correctness
            AssetDatabase.SaveAssets();

            // The EditorBuildSettings asset does not reliably flush to disk in
            // batchmode (Unity 6 Build Profiles shadow the legacy scene list), so
            // persist the m_Scenes block directly to guarantee the committed
            // config + a fresh-process preflight see the right scenes.
            PersistSceneListToDisk();

            Debug.Log("[Release] Applied approved build config: identity "
                + $"({ReleaseIdentity.CompanyName} / {ReleaseIdentity.ApplicationIdentifier}) "
                + $"+ scene list [{string.Join(", ", paths)}].");
        }

        private static void PersistSceneListToDisk()
        {
            string path = Path.GetFullPath(Path.Combine(
                Application.dataPath, "..", "ProjectSettings/EditorBuildSettings.asset"));
            if (!File.Exists(path)) { Debug.LogWarning("[Release] EditorBuildSettings.asset not found."); return; }

            var block = new StringBuilder();
            block.Append("  m_Scenes:\n");
            foreach (var scenePath in ReleaseSceneContract.Scenes)
            {
                string guid = AssetDatabase.AssetPathToGUID(scenePath);
                block.Append("  - enabled: 1\n");
                block.Append($"    path: {scenePath}\n");
                block.Append($"    guid: {guid}\n");
            }

            string text = File.ReadAllText(path);
            // Replace the m_Scenes block (everything from "  m_Scenes:" up to the
            // next top-level "  m_configObjects:" key), preserving the rest.
            var rx = new Regex("  m_Scenes:\\r?\\n(?:.*\\r?\\n)*?(?=  m_configObjects:)");
            if (rx.IsMatch(text))
            {
                File.WriteAllText(path, rx.Replace(text, block.ToString(), 1));
                Debug.Log("[Release] Persisted EditorBuildSettings m_Scenes to disk.");
            }
            else
            {
                Debug.LogWarning("[Release] Could not locate m_Scenes/m_configObjects block; "
                    + "left EditorBuildSettings.asset unchanged.");
            }
        }
    }
}
