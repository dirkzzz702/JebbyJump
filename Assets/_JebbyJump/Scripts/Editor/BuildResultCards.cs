using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // Re-skin the Level Complete + Game Over result cards with the GUI02 cream art
    // (9-slice cream card + cream/gold pill buttons + cocoa text), replacing the old
    // dark placeholder panels. The rank letter's colour is driven at runtime by
    // HUDController (re-tuned for the cream card). Idempotent.
    public static class BuildResultCards
    {
        private const string Dir = "Assets/_JebbyJump/Art/Sprites/UI/";
        private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        private static readonly Color Cocoa = new Color(0.29f, 0.19f, 0.12f);

        [MenuItem("Jebby Jump/Scaffold/Build Result Cards")]
        public static void Run()
        {
            foreach (var s in new[] { "ui_result_card_9s", "ui_result_btn_9s", "ui_result_btn_primary_9s",
                "ui_row_icon_time_01", "ui_row_icon_best_01", "ui_row_icon_rank_01",
                "ui_star_gold_01", "ui_rank_medal_01", "ui_gameover_mascot_01" })
                EnsureSprite(s + ".png");

            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            int nb = 0;
            foreach (var panelName in new[] { "LevelCompletePanel", "GameOverPanel" })
            {
                var panel = FindDeep(scene, panelName);
                if (panel == null) { Debug.LogWarning("[ResultCards] missing " + panelName); continue; }
                var card = Find(panel.transform, "Card");
                if (card == null) { Debug.LogWarning("[ResultCards] no Card under " + panelName); continue; }

                // Card background -> cream 9-slice.
                var cimg = card.GetComponent<Image>();
                if (cimg != null) SkinImage(cimg, "ui_result_card_9s");

                // Cocoa all card texts (button labels handled below; the rank letter
                // is recoloured at runtime by HUDController).
                foreach (var tmp in card.GetComponentsInChildren<TMP_Text>(true))
                {
                    if (tmp.GetComponentInParent<Button>() != null) continue;
                    tmp.color = Cocoa; EditorUtility.SetDirty(tmp);
                }

                // Buttons -> cream / gold pills, cocoa bold labels, tint transition.
                foreach (var btn in card.GetComponentsInChildren<Button>(true))
                {
                    bool primary = btn.name.Contains("Next")
                        || (panelName == "GameOverPanel" && btn.name.Contains("Retry"));
                    var bimg = btn.image != null ? btn.image : btn.GetComponent<Image>();
                    if (bimg != null)
                    {
                        SkinImage(bimg, primary ? "ui_result_btn_primary_9s" : "ui_result_btn_9s");
                        btn.targetGraphic = bimg;
                        btn.transition = Selectable.Transition.ColorTint;
                        var cb = btn.colors;
                        cb.normalColor = Color.white; cb.highlightedColor = Color.white;
                        cb.pressedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
                        cb.selectedColor = Color.white; cb.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
                        cb.colorMultiplier = 1f; cb.fadeDuration = 0.1f; btn.colors = cb;
                    }
                    var lbl = btn.GetComponentInChildren<TMP_Text>(true);
                    if (lbl != null) { lbl.color = Cocoa; lbl.fontStyle |= FontStyles.Bold; EditorUtility.SetDirty(lbl); }
                    EditorUtility.SetDirty(btn);
                    nb++;
                }

                if (panelName == "LevelCompletePanel") BuildLevelCompleteExtras(card);
                else BuildGameOverExtras(card);
            }
            WireResultRefs(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Canvas.ForceUpdateCanvases();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            Debug.Log($"[ResultCards] re-skinned Level-Complete + Game-Over cards + {nb} buttons.");
        }

        private static void SkinImage(Image img, string sprite)
        {
            var sp = Sprite(sprite);
            if (sp == null) return;
            img.sprite = sp; img.type = Image.Type.Sliced; img.fillCenter = true;
            img.color = Color.white; img.useSpriteMesh = false; img.SetAllDirty();
            EditorUtility.SetDirty(img);
        }

        private static Sprite Sprite(string n) => AssetDatabase.LoadAssetAtPath<Sprite>(Dir + n + ".png");

        private static void EnsureSprite(string file)
        {
            var imp = AssetImporter.GetAtPath(Dir + file) as TextureImporter;
            if (imp == null) return;
            bool ch = false;
            if (imp.textureType != TextureImporterType.Sprite)
            { imp.textureType = TextureImporterType.Sprite; ch = true; }
            if (imp.spriteImportMode != SpriteImportMode.Single)   // icons imported as Multiple load null
            { imp.spriteImportMode = SpriteImportMode.Single; ch = true; }
            if (ch) imp.SaveAndReimport();
            AssetDatabase.ImportAsset(Dir + file, ImportAssetOptions.ForceUpdate);
        }

        // ---- mockup layout: inset rows panel, 4 icon+label rows, right-column
        // values (Time/Best times, rank medal, 3 stars), Game Over mascot. ----
        private static readonly float[] RowY = { 112f, 52f, -8f, -68f };

        private static void BuildLevelCompleteExtras(Transform card)
        {
            // clean up the first-pass row icons (renamed since) so re-runs don't dup
            foreach (var stale in new[] { "RowIcon_TimeText", "RowIcon_BestTimeText",
                "RowIcon_RankText", "RowIcon_StarsText" })
            {
                var s = Find(card, stale);
                if (s != null) Object.DestroyImmediate(s.gameObject);
            }

            // inset cream panel (rounded) behind the four rows
            var panel = MakeChild(card, "RowsPanel", typeof(RectTransform), typeof(Image));
            Center(panel, new Vector2(-48f, 22f), new Vector2(504f, 258f));
            var pimg = panel.GetComponent<Image>();
            pimg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            pimg.type = Image.Type.Sliced; pimg.color = new Color(0.96f, 0.90f, 0.77f, 1f);
            pimg.raycastTarget = false; EditorUtility.SetDirty(pimg);
            panel.SetAsFirstSibling();

            // faint dividers between the rows
            float[] divY = { 82f, 22f, -38f };
            for (int i = 0; i < divY.Length; i++)
            {
                var d = MakeChild(card, "Divider" + i, typeof(RectTransform), typeof(Image));
                Center(d, new Vector2(-48f, divY[i]), new Vector2(446f, 2f));
                var dimg = d.GetComponent<Image>();
                dimg.sprite = null; dimg.color = new Color(0.29f, 0.19f, 0.12f, 0.22f);
                dimg.raycastTarget = false; EditorUtility.SetDirty(dimg);
            }

            var refTmp = Find(card, "TitleText")?.GetComponent<TMP_Text>();

            // row icons (far left)
            var icons = new[] { "ui_row_icon_time_01", "ui_row_icon_best_01",
                "ui_row_icon_rank_01", "ui_star_gold_01" };
            for (int i = 0; i < 4; i++)
                PlaceIcon(card, "RowIcon" + i, icons[i], new Vector2(-250f, RowY[i]), new Vector2(46f, 46f));

            // left labels: Time/Best are new; Rank/Stars reuse the existing texts
            MakeLabel(card, "TimeLabel", "Time", RowY[0], refTmp);
            MakeLabel(card, "BestLabel", "Best", RowY[1], refTmp);
            PlaceLabel(Find(card, "RankText") as RectTransform, RowY[2]);
            PlaceLabel(Find(card, "StarsText") as RectTransform, RowY[3]);

            // right values: the two time strings (right-aligned)
            PlaceValue(Find(card, "TimeText") as RectTransform, RowY[0]);
            PlaceValue(Find(card, "BestTimeText") as RectTransform, RowY[1]);

            // clean, self-consistent placeholders (HUDController overwrites these
            // at runtime: labels stay, the times become the cleared/best time).
            SetText(card, "TimeText", "--");
            SetText(card, "BestTimeText", "--");
            SetText(card, "RankText", "Rank");
            SetText(card, "StarsText", "Stars");

            // rank medal + coloured letter (spans the Best/Rank rows, right)
            var medal = PlaceIcon(card, "RankMedal", "ui_rank_medal_01", new Vector2(212f, 22f), new Vector2(150f, 150f));
            var letter = MakeChild(medal, "RankMedalLetter", typeof(RectTransform), typeof(TextMeshProUGUI));
            Center(letter, new Vector2(0f, 6f), new Vector2(96f, 96f));
            var lt = letter.GetComponent<TextMeshProUGUI>();
            lt.text = "A"; lt.alignment = TextAlignmentOptions.Center; lt.fontStyle = FontStyles.Bold;
            lt.enableAutoSizing = true; lt.fontSizeMax = 60f; lt.fontSizeMin = 20f; lt.raycastTarget = false;
            if (refTmp != null) { lt.font = refTmp.font; lt.fontSharedMaterial = refTmp.fontSharedMaterial; }
            lt.color = Cocoa; EditorUtility.SetDirty(lt);

            // three mastery stars on the Stars row (right of the label)
            for (int i = 0; i < 3; i++)
                PlaceIcon(card, "Star" + i, "ui_star_gold_01", new Vector2(4f + i * 60f, RowY[3]), new Vector2(46f, 46f));
        }

        private static void BuildGameOverExtras(Transform card)
        {
            PlaceIcon(card, "GameOverMascot", "ui_gameover_mascot_01", new Vector2(0f, -12f), new Vector2(205f, 205f));
        }

        private static void SetText(Transform card, string name, string text)
        {
            var t = Find(card, name)?.GetComponent<TMP_Text>();
            if (t != null) { t.text = text; EditorUtility.SetDirty(t); }
        }

        private static void Center(RectTransform rt, Vector2 pos, Vector2 size)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos; rt.sizeDelta = size;
        }

        private static void MakeLabel(Transform card, string name, string text, float y, TMP_Text refTmp)
        {
            var rt = MakeChild(card, name, typeof(RectTransform), typeof(TextMeshProUGUI));
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(-205f, y); rt.sizeDelta = new Vector2(180f, 44f);
            var t = rt.GetComponent<TextMeshProUGUI>();
            t.text = text; t.alignment = TextAlignmentOptions.MidlineLeft; t.fontStyle = FontStyles.Bold;
            t.enableAutoSizing = true; t.fontSizeMax = 32f; t.fontSizeMin = 12f; t.raycastTarget = false;
            if (refTmp != null) { t.font = refTmp.font; t.fontSharedMaterial = refTmp.fontSharedMaterial; }
            t.color = Cocoa; EditorUtility.SetDirty(t);
        }

        private static void PlaceLabel(RectTransform rt, float y)
        {
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(-205f, y); rt.sizeDelta = new Vector2(180f, 44f);
            var t = rt.GetComponent<TMP_Text>();
            if (t != null)
            {
                t.alignment = TextAlignmentOptions.MidlineLeft; t.fontStyle |= FontStyles.Bold;
                t.enableAutoSizing = true; t.fontSizeMax = 32f; t.fontSizeMin = 12f;
                EditorUtility.SetDirty(t);
            }
        }

        private static void PlaceValue(RectTransform rt, float y)
        {
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(1f, 0.5f);
            rt.anchoredPosition = new Vector2(128f, y); rt.sizeDelta = new Vector2(150f, 44f);
            var t = rt.GetComponent<TMP_Text>();
            if (t != null)
            {
                t.alignment = TextAlignmentOptions.MidlineRight;
                t.enableAutoSizing = true; t.fontSizeMax = 30f; t.fontSizeMin = 12f;
                EditorUtility.SetDirty(t);
            }
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

        private static RectTransform PlaceIcon(Transform card, string name, string sprite, Vector2 pos, Vector2 size)
        {
            var rt = MakeChild(card, name, typeof(RectTransform), typeof(Image));
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos; rt.sizeDelta = size;
            var img = rt.GetComponent<Image>();
            var sp = Sprite(sprite); if (sp != null) img.sprite = sp;
            img.type = Image.Type.Simple; img.preserveAspect = true; img.raycastTarget = false; img.color = Color.white;
            img.useSpriteMesh = false;
            EditorUtility.SetDirty(img);
            return rt;
        }

        private static void WireResultRefs(UnityEngine.SceneManagement.Scene s)
        {
            var hud = Object.FindAnyObjectByType<JebbyJump.UI.HUDController>(FindObjectsInactive.Include);
            var lc = FindDeep(s, "LevelCompletePanel");
            if (hud == null || lc == null) return;
            var card = Find(lc.transform, "Card");
            if (card == null) return;
            var so = new SerializedObject(hud);
            var letter = Find(card, "RankMedalLetter");
            var pl = so.FindProperty("_rankMedalLetter");
            if (pl != null && letter != null) pl.objectReferenceValue = letter.GetComponent<TextMeshProUGUI>();
            var ps = so.FindProperty("_starIcons");
            if (ps != null)
            {
                ps.arraySize = 3;
                for (int i = 0; i < 3; i++)
                {
                    var st = Find(card, "Star" + i);
                    ps.GetArrayElementAtIndex(i).objectReferenceValue = st != null ? st.GetComponent<Image>() : null;
                }
            }
            so.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(hud);
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
