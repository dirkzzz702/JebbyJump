using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // P35A step 1 — Main Menu "jelly" button chrome. Swaps the flat cocoa pill
    // for a neutral glossy pill (ui_btn_jelly_9s) tinted per role via the
    // Selectable ColorBlock (sprite is greyscale; the tint carries the colour,
    // exactly like the platform base). Warm candy palette, dark-cocoa labels for
    // legibility on the light pills, a soft drop shadow for depth, and colour
    // hierarchy: honey primary (Continue) > cream secondaries > muted Quit.
    //
    // Deliberately touches NO RectTransforms, so the UI-overlap regression tests
    // cannot move. Owns only the five MainMenuController buttons; coexists with
    // StyleTypography (which skips non-white labels and non-pill buttons) and
    // WireShellUiKit (which skips custom-art sprites). Idempotent.
    public static class StyleMenuButtons
    {
        private const string JellyPath =
            "Assets/_JebbyJump/Art/Sprites/UI/ui_btn_jelly_9s.png";
        private const string MainMenuScenePath =
            "Assets/_JebbyJump/Scenes/MainMenu.unity";

        // Role tints (multiplied over the greyscale jelly at runtime).
        private static readonly Color Primary   = new Color(1.00f, 0.78f, 0.34f); // honey
        private static readonly Color Secondary = new Color(1.00f, 0.91f, 0.80f); // cream
        private static readonly Color Quiet      = new Color(0.88f, 0.83f, 0.78f); // muted
        private static readonly Color Cocoa      = new Color(0.29f, 0.19f, 0.12f); // label ink
        private static readonly Color ShadowCol  = new Color(0.28f, 0.18f, 0.10f, 0.35f);

        // Vertical gap (canvas units) between menu buttons — the stack is
        // re-centred on its current centre, so only the spacing grows.
        private const float GapUnits = 46f;

        [MenuItem("Jebby Jump/QA/Style Menu Buttons")]
        public static void Run()
        {
            // Vector4 border order: X=left, Y=bottom, Z=right, W=top (matches the pill).
            EnsureImport(JellyPath, new Vector4(48, 40, 48, 40));
            var jelly = AssetDatabase.LoadAssetAtPath<Sprite>(JellyPath);
            if (jelly == null)
            { Debug.LogError("[StyleMenuButtons] jelly sprite missing/not imported"); return; }

            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            var menu = Object.FindAnyObjectByType<MainMenuController>(FindObjectsInactive.Include);
            if (menu == null)
            { Debug.LogError("[StyleMenuButtons] No MainMenuController."); return; }

            var so = new SerializedObject(menu);
            int n = 0;
            n += Apply(so, "_continueButton", jelly, Primary,   labelBump: 6f);
            n += Apply(so, "_startButton",    jelly, Secondary, labelBump: 0f);
            n += Apply(so, "_wardrobeButton", jelly, Secondary, labelBump: 0f);
            n += Apply(so, "_settingsButton", jelly, Secondary, labelBump: 0f);
            n += Apply(so, "_quitButton",     jelly, Quiet,     labelBump: 0f);

            ReSpace(so, new[] { "_continueButton", "_startButton", "_wardrobeButton",
                "_settingsButton", "_quitButton" });

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[StyleMenuButtons] styled " + n + " menu button(s) with the jelly kit.");
        }

        // Re-space the button stack: keep its current vertical centre, but set a
        // uniform GapUnits between consecutive buttons. Order is preserved by
        // current Y (top -> bottom). Only anchoredPosition.y changes.
        private static void ReSpace(SerializedObject menuSo, string[] fields)
        {
            var rts = new System.Collections.Generic.List<RectTransform>();
            foreach (var f in fields)
            {
                var b = menuSo.FindProperty(f)?.objectReferenceValue as Button;
                if (b != null) rts.Add((RectTransform)b.transform);
            }
            if (rts.Count < 2) return;

            rts.Sort((a, b) => b.anchoredPosition.y.CompareTo(a.anchoredPosition.y)); // top first
            float centreY = 0f;
            foreach (var rt in rts) centreY += rt.anchoredPosition.y;
            centreY /= rts.Count;

            float totalH = 0f;
            foreach (var rt in rts) totalH += rt.rect.height;
            totalH += GapUnits * (rts.Count - 1);

            float cursor = centreY + totalH / 2f; // top edge of the stack
            foreach (var rt in rts)
            {
                float h = rt.rect.height;
                float y = cursor - h / 2f;
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
                cursor -= h + GapUnits;
            }
        }

        private static int Apply(SerializedObject menuSo, string field,
            Sprite jelly, Color tint, float labelBump)
        {
            var btn = menuSo.FindProperty(field)?.objectReferenceValue as Button;
            if (btn == null) return 0;

            var img = btn.image != null ? btn.image : btn.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = jelly;
                img.type = Image.Type.Sliced;
                img.color = Color.white; // base white; the ColorBlock carries the tint
            }

            btn.transition = Selectable.Transition.ColorTint;
            var cb = btn.colors;
            cb.normalColor      = tint;
            cb.highlightedColor = Scale(tint, 1.06f);
            cb.pressedColor     = Scale(tint, 0.86f);
            cb.selectedColor    = Scale(tint, 1.06f);
            cb.disabledColor    = new Color(tint.r * 0.7f, tint.g * 0.7f, tint.b * 0.7f, 0.5f);
            cb.colorMultiplier  = 1f;
            cb.fadeDuration     = 0.1f;
            btn.colors = cb;

            // Soft drop shadow for grounding (no RectTransform change).
            var shadow = btn.GetComponent<Shadow>();
            if (shadow == null) shadow = btn.gameObject.AddComponent<Shadow>();
            shadow.effectColor = ShadowCol;
            shadow.effectDistance = new Vector2(0f, -5f);
            shadow.useGraphicAlpha = true;

            // Label: dark cocoa ink on the light pill (survives StyleTypography,
            // which only re-lightens still-white labels).
            var tmp = btn.GetComponentInChildren<TMP_Text>(true);
            if (tmp != null)
            {
                tmp.enableVertexGradient = false;
                tmp.color = Cocoa;
                if ((tmp.fontStyle & FontStyles.Bold) == 0) tmp.fontStyle |= FontStyles.Bold;
                if (labelBump > 0f) tmp.fontSize += labelBump;
                EditorUtility.SetDirty(tmp);
            }
            EditorUtility.SetDirty(btn);
            return 1;
        }

        private static Color Scale(Color c, float m)
            => new Color(Mathf.Clamp01(c.r * m), Mathf.Clamp01(c.g * m), Mathf.Clamp01(c.b * m), c.a);

        private static void EnsureImport(string path, Vector4 border)
        {
            var imp = AssetImporter.GetAtPath(path) as TextureImporter;
            if (imp == null) return;
            var s = new TextureImporterSettings();
            imp.ReadTextureSettings(s);
            bool changed = false;
            if (s.textureType != TextureImporterType.Sprite)
            { s.textureType = TextureImporterType.Sprite; changed = true; }
            if (s.spriteMode != (int)SpriteImportMode.Single)
            { s.spriteMode = (int)SpriteImportMode.Single; changed = true; }
            if (s.spriteMeshType != SpriteMeshType.FullRect)
            { s.spriteMeshType = SpriteMeshType.FullRect; changed = true; }
            if (s.spriteBorder != border)
            { s.spriteBorder = border; changed = true; }
            if (!s.alphaIsTransparency)
            { s.alphaIsTransparency = true; changed = true; }
            if (changed) { imp.SetTextureSettings(s); imp.SaveAndReimport(); }
        }
    }
}
