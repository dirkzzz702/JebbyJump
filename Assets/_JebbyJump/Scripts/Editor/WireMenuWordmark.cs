using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // Art batch 002 integration (ART-010): wires the accepted "Jebby Jump"
    // wordmark image into the Main Menu title slot, replacing the plain TMP
    // title text. Idempotent + UI-only (adds one raycast-off Image sized to the
    // title slot, disables TitleText). Inserts the Image right after TitleText
    // so it stays BEFORE MenuSafeArea + the modal panels (preserves the P33
    // modality/sibling-order contract). The wordmark is a brand CANDIDATE:
    // store/public use still needs human legal sign-off; this only wires it into
    // the in-game title.
    public static class WireMenuWordmark
    {
        private const string ScenePath = "Assets/_JebbyJump/Scenes/MainMenu.unity";
        private const string SpritePath =
            "Assets/_JebbyJump/Art/Sprites/UI/ui_wordmark_jebbyjump.png";
        private const string WordmarkName = "TitleWordmark";

        [MenuItem("Jebby Jump/Release/Wire Menu Wordmark")]
        public static void Run()
        {
            EnsureSpriteImport();
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
            if (sprite == null)
            {
                Debug.LogError("[WireMenuWordmark] wordmark not imported as Sprite: " + SpritePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var title = FindInScene(scene, "TitleText");
            if (title == null)
            {
                Debug.LogError("[WireMenuWordmark] TitleText not found in MainMenu");
                return;
            }
            var titleRT = title.transform as RectTransform;
            var parent = titleRT.parent as RectTransform;

            var existing = FindInScene(scene, WordmarkName);
            var go = existing != null
                ? existing
                : new GameObject(WordmarkName,
                    typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            if (existing == null) go.transform.SetParent(parent, false);

            var rt = go.transform as RectTransform;
            rt.anchorMin = titleRT.anchorMin;
            rt.anchorMax = titleRT.anchorMax;
            rt.pivot = titleRT.pivot;
            rt.anchoredPosition = titleRT.anchoredPosition;
            // 3:1 wordmark preserved-aspect at the ~700-unit title width.
            rt.sizeDelta = new Vector2(700f, 233f);
            // Right after TitleText -> still earlier than MenuSafeArea + panels.
            rt.SetSiblingIndex(titleRT.GetSiblingIndex() + 1);

            var img = go.GetComponent<Image>();
            img.sprite = sprite;
            img.preserveAspect = true;
            img.raycastTarget = false;
            img.color = Color.white;

            title.SetActive(false); // plain TMP title replaced by the wordmark image

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[WireMenuWordmark] wordmark wired into Main Menu title slot; "
                + "TitleText disabled. (CANDIDATE - store/public use pending legal sign-off.)");
        }

        private static void EnsureSpriteImport()
        {
            var imp = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
            if (imp == null) return;
            bool changed = false;
            if (imp.textureType != TextureImporterType.Sprite)
            {
                imp.textureType = TextureImporterType.Sprite;
                changed = true;
            }
            if (imp.spriteImportMode != SpriteImportMode.Single)
            {
                imp.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }
            if (imp.alphaIsTransparency == false)
            {
                imp.alphaIsTransparency = true;
                changed = true;
            }
            if (changed) imp.SaveAndReimport();
        }

        private static GameObject FindInScene(UnityEngine.SceneManagement.Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name) return root;
                var f = FindInChildren(root.transform, name);
                if (f != null) return f.gameObject;
            }
            return null;
        }

        private static Transform FindInChildren(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var c = t.GetChild(i);
                if (c.name == name) return c;
                var r = FindInChildren(c, name);
                if (r != null) return r;
            }
            return null;
        }
    }
}
