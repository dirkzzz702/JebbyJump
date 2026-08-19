using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // Builds the "Burst Word" gameplay feedback popup under HUDCanvas/SafeArea/FeedbackRoot:
    // a soft colour glow + sparkles behind a big cream word with a gold outline. Wires
    // GameFeedbackUI (popup / group / word / glow / sparkles) so it themes the glow by tone
    // and pops+fades at runtime. Also recolours the live timer text to cocoa (it was light
    // cream, invisible on the cream ribbon). Idempotent.
    public static class BuildGameplayFeedback
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        private static readonly Color Cream = new Color(1f, 0.957f, 0.863f);    // word face (#FFF4DC)
        private static readonly Color Ink   = new Color(0.286f, 0.196f, 0.110f); // #49321C cocoa (timer)

        [MenuItem("Jebby Jump/Scaffold/Build Gameplay Feedback")]
        public static void Run()
        {
            EnsureSprite("ui_feedback_glow.png");
            EnsureSprite("ui_feedback_spark.png");

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var root = FindDeep(scene, "FeedbackRoot");
            if (root == null) { Debug.LogWarning("[Feedback] no FeedbackRoot in scene"); return; }
            BuildBurst(root.transform);
            RecolorTimer(scene);
            WireFeedback(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Feedback] Burst Word feedback built + wired + timer recoloured cocoa.");
        }

        private static void BuildBurst(Transform root)
        {
            // FeedbackRoot itself is the popup that scales + fades as one unit.
            var rrt = root as RectTransform;
            if (rrt != null)
            {
                rrt.anchorMin = rrt.anchorMax = new Vector2(0.5f, 0.5f);
                rrt.pivot = new Vector2(0.5f, 0.5f);
                rrt.anchoredPosition = new Vector2(0f, 80f);   // upper-centre, clear of the top HUD
                rrt.sizeDelta = new Vector2(560f, 240f);
                rrt.localScale = Vector3.one;
            }
            if (root.GetComponent<CanvasGroup>() == null) root.gameObject.AddComponent<CanvasGroup>();
            // The old feedback had a dark semi-transparent panel Image on the root; the
            // Burst Word has NO container, so remove it (else it shows as an ugly box).
            var bg = root.GetComponent<Image>();
            if (bg != null) Object.DestroyImmediate(bg);

            // radial glow (behind everything), tinted per tone at runtime
            var glow = MakeChild(root, "Glow", typeof(RectTransform), typeof(Image));
            Center(glow, Vector2.zero, new Vector2(430f, 430f));
            glow.SetAsFirstSibling();
            var gi = glow.GetComponent<Image>();
            gi.sprite = Sprite("ui_feedback_glow"); gi.type = Image.Type.Simple;
            gi.raycastTarget = false; gi.color = new Color(1f, 0.78f, 0.32f, 0.9f);
            EditorUtility.SetDirty(gi);

            // decorative sparkles
            var spk = Sprite("ui_feedback_spark");
            Vector2[] pos = { new Vector2(-192f, 74f), new Vector2(182f, 60f), new Vector2(-150f, -92f), new Vector2(162f, -84f) };
            float[] sz = { 36f, 26f, 30f, 22f };
            for (int i = 0; i < 4; i++)
            {
                var s = MakeChild(root, "Sparkle" + i, typeof(RectTransform), typeof(Image));
                Center(s, pos[i], new Vector2(sz[i], sz[i]));
                var si = s.GetComponent<Image>();
                si.sprite = spk; si.type = Image.Type.Simple;
                si.raycastTarget = false; si.color = new Color(1f, 1f, 1f, 0.92f);
                EditorUtility.SetDirty(si);
            }

            // the word (reuse the existing FeedbackText), styled + on top
            var word = Find(root, "FeedbackText") as RectTransform;
            if (word == null) word = MakeChild(root, "FeedbackText", typeof(RectTransform), typeof(TextMeshProUGUI));
            Center(word, Vector2.zero, new Vector2(520f, 210f));
            word.SetAsLastSibling();
            var wt = word.GetComponent<TextMeshProUGUI>();
            if (wt == null) wt = word.gameObject.AddComponent<TextMeshProUGUI>();
            wt.text = "Go!";
            wt.alignment = TextAlignmentOptions.Center;
            wt.enableWordWrapping = true;
            wt.enableAutoSizing = false; wt.fontSize = 60f;
            wt.characterSpacing = 0f; wt.color = Cream;
            wt.fontStyle = FontStyles.Bold; wt.raycastTarget = false;
            var font = FredokaFont(); if (font != null) wt.font = font;
            var mat = FeedbackMat(); if (mat != null) wt.fontSharedMaterial = mat;
            EditorUtility.SetDirty(wt);

            // Hidden by default (GameFeedbackUI shows it per message at runtime).
            var grp2 = root.GetComponent<CanvasGroup>(); if (grp2 != null) grp2.alpha = 1f;
            root.localScale = Vector3.one;
            root.gameObject.SetActive(false);
        }

        private static void RecolorTimer(UnityEngine.SceneManagement.Scene s)
        {
            var t = FindDeep(s, "LiveTimerText")?.GetComponent<TMP_Text>();
            if (t != null)
            {
                t.color = Ink; t.enableVertexGradient = false;
                EditorUtility.SetDirty(t);
            }
        }

        private static void WireFeedback(UnityEngine.SceneManagement.Scene s)
        {
            var fb = Object.FindAnyObjectByType<JebbyJump.UI.GameFeedbackUI>(FindObjectsInactive.Include);
            var root = FindDeep(s, "FeedbackRoot");
            if (fb == null || root == null) return;
            var so = new SerializedObject(fb);
            Set(so, "_popup", root.GetComponent<RectTransform>());
            Set(so, "_group", root.GetComponent<CanvasGroup>());
            Set(so, "_word", Find(root.transform, "FeedbackText")?.GetComponent<TextMeshProUGUI>());
            Set(so, "_glow", Find(root.transform, "Glow")?.GetComponent<Image>());
            var sp = so.FindProperty("_sparkles");
            if (sp != null)
            {
                sp.arraySize = 4;
                for (int i = 0; i < 4; i++)
                    sp.GetArrayElementAtIndex(i).objectReferenceValue = Find(root.transform, "Sparkle" + i)?.GetComponent<Image>();
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(fb);
        }

        private static void Set(SerializedObject so, string prop, Object value)
        {
            var p = so.FindProperty(prop);
            if (p != null) p.objectReferenceValue = value;
        }

        // Cream word gets a gold outline (SDF outline is core - no shader keyword needed,
        // so this is safe on any TMP distance-field material). LC-style load-or-create.
        private static Material _fbMat;
        private static Material FeedbackMat()
        {
            if (_fbMat != null) return _fbMat;
            var baseMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_JebbyJump/Art/Fonts/Fredoka SDF Bold.mat");
            if (baseMat == null) return null;
            const string path = "Assets/_JebbyJump/Art/Fonts/Fredoka SDF Feedback.mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            bool created = m == null;
            if (created) m = new Material(baseMat);
            m.CopyPropertiesFromMaterial(baseMat);         // keep shader + atlas in sync
            m.SetFloat("_FaceDilate", 0.08f);
            // bold gold outline (reads on any world background)
            m.SetColor("_OutlineColor", new Color(0.60f, 0.35f, 0.05f, 1f)); // deep gold
            m.SetFloat("_OutlineWidth", 0.30f);
            // hard drop shadow underneath -> chunky "sticker" depth
            m.EnableKeyword("UNDERLAY_ON");
            m.SetColor("_UnderlayColor", new Color(0.40f, 0.23f, 0.03f, 0.9f));
            m.SetFloat("_UnderlayOffsetX", 0f);
            m.SetFloat("_UnderlayOffsetY", -1.1f);
            m.SetFloat("_UnderlaySoftness", 0.05f);
            m.SetFloat("_UnderlayDilate", 0.05f);
            if (created) AssetDatabase.CreateAsset(m, path);
            EditorUtility.SetDirty(m);
            AssetDatabase.SaveAssets();
            _fbMat = m;
            return m;
        }

        private static TMP_FontAsset _fredoka;
        private static TMP_FontAsset FredokaFont() => _fredoka != null ? _fredoka
            : (_fredoka = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/_JebbyJump/Art/Fonts/Fredoka SDF.asset"));

        // Crisp UI sprite import (uncompressed, no mip, sRGB, straight alpha, clamp).
        private static void EnsureSprite(string file)
        {
            var imp = AssetImporter.GetAtPath(Dir + file) as TextureImporter;
            if (imp == null) return;
            var st = new TextureImporterSettings();
            imp.ReadTextureSettings(st);
            st.textureType = TextureImporterType.Sprite;
            st.spriteMode = (int)SpriteImportMode.Single;
            st.spriteMeshType = SpriteMeshType.FullRect;
            st.spriteBorder = Vector4.zero;
            st.alphaSource = TextureImporterAlphaSource.FromInput;
            st.alphaIsTransparency = true;
            st.mipmapEnabled = false;
            st.sRGBTexture = true;
            st.wrapMode = TextureWrapMode.Clamp;
            st.filterMode = FilterMode.Bilinear;
            st.npotScale = TextureImporterNPOTScale.None;
            imp.SetTextureSettings(st);
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.SaveAndReimport();
        }

        private static Sprite Sprite(string n) => AssetDatabase.LoadAssetAtPath<Sprite>(Dir + n + ".png");

        private static void Center(RectTransform rt, Vector2 pos, Vector2 size)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos; rt.sizeDelta = size; rt.localScale = Vector3.one;
        }

        private static RectTransform MakeChild(Transform parent, string name, params System.Type[] comps)
        {
            var t = parent.Find(name) as RectTransform;
            if (t == null)
            {
                var go = new GameObject(name, comps);
                t = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
                t.SetParent(parent, false);
            }
            return t;
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
    }
}
