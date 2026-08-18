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

            // Claude Design plates (repaired export): painted panel BACKGROUNDS only,
            // drawn Simple at native size as the Card image. LC keeps frame/cream/gem/
            // inset/dividers/row icons/medal-laurel; GO keeps frame/cream/gem + a clean
            // cream interior. They carry NO buttons, button shadows or ghost outlines:
            // Retry/Next/Main Menu are independent sliced UI Images (LayoutResultButton),
            // and the live text / 3 stars / rank letter / (GO) per-world mascot are
            // overlays. Import per ARTWORK_MANIFEST: Single, FullRect, compression off,
            // mip off, sRGB, straight alpha, no atlas, clamp, bilinear, PPU100.
            foreach (var s in new[] { "ui_lc_card", "ui_go_card", "ui_result_star" })
                EnsureSimpleSprite(s + ".png");

            // Level-Complete buttons repainted from the Claude Design mockup art
            // (btn_retry_s / btn_next_gold / btn_menu_s) so the visible pills + labels
            // match the reference (the older ui_result_btn_*_lc pills sat inset and read
            // too small). HQ import + a ~60px horizontal 9-slice for clean caps.
            foreach (var b in new[] { "ui_lc_btn_retry", "ui_lc_btn_next", "ui_lc_btn_menu" })
                EnsureSimpleSprite(b + ".png", 60);

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
                    // Button LABELS are intentionally NOT styled here: all five (both
                    // panels) get ONE identical style + ONE shared size from
                    // UnifyResultButtonLabels after both panels are built, so no
                    // per-panel divergence can creep in.
                    EditorUtility.SetDirty(btn);
                    nb++;
                }

                if (panelName == "LevelCompletePanel") BuildLevelCompleteExtras(card);
                else BuildGameOverExtras(card);
            }
            UnifyResultButtonLabels(scene);   // ONE identical label style + size, all 5 buttons
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

        // Design-plate import (ARTWORK_MANIFEST settings): Single, FullRect, no
        // compression, no mipmaps, sRGB, straight alpha, clamp, bilinear, PPU 100,
        // no border (Simple draw). Idempotent.
        private static void EnsureSimpleSprite(string file, int hBorder = 0)
        {
            var imp = AssetImporter.GetAtPath(Dir + file) as TextureImporter;
            if (imp == null) return;
            var s = new TextureImporterSettings();
            imp.ReadTextureSettings(s);
            s.textureType = TextureImporterType.Sprite;
            s.spriteMode = (int)SpriteImportMode.Single;
            s.spriteMeshType = SpriteMeshType.FullRect;
            // hBorder>0 -> a horizontal 9-slice (rounded button caps stay fixed, the
            // straight middle flexes); plates use 0 (Simple, no slice).
            s.spriteBorder = new Vector4(hBorder, 0, hBorder, 0);
            s.spritePixelsPerUnit = 100f;
            s.mipmapEnabled = false;
            s.sRGBTexture = true;
            s.alphaIsTransparency = true;
            s.alphaSource = TextureImporterAlphaSource.FromInput;
            s.wrapMode = TextureWrapMode.Clamp;
            s.filterMode = FilterMode.Bilinear;
            s.npotScale = TextureImporterNPOTScale.None;
            imp.SetTextureSettings(s);
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.mipmapEnabled = false;
            imp.SaveAndReimport();
        }

        // Skin an Image as a Simple, native design plate (no 9-slice stretch).
        private static void SkinSimple(Image img, string sprite)
        {
            var sp = Sprite(sprite);
            if (sp == null) return;
            img.sprite = sp; img.type = Image.Type.Simple; img.preserveAspect = false;
            img.color = Color.white; img.useSpriteMesh = false;
            img.pixelsPerUnitMultiplier = 1f; img.SetAllDirty();
            EditorUtility.SetDirty(img);
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

        // Shared result-button label end margin (clears the pill's rounded ends).
        // UnifyResultButtonLabels applies this to ALL FIVE labels (both panels); 14u lets
        // them sit large inside the design pills to match the mockup. AdjustGameUiLayout's
        // per-panel floor is lowered to 14u (both panels) so that generator stays a no-op.
        private const float LcBtnLabelMargin = 14f;

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
            // Claude Design plate `ui_lc_card` (repaired, 667x573 native): panel
            // BACKGROUND only - frame, cream, gem, inset, dividers, 4 row icons, medal
            // laurel/disc (dynamic rank letter erased). It has NO buttons baked in.
            // Overlays: TMP title / labels / values, the rank letter on the disc, 3
            // dynamic stars, and the 3 independent sliced buttons (LayoutResultButton).
            // Positions map the approved layout to Unity centre-origin: (x-333.5, 286.5-y).
            var crt = card as RectTransform;
            if (crt != null) crt.sizeDelta = new Vector2(667f, 573f);
            var cimg = card.GetComponent<Image>();
            if (cimg != null) SkinSimple(cimg, "ui_lc_card");

            // remove the sub-elements now baked into the plate
            foreach (var stale in new[] { "RowsPanel", "Divider0", "Divider1", "Divider2",
                "RowIcon0", "RowIcon1", "RowIcon2", "RowIcon3", "RankMedal",
                "RowIcon_TimeText", "RowIcon_BestTimeText", "RowIcon_RankText", "RowIcon_StarsText" })
            {
                var s = Find(card, stale);
                if (s != null) Object.DestroyImmediate(s.gameObject);
            }

            var refTmp = Find(card, "TitleText")?.GetComponent<TMP_Text>();

            // title
            var title = Find(card, "TitleText") as RectTransform;
            if (title != null)
            {
                Center(title, new Vector2(0f, 186f), new Vector2(560f, 74f));
                var tt = title.GetComponent<TMP_Text>();
                if (tt != null)
                {
                    tt.alignment = TextAlignmentOptions.Center;
                    tt.enableWordWrapping = false; tt.enableAutoSizing = false; tt.fontSize = 46f;
                    tt.color = Cocoa; tt.enableVertexGradient = false; ApplyBold(tt);
                }
            }
            SetText(card, "TitleText", "Level Complete!");

            // four rows: labels (left) + the two values (left-aligned column)
            float[] rowY = { 99.5f, 41.5f, -17.5f, -79.5f };
            LcRowLabel(card, "TimeLabel", "Time", rowY[0], refTmp, true);
            LcRowLabel(card, "BestLabel", "Best", rowY[1], refTmp, true);
            LcRowLabel(card, "RankText", null, rowY[2], refTmp, false);
            LcRowLabel(card, "StarsText", null, rowY[3], refTmp, false);
            LcRowValue(card, "TimeText", rowY[0], refTmp);
            LcRowValue(card, "BestTimeText", rowY[1], refTmp);
            SetText(card, "TimeText", "--"); SetText(card, "BestTimeText", "--");
            SetText(card, "RankText", "Rank"); SetText(card, "StarsText", "Stars");

            // "New!" badge for a new best time - a separate gold tag in the gap between
            // the "Best" label and its value (HUDController toggles it), so it never
            // shrinks the value or reaches the rank medal. Hidden by default.
            var newBadge = MakeChild(card, "BestNewBadge", typeof(RectTransform), typeof(TextMeshProUGUI));
            newBadge.anchorMin = newBadge.anchorMax = new Vector2(0.5f, 0.5f);
            newBadge.pivot = new Vector2(1f, 0.5f);
            newBadge.anchoredPosition = new Vector2(20f, rowY[1]); newBadge.sizeDelta = new Vector2(70f, 30f);
            var nb = newBadge.GetComponent<TextMeshProUGUI>();
            nb.text = "New!"; nb.alignment = TextAlignmentOptions.MidlineRight;
            nb.enableWordWrapping = false; nb.enableAutoSizing = false; nb.fontSize = 19f;
            nb.fontStyle = FontStyles.Bold; nb.raycastTarget = false;
            nb.color = new Color(0.93f, 0.60f, 0.13f);   // warm orange-gold highlight
            if (refTmp != null) nb.font = refTmp.font; ApplyBold(nb);
            EditorUtility.SetDirty(nb);
            newBadge.gameObject.SetActive(false);

            // live rank letter on the baked disc (S/A/B/C), styled to the design "A"
            var letter = MakeChild(card, "RankMedalLetter", typeof(RectTransform), typeof(TextMeshProUGUI));
            // Centre on the disc (plate px ~530,278 -> card 196.5,8.5) and size the glyph
            // to fill it like the design "A" (cap ~71px on the ~90px disc -> ~100pt).
            Center(letter, new Vector2(196.5f, 8.5f), new Vector2(120f, 120f));
            var lt = letter.GetComponent<TextMeshProUGUI>();
            lt.text = "A"; lt.alignment = TextAlignmentOptions.Center; lt.fontStyle = FontStyles.Bold;
            lt.enableAutoSizing = false; lt.fontSize = 100f; lt.raycastTarget = false;
            if (refTmp != null) lt.font = refTmp.font;
            var rankMat = LcRankMat();
            if (rankMat != null) lt.fontSharedMaterial = rankMat;
            else if (refTmp != null) lt.fontSharedMaterial = refTmp.fontSharedMaterial;
            lt.color = new Color(0.984f, 0.867f, 0.349f);   // design gold #FBDD59
            EditorUtility.SetDirty(lt); letter.SetAsLastSibling();

            // 3 dynamic stars on the Stars row (HUDController fills/dims per count)
            for (int i = 0; i < 3; i++)
                PlaceIcon(card, "Star" + i, "ui_result_star",
                    new Vector2(-8.5f + i * 63f, -73.5f), new Vector2(54f, 54f));

            // interactive buttons: 3 independent sliced UI buttons (Retry = cream, Next
            // Level = gold primary, Main Menu = cream) with separate TMP labels. Widths
            // keep a small gap so adjacent rects never overlap. ColorTint (set in the
            // skin loop) darkens only the pressed button - the clean cream plate behind
            // has no baked pill to expose.
            LayoutResultButton(card, "RetryButton", "ui_lc_btn_retry", -203.5f, -169.5f, 180f, 106f);
            LayoutResultButton(card, "NextLevelButton", "ui_lc_btn_next", -6.5f, -169.5f, 208f, 110f);
            LayoutResultButton(card, "MainMenuButton", "ui_lc_btn_menu", 198.5f, -169.5f, 192f, 106f);
            // labels styled + sized by UnifyResultButtonLabels (both panels together)
        }

        // LC row label: left-aligned (pivot 0) at x=-170.5; created when `text` is given.
        // Box width 116 (was 168): the labels are short left-aligned words ("Time".."Stars",
        // widest ~90u), so 116 fits them AND ends the rect at x-54.5 - clear of the "New!"
        // badge (x-50..20) on the Best row, so the overlap-regression audit stays at 0.
        private static void LcRowLabel(Transform card, string name, string text, float y, TMP_Text refTmp, bool create)
        {
            var rt = create
                ? MakeChild(card, name, typeof(RectTransform), typeof(TextMeshProUGUI))
                : Find(card, name) as RectTransform;
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(-170.5f, y); rt.sizeDelta = new Vector2(116f, 46f);
            var t = rt.GetComponent<TMP_Text>();
            if (t == null) return;
            if (text != null) t.text = text;
            t.alignment = TextAlignmentOptions.MidlineLeft; t.enableWordWrapping = false;
            t.enableAutoSizing = false; t.fontSize = RowTextSize; t.overflowMode = TextOverflowModes.Overflow;
            t.characterSpacing = 0f; t.color = Cocoa; if (refTmp != null) t.font = refTmp.font; ApplyBold(t);
        }

        // LC row value: left-aligned column at x=26, sized SMALLER than the labels
        // (design: values ~0.75x the label height) and boxed to ~108u so the numerals
        // end before the medal laurel (~x135) and the box clears the rank-letter box.
        private const float ValueTextSize = 24f;
        private static void LcRowValue(Transform card, string name, float y, TMP_Text refTmp)
        {
            var rt = Find(card, name) as RectTransform;
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(26f, y); rt.sizeDelta = new Vector2(108f, 40f);
            var t = rt.GetComponent<TMP_Text>();
            if (t == null) return;
            t.alignment = TextAlignmentOptions.MidlineLeft; t.enableWordWrapping = false;
            t.enableAutoSizing = false; t.fontSize = ValueTextSize; t.overflowMode = TextOverflowModes.Overflow;
            t.characterSpacing = 0f; t.color = Cocoa; if (refTmp != null) t.font = refTmp.font; ApplyBold(t);
        }

        // A result button = an independent, VISIBLE sliced UI Image (Retry / Next Level /
        // Main Menu). The panel background behind it is clean cream with NO baked pill, so
        // the button moves and presses as one control: its ColorTint press-darkening (set
        // in the skin loop, targetGraphic = this Image) is the only thing that changes,
        // never a shape underneath. Sizes/positions are the approved button-row layout,
        // kept a gap apart so adjacent rects never overlap. The label is a separate TMP
        // child (skinned by Fit*ButtonLabels). Keeps the existing Button/onClick wiring.
        // Idempotent.
        private static void LayoutResultButton(Transform card, string name, string sprite, float x, float y, float w, float h)
        {
            var rt = Find(card, name) as RectTransform;
            if (rt == null) return;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(x, y); rt.sizeDelta = new Vector2(w, h);
            var img = rt.GetComponent<Image>();
            if (img != null)
            {
                var sp = Sprite(sprite);
                if (sp != null) img.sprite = sp;
                img.type = Image.Type.Sliced; img.fillCenter = true;
                img.color = Color.white; img.useSpriteMesh = false; img.raycastTarget = true;
                img.SetAllDirty(); EditorUtility.SetDirty(img);
            }
            EditorUtility.SetDirty(rt);
        }

        private static void BuildGameOverExtras(Transform card)
        {
            // Claude Design plate `ui_go_card` (repaired, 575x558 native): panel
            // BACKGROUND only - frame, cream, gem and a clean cream interior. NO buttons
            // and NO cactus are baked in: the two buttons are independent sliced UI Images
            // and the per-world mascot is an overlay (WorldThemeApplier swaps GameOverMascot).
            // Overlay: TMP title, the mascot, the 2 sliced buttons + labels.
            // Unity centre-origin = (x-287.5, 279-y).
            var crt = card as RectTransform;
            if (crt != null) crt.sizeDelta = new Vector2(575f, 558f);
            var cimg = card.GetComponent<Image>();
            if (cimg != null) SkinSimple(cimg, "ui_go_card");

            var title = Find(card, "TitleText") as RectTransform;
            if (title != null)
            {
                Center(title, new Vector2(0f, 156f), new Vector2(420f, 78f));
                var tt = title.GetComponent<TMP_Text>();
                if (tt != null)
                {
                    tt.alignment = TextAlignmentOptions.Center;
                    tt.enableWordWrapping = false; tt.enableAutoSizing = false; tt.fontSize = 48f;
                    tt.color = Cocoa; tt.enableVertexGradient = false; ApplyBold(tt);
                }
            }
            SetText(card, "TitleText", "Game Over");

            // per-world mascot over the erased cactus spot (WorldThemeApplier swaps the
            // sprite). Sized so W01's visible bounds ~match the design cactus footprint.
            PlaceIcon(card, "GameOverMascot", "ui_gameover_mascot_01", new Vector2(0f, 12f), new Vector2(420f, 420f));

            // interactive buttons: 2 independent sliced cream UI buttons (Retry, Main
            // Menu) with separate TMP labels, kept a clear centre gap apart so the rects
            // never overlap. ColorTint darkens only the pressed button (clean cream plate
            // behind, no baked pill).
            LayoutResultButton(card, "RetryButton", "ui_result_btn_9s", -124.5f, -164f, 230f, 106f);
            LayoutResultButton(card, "MainMenuButton", "ui_result_btn_9s", 117.5f, -164f, 228f, 106f);
            // labels styled + sized by UnifyResultButtonLabels (both panels together)
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

        // ONE identical label style + ONE shared font size across ALL FIVE result
        // buttons - Level Complete (Retry / Next Level / Main Menu) AND Game Over
        // (Retry / Main Menu) - using the Level Complete label style as the reference.
        // No per-panel/per-label sizing, no TMP autosize, no RectTransform scaling.
        // The single size is the largest that fits the LONGEST label inside the
        // NARROWEST button (margin-inset), so "Main Menu"/"Next Level" fit comfortably;
        // the wider Game Over pills never lower it. Deterministic + idempotent. Run
        // AFTER both panels are built. Keeps StyleTypography (excludes result_btn/lc_btn)
        // and AdjustGameUiLayout (14u floor, both panels) as no-ops on these labels.
        private static void UnifyResultButtonLabels(UnityEngine.SceneManagement.Scene scene)
        {
            var mat = LcButtonMat();
            var font = FredokaFont();
            var spec = new[]
            {
                ("LevelCompletePanel", "RetryButton"),
                ("LevelCompletePanel", "NextLevelButton"),
                ("LevelCompletePanel", "MainMenuButton"),
                ("GameOverPanel", "RetryButton"),
                ("GameOverPanel", "MainMenuButton"),
            };
            var rects = new System.Collections.Generic.List<RectTransform>();
            var labels = new System.Collections.Generic.List<TMP_Text>();
            foreach (var (panelName, btnName) in spec)
            {
                var panel = FindDeep(scene, panelName);
                var card = panel != null ? Find(panel.transform, "Card") : null;
                var b = card != null ? Find(card, btnName) as RectTransform : null;
                var lbl = b != null ? b.GetComponentInChildren<TMP_Text>(true) : null;
                if (b == null || lbl == null) continue;
                rects.Add(b); labels.Add(lbl);
            }
            if (labels.Count == 0) return;

            // Identical style on every label. Weight comes from the LC material (a single
            // controlled face dilation), NOT fontStyle.Bold, so all five share one weight.
            const float baseSize = 40f;   // measurement ceiling
            foreach (var lbl in labels)
            {
                if (font != null) lbl.font = font;
                if (mat != null) lbl.fontSharedMaterial = mat;
                lbl.color = Cocoa;
                lbl.fontStyle &= ~FontStyles.Bold;
                lbl.characterSpacing = 0f;
                lbl.wordSpacing = 0f;
                lbl.lineSpacing = 0f;
                lbl.alignment = TextAlignmentOptions.Center;
                lbl.enableWordWrapping = false;
                lbl.enableAutoSizing = false;
                lbl.overflowMode = TextOverflowModes.Overflow;
                lbl.margin = new Vector4(LcBtnLabelMargin, 0f, LcBtnLabelMargin, 0f);
                lbl.rectTransform.localScale = Vector3.one;
                lbl.fontSize = baseSize;
                try { lbl.ForceMeshUpdate(); } catch { }   // initialise for a valid measure
            }

            // ONE shared size = the tightest fit across all five, measured at baseSize.
            float shared = baseSize;
            for (int i = 0; i < labels.Count; i++)
            {
                var lbl = labels[i];
                float pref;
                try { pref = lbl.GetPreferredValues(lbl.text, Mathf.Infinity, Mathf.Infinity).x; }
                catch { pref = 0f; }
                float est = (lbl.text != null ? lbl.text.Length : 0) * baseSize * 0.55f;
                pref = Mathf.Max(pref, est);
                float avail = rects[i].sizeDelta.x - LcBtnLabelMargin * 2f;
                if (pref > avail && pref > 0f) shared = Mathf.Min(shared, baseSize * avail / pref);
            }
            shared = Mathf.Max(20f, Mathf.Floor(shared * 2f) / 2f);
            foreach (var lbl in labels)
            {
                lbl.fontSize = shared;
                EditorUtility.SetDirty(lbl);
                EditorUtility.SetDirty(lbl.rectTransform);
            }
        }

        // The project's Fredoka SDF font asset - assigned to all five result-button
        // labels so their font (as well as material) is identical.
        private static TMP_FontAsset _fredoka;
        private static TMP_FontAsset FredokaFont() => _fredoka != null ? _fredoka
            : (_fredoka = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/_JebbyJump/Art/Fonts/Fredoka SDF.asset"));

        // Dedicated rank-letter material (NOT the shared title/bold material): heavier
        // face dilation + a crisp dark-orange edge (mockup's outlined gold "S/A/B/C").
        // Derived from the working Fredoka bold material so glyphs still render; only an
        // LC asset, so accepted UI is untouched. Idempotent (load-or-create).
        private static Material _lcRankMat;
        private static Material LcRankMat() => LcMat(ref _lcRankMat,
            "Fredoka SDF LC Rank.mat", 0.16f, 0.14f, new Color(0.36f, 0.18f, 0.05f, 1f));

        // LC BUTTON-LABEL material: single controlled face dilation instead of the
        // faux double-bold (fontStyle.Bold + shared 0.10 material) that over-widened the
        // Light glyphs and forced 20-23pt. ~0.14 ≈ the old effective weight but WITHOUT
        // the bold spacing, so the labels are narrower and fit ~30pt. LC-only asset.
        private static Material _lcBtnMat;
        private static Material LcButtonMat() => LcMat(ref _lcBtnMat,
            "Fredoka SDF LC.mat", 0.14f, 0f, default);

        // Load-or-create an LC-only TMP material derived from the working bold material
        // (keeps shader + atlas), overriding face dilation + optional edge. Idempotent.
        private static Material LcMat(ref Material cache, string file, float dilate, float outlineW, Color outlineC)
        {
            if (cache != null) return cache;
            var baseMat = BoldMat();
            if (baseMat == null) return null;
            string path = "Assets/_JebbyJump/Art/Fonts/" + file;
            var m = AssetDatabase.LoadAssetAtPath<Material>(path);
            bool created = m == null;
            if (created) m = new Material(baseMat);
            m.CopyPropertiesFromMaterial(baseMat);        // keep shader + atlas in sync
            m.SetFloat("_FaceDilate", dilate);
            if (outlineW > 0f)
            {
                m.SetFloat("_OutlineWidth", outlineW);
                m.SetColor("_OutlineColor", outlineC);
            }
            else m.SetFloat("_OutlineWidth", 0f);
            if (created) AssetDatabase.CreateAsset(m, path);
            EditorUtility.SetDirty(m);
            cache = m;
            return m;
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
            var badge = Find(card, "BestNewBadge");
            var pb = so.FindProperty("_levelCompleteNewBadge");
            if (pb != null && badge != null) pb.objectReferenceValue = badge.gameObject;
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
