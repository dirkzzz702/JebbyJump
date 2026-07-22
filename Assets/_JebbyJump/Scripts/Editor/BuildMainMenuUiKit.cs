using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // P35B — dress the Main Menu in the UI01 theme kit (the approved concept):
    // solid 9-slice button frames (honey primary + cream secondaries), a left
    // icon per button, Fredoka labels centred in the remaining space, and the
    // full wordmark lockup as the title. Supersedes the interim jelly styling
    // (StyleMenuButtons): swaps the sprite, resets the ColorBlock to a white
    // base (the frame art carries the colour), and removes the interim Shadow.
    //
    // Touches only sprites/colours/child-icons + the label rect INSIDE each
    // button; button RectTransforms and positions are unchanged, so the
    // UI-overlap regression stays at 0. Idempotent.
    public static class BuildMainMenuUiKit
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private const string MainMenuScenePath = "Assets/_JebbyJump/Scenes/MainMenu.unity";

        private static readonly Color Cocoa = new Color(0.29f, 0.19f, 0.12f);

        [MenuItem("Jebby Jump/Scaffold/Build Main Menu UI Kit")]
        public static void Run()
        {
            // Vector4 border = (left, bottom, right, top). Corner radius ~63/83.
            EnsureImport("ui_btn_primary_9s.png",   new Vector4(68, 68, 68, 68));
            EnsureImport("ui_btn_secondary_9s.png", new Vector4(68, 68, 68, 68));
            EnsureImport("ui_panel_frame_9s.png",   new Vector4(88, 88, 88, 88));
            foreach (var s in new[] { "ui_icon_continue_star", "ui_icon_levelselect_island",
                "ui_icon_wardrobe_hanger", "ui_icon_settings_gear", "ui_icon_quit_door",
                "ui_wordmark_lockup", "ui_deco_cloud_corner", "ui_deco_vine_corner",
                "ui_deco_island_base", "ui_deco_gem" })
                EnsureImport(s + ".png", Vector4.zero);

            var primary = Sprite("ui_btn_primary_9s");
            var secondary = Sprite("ui_btn_secondary_9s");
            if (primary == null || secondary == null)
            { Debug.LogError("[MenuKit] frame sprites missing"); return; }

            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            var menu = Object.FindAnyObjectByType<MainMenuController>(FindObjectsInactive.Include);
            if (menu == null) { Debug.LogError("[MenuKit] No MainMenuController."); return; }
            var so = new SerializedObject(menu);

            int n = 0;
            n += Style(so, "_continueButton", primary,   "ui_icon_continue_star");
            n += Style(so, "_startButton",    secondary, "ui_icon_levelselect_island");
            n += Style(so, "_wardrobeButton", secondary, "ui_icon_wardrobe_hanger");
            n += Style(so, "_settingsButton", secondary, "ui_icon_settings_gear");
            n += Style(so, "_quitButton",     secondary, "ui_icon_quit_door");

            // Wordmark lockup as the title.
            var wm = FindByName(scene, "TitleWordmark");
            var wmImg = wm != null ? wm.GetComponent<Image>() : null;
            var lockup = Sprite("ui_wordmark_lockup");
            if (wmImg != null && lockup != null)
            {
                wmImg.sprite = lockup;
                wmImg.type = Image.Type.Simple;
                wmImg.preserveAspect = true;
                wmImg.color = Color.white;
                EditorUtility.SetDirty(wmImg);
                n++;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[MenuKit] Applied UI01 kit to " + n + " element(s).");
        }

        private static int Style(SerializedObject menuSo, string field, Sprite frame, string iconName)
        {
            var btn = menuSo.FindProperty(field)?.objectReferenceValue as Button;
            if (btn == null) return 0;

            var img = btn.image != null ? btn.image : btn.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = frame;
                img.type = Image.Type.Sliced;
                img.color = Color.white; // frame art carries the colour
            }

            // White-based ColorBlock (press darkens for feedback).
            btn.transition = Selectable.Transition.ColorTint;
            var cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
            cb.pressedColor = new Color(0.86f, 0.86f, 0.86f, 1f);
            cb.selectedColor = Color.white;
            cb.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
            cb.colorMultiplier = 1f; cb.fadeDuration = 0.1f;
            btn.colors = cb;

            // Drop the interim jelly shadow (the frame carries its own depth).
            var shadow = btn.GetComponent<Shadow>();
            if (shadow != null) Object.DestroyImmediate(shadow);

            // Left icon (child "KitIcon"), created once, reused after.
            var iconTr = btn.transform.Find("KitIcon") as RectTransform;
            if (iconTr == null)
            {
                var go = new GameObject("KitIcon", typeof(RectTransform), typeof(Image));
                iconTr = (RectTransform)go.transform;
                iconTr.SetParent(btn.transform, false);
            }
            iconTr.anchorMin = iconTr.anchorMax = new Vector2(0f, 0.5f);
            iconTr.pivot = new Vector2(0.5f, 0.5f);
            iconTr.sizeDelta = new Vector2(76f, 76f);
            iconTr.anchoredPosition = new Vector2(66f, 0f);
            var iconImg = iconTr.GetComponent<Image>();
            iconImg.sprite = Sprite(iconName);
            iconImg.preserveAspect = true;
            iconImg.raycastTarget = false;
            iconImg.color = Color.white;

            // Label centred in the space right of the icon.
            var label = btn.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                var lrt = label.rectTransform;
                lrt.anchorMin = new Vector2(0f, 0f);
                lrt.anchorMax = new Vector2(1f, 1f);
                lrt.offsetMin = new Vector2(120f, 0f); // clear the icon
                lrt.offsetMax = new Vector2(-40f, 0f);
                label.alignment = TextAlignmentOptions.Center;
                label.enableAutoSizing = false;
                label.color = Cocoa;
                EditorUtility.SetDirty(label);
            }
            EditorUtility.SetDirty(btn);
            return 1;
        }

        private static Sprite Sprite(string name)
            => AssetDatabase.LoadAssetAtPath<Sprite>(Dir + name + ".png");

        private static GameObject FindByName(UnityEngine.SceneManagement.Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name) return root;
                var t = FindDeep(root.transform, name);
                if (t != null) return t.gameObject;
            }
            return null;
        }

        private static Transform FindDeep(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var c = t.GetChild(i);
                if (c.name == name) return c;
                var r = FindDeep(c, name);
                if (r != null) return r;
            }
            return null;
        }

        private static void EnsureImport(string file, Vector4 border)
        {
            string path = Dir + file;
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
            if (!s.alphaIsTransparency) { s.alphaIsTransparency = true; changed = true; }
            if (s.spriteBorder != border) { s.spriteBorder = border; changed = true; }
            if (changed) { imp.SetTextureSettings(s); imp.SaveAndReimport(); }
        }
    }
}
