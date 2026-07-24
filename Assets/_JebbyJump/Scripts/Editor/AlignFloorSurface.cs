using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JebbyJump.EditorTools
{
    // Jebby was standing sunk in the MIDDLE of the floor band: the Floor's
    // collider top is at world y 0.25 (where the feet rest), but the floor art's
    // grass surface rendered ~0.7u higher, so the feet landed deep in the pebbles.
    // The floor sprite is only 128px tall, so the earlier "shift the art down"
    // fix couldn't close a 0.7u gap. Instead reposition the FloorVisual so its
    // grass surface meets the collider top. Visual-only (physics/spawn unchanged);
    // WorldThemeApplier only swaps the sprite, so this persists across worlds
    // (all floor arts share the row-~22 surface). Idempotent.
    public static class AlignFloorSurface
    {
        private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        // Tuned so the feet (collider top, world 0.25) sit at the grass BASE with
        // the sprouts poking up around them (grounded look, not floating on tips).
        private const float FloorVisualLocalY = 0.25f;

        [MenuItem("Jebby Jump/Release/Align Floor Surface")]
        public static void Run()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Transform fv = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                fv = root.name == "FloorVisual" ? root.transform : DeepFind(root.transform, "FloorVisual");
                if (fv != null) break;
            }
            if (fv == null) { Debug.LogError("[FloorFix] FloorVisual not found"); return; }

            var p = fv.localPosition;
            if (Mathf.Abs(p.y - FloorVisualLocalY) > 1e-4f)
            {
                fv.localPosition = new Vector3(p.x, FloorVisualLocalY, p.z);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            Debug.Log($"[FloorFix] FloorVisual localY -> {FloorVisualLocalY} (was {p.y}).");
        }

        private static Transform DeepFind(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var c = t.GetChild(i);
                if (c.name == name) return c;
                var r = DeepFind(c, name);
                if (r != null) return r;
            }
            return null;
        }
    }
}
