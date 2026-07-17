using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // Art batch 004 integration (ART-006): wires the main-menu background into
    // MainMenu.unity as the FIRST sibling under MainMenuCanvas (everything else
    // renders above it; the P33 stack-order contract is preserved because all
    // siblings shift uniformly). Envelope-cover scaling via AspectRatioFitter
    // so 16:9..20:9 crop instead of letterboxing. Idempotent.
    public static class WireMenuBackground
    {
        private const string ScenePath = "Assets/_JebbyJump/Scenes/MainMenu.unity";
        private const string SpritePath =
            "Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png";
        private const string BgName = "MenuBackground";
        private const float Aspect = 2400f / 1080f;

        [MenuItem("Jebby Jump/Release/Wire Menu Background")]
        public static void Run()
        {
            EnsureImport();
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(SpritePath);
            if (sprite == null)
            {
                Debug.LogError("[WireMenuBackground] background not imported as Sprite: " + SpritePath);
                return;
            }

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject canvasGo = null;
            foreach (var root in scene.GetRootGameObjects())
                if (root.name == "MainMenuCanvas") { canvasGo = root; break; }
            if (canvasGo == null)
            {
                Debug.LogError("[WireMenuBackground] MainMenuCanvas not found");
                return;
            }

            var existing = canvasGo.transform.Find(BgName);
            var go = existing != null ? existing.gameObject
                : new GameObject(BgName, typeof(RectTransform), typeof(CanvasRenderer),
                    typeof(Image), typeof(AspectRatioFitter));
            if (existing == null) go.transform.SetParent(canvasGo.transform, false);

            var rt = go.transform as RectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.SetSiblingIndex(0); // behind everything

            var img = go.GetComponent<Image>();
            img.sprite = sprite;
            img.raycastTarget = false;
            img.color = Color.white;

            var fitter = go.GetComponent<AspectRatioFitter>();
            if (fitter == null) fitter = go.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = Aspect;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[WireMenuBackground] background wired at sibling 0 "
                + "(envelope-cover " + Aspect.ToString("F3") + "); saved MainMenu.unity");
        }

        private static void EnsureImport()
        {
            var imp = AssetImporter.GetAtPath(SpritePath) as TextureImporter;
            if (imp == null) return;
            bool changed = false;
            if (imp.textureType != TextureImporterType.Sprite)
            { imp.textureType = TextureImporterType.Sprite; changed = true; }
            if (imp.spriteImportMode != SpriteImportMode.Single)
            { imp.spriteImportMode = SpriteImportMode.Single; changed = true; }
            // Opaque full-screen art: cap memory (2400 wide source).
            if (imp.maxTextureSize < 2048)
            { imp.maxTextureSize = 2048; changed = true; }
            if (changed) imp.SaveAndReimport();
        }
    }
}
