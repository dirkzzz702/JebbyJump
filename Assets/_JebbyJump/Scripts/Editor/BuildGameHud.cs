using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // GUI01 HUD wiring: dress the in-game top HUD in the cloud-kingdom art -
    // level badge, timer ribbon, round pause button, heart lives, and the hint
    // banner. Frames go on as fixed-size Simple sprites (their gem sits at the
    // centre, so 9-slice would shift it); text is recoloured cocoa and centred in
    // each frame's blank zone. Heart art is set on HUDController._lifeIconSprite
    // (it spawns one per life at 52x52). Tunable size/offset constants below.
    // Idempotent; only sprites/colours/sizes + the timer banner child change.
    public static class BuildGameHud
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private const string GameScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        private static readonly Color Cocoa = new Color(0.29f, 0.19f, 0.12f);

        // display size (w,h) + label vertical offset within the frame
        private static readonly Vector2 BadgeSize = new Vector2(320f, 128f);
        private const float BadgeLabelDY = -6f;   // below the top gem
        private static readonly Vector2 TimerSize = new Vector2(300f, 105f);
        private const float TimerLabelDY = 8f;    // above the bottom gem
        private static readonly Vector2 HintSize = new Vector2(480f, 160f);
        private const float HintLabelDY = -8f;
        private static readonly Vector2 PauseSize = new Vector2(96f, 96f);

        [MenuItem("Jebby Jump/Scaffold/Build Game HUD")]
        public static void Run()
        {
            foreach (var s in new[] { "ui_hud_level_badge_9s", "ui_hud_timer_banner_9s",
                "ui_hud_pause_btn", "ui_hint_banner_9s", "ui_hud_heart_01" })
                EnsureSprite(s + ".png");

            var scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);

            // Level badge
            FrameElement(scene, "LevelBadgeRoot", "ui_hud_level_badge_9s", BadgeSize);
            LabelIn(scene, "LevelText", BadgeLabelDY, 30f);

            // Pause button
            FrameElement(scene, "PauseButton", "ui_hud_pause_btn", PauseSize);

            // Hint banner
            FrameElement(scene, "TutorialHintRoot", "ui_hint_banner_9s", HintSize);
            LabelIn(scene, "TutorialHintText", HintLabelDY, 30f);

            // Timer: add a banner behind the live-timer text
            WireTimer(scene);

            // Hearts: swap the life-icon sprite on the HUD controller
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
            Debug.Log("[GameHUD] wired badge/timer/pause/hearts/hint.");
        }

        private static void FrameElement(UnityEngine.SceneManagement.Scene s,
            string name, string spriteName, Vector2 size)
        {
            var go = FindDeep(s, name);
            if (go == null) { Debug.LogWarning("[GameHUD] missing " + name); return; }
            var img = go.GetComponent<Image>() ?? go.AddComponent<Image>();
            var sp = Sprite(spriteName);
            if (sp == null) return;
            img.sprite = sp; img.type = Image.Type.Simple; img.preserveAspect = true;
            img.color = Color.white;
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = size;
            EditorUtility.SetDirty(img);
        }

        private static void LabelIn(UnityEngine.SceneManagement.Scene s, string name,
            float dy, float size)
        {
            var go = FindDeep(s, name);
            var tmp = go != null ? go.GetComponent<TMP_Text>() : null;
            if (tmp == null) return;
            tmp.color = Cocoa;
            tmp.enableVertexGradient = false;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMax = size; tmp.fontSizeMin = 16f;
            var rt = tmp.rectTransform;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, dy);
            EditorUtility.SetDirty(tmp);
        }

        private static void WireTimer(UnityEngine.SceneManagement.Scene s)
        {
            var textGo = FindDeep(s, "LiveTimerText");
            var tmp = textGo != null ? textGo.GetComponent<TMP_Text>() : null;
            if (tmp == null) { Debug.LogWarning("[GameHUD] no LiveTimerText"); return; }
            var parent = textGo.transform.parent;
            var banner = parent.Find("TimerBanner") as RectTransform;
            if (banner == null)
            {
                var go = new GameObject("TimerBanner", typeof(RectTransform), typeof(Image));
                banner = (RectTransform)go.transform;
                banner.SetParent(parent, false);
            }
            banner.SetAsFirstSibling(); // behind the text
            var trt = (RectTransform)textGo.transform;
            banner.anchorMin = trt.anchorMin; banner.anchorMax = trt.anchorMax;
            banner.pivot = trt.pivot;
            banner.anchoredPosition = trt.anchoredPosition;
            banner.sizeDelta = TimerSize;
            var bimg = banner.GetComponent<Image>();
            bimg.sprite = Sprite("ui_hud_timer_banner_9s");
            bimg.type = Image.Type.Simple; bimg.preserveAspect = true; bimg.raycastTarget = false;
            // recolour the timer text for the light banner
            tmp.color = Cocoa; tmp.enableVertexGradient = false;
            tmp.alignment = TextAlignmentOptions.Center;
            var t = tmp.rectTransform;
            t.anchoredPosition = new Vector2(t.anchoredPosition.x, t.anchoredPosition.y + TimerLabelDY);
            EditorUtility.SetDirty(tmp); EditorUtility.SetDirty(bimg);
        }

        private static Sprite Sprite(string n) =>
            AssetDatabase.LoadAssetAtPath<Sprite>(Dir + n + ".png");

        private static GameObject FindDeep(UnityEngine.SceneManagement.Scene s, string name)
        {
            foreach (var root in s.GetRootGameObjects())
            {
                if (root.name == name) return root;
                var t = Find(root.transform, name);
                if (t != null) return t.gameObject;
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
