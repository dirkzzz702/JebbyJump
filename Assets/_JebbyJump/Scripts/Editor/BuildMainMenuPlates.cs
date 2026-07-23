using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // P35D (final) — dress the Main Menu in the bespoke UI02 plates, with layout
    // measured from the approved mockup (mockup_mainmenu.png):
    //   * wordmark sized to the mockup (fixes the too-small render),
    //   * button pitch = 98 (mockup gap), positions measured,
    //   * per-plate width normalization so all buttons render identical size
    //     (the fix-pass made heights identical; this flattens the ~6% width
    //     residual to an exact match),
    //   * island placed flush under Quit (no gap), gem on the island.
    //
    // The visible plate overflows the button as a raycast-off child (decoration
    // margins nestle between buttons like the mockup); the interactive rect stays
    // frame-sized so buttons never overlap (overlap audit 0) and the plate is the
    // Selectable's targetGraphic (press still tints it). Idempotent.
    public static class BuildMainMenuPlates
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private const string MainMenuScenePath = "Assets/_JebbyJump/Scenes/MainMenu.unity";
        private static readonly Color Cocoa = new Color(0.29f, 0.19f, 0.12f);

        private const float PlateH = 168f;   // visual plate height (frame renders ~90)
        private const float FrameW = 465f;   // interactive button width
        private const float FrameH = 88f;    // interactive button height (< pitch 98)
        private const float LabelDX = 51f;   // label offset into the blank safe-zone

        // field, plate sprite, plate display WIDTH (normalizes frame to 465 on
        // screen), visual x-offset (centres the frame), button Y (pitch 98).
        private static readonly (string field, string plate, float pw, float dx, float y)[] Buttons =
        {
            ("_continueButton", "ui_plate_continue",    550f,  0f,   73f),
            ("_startButton",    "ui_plate_levelselect", 549f,  2f,  -25f),
            ("_wardrobeButton", "ui_plate_wardrobe",    589f,  3f, -123f),
            ("_settingsButton", "ui_plate_settings",    567f, -8f, -221f),
            ("_quitButton",     "ui_plate_quit",        580f, -5f, -319f),
        };

        [MenuItem("Jebby Jump/Scaffold/Build Main Menu Plates")]
        public static void Run()
        {
            foreach (var b in Buttons) EnsureImport(b.plate + ".png", readable: false);
            EnsureImport("ui_menu_island.png", readable: false);
            EnsureImport("ui_menu_gem.png", readable: false);

            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            var menu = Object.FindAnyObjectByType<MainMenuController>(FindObjectsInactive.Include);
            if (menu == null) { Debug.LogError("[Plates] No MainMenuController."); return; }
            var so = new SerializedObject(menu);

            int n = 0;
            foreach (var b in Buttons) n += Plate(so, b.field, b.plate, b.pw, b.dx, b.y);

            var safe = FindDeep(scene, "MenuSafeArea");
            if (safe != null)
            {
                foreach (var name in new[] { "MenuDecorBack", "MenuDecorFront",
                    "MenuPlatesBack", "MenuPlatesFront" })
                {
                    var old = safe.Find(name);
                    if (old != null) Object.DestroyImmediate(old.gameObject);
                }
                // Island flush under Quit (grass top at Quit frame bottom ~ -363).
                MakeDecorLayer(safe, "MenuPlatesBack", asFirst: true,
                    ("ui_menu_island", 0f, -433f, 800f));
                MakeDecorLayer(safe, "MenuPlatesFront", asFirst: false,
                    ("ui_menu_gem", 0f, -500f, 92f));
            }

            // Wordmark sized to the mockup (asset aspect 1.78; rect 817x459 renders
            // the letters ~736 wide, matching the mockup; centred at y=302).
            var wm = FindDeep(scene, "TitleWordmark");
            var wmImg = wm != null ? wm.GetComponent<Image>() : null;
            var lockup = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + "ui_wordmark_lockup.png");
            if (wm != null && wmImg != null && lockup != null)
            {
                var wrt = (RectTransform)wm.transform;
                wrt.sizeDelta = new Vector2(817f, 459f);
                wrt.anchoredPosition = new Vector2(0f, 302f);
                wmImg.sprite = lockup; wmImg.type = Image.Type.Simple;
                wmImg.preserveAspect = true; wmImg.color = Color.white;
                EditorUtility.SetDirty(wmImg);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Plates] Applied " + n + " uniform plate button(s) + island/gem + wordmark.");
        }

        private static int Plate(SerializedObject menuSo, string field, string plateName,
            float plateW, float visualDX, float posY)
        {
            var btn = menuSo.FindProperty(field)?.objectReferenceValue as Button;
            if (btn == null) return 0;
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(Dir + plateName + ".png");
            if (sprite == null) { Debug.LogWarning("[Plates] missing " + plateName); return 0; }

            // Interactive button: frame-sized, positioned by the measured pitch.
            var rt = (RectTransform)btn.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(FrameW, FrameH);
            rt.anchoredPosition = new Vector2(0f, posY);

            var clickImg = btn.image != null ? btn.image : btn.GetComponent<Image>();
            if (clickImg != null)
            {
                clickImg.sprite = null;
                clickImg.color = new Color(1f, 1f, 1f, 0f);
                clickImg.raycastTarget = true;
            }

            // Visible plate: overflows the button; width-normalized per plate.
            var visualTr = btn.transform.Find("PlateVisual") as RectTransform;
            if (visualTr == null)
            {
                var go = new GameObject("PlateVisual", typeof(RectTransform), typeof(Image));
                visualTr = (RectTransform)go.transform;
                visualTr.SetParent(btn.transform, false);
            }
            visualTr.SetAsFirstSibling();
            visualTr.anchorMin = visualTr.anchorMax = new Vector2(0.5f, 0.5f);
            visualTr.pivot = new Vector2(0.5f, 0.5f);
            visualTr.sizeDelta = new Vector2(plateW, PlateH);
            visualTr.anchoredPosition = new Vector2(visualDX, 0f);
            var plateImg = visualTr.GetComponent<Image>();
            plateImg.sprite = sprite;
            plateImg.type = Image.Type.Simple;
            plateImg.preserveAspect = false; // normalized W/H are intentional
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
            var kitIcon = btn.transform.Find("KitIcon");
            if (kitIcon != null) Object.DestroyImmediate(kitIcon.gameObject);

            var label = btn.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                var lrt = label.rectTransform;
                lrt.anchorMin = lrt.anchorMax = new Vector2(0.5f, 0.5f);
                lrt.pivot = new Vector2(0.5f, 0.5f);
                lrt.sizeDelta = new Vector2(340f, 70f);
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
