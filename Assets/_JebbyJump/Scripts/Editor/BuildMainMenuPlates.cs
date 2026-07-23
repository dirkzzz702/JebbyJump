using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // P35D — replace the Main Menu's generic frame+icon+overlay buttons with the
    // bespoke UI02 plates (each button is one complete art piece: clean rounded
    // frame + baked icon + clouds/vines/flowers, blank label zone). The plate is
    // the button's own graphic; alpha hit-testing keeps only the opaque frame
    // clickable (transparent decoration margins nestle between buttons like the
    // mockup without stealing clicks). Places the new island base + gem, and
    // retires the prior KitIcon children + MenuDecor overlay layers.
    //
    // Idempotent. Fixed-size plates (the mockup is a fixed layout), so buttons are
    // no longer width-flexible - by design.
    public static class BuildMainMenuPlates
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private const string MainMenuScenePath = "Assets/_JebbyJump/Scenes/MainMenu.unity";
        private static readonly Color Cocoa = new Color(0.29f, 0.19f, 0.12f);

        // Plate display size (canvas 1280x384 -> same 3.333:1). The visual plate
        // overflows the button; the interactive button stays frame-sized so it
        // never overlaps its neighbours (decoration margins nestle harmlessly).
        private const float PlateW = 600f, PlateH = 180f;   // visual (child)
        private const float FrameW = 490f, FrameH = 100f;   // interactive button rect
        // Label safe-zone centre offset from plate centre (zone x390-1120 of 1280).
        private const float LabelDX = 54f;

        [MenuItem("Jebby Jump/Scaffold/Build Main Menu Plates")]
        public static void Run()
        {
            foreach (var p in new[] { "ui_plate_continue", "ui_plate_levelselect",
                "ui_plate_wardrobe", "ui_plate_settings", "ui_plate_quit" })
                EnsureImport(p + ".png", readable: true);
            EnsureImport("ui_menu_island.png", readable: false);
            EnsureImport("ui_menu_gem.png", readable: false);

            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            var menu = Object.FindAnyObjectByType<MainMenuController>(FindObjectsInactive.Include);
            if (menu == null) { Debug.LogError("[Plates] No MainMenuController."); return; }
            var so = new SerializedObject(menu);

            int n = 0;
            n += Plate(so, "_continueButton", "ui_plate_continue");
            n += Plate(so, "_startButton",    "ui_plate_levelselect");
            n += Plate(so, "_wardrobeButton", "ui_plate_wardrobe");
            n += Plate(so, "_settingsButton", "ui_plate_settings");
            n += Plate(so, "_quitButton",     "ui_plate_quit");

            // Retire the old overlay decor layers (plates + island/gem replace them).
            var safe = FindDeep(scene, "MenuSafeArea");
            if (safe != null)
            {
                foreach (var name in new[] { "MenuDecorBack", "MenuDecorFront",
                    "MenuPlatesBack", "MenuPlatesFront" })
                {
                    var old = safe.Find(name);
                    if (old != null) Object.DestroyImmediate(old.gameObject);
                }
                MakeDecorLayer(safe, "MenuPlatesBack", asFirst: true,
                    ("ui_menu_island", 0f, -470f, 820f));
                MakeDecorLayer(safe, "MenuPlatesFront", asFirst: false,
                    ("ui_menu_gem", 0f, -505f, 92f));
            }

            // Wordmark lockup as the title (keeps this the single menu builder).
            var wm = FindDeep(scene, "TitleWordmark");
            var wmImg = wm != null ? wm.GetComponent<Image>() : null;
            var lockup = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + "ui_wordmark_lockup.png");
            if (wmImg != null && lockup != null && wmImg.sprite != lockup)
            {
                wmImg.sprite = lockup; wmImg.type = Image.Type.Simple;
                wmImg.preserveAspect = true; wmImg.color = Color.white;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Plates] Applied " + n + " plate button(s) + island/gem.");
        }

        private static int Plate(SerializedObject menuSo, string field, string plateName)
        {
            var btn = menuSo.FindProperty(field)?.objectReferenceValue as Button;
            if (btn == null) return 0;
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + plateName + ".png");
            if (sprite == null) { Debug.LogWarning("[Plates] missing " + plateName); return 0; }

            // Interactive button rect = frame-sized (never overlaps neighbours).
            var rt = (RectTransform)btn.transform;
            rt.sizeDelta = new Vector2(FrameW, FrameH);

            // The button's own graphic becomes an invisible click surface.
            var clickImg = btn.image != null ? btn.image : btn.GetComponent<Image>();
            if (clickImg != null)
            {
                clickImg.sprite = null;
                clickImg.color = new Color(1f, 1f, 1f, 0f); // transparent
                clickImg.raycastTarget = true;
            }

            // The visible plate is a child that overflows the button rect (so the
            // decoration margins nestle between buttons), raycast off. It is the
            // Selectable's targetGraphic, so press-tint still animates it.
            var visualTr = btn.transform.Find("PlateVisual") as RectTransform;
            if (visualTr == null)
            {
                var go = new GameObject("PlateVisual", typeof(RectTransform), typeof(Image));
                visualTr = (RectTransform)go.transform;
                visualTr.SetParent(btn.transform, false);
            }
            visualTr.SetAsFirstSibling(); // behind the label
            visualTr.anchorMin = visualTr.anchorMax = new Vector2(0.5f, 0.5f);
            visualTr.pivot = new Vector2(0.5f, 0.5f);
            visualTr.sizeDelta = new Vector2(PlateW, PlateH);
            visualTr.anchoredPosition = Vector2.zero;
            var plateImg = visualTr.GetComponent<Image>();
            plateImg.sprite = sprite;
            plateImg.type = Image.Type.Simple;
            plateImg.preserveAspect = true;
            plateImg.color = Color.white;
            plateImg.raycastTarget = false;

            btn.transition = Selectable.Transition.ColorTint;
            btn.targetGraphic = plateImg;
            var cb = btn.colors;
            cb.normalColor = Color.white; cb.highlightedColor = Color.white;
            cb.pressedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
            cb.selectedColor = Color.white;
            cb.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
            cb.colorMultiplier = 1f; cb.fadeDuration = 0.1f;
            btn.colors = cb;

            var shadow = btn.GetComponent<Shadow>();
            if (shadow != null) Object.DestroyImmediate(shadow);

            // Icon is baked into the plate now - remove the separate KitIcon.
            var kitIcon = btn.transform.Find("KitIcon");
            if (kitIcon != null) Object.DestroyImmediate(kitIcon.gameObject);

            // Label centred in the blank safe-zone (right of the baked icon).
            var label = btn.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                var lrt = label.rectTransform;
                lrt.anchorMin = lrt.anchorMax = new Vector2(0.5f, 0.5f);
                lrt.pivot = new Vector2(0.5f, 0.5f);
                lrt.sizeDelta = new Vector2(340f, 96f);
                lrt.anchoredPosition = new Vector2(LabelDX, 0f);
                label.alignment = TextAlignmentOptions.Center;
                label.enableAutoSizing = false;
                label.color = Cocoa;
                EditorUtility.SetDirty(label);
            }
            EditorUtility.SetDirty(btn);
            return 1;
        }

        private static void MakeDecorLayer(Transform parent, string name, bool asFirst,
            params (string n, float x, float y, float w)[] items)
        {
            var layer = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            layer.SetParent(parent, false);
            layer.anchorMin = Vector2.zero; layer.anchorMax = Vector2.one;
            layer.offsetMin = Vector2.zero; layer.offsetMax = Vector2.zero;
            if (asFirst) layer.SetAsFirstSibling(); else layer.SetAsLastSibling();
            foreach (var it in items)
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + it.n + ".png");
                if (sprite == null) continue;
                var go = new GameObject(it.n, typeof(RectTransform), typeof(Image));
                var rt = (RectTransform)go.transform;
                rt.SetParent(layer, false);
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                float aspect = sprite.rect.height / sprite.rect.width;
                rt.sizeDelta = new Vector2(it.w, it.w * aspect);
                rt.anchoredPosition = new Vector2(it.x, it.y);
                var img = go.GetComponent<Image>();
                img.sprite = sprite; img.preserveAspect = true; img.raycastTarget = false;
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

        private static void EnsureImport(string file, bool readable)
        {
            string path = Dir + file;
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) return;
            bool changed = false;
            if (imp.textureType != TextureImporterType.Sprite)
            { imp.textureType = TextureImporterType.Sprite; changed = true; }
            if (imp.spriteImportMode != SpriteImportMode.Single)
            { imp.spriteImportMode = SpriteImportMode.Single; changed = true; }
            if (imp.alphaIsTransparency != true) { imp.alphaIsTransparency = true; changed = true; }
            if (imp.isReadable != readable) { imp.isReadable = readable; changed = true; }
            if (changed) imp.SaveAndReimport();
        }
    }
}
