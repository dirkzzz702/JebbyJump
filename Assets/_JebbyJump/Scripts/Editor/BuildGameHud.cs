using System.Collections.Generic;
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
        // Layout grid, NOT measured off the art board (that board was a sprite
        // sheet, not a 1:1 screen). Each rect is sized to the art's REAL opaque
        // aspect (badge 1.35, timer 1.62, pause 0.97, hint 1.20) so preserveAspect
        // fills the rect exactly and the label centres on the visible art, not on
        // an empty box. Anchor = which screen corner; pos = element CENTRE from it.
        //   top band baseline y=-92 (pause + timer share it); badge is the centred
        //   hero a touch lower; hint is centred mid-upper; hearts are wired below.
        private static readonly El[] Elements =
        {
            new El{ name="LevelBadgeRoot", sprite="ui_hud_level_badge_9s",
                anchor=new Vector2(0.5f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(0,-128),
                size=new Vector2(284,210), text="LevelText", blank=0.60f, font=46 }, // aspect 1.35
            new El{ name="PauseButton", sprite="ui_hud_pause_btn",
                anchor=new Vector2(1f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(-85,-92),
                size=new Vector2(93,96), text=null, blank=0.5f, font=0 },              // aspect 0.97
            new El{ name="TutorialHintRoot", sprite="ui_hint_banner_9s",
                anchor=new Vector2(0.5f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(0,-300),
                size=new Vector2(288,240), text="TutorialHintText", blank=0.56f, font=34 }, // aspect 1.20
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
            WireLives(scene);

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

            // Idempotency: earlier runs re-parented the text under the banner, so the
            // next run's Find() missed it and spawned ANOTHER banner (the duplicate
            // empty ribbons + floating text bug). Re-anchor the text to a STABLE HUD
            // parent (the level badge's parent), destroy EVERY existing TimerBanner,
            // then build exactly one.
            var badge = FindDeep(s, "LevelBadgeRoot");
            var hud = badge != null ? badge.transform.parent : textGo.transform.parent;
            textGo.transform.SetParent(hud, false);
            foreach (var old in FindAll(s, "TimerBanner")) Object.DestroyImmediate(old);

            var newGo = new GameObject("TimerBanner", typeof(RectTransform), typeof(Image));
            var banner = (RectTransform)newGo.transform;
            banner.SetParent(hud, false);
            // Timer sits on the top-right band, just LEFT of the pause button.
            // Rect sized to the art's real aspect (1.62) so it fills exactly.
            const float th = 108f, tw = th * 1.62f; // 108 x 175
            banner.anchorMin = banner.anchorMax = new Vector2(1f, 1f);
            banner.pivot = new Vector2(0.5f, 0.5f);
            banner.sizeDelta = new Vector2(tw, th);
            banner.anchoredPosition = new Vector2(-240f, -92f);
            var bimg = banner.GetComponent<Image>();
            bimg.sprite = Sprite("ui_hud_timer_banner_9s");
            bimg.type = Image.Type.Simple; bimg.preserveAspect = true; bimg.raycastTarget = false;
            // nest the timer text INSIDE the banner, centred just above the gem
            var tbd = Find(textGo.transform, "Backdrop"); if (tbd != null) tbd.gameObject.SetActive(false);
            var trt = (RectTransform)textGo.transform;
            trt.SetParent(banner, false);
            trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 0.5f);
            trt.pivot = new Vector2(0.5f, 0.5f);
            trt.sizeDelta = new Vector2(tw * 0.80f, th * 0.40f);
            // Cream text panel sits at ~62% down the ribbon art (measured); centre
            // the time there so it lands ON the panel, above the bottom gem.
            trt.anchoredPosition = new Vector2(0f, -(0.60f - 0.5f) * th);
            tmp.color = Cocoa; tmp.enableVertexGradient = false;
            tmp.fontStyle |= FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = true; tmp.fontSizeMax = 32f; tmp.fontSizeMin = 14f;
            EditorUtility.SetDirty(tmp); EditorUtility.SetDirty(bimg);
        }

        // Hearts row, top-left. Anchor the container to the corner and give it a
        // HorizontalLayoutGroup so the runtime-spawned hearts space evenly.
        private static void WireLives(UnityEngine.SceneManagement.Scene s)
        {
            var go = FindDeep(s, "LivesIconContainer");
            if (go == null) { Debug.LogWarning("[GameHUD] missing LivesIconContainer"); return; }
            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(48f, -46f);
            rt.sizeDelta = new Vector2(320f, 74f);
            var lg = go.GetComponent<HorizontalLayoutGroup>() ?? go.AddComponent<HorizontalLayoutGroup>();
            lg.spacing = 14f;
            lg.childAlignment = TextAnchor.UpperLeft;
            lg.childControlWidth = false; lg.childControlHeight = false;
            lg.childForceExpandWidth = false; lg.childForceExpandHeight = false;
            EditorUtility.SetDirty(go);
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
        private static List<GameObject> FindAll(UnityEngine.SceneManagement.Scene s, string name)
        {
            var res = new List<GameObject>();
            foreach (var root in s.GetRootGameObjects()) CollectByName(root.transform, name, res);
            return res;
        }
        private static void CollectByName(Transform t, string name, List<GameObject> res)
        {
            for (int i = 0; i < t.childCount; i++)
            { var c = t.GetChild(i); if (c.name == name) res.Add(c.gameObject); CollectByName(c, name, res); }
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
            // Force a reimport so edits to the PNG bytes (re-crops) always land, even
            // when Refresh misses the external change and Unity serves a stale sprite.
            AssetDatabase.ImportAsset(Dir + file, ImportAssetOptions.ForceUpdate);
        }
    }
}
