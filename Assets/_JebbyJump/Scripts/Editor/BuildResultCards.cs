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
        // Dark cocoa (#49321C) - the mockup's label/value/title ink weight.
        private static readonly Color Cocoa = new Color(0.286f, 0.196f, 0.110f);

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

            // Level-Complete-specific button pills: the shared ui_result_btn art has
            // ~13% horizontal transparent padding, so its RECT is wider than the
            // visible pill - at the mockup's ~2-3% gaps the hit rects would overlap.
            // These LC derivatives are cropped so pill==rect (border x/z 64->46);
            // Game Over keeps the un-cropped shared pills (unchanged, not re-verified
            // here). Plus a white-base ivory rows-panel 9-slice (the old grey
            // Background.psd tinted beige rendered muddy #CEC1A4).
            EnsureSlicedSprite("ui_result_btn_lc_9s.png", 46, 0, 46, 0);
            EnsureSlicedSprite("ui_result_btn_primary_lc_9s.png", 46, 0, 46, 0);
            EnsureSlicedSprite("ui_rows_panel_9s.png", 44, 44, 44, 44);

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
                    bool lcBtn = panelName == "LevelCompletePanel";
                    var bimg = btn.image != null ? btn.image : btn.GetComponent<Image>();
                    if (bimg != null)
                    {
                        SkinImage(bimg, primary
                            ? (lcBtn ? "ui_result_btn_primary_lc_9s" : "ui_result_btn_primary_9s")
                            : (lcBtn ? "ui_result_btn_lc_9s" : "ui_result_btn_9s"));
                        btn.targetGraphic = bimg;
                        btn.transition = Selectable.Transition.ColorTint;
                        var cb = btn.colors;
                        cb.normalColor = Color.white; cb.highlightedColor = Color.white;
                        cb.pressedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
                        cb.selectedColor = Color.white; cb.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.6f);
                        cb.colorMultiplier = 1f; cb.fadeDuration = 0.1f; btn.colors = cb;
                    }
                    var lbl = btn.GetComponentInChildren<TMP_Text>(true);
                    if (lbl != null)
                    {
                        // Bake the FINAL converged label state so AdjustGameUiLayout
                        // (margin>=24) and StyleTypography (autosize off, uniform fit)
                        // are no-ops in any order. Font size is set uniformly per
                        // panel by FitButtonLabels (below), on the LONGEST label.
                        lbl.color = Cocoa; lbl.fontStyle |= FontStyles.Bold;
                        lbl.characterSpacing = Mathf.Max(lbl.characterSpacing, 2f);
                        lbl.enableWordWrapping = false;
                        lbl.enableAutoSizing = false;
                        lbl.overflowMode = TextOverflowModes.Overflow;
                        lbl.margin = new Vector4(BtnLabelMargin, 0f, BtnLabelMargin, 0f);
                        ApplyBold(lbl);
                    }
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

        // Ensure a Single sliced sprite with a set 9-slice border (for the
        // procedurally-generated LC pills + ivory rows panel whose default import
        // has no border). Idempotent.
        private static void EnsureSlicedSprite(string file, int l, int t, int r, int b)
        {
            var imp = AssetImporter.GetAtPath(Dir + file) as TextureImporter;
            if (imp == null) return;
            var s = new TextureImporterSettings();
            imp.ReadTextureSettings(s);
            bool ch = false;
            if (s.textureType != TextureImporterType.Sprite) { s.textureType = TextureImporterType.Sprite; ch = true; }
            if (s.spriteMode != (int)SpriteImportMode.Single) { s.spriteMode = (int)SpriteImportMode.Single; ch = true; }
            if (!s.alphaIsTransparency) { s.alphaIsTransparency = true; ch = true; }
            var border = new Vector4(l, b, r, t);   // Unity order: x=left y=bottom z=right w=top
            if (s.spriteBorder != border) { s.spriteBorder = border; ch = true; }
            if (ch) { imp.SetTextureSettings(s); imp.SaveAndReimport(); }
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

        // ---- mockup layout (pixel-mapped from mockup_ui.png; LC card = 700x610,
        // centre origin): inset rows panel, 4 icon+label rows, right-column
        // values (Time/Best times, rank medal, 3 stars), Game Over mascot. ----
        // Row centres at 31/42/54/65% down the 610 card: y = (0.5 - pct)*610.
        private static readonly float[] RowY = { 116f, 49f, -24f, -92f };
        // One deterministic size for every LC row label AND value (mockup: labels
        // and values share weight/size). Autosizing is OFF everywhere on the rows
        // so BuildResultCards / StyleTypography / AdjustGameUiLayout converge.
        private const float RowTextSize = 34f;
        // Warm GOLD rank letter on the medal's blue centre (matches the mockup's
        // gold "A"). Works on both the interim blue disc and the future medal art
        // whose own blue centre replaces that disc.
        private static readonly Color MedalLetter = new Color(0.94f, 0.72f, 0.24f);

        // Button-label end margin (clears the pill's rounded ends). Matches
        // StyleTypography.LabelMargin / AdjustGameUiLayout's >=24 floor so those
        // tools don't change it -> deterministic across generators.
        private const float BtnLabelMargin = 26f;

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
            // Card grown to 700x610 (aspect ~1.15, matching the mockup - the old
            // 700x560 read ~1.28 wide). Render the ornate frame at native scale.
            var crt = card as RectTransform;
            if (crt != null) crt.sizeDelta = new Vector2(700f, 610f);
            var cimg = card.GetComponent<Image>();
            if (cimg != null) { cimg.pixelsPerUnitMultiplier = 1f; EditorUtility.SetDirty(cimg); }
            // Pin the title just below the gem (~19% down the taller card).
            var lcTitle = Find(card, "TitleText") as RectTransform;
            if (lcTitle != null)
            {
                lcTitle.anchorMin = lcTitle.anchorMax = new Vector2(0.5f, 0.5f);
                lcTitle.pivot = new Vector2(0.5f, 0.5f);
                lcTitle.anchoredPosition = new Vector2(0f, 190f);
                lcTitle.sizeDelta = new Vector2(540f, 82f);
                var tt = lcTitle.GetComponent<TMP_Text>();
                if (tt != null)
                {
                    tt.alignment = TextAlignmentOptions.Center;
                    tt.enableAutoSizing = true; tt.fontSizeMax = 56f; tt.fontSizeMin = 24f;
                    // Cocoa, NO gradient (mockup title is solid cocoa; clears any stale
                    // cream->gold vertex gradient a prior StyleTypography run baked).
                    tt.color = Cocoa; tt.enableVertexGradient = false; ApplyBold(tt);
                }
            }

            // clean up the first-pass row icons (renamed since) so re-runs don't dup
            foreach (var stale in new[] { "RowIcon_TimeText", "RowIcon_BestTimeText",
                "RowIcon_RankText", "RowIcon_StarsText" })
            {
                var s = Find(card, stale);
                if (s != null) Object.DestroyImmediate(s.gameObject);
            }

            // inset ivory panel behind the four rows. White-base rounded 9-slice
            // (ui_rows_panel_9s = ivory #FFF2DE fill + subtle peach border) so the
            // fill is a clean warm ivory, NOT the muddy grey the old Background.psd
            // multiplied by a beige tint produced. Bounds ~x8-91% / y25-71% of card.
            var panel = MakeChild(card, "RowsPanel", typeof(RectTransform), typeof(Image));
            Center(panel, new Vector2(-4f, 12f), new Vector2(580f, 282f));
            var pimg = panel.GetComponent<Image>();
            var rowsSprite = Sprite("ui_rows_panel_9s");
            if (rowsSprite != null) pimg.sprite = rowsSprite;
            pimg.type = Image.Type.Sliced; pimg.color = Color.white;
            pimg.pixelsPerUnitMultiplier = 1f; pimg.useSpriteMesh = false;
            pimg.raycastTarget = false; EditorUtility.SetDirty(pimg);
            panel.SetAsFirstSibling();

            // subtle peach/gold dividers between rows (stop left of the medal)
            var dot = Sprite("ui_dot_sep");
            float[] divY = { 82f, 13f, -58f };
            for (int i = 0; i < divY.Length; i++)
            {
                var d = MakeChild(card, "Divider" + i, typeof(RectTransform), typeof(Image));
                Center(d, new Vector2(-60f, divY[i]), new Vector2(340f, 3f));
                var dimg = d.GetComponent<Image>();
                if (dot != null) { dimg.sprite = dot; dimg.type = Image.Type.Tiled; }
                else dimg.sprite = null;
                dimg.color = new Color(0.86f, 0.73f, 0.52f, 0.5f);   // subtle peach/gold
                dimg.raycastTarget = false; dimg.SetAllDirty(); EditorUtility.SetDirty(dimg);
            }

            var refTmp = Find(card, "TitleText")?.GetComponent<TMP_Text>();

            // row icons (far left). Per-icon RectTransform sizes normalise their
            // VISIBLE alpha area (crown/shield/stopwatch/star differ) to ~feel equal
            // (~2400u^2, ~70-76% of row height). Left edge clears the panel border.
            var icons = new[] { "ui_row_icon_time_01", "ui_row_icon_best_01",
                "ui_row_icon_rank_01", "ui_star_gold_01" };
            var iconSize = new[] { 61f, 66f, 58f, 58f };
            for (int i = 0; i < 4; i++)
                PlaceIcon(card, "RowIcon" + i, icons[i],
                    new Vector2(-256f, RowY[i]), new Vector2(iconSize[i], iconSize[i]));

            // left labels: Time/Best are new; Rank/Stars reuse the existing texts
            MakeLabel(card, "TimeLabel", "Time", RowY[0], refTmp);
            MakeLabel(card, "BestLabel", "Best", RowY[1], refTmp);
            PlaceLabel(Find(card, "RankText") as RectTransform, RowY[2]);
            PlaceLabel(Find(card, "StarsText") as RectTransform, RowY[3]);

            // right values: the two time strings (right-aligned at x=120, clear of
            // the medal). Best box is WIDE (250) so the runtime "  New!" suffix fits
            // at the common value size instead of auto-shrinking the numerals.
            PlaceValue(Find(card, "TimeText") as RectTransform, RowY[0]);
            PlaceValue(Find(card, "BestTimeText") as RectTransform, RowY[1]);

            // clean, self-consistent placeholders (HUDController overwrites these
            // at runtime: labels stay, the times become the cleared/best time).
            SetText(card, "TimeText", "--");
            SetText(card, "BestTimeText", "--");
            SetText(card, "RankText", "Rank");
            SetText(card, "StarsText", "Stars");
            SetText(card, "TitleText", "Level Complete!"); // mixed-case, matches mockup

            // rank medal + laurel: enlarged to ~20%W x 24%H visible (centre ~80%,49%).
            // The 1024 PNG's meaningful alpha is ~70% of its canvas, so a ~196 rect
            // renders the laurel at the mockup size. The gold letter sits on its
            // blue centre (colour owned here; HUDController sets only the letter).
            var medal = PlaceIcon(card, "RankMedal", "ui_rank_medal_01", new Vector2(214f, 6f), new Vector2(196f, 196f));
            // Drop the old interim blue disc a previous build may have added.
            var staleDisc = Find(medal, "MedalDisc");
            if (staleDisc != null) Object.DestroyImmediate(staleDisc.gameObject);

            var letter = MakeChild(medal, "RankMedalLetter", typeof(RectTransform), typeof(TextMeshProUGUI));
            Center(letter, new Vector2(0f, 12f), new Vector2(96f, 96f));
            var lt = letter.GetComponent<TextMeshProUGUI>();
            lt.text = "A"; lt.alignment = TextAlignmentOptions.Center; lt.fontStyle = FontStyles.Bold;
            lt.enableAutoSizing = true; lt.fontSizeMax = 80f; lt.fontSizeMin = 24f; lt.raycastTarget = false;
            if (refTmp != null) { lt.font = refTmp.font; lt.fontSharedMaterial = refTmp.fontSharedMaterial; }
            lt.color = MedalLetter; EditorUtility.SetDirty(lt);
            letter.SetAsLastSibling(); // ensure the letter draws over the disc

            // three mastery stars on the Stars row (right of the label, left of the
            // medal's lower laurel). HUDController fills/dims them per earned count.
            for (int i = 0; i < 3; i++)
                PlaceIcon(card, "Star" + i, "ui_star_gold_01", new Vector2(-24f + i * 70f, RowY[3]), new Vector2(54f, 54f));

            // buttons row (pixel-mapped: Retry / Next Level / Main Menu). Retry ==
            // Main Menu rect; Next Level wider (mockup). Height 96 (>=90 accessibility
            // floor - agrees with BuildGameShellCanvas.EnsureMinHeight). Cropped LC
            // pills => rect == visible, so these tight ~2.4% gaps DON'T overlap the
            // hit rects (Retry right -103 < Next left -108).
            PlaceButton(card, "RetryButton", new Vector2(-229f, -193f), new Vector2(206f, 96f));
            PlaceButton(card, "NextLevelButton", new Vector2(0f, -193f), new Vector2(217f, 96f));
            PlaceButton(card, "MainMenuButton", new Vector2(229f, -193f), new Vector2(206f, 96f));
            FitButtonLabels(card,
                new[] { "RetryButton", "NextLevelButton", "MainMenuButton" }, 34f);
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
                    // Solid cocoa (accepted GO look); clear any stale vertex gradient.
                    tt.color = Cocoa; tt.enableVertexGradient = false; ApplyBold(tt);
                }
            }

            // Big centred sad-cactus mascot. Sized by the art's ALPHA bounds (the
            // 640px canvas is ~66%W / 55%H visible cactus with heavy transparent
            // padding + faint stray alpha): a 486u rect renders W01 at ~52%W /
            // ~45%H of the panel, matching the mockup. Shared Image for all 10
            // world mascots (WorldThemeApplier swaps the sprite) - verified W02/W10
            // do not clip at this size.
            PlaceIcon(card, "GameOverMascot", "ui_gameover_mascot_01", new Vector2(0f, -20f), new Vector2(486f, 486f));

            // Retry + Main Menu: EQUAL RectTransform size, ~39-40% panel width each,
            // ~6% centre gap, ~8% side clearance, ~10% bottom clearance (mockup).
            PlaceButton(card, "RetryButton", new Vector2(-140f, -190f), new Vector2(270f, 100f));
            PlaceButton(card, "MainMenuButton", new Vector2(140f, -190f), new Vector2(270f, 100f));
            FitButtonLabels(card, new[] { "RetryButton", "MainMenuButton" }, 34f);
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
            rt.anchoredPosition = new Vector2(-200f, y); rt.sizeDelta = new Vector2(150f, 46f);
            var t = rt.GetComponent<TextMeshProUGUI>();
            t.text = text; t.alignment = TextAlignmentOptions.MidlineLeft;
            t.enableAutoSizing = false; t.fontSize = RowTextSize;
            t.enableWordWrapping = false; t.overflowMode = TextOverflowModes.Overflow;
            t.raycastTarget = false;
            if (refTmp != null) t.font = refTmp.font;
            t.color = Cocoa; ApplyBold(t);
        }

        private static void PlaceLabel(RectTransform rt, float y)
        {
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(-200f, y); rt.sizeDelta = new Vector2(150f, 46f);
            var t = rt.GetComponent<TMP_Text>();
            if (t != null)
            {
                t.alignment = TextAlignmentOptions.MidlineLeft;
                t.enableAutoSizing = false; t.fontSize = RowTextSize;
                t.enableWordWrapping = false; t.overflowMode = TextOverflowModes.Overflow;
                t.color = Cocoa; ApplyBold(t);
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

        // Uniform button-label sizing: every label in the row shares the largest
        // font that fits the LONGEST label inside the NARROWEST button (with 26u
        // end margins). Deterministic + idempotent - mirrors StyleTypography's
        // horizontal-row FitGroup so re-running either converges to the same size.
        // Call AFTER the buttons are sized.
        private static void FitButtonLabels(Transform card, string[] names, float baseSize)
        {
            var labels = new System.Collections.Generic.List<TMP_Text>();
            float ratio = 1f;
            foreach (var n in names)
            {
                var b = Find(card, n) as RectTransform;
                var lbl = b != null ? b.GetComponentInChildren<TMP_Text>(true) : null;
                if (lbl == null || b == null) continue;
                labels.Add(lbl);
                lbl.enableAutoSizing = false;
                if (!Mathf.Approximately(lbl.fontSize, baseSize)) lbl.fontSize = baseSize;
                float avail = b.sizeDelta.x - lbl.margin.x - lbl.margin.z - 4f;
                float pref;
                try { pref = lbl.GetPreferredValues(lbl.text, Mathf.Infinity, Mathf.Infinity).x; }
                catch { pref = 0f; }
                // Deterministic fallback: a never-awoken TMP returns 0 in edit mode,
                // which would leave a too-wide label un-shrunk. Estimate ~0.57em/char
                // at the base size (Fredoka bold is wide) and use the larger of the
                // two, so the fit never depends on TMP being initialised.
                float est = (lbl.text != null ? lbl.text.Length : 0) * baseSize * 0.57f;
                pref = Mathf.Max(pref, est);
                if (pref > avail && pref > 0f) ratio = Mathf.Min(ratio, avail / pref);
            }
            if (labels.Count == 0) return;
            float shared = Mathf.Max(20f, Mathf.Floor(baseSize * ratio * 2f) / 2f);
            foreach (var lbl in labels)
            {
                if (!Mathf.Approximately(lbl.fontSize, shared)) lbl.fontSize = shared;
                EditorUtility.SetDirty(lbl);
            }
        }

        private static void PlaceValue(RectTransform rt, float y)
        {
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(1f, 0.5f);
            // Right edge at x=120 (~67% of card - left of the medal). Box kept narrow
            // (160) so it doesn't overlap the left label box; autosize is OFF and
            // overflow is ON, so the runtime "00:23.11  New!" renders at the common 34pt
            // (overflowing left, ink ends ~-96, clear of the label ink) - NOT shrunk.
            rt.anchoredPosition = new Vector2(120f, y); rt.sizeDelta = new Vector2(160f, 46f);
            var t = rt.GetComponent<TMP_Text>();
            if (t != null)
            {
                t.alignment = TextAlignmentOptions.MidlineRight;
                t.enableAutoSizing = false; t.fontSize = RowTextSize;
                t.enableWordWrapping = false; t.overflowMode = TextOverflowModes.Overflow;
                t.raycastTarget = false; t.color = Cocoa; ApplyBold(t);
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
