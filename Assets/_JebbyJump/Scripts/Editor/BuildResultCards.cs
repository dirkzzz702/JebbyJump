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
                "ui_star_gold_01", "ui_rank_medal_01" })
                EnsureSprite(s + ".png");
            // Per-world Game Over mascots (import as Single so the catalog can load
            // them; the project default imports fresh PNGs as Multiple).
            for (int w = 1; w <= 10; w++)
                EnsureSprite("ui_gameover_mascot_" + w.ToString("00") + ".png");
            EnsureDotSprite("ui_dot_sep.png");   // dotted row separators (Tiled)

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
                    // Only Level Complete's "Next Level" is the gold primary; the
                    // mockup's Game Over buttons are BOTH cream.
                    bool primary = btn.name.Contains("Next");
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

        // The dotted-line tile needs a Single sprite with FullRect mesh + Repeat
        // wrap so a UI Image (Tiled) repeats it cleanly across the divider.
        private static void EnsureDotSprite(string file)
        {
            var imp = AssetImporter.GetAtPath(Dir + file) as TextureImporter;
            if (imp == null) return;
            var s = new TextureImporterSettings();
            imp.ReadTextureSettings(s);
            bool ch = false;
            if (s.textureType != TextureImporterType.Sprite) { s.textureType = TextureImporterType.Sprite; ch = true; }
            if (s.spriteMode != (int)SpriteImportMode.Single) { s.spriteMode = (int)SpriteImportMode.Single; ch = true; }
            if (s.spriteMeshType != SpriteMeshType.FullRect) { s.spriteMeshType = SpriteMeshType.FullRect; ch = true; }
            if (s.wrapMode != TextureWrapMode.Repeat) { s.wrapMode = TextureWrapMode.Repeat; ch = true; }
            if (s.filterMode != FilterMode.Point) { s.filterMode = FilterMode.Point; ch = true; }
            if (!s.alphaIsTransparency) { s.alphaIsTransparency = true; ch = true; }
            if (ch) { imp.SetTextureSettings(s); imp.SaveAndReimport(); }
            AssetDatabase.ImportAsset(Dir + file, ImportAssetOptions.ForceUpdate);
        }

        // ---- mockup layout (pixel-mapped from mockup_ui.png; card = 700x480,
        // centre origin): inset rows panel, 4 icon+label rows, right-column
        // values (Time/Best times, rank medal, 3 stars), Game Over mascot. ----
        private static readonly float[] RowY = { 101f, 43f, -20f, -82f };
        // Warm GOLD rank letter on the medal's blue centre (matches the mockup's
        // gold "A"). Works on both the interim blue disc and the future medal art
        // whose own blue centre replaces that disc.
        private static readonly Color MedalLetter = new Color(0.94f, 0.72f, 0.24f);

        // Real bold face (mockup titles/labels are heavier than TMP faux-bold).
        private static Material _boldMat;
        private static Material BoldMat() => _boldMat != null ? _boldMat
            : (_boldMat = AssetDatabase.LoadAssetAtPath<Material>(
                "Assets/_JebbyJump/Art/Fonts/Fredoka SDF Bold.mat"));
        private static void ApplyBold(TMP_Text t)
        {
            if (t == null) return;
            var m = BoldMat();
            if (m != null) t.fontSharedMaterial = m;
            EditorUtility.SetDirty(t);
        }

        private static void BuildLevelCompleteExtras(Transform card)
        {
            // The new ornate frame has a taller gem area; the 480-tall card made the
            // gem collide with the title. Grow the card so gem + title + rows +
            // buttons all fit, and render the frame at native scale.
            var crt = card as RectTransform;
            if (crt != null) crt.sizeDelta = new Vector2(700f, 560f);
            var cimg = card.GetComponent<Image>();
            if (cimg != null) { cimg.pixelsPerUnitMultiplier = 1f; EditorUtility.SetDirty(cimg); }
            // Pin the title just below the gem.
            var lcTitle = Find(card, "TitleText") as RectTransform;
            if (lcTitle != null)
            {
                lcTitle.anchorMin = lcTitle.anchorMax = new Vector2(0.5f, 0.5f);
                lcTitle.pivot = new Vector2(0.5f, 0.5f);
                lcTitle.anchoredPosition = new Vector2(0f, 172f);
                lcTitle.sizeDelta = new Vector2(540f, 80f);
                var tt = lcTitle.GetComponent<TMP_Text>();
                if (tt != null)
                {
                    tt.alignment = TextAlignmentOptions.Center;
                    tt.enableAutoSizing = true; tt.fontSizeMax = 56f; tt.fontSizeMin = 24f;
                    ApplyBold(tt);
                }
            }

            // clean up the first-pass row icons (renamed since) so re-runs don't dup
            foreach (var stale in new[] { "RowIcon_TimeText", "RowIcon_BestTimeText",
                "RowIcon_RankText", "RowIcon_StarsText" })
            {
                var s = Find(card, stale);
                if (s != null) Object.DestroyImmediate(s.gameObject);
            }

            // inset cream panel (rounded) behind the four rows
            var panel = MakeChild(card, "RowsPanel", typeof(RectTransform), typeof(Image));
            Center(panel, new Vector2(7f, 4f), new Vector2(592f, 263f));
            var pimg = panel.GetComponent<Image>();
            pimg.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            pimg.type = Image.Type.Sliced; pimg.color = new Color(0.96f, 0.90f, 0.77f, 1f);
            pimg.raycastTarget = false; EditorUtility.SetDirty(pimg);
            panel.SetAsFirstSibling();

            // dotted dividers between rows (tiled dot sprite, stop left of the medal)
            var dot = Sprite("ui_dot_sep");
            float[] divY = { 72f, 11f, -51f };
            for (int i = 0; i < divY.Length; i++)
            {
                var d = MakeChild(card, "Divider" + i, typeof(RectTransform), typeof(Image));
                Center(d, new Vector2(-50f, divY[i]), new Vector2(426f, 3f));
                var dimg = d.GetComponent<Image>();
                if (dot != null) { dimg.sprite = dot; dimg.type = Image.Type.Tiled; }
                else dimg.sprite = null;
                dimg.color = new Color(0.42f, 0.32f, 0.22f, 0.55f);
                dimg.raycastTarget = false; dimg.SetAllDirty(); EditorUtility.SetDirty(dimg);
            }

            var refTmp = Find(card, "TitleText")?.GetComponent<TMP_Text>();

            // row icons (far left)
            var icons = new[] { "ui_row_icon_time_01", "ui_row_icon_best_01",
                "ui_row_icon_rank_01", "ui_star_gold_01" };
            for (int i = 0; i < 4; i++)
                PlaceIcon(card, "RowIcon" + i, icons[i], new Vector2(-240f, RowY[i]), new Vector2(62f, 62f));

            // left labels: Time/Best are new; Rank/Stars reuse the existing texts
            MakeLabel(card, "TimeLabel", "Time", RowY[0], refTmp);
            MakeLabel(card, "BestLabel", "Best", RowY[1], refTmp);
            PlaceLabel(Find(card, "RankText") as RectTransform, RowY[2]);
            PlaceLabel(Find(card, "StarsText") as RectTransform, RowY[3]);

            // right values: the two time strings (right-aligned, clear of the medal)
            PlaceValue(Find(card, "TimeText") as RectTransform, RowY[0]);
            PlaceValue(Find(card, "BestTimeText") as RectTransform, RowY[1]);

            // clean, self-consistent placeholders (HUDController overwrites these
            // at runtime: labels stay, the times become the cleared/best time).
            SetText(card, "TimeText", "--");
            SetText(card, "BestTimeText", "--");
            SetText(card, "RankText", "Rank");
            SetText(card, "StarsText", "Stars");
            SetText(card, "TitleText", "Level Complete!"); // mixed-case, matches mockup

            // rank medal (spans the Best/Rank rows, right edge). The art has its
            // own blue centre now; the gold letter sits directly on it.
            var medal = PlaceIcon(card, "RankMedal", "ui_rank_medal_01", new Vector2(232f, 8f), new Vector2(122f, 122f));
            // Drop the old interim blue disc a previous build may have added.
            var staleDisc = Find(medal, "MedalDisc");
            if (staleDisc != null) Object.DestroyImmediate(staleDisc.gameObject);

            var letter = MakeChild(medal, "RankMedalLetter", typeof(RectTransform), typeof(TextMeshProUGUI));
            Center(letter, new Vector2(0f, 9f), new Vector2(70f, 70f));
            var lt = letter.GetComponent<TextMeshProUGUI>();
            lt.text = "A"; lt.alignment = TextAlignmentOptions.Center; lt.fontStyle = FontStyles.Bold;
            lt.enableAutoSizing = true; lt.fontSizeMax = 52f; lt.fontSizeMin = 18f; lt.raycastTarget = false;
            if (refTmp != null) { lt.font = refTmp.font; lt.fontSharedMaterial = refTmp.fontSharedMaterial; }
            lt.color = MedalLetter; EditorUtility.SetDirty(lt);
            letter.SetAsLastSibling(); // ensure the letter draws over the disc

            // three mastery stars on the Stars row (right of the label)
            for (int i = 0; i < 3; i++)
                PlaceIcon(card, "Star" + i, "ui_star_gold_01", new Vector2(-6f + i * 73f, RowY[3]), new Vector2(52f, 52f));

            // buttons row (pixel-mapped: Retry / Next Level / Main Menu)
            PlaceButton(card, "RetryButton", new Vector2(-224f, -188f), new Vector2(178f, 92f));
            PlaceButton(card, "NextLevelButton", new Vector2(6f, -188f), new Vector2(214f, 92f));
            PlaceButton(card, "MainMenuButton", new Vector2(232f, -188f), new Vector2(206f, 92f));
        }

        private static void BuildGameOverExtras(Transform card)
        {
            // Mockup Game Over card is essentially SQUARE (~1.02 aspect, ~32% of
            // screen width) - NOT the wide Level Complete shape. Match it.
            var crt = card as RectTransform;
            if (crt != null) crt.sizeDelta = new Vector2(622f, 610f);
            // New frame art already has a thin baked border, so render the 9-slice
            // at native scale (reset any prior slimming multiplier).
            var cimg = card.GetComponent<Image>();
            if (cimg != null) { cimg.pixelsPerUnitMultiplier = 1f; EditorUtility.SetDirty(cimg); }

            SetText(card, "TitleText", "Game Over"); // mixed-case, matches mockup
            // Big BOLD title near the top (its default top-half stretch anchor would
            // otherwise drift into the enlarged mascot).
            var title = Find(card, "TitleText") as RectTransform;
            if (title != null)
            {
                title.anchorMin = title.anchorMax = new Vector2(0.5f, 0.5f);
                title.pivot = new Vector2(0.5f, 0.5f);
                title.anchoredPosition = new Vector2(0f, 189f);
                title.sizeDelta = new Vector2(360f, 84f);
                var tt = title.GetComponent<TMP_Text>();
                if (tt != null)
                {
                    tt.alignment = TextAlignmentOptions.Center;
                    tt.enableAutoSizing = true; tt.fontSizeMax = 64f; tt.fontSizeMin = 24f;
                    ApplyBold(tt);
                }
            }

            // Big centred sad-cactus mascot (mockup fills ~half the card).
            PlaceIcon(card, "GameOverMascot", "ui_gameover_mascot_01", new Vector2(0f, 8f), new Vector2(392f, 392f));

            // Buttons: Retry (left) + Main Menu (right, wider), thin-bordered cream
            // pills near the bottom - proportioned to the mockup.
            PlaceButton(card, "RetryButton", new Vector2(-159f, -205f), new Vector2(243f, 96f));
            PlaceButton(card, "MainMenuButton", new Vector2(140f, -205f), new Vector2(280f, 96f));
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
            rt.anchoredPosition = new Vector2(-182f, y); rt.sizeDelta = new Vector2(182f, 46f);
            var t = rt.GetComponent<TextMeshProUGUI>();
            t.text = text; t.alignment = TextAlignmentOptions.MidlineLeft;
            t.enableAutoSizing = true; t.fontSizeMax = 34f; t.fontSizeMin = 12f; t.raycastTarget = false;
            if (refTmp != null) t.font = refTmp.font;
            t.color = Cocoa; ApplyBold(t);
        }

        private static void PlaceLabel(RectTransform rt, float y)
        {
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(-182f, y); rt.sizeDelta = new Vector2(182f, 46f);
            var t = rt.GetComponent<TMP_Text>();
            if (t != null)
            {
                t.alignment = TextAlignmentOptions.MidlineLeft;
                t.enableAutoSizing = true; t.fontSizeMax = 34f; t.fontSizeMin = 12f;
                ApplyBold(t);
            }
        }

        private static void PlaceButton(Transform card, string name, Vector2 pos, Vector2 size)
        {
            var rt = Find(card, name) as RectTransform;
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos; rt.sizeDelta = size;
            EditorUtility.SetDirty(rt);
        }

        private static void PlaceValue(RectTransform rt, float y)
        {
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(1f, 0.5f);
            rt.anchoredPosition = new Vector2(160f, y); rt.sizeDelta = new Vector2(150f, 46f);
            var t = rt.GetComponent<TMP_Text>();
            if (t != null)
            {
                t.alignment = TextAlignmentOptions.MidlineRight;
                t.enableAutoSizing = true; t.fontSizeMax = 33f; t.fontSizeMin = 12f;
                ApplyBold(t);
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
            // Per-world Game Over mascot: point the WorldThemeApplier at the
            // GameOverMascot Image so it swaps the mascot per world at runtime.
            var applier = Object.FindAnyObjectByType<JebbyJump.World.WorldThemeApplier>(FindObjectsInactive.Include);
            var goPanel = FindDeep(s, "GameOverPanel");
            var goCard = goPanel != null ? Find(goPanel.transform, "Card") : null;
            var mascot = goCard != null ? Find(goCard, "GameOverMascot") : null;
            if (applier != null && mascot != null)
            {
                var aso = new SerializedObject(applier);
                var mp = aso.FindProperty("_gameOverMascot");
                if (mp != null)
                {
                    mp.objectReferenceValue = mascot.GetComponent<Image>();
                    aso.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(applier);
                }
            }

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
