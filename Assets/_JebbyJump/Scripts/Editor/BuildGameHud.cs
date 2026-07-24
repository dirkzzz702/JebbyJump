using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // GUI01 HUD wiring (v2): cloud-kingdom art on the in-game HUD, laid out with
    // EXPLICIT fixed anchors so nothing stretches/letterboxes. The frame art was
    // cropped tight to its content, so sizeDelta = the visible element (aspect
    // matched, no margin box). Text is recoloured cocoa and dropped into each
    // frame's blank zone (below the top gem). Heart art rides on
    // HUDController._lifeIconSprite. All positions/sizes are tunable constants.
    public static class BuildGameHud
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private const string GameScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        private static readonly Color Cocoa = new Color(0.29f, 0.19f, 0.12f);

        // name, sprite, anchor(x,y), pivot(x,y), anchoredPos, size, textChild,
        // blankFraction (0..1 down = where the cream text zone centres), fontMax
        private struct El
        {
            public string name, sprite, text; public Vector2 anchor, pivot, pos, size;
            public float blank, font;
        }
        // Measured from mockup_ui.png (1672x941 -> 1920x1080). Pivot = centre;
        // pos = element CENTRE relative to its anchor.
        private static readonly El[] Elements =
        {
            new El{ name="LevelBadgeRoot", sprite="ui_hud_level_badge_9s",
                anchor=new Vector2(0.5f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(0,-127),
                size=new Vector2(358,218), text="LevelText", blank=0.58f, font=48 },
            new El{ name="PauseButton", sprite="ui_hud_pause_btn",
                anchor=new Vector2(1f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(-106,-111),
                size=new Vector2(115,112), text=null, blank=0.5f, font=0 },
            new El{ name="TutorialHintRoot", sprite="ui_hint_banner_9s",
                anchor=new Vector2(0.5f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(0,-319),
                size=new Vector2(599,184), text="TutorialHintText", blank=0.62f, font=44 },
        };

        [MenuItem("Jebby Jump/Scaffold/Build Game HUD")]
        public static void Run()
        {
            foreach (var s in new[] { "ui_hud_level_badge_9s", "ui_hud_timer_banner_9s",
                "ui_hud_pause_btn", "ui_hint_banner_9s", "ui_hud_heart_01" })
                EnsureSprite(s + ".png");

            var scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);

            foreach (var e in Elements) Place(scene, e);
            WireTimer(scene);

            // Remove leftovers that clash with the new art:
            Hide(scene, "LevelText", "Backdrop");   // the "mask" behind the level text
            Hide(scene, "PauseButton", "Label");    // old "||" glyph (baked into the art now)

            var hud = Object.FindAnyObjectByType<HUDController>(FindObjectsInactive.Include);
            if (hud != null)
            {
                var so = new SerializedObject(hud);
                var p = so.FindProperty("_lifeIconSprite");
                var heart = Sprite("ui_hud_heart_01");
                if (p != null && heart != null && p.objectReferenceValue != heart)
                { p.objectReferenceValue = heart; so.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(hud); }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[GameHUD] wired badge/timer/pause/hearts/hint (fixed anchors, cropped art).");
        }

        private static void Place(UnityEngine.SceneManagement.Scene s, El e)
        {
            var go = FindDeep(s, e.name);
            if (go == null) { Debug.LogWarning("[GameHUD] missing " + e.name); return; }
            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = e.anchor;
            rt.pivot = e.pivot;
            rt.sizeDelta = e.size;
            rt.anchoredPosition = e.pos;

            var img = go.GetComponent<Image>() ?? go.AddComponent<Image>();
            var sp = Sprite(e.sprite);
            if (sp != null) { img.sprite = sp; img.type = Image.Type.Simple; img.preserveAspect = true; img.color = Color.white; EditorUtility.SetDirty(img); }

            if (e.text != null)
            {
                var t = FindDeep(s, e.text);
                var tmp = t != null ? t.GetComponent<TMP_Text>() : null;
                if (tmp != null) StyleLabel(tmp, e.size, e.blank, e.font);
            }
        }

        // centre the label in the frame's blank zone (below the top gem) as a
        // child-stretch, so it tracks the frame regardless of parenting.
        private static void StyleLabel(TMP_Text tmp, Vector2 frame, float blankFrac, float font)
        {
            tmp.color = Cocoa; tmp.enableVertexGradient = false;
            tmp.fontStyle |= FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = true; tmp.fontSizeMax = font; tmp.fontSizeMin = 16f;
            var rt = tmp.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(frame.x * 0.72f, frame.y * 0.42f);
            rt.anchoredPosition = new Vector2(0f, -(blankFrac - 0.5f) * frame.y);
            EditorUtility.SetDirty(tmp);
        }

        private static void WireTimer(UnityEngine.SceneManagement.Scene s)
        {
            var textGo = FindDeep(s, "LiveTimerText");
            var tmp = textGo != null ? textGo.GetComponent<TMP_Text>() : null;
            if (tmp == null) return;
            var parent = textGo.transform.parent;
            var banner = parent.Find("TimerBanner") as RectTransform;
            if (banner == null)
            {
                var go = new GameObject("TimerBanner", typeof(RectTransform), typeof(Image));
                banner = (RectTransform)go.transform; banner.SetParent(parent, false);
            }
            // measured: timer 429x129, centre at local-x +544, y 131 from top
            banner.anchorMin = banner.anchorMax = new Vector2(1f, 1f);
            banner.pivot = new Vector2(0.5f, 0.5f);
            banner.sizeDelta = new Vector2(429f, 129f);
            banner.anchoredPosition = new Vector2(-416f, -131f);
            var bimg = banner.GetComponent<Image>();
            bimg.sprite = Sprite("ui_hud_timer_banner_9s");
            bimg.type = Image.Type.Simple; bimg.preserveAspect = true; bimg.raycastTarget = false;
            // nest the timer text INSIDE the banner, centred in its blank zone
            var tbd = Find(textGo.transform, "Backdrop"); if (tbd != null) tbd.gameObject.SetActive(false);
            var trt = (RectTransform)textGo.transform;
            trt.SetParent(banner, false);
            trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 0.5f);
            trt.pivot = new Vector2(0.5f, 0.5f);
            trt.sizeDelta = new Vector2(300f, 70f);
            trt.anchoredPosition = new Vector2(0f, -(0.42f - 0.5f) * 129f); // above the bottom gem
            tmp.color = Cocoa; tmp.enableVertexGradient = false;
            tmp.fontStyle |= FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = true; tmp.fontSizeMax = 40f; tmp.fontSizeMin = 16f;
            EditorUtility.SetDirty(tmp); EditorUtility.SetDirty(bimg);
        }

        private static void Hide(UnityEngine.SceneManagement.Scene s, string parent, string child)
        {
            var p = FindDeep(s, parent);
            if (p == null) return;
            var c = Find(p.transform, child);
            if (c != null) c.gameObject.SetActive(false);
        }

        private static Sprite Sprite(string n) => AssetDatabase.LoadAssetAtPath<Sprite>(Dir + n + ".png");

        private static GameObject FindDeep(UnityEngine.SceneManagement.Scene s, string name)
        {
            foreach (var root in s.GetRootGameObjects())
            { if (root.name == name) return root; var t = Find(root.transform, name); if (t != null) return t.gameObject; }
            return null;
        }
        private static Transform Find(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            { var c = t.GetChild(i); if (c.name == name) return c; var r = Find(c, name); if (r != null) return r; }
            return null;
        }
        private static void EnsureSprite(string file)
        {
            var imp = AssetImporter.GetAtPath(Dir + file) as TextureImporter;
            if (imp == null) return;
            bool ch = false;
            if (imp.textureType != TextureImporterType.Sprite) { imp.textureType = TextureImporterType.Sprite; ch = true; }
            if (imp.spriteImportMode != SpriteImportMode.Single) { imp.spriteImportMode = SpriteImportMode.Single; ch = true; }
            if (!imp.alphaIsTransparency) { imp.alphaIsTransparency = true; ch = true; }
            if (ch) imp.SaveAndReimport();
        }
    }
}
