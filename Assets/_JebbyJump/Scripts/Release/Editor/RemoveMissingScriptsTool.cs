using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JebbyJump.Release
{
    // Explicit, separate scene-hygiene command (NOT part of the read-only preflight):
    // removes orphaned missing-script components from the release build scenes, using
    // Unity's canonical API (no hand-edited YAML). Used to clean the long-orphaned
    // BackgroundFollower reference flagged by the RC warning gate. Behavior-neutral:
    // missing scripts are null at runtime and do nothing.
    public static class RemoveMissingScriptsTool
    {
        [MenuItem("Jebby Jump/Release/Remove Missing Scripts In Build Scenes")]
        public static void Run()
        {
            int total = 0;
            foreach (var scenePath in ReleaseSceneContract.Scenes)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                int removed = 0;
                foreach (var root in scene.GetRootGameObjects())
                    removed += RemoveRecursive(root);
                if (removed > 0)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }
                total += removed;
                Debug.Log($"[Release] {scenePath}: removed {removed} missing-script component(s).");
            }
            Debug.Log($"[Release] Removed {total} missing-script component(s) total.");
        }

        private static int RemoveRecursive(GameObject go)
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            foreach (Transform child in go.transform)
                removed += RemoveRecursive(child.gameObject);
            return removed;
        }
    }
}
