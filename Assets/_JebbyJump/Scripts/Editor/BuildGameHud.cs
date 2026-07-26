using System.Collections.Generic;
using JebbyJump.Sequence;
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
            public bool stretch; // fill the rect (preserveAspect off) to reshape the art
            public bool bold;    // apply the Face-Dilate bold Fredoka material
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
                anchor=new Vector2(0.5f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(0,-108),
                size=new Vector2(330,156), text="LevelText",                 // flattened further in height (art 1.66 -> rect 2.12)
                pxc=0.50f, pxw=0.73f, pyc=0.53f, pyh=0.68f, font=46, stretch=true, bold=true },
            new El{ name="PauseButton", sprite="ui_hud_pause_btn",
                anchor=new Vector2(1f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(-82,-86),
                size=new Vector2(110,110), text=null,                        // aligned baseline with the (bigger) timer
                pxc=0.5f, pxw=0.5f, pyc=0.5f, pyh=0.5f, font=0 },
            // Hint lives top-centre (the badge's zone, free once the badge hides),
            // NOT over the platforms; widened a touch so the text reads bigger.
            new El{ name="TutorialHintRoot", sprite="ui_hint_banner_9s",
                anchor=new Vector2(0.5f,1f), pivot=new Vector2(0.5f,0.5f), pos=new Vector2(0,-140),
                size=new Vector2(400,230), text="TutorialHintText",          // stray top fragment cropped; banner aspect 1.74
                pxc=0.51f, pxw=0.91f, pyc=0.52f, pyh=0.67f, font=40, stretch=true },
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
            WireLevelIntro(scene);

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
            // Force the edit-mode canvases + Game view to rebuild so the badge (and
            // other frames) don't keep drawing a stale cached mesh after the sprite
            // reimport / useSpriteMesh change.
            Canvas.ForceUpdateCanvases();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
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
            if (sp != null)
            {
                img.sprite = null; img.sprite = sp;   // null->set forces a full mesh rebuild
                img.type = Image.Type.Simple;
                img.preserveAspect = !e.stretch;
                img.useSpriteMesh = false;   // full quad, never the sprite's tight outline
                img.color = Color.white;
                img.SetAllDirty();
                EditorUtility.SetDirty(img);
            }

            if (e.text != null)
            {
                var t = FindDeep(s, e.text);
                var tmp = t != null ? t.GetComponent<TMP_Text>() : null;
                if (tmp != null) StyleLabel(tmp, e.size, e.pxc, e.pxw, e.pyc, e.pyh, e.font, e.bold);
            }
        }

        // Place the label ON the frame's measured cream panel (centre + size as
        // fractions), so the text fits INSIDE the panel and never spills onto the
        // frame's border/gem. A small margin keeps it off the panel edge.
        private static void StyleLabel(TMP_Text tmp, Vector2 frame,
            float pxc, float pxw, float pyc, float pyh, float font, bool bold)
        {
            tmp.color = Cocoa; tmp.enableVertexGradient = false;
            tmp.fontStyle |= FontStyles.Bold;
            if (bold) { var bm = BoldMat(); if (bm != null) tmp.fontSharedMaterial = bm; }
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
            // Timer sits on the top-right band, just LEFT of the pause button,
            // aligned on the same centre-y. Enlarged so its cream body reads at a
            // similar size to the pause and the time is bigger. Art aspect 1.62.
            const float th = 148f, tw = th * 1.62f; // ~240 x 148
            banner.anchorMin = banner.anchorMax = new Vector2(1f, 1f);
            banner.pivot = new Vector2(0.5f, 0.5f);
            banner.sizeDelta = new Vector2(tw, th);
            banner.anchoredPosition = new Vector2(-278f, -86f);
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
            var tbm = BoldMat(); if (tbm != null) tmp.fontSharedMaterial = tbm;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = true; tmp.fontSizeMax = 36f; tmp.fontSizeMin = 12f;
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

        // Point MemoryPhaseController at the level badge so it can flash "Level X"
        // for a beat, then hide it before the memory swatch shows.
        private static void WireLevelIntro(UnityEngine.SceneManagement.Scene s)
        {
            var mpc = Object.FindAnyObjectByType<MemoryPhaseController>(FindObjectsInactive.Include);
            var badge = FindDeep(s, "LevelBadgeRoot");
            if (mpc == null || badge == null) return;
            var so = new SerializedObject(mpc);
            var p = so.FindProperty("_levelBadgeRoot");
            if (p != null && p.objectReferenceValue != badge)
            { p.objectReferenceValue = badge; so.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(mpc); }
        }

        private static void Hide(UnityEngine.SceneManagement.Scene s, string parent, string child)
        {
            var p = FindDeep(s, parent);
            if (p == null) return;
            var c = Find(p.transform, child);
            if (c != null) c.gameObject.SetActive(false);
        }

        private static Sprite Sprite(string n) => AssetDatabase.LoadAssetAtPath<Sprite>(Dir + n + ".png");

        // Scoped Face-Dilate bold Fredoka material (same one the menu labels use)
        // for "one size bolder" HUD text. Loaded once.
        private static Material _boldMat;
        private static Material BoldMat()
        {
            if (_boldMat == null)
                _boldMat = AssetDatabase.LoadAssetAtPath<Material>(
                    "Assets/_JebbyJump/Art/Fonts/Fredoka SDF Bold.mat");
            return _boldMat;
        }

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
            // FullRect mesh: a Tight mesh hugs the opaque outline and (with the
            // de-fringed, semi-transparent rounded edges) clips the frame's bottom
            // into a flat cut - worse when stretched. FullRect = a plain quad.
            var st = new TextureImporterSettings();
            imp.ReadTextureSettings(st);
            if (st.spriteMeshType != SpriteMeshType.FullRect)
            { st.spriteMeshType = SpriteMeshType.FullRect; imp.SetTextureSettings(st); ch = true; }
            if (ch) imp.SaveAndReimport();
            // Force a reimport so edits to the PNG bytes (re-crops) always land, even
            // when Refresh misses the external change and Unity serves a stale sprite.
            AssetDatabase.ImportAsset(Dir + file, ImportAssetOptions.ForceUpdate);
        }
    }
}
