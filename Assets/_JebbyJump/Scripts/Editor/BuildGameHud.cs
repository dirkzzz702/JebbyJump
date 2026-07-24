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
        // then the cream PANEL geometry MEASURED from the art (centre + size as
        // fractions of the frame): pxc/pyc = panel centre, pxw/pyh = panel size,
        // fontMax. The label is placed on the real panel, not the frame centre.
        private struct El
        {
            public string name, sprite, text; public Vector2 anchor, pivot, pos, size;
            public float pxc, pxw, pyc, pyh, font;
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
                anchor=new Vector2(0.5f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(0,-132),
                size=new Vector2(300,227), text="LevelText",                 // aspect 1.32 (matches art)
                pxc=0.55f, pxw=0.69f, pyc=0.61f, pyh=0.57f, font=46 },
            new El{ name="PauseButton", sprite="ui_hud_pause_btn",
                anchor=new Vector2(1f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(-85,-92),
                size=new Vector2(93,96), text=null,                          // aspect 0.97
                pxc=0.5f, pxw=0.5f, pyc=0.5f, pyh=0.5f, font=0 },
            new El{ name="TutorialHintRoot", sprite="ui_hint_banner_9s",
                anchor=new Vector2(0.5f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(0,-300),
                size=new Vector2(288,240), text="TutorialHintText",          // aspect 1.20
                pxc=0.50f, pxw=0.86f, pyc=0.67f, pyh=0.45f, font=32 },
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
                if (tmp != null) StyleLabel(tmp, e.size, e.pxc, e.pxw, e.pyc, e.pyh, e.font);
            }
        }

        // Place the label ON the frame's measured cream panel (centre + size as
        // fractions), so the text fits INSIDE the panel and never spills onto the
        // frame's border/gem. A small margin keeps it off the panel edge.
        private static void StyleLabel(TMP_Text tmp, Vector2 frame,
            float pxc, float pxw, float pyc, float pyh, float font)
        {
            tmp.color = Cocoa; tmp.enableVertexGradient = false;
            tmp.fontStyle |= FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = true; tmp.fontSizeMax = font; tmp.fontSizeMin = 12f;
            var rt = tmp.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(frame.x * pxw * 0.92f, frame.y * pyh * 0.90f);
            rt.anchoredPosition = new Vector2((pxc - 0.5f) * frame.x, -(pyc - 0.5f) * frame.y);
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
            // Rect sized to the art's real aspect (1.62); enlarged so "00:00.00"
            // fits INSIDE the cream panel (which is only ~61% of the ribbon width).
            const float tw = 210f, th = tw / 1.62f; // 210 x 130
            banner.anchorMin = banner.anchorMax = new Vector2(1f, 1f);
            banner.pivot = new Vector2(0.5f, 0.5f);
            banner.sizeDelta = new Vector2(tw, th);
            banner.anchoredPosition = new Vector2(-256f, -95f);
            var bimg = banner.GetComponent<Image>();
            bimg.sprite = Sprite("ui_hud_timer_banner_9s");
            bimg.type = Image.Type.Simple; bimg.preserveAspect = true; bimg.raycastTarget = false;
            // nest the timer text INSIDE the banner, centred just above the gem
            var tbd = Find(textGo.transform, "Backdrop"); if (tbd != null) tbd.gameObject.SetActive(false);
            var trt = (RectTransform)textGo.transform;
            trt.SetParent(banner, false);
            trt.anchorMin = trt.anchorMax = new Vector2(0.5f, 0.5f);
            trt.pivot = new Vector2(0.5f, 0.5f);
            // Measured timer panel: centre 47%x / 62%y, size 61%w x 40%h.
            trt.sizeDelta = new Vector2(tw * 0.61f * 0.92f, th * 0.40f * 0.90f);
            trt.anchoredPosition = new Vector2((0.47f - 0.5f) * tw, -(0.62f - 0.5f) * th);
            tmp.color = Cocoa; tmp.enableVertexGradient = false;
            tmp.fontStyle |= FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = true; tmp.fontSizeMax = 28f; tmp.fontSizeMin = 12f;
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
