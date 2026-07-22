using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // P35B polish — sprinkle the UI01 decoration pieces (island base, gem, cloud
    // and vine corners) around the Main Menu button stack to approach the concept
    // mockup. Two layers under MenuSafeArea: "MenuDecorBack" (first sibling, behind
    // the buttons) and "MenuDecorFront" (last sibling, over the button edges).
    // All pieces are non-interactive (raycastTarget off) and non-text, so button
    // clicks and the UI-overlap regression are unaffected. Idempotent: both layers
    // are rebuilt fresh each run.
    //
    // Coords are MenuSafeArea-local (centre origin), tuned to the 5-button stack
    // (x=0; y 252..-292; 400x90). Positive Y is up.
    public static class BuildMainMenuDecor
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private const string MainMenuScenePath = "Assets/_JebbyJump/Scenes/MainMenu.unity";

        // name, x, y, width(px), flipX
        private static readonly (string n, float x, float y, float w, bool flip)[] Back =
        {
            ("ui_deco_island_base", 0f,   -455f, 760f, false),
            ("ui_deco_cloud_corner", -240f, -250f, 180f, false),
            ("ui_deco_cloud_corner",  240f, -250f, 180f, true),
            ("ui_deco_cloud_corner", -238f,  300f, 150f, false),
            ("ui_deco_cloud_corner",  238f,  300f, 150f, true),
        };
        private static readonly (string n, float x, float y, float w, bool flip)[] Front =
        {
            ("ui_deco_vine_corner", -232f, 175f, 235f, false),
            ("ui_deco_vine_corner",  232f, 175f, 235f, true),
            ("ui_deco_gem",           0f, -505f,  92f, false),
        };

        [MenuItem("Jebby Jump/Scaffold/Build Main Menu Decor")]
        public static void Run()
        {
            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            var safe = FindDeep(scene, "MenuSafeArea");
            if (safe == null) { Debug.LogError("[MenuDecor] MenuSafeArea not found."); return; }

            Rebuild(safe, "MenuDecorBack", Back, asFirst: true);
            Rebuild(safe, "MenuDecorFront", Front, asFirst: false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MenuDecor] Rebuilt decor: " + Back.Length + " back + " + Front.Length + " front piece(s).");
        }

        private static void Rebuild(Transform parent, string layerName,
            (string n, float x, float y, float w, bool flip)[] items, bool asFirst)
        {
            var old = parent.Find(layerName);
            if (old != null) Object.DestroyImmediate(old.gameObject);

            var layer = new GameObject(layerName, typeof(RectTransform)).GetComponent<RectTransform>();
            layer.SetParent(parent, false);
            layer.anchorMin = Vector2.zero; layer.anchorMax = Vector2.one;
            layer.offsetMin = Vector2.zero; layer.offsetMax = Vector2.zero;
            if (asFirst) layer.SetAsFirstSibling(); else layer.SetAsLastSibling();

            foreach (var it in items)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + it.n + ".png");
                if (sprite == null) { Debug.LogWarning("[MenuDecor] missing " + it.n); continue; }
                var go = new GameObject(it.n, typeof(RectTransform), typeof(Image));
                var rt = (RectTransform)go.transform;
                rt.SetParent(layer, false);
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                float aspect = sprite.rect.height / sprite.rect.width;
                rt.sizeDelta = new Vector2(it.w, it.w * aspect);
                rt.anchoredPosition = new Vector2(it.x, it.y);
                if (it.flip) rt.localScale = new Vector3(-1f, 1f, 1f);
                var img = go.GetComponent<Image>();
                img.sprite = sprite;
                img.preserveAspect = true;
                img.raycastTarget = false;
            }
        }

        private static Transform FindDeep(UnityEngine.SceneManagement.Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name) return root.transform;
                var t = Find(root.transform, name);
                if (t != null) return t;
            }
            return null;
        }

        private static Transform Find(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var c = t.GetChild(i);
                if (c.name == name) return c;
                var r = Find(c, name);
                if (r != null) return r;
            }
            return null;
        }
    }
}
