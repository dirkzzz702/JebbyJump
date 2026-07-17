using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // One-time, idempotent UI-only layout fix for the overlaps found by UiOverlapAuditTool.
    // Re-spaces the LevelComplete + Pause vertical groups, lifts the staggered Skill3 button
    // clear of Skill1, and disables word-wrap on the LevelComplete stat texts so a long
    // dynamic value can't wrap to a 2nd line and re-overlap. Touches ONLY RectTransform
    // anchoredPosition/sizeDelta and TMP_Text.enableWordWrapping — no gameplay, script,
    // prefab, or logic change. Re-running is a no-op once spacing/wrap settings hold.
    public static class AdjustGameUiLayout
    {
        private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";

        [MenuItem("Jebby Jump/QA/Fix UI Overlaps")]
        public static void Run()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            int groups = 0;

            // LevelCompletePanel: the stats card (parent of TimeText) is a real 700x400 box
            // -> re-space around its centre (0) and grow the box if the content needs it.
            groups += RespacePanel(scene, "LevelCompletePanel", "TimeText",
                gap: 16f, pad: 26f, centreOnCard: true);

            // PausePanel: title + buttons are centred children of a full-screen container
            // -> re-space around the group's own centre (no box to grow).
            groups += RespacePanel(scene, "PausePanel", "Title",
                gap: 22f, pad: 0f, centreOnCard: false);

            // Approved 2026-07-17: the three skill buttons sit on a uniform
            // 160u arc around Btn_Jump (-130,140) at 90/135/180 degrees.
            // Verified issue: radii were 135/138/210 (Skill3 the old "nudge"
            // outlier) with a 7.7u Skill1-Skill2 edge gap. The arc gives
            // ~22.5u adjacent gaps + 40u clearance off the jump button.
            // (Supersedes the P33 NudgeSkill3 vertical lift.)
            groups += PinPos(scene, "Btn_Skill3", -130f, 300f); // 90 deg (top)
            groups += PinPos(scene, "Btn_Skill1", -243f, 253f); // 135 deg
            groups += PinPos(scene, "Btn_Skill2", -290f, 140f); // 180 deg (left)

            // LevelComplete stat texts carry variable-length suffixes ("(New!)", "(New Star
            // Best!)"). Disable word-wrap so a long value can't wrap to a 2nd line and
            // overflow into the row below, re-introducing an overlap.
            groups += SetResultTextsNoWrap(scene);

            // The Retry / Next Level / Main Menu row renders with the button edges (and
            // "Next Level"/"Main Menu" labels) touching -> re-space horizontally.
            groups += RespaceResultButtonRow(scene);

            // The button RECTS have 20u gaps, but the labels use TMP Overflow and spill
            // wider than their 180u buttons ("Next Level"/"Main Menu" glyphs ran together
            // in the rendered panel). Make labels fit: single-line + auto-size shrink.
            groups += FitPanelButtonLabels(scene, "LevelCompletePanel");
            groups += FitPanelButtonLabels(scene, "GameOverPanel");

            // Pause glyph was hairline "||" at 32pt regular in a 96u button
            // (verified 2026-07-17) - underweight next to the ornate arrow
            // icons. Bold 52pt + spacing reads as two solid pause bars.
            groups += StrengthenPauseGlyph(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[FixUiOverlaps] adjusted {groups} group(s); saved {ScenePath}");
        }

        private static int RespacePanel(UnityEngine.SceneManagement.Scene scene, string panelName,
            string anchorChild, float gap, float pad, bool centreOnCard)
        {
            var panel = FindByName(scene, panelName);
            if (panel == null) return 0;
            bool was = panel.activeSelf;
            panel.SetActive(true);
            Canvas.ForceUpdateCanvases();
            var child = FindInChildren(panel.transform, anchorChild);
            var card = child != null ? child.parent as RectTransform : null;
            if (card == null) { panel.SetActive(was); return 0; }
            LayoutRebuilder.ForceRebuildLayoutImmediate(card);
            RespaceGroup(card, gap, pad, centreOnCard);
            panel.SetActive(was);
            return 1;
        }

        // Re-spaces the card's vertically point-anchored graphic children as one centred
        // vertical stack. Elements sharing a Y (a button row) are kept on one row.
        private static void RespaceGroup(RectTransform card, float gap, float pad, bool centreOnCard)
        {
            var items = new List<Item>();
            for (int i = 0; i < card.childCount; i++)
            {
                var c = card.GetChild(i) as RectTransform;
                if (c == null) continue;
                if (!Mathf.Approximately(c.anchorMin.y, c.anchorMax.y)) continue;      // skip vertical-stretch bg
                if (c.GetComponent<Graphic>() == null && c.GetComponent<Selectable>() == null
                    && c.GetComponentInChildren<Graphic>(true) == null) continue;
                items.Add(new Item { RT = c, Y = c.anchoredPosition.y, H = c.rect.height });
            }
            if (items.Count == 0) return;
            items.Sort((a, b) => b.Y.CompareTo(a.Y));                                   // top first

            // group same-Y items into rows
            var rows = new List<List<Item>>();
            foreach (var it in items)
            {
                if (rows.Count > 0 && Mathf.Abs(rows[rows.Count - 1][0].Y - it.Y) < 12f)
                    rows[rows.Count - 1].Add(it);
                else rows.Add(new List<Item> { it });
            }

            float total = (rows.Count - 1) * gap;
            foreach (var row in rows) total += RowHeight(row);

            // centre: card centre (0) for a real box, else the group's current midpoint
            float centre = 0f;
            if (!centreOnCard)
            {
                float maxTop = float.MinValue, minBot = float.MaxValue;
                foreach (var it in items)
                {
                    maxTop = Mathf.Max(maxTop, it.Y + it.H / 2f);
                    minBot = Mathf.Min(minBot, it.Y - it.H / 2f);
                }
                centre = (maxTop + minBot) / 2f;
            }
            else if (total + pad * 2f > card.rect.height) // grow the box to fit the content
            {
                card.sizeDelta = new Vector2(card.sizeDelta.x, total + pad * 2f);
            }

            float top = centre + total / 2f;
            foreach (var row in rows)
            {
                float rh = RowHeight(row);
                float rowCentre = top - rh / 2f;
                foreach (var it in row)
                {
                    var p = it.RT.anchoredPosition;
                    it.RT.anchoredPosition = new Vector2(p.x, rowCentre);
                }
                top -= rh + gap;
            }
        }

        private static float RowHeight(List<Item> row)
        {
            float h = 0f;
            foreach (var it in row) h = Mathf.Max(h, it.H);
            return h;
        }

        // The four LevelComplete result stat texts whose runtime strings vary in length.
        private static readonly string[] ResultStatTexts =
            { "TimeText", "BestTimeText", "RankText", "StarsText" };

        // Disables word-wrap on the LevelComplete stat texts. Their runtime strings have
        // variable-length suffixes ("Best: 00:12.34  (New!)", "Stars: 3/3  (New Star
        // Best!)"); with wrapping on, a long value wraps to a 2nd line and overflows its
        // box into the row below. Single-line keeps each within its row. Idempotent (skips
        // texts already single-line). Matches WardrobePanelController's enableWordWrapping use.
        private static int SetResultTextsNoWrap(UnityEngine.SceneManagement.Scene scene)
        {
            var panel = FindByName(scene, "LevelCompletePanel");
            if (panel == null) return 0;
            int changed = 0;
            foreach (var name in ResultStatTexts)
            {
                var t = FindInChildren(panel.transform, name);
                var tmp = t != null ? t.GetComponent<TMP_Text>() : null;
                if (tmp == null || !tmp.enableWordWrapping) continue;
                tmp.enableWordWrapping = false;
                changed++;
            }
            return changed > 0 ? 1 : 0;
        }

        // Evenly re-spaces the LevelComplete action-button row (Retry / Next Level /
        // Main Menu) around the card centre with a fixed gap. The rendered row had the
        // button edges touching (labels visually running together) — below the audit's
        // 16u^2 graze threshold, hence unflagged. Idempotent: skips when every adjacent
        // gap is already >= 8. Sets ONLY anchoredPosition.x.
        private static int RespaceResultButtonRow(UnityEngine.SceneManagement.Scene scene)
        {
            var panel = FindByName(scene, "LevelCompletePanel");
            if (panel == null) return 0;
            bool was = panel.activeSelf;
            panel.SetActive(true);
            Canvas.ForceUpdateCanvases();
            var anchor = FindInChildren(panel.transform, "TimeText");
            var card = anchor != null ? anchor.parent as RectTransform : null;
            if (card == null) { panel.SetActive(was); return 0; }
            LayoutRebuilder.ForceRebuildLayoutImmediate(card);

            var row = new List<RectTransform>();
            foreach (var name in new[] { "RetryButton", "NextLevelButton", "MainMenuButton" })
            {
                var t = FindInChildren(card, name) as RectTransform;
                if (t != null) row.Add(t);
            }
            if (row.Count < 2) { panel.SetActive(was); return 0; }
            row.Sort((a, b) => a.anchoredPosition.x.CompareTo(b.anchoredPosition.x));

            bool cramped = false;
            for (int i = 1; i < row.Count; i++)
            {
                float prevRight = row[i - 1].anchoredPosition.x + row[i - 1].rect.width / 2f;
                float left = row[i].anchoredPosition.x - row[i].rect.width / 2f;
                if (left - prevRight < 8f) { cramped = true; break; }
            }
            if (!cramped) { panel.SetActive(was); return 0; }

            const float gap = 24f;
            float total = (row.Count - 1) * gap;
            foreach (var t in row) total += t.rect.width;
            float x = -total / 2f;
            foreach (var t in row)
            {
                var p = t.anchoredPosition;
                t.anchoredPosition = new Vector2(x + t.rect.width / 2f, p.y);
                x += t.rect.width + gap;
            }
            panel.SetActive(was);
            return 1;
        }

        private static readonly string[] ResultActionButtons =
            { "RetryButton", "NextLevelButton", "MainMenuButton" };

        // Makes the action-button labels fit their buttons: wrapping off (single line)
        // + TMP auto-size shrink (max = current size so short labels look unchanged).
        // Without this, TMP's default Overflow renders long labels wider than the
        // button rect and into the neighbouring button. Idempotent (skips labels
        // already auto-sizing).
        private static int FitPanelButtonLabels(UnityEngine.SceneManagement.Scene scene, string panelName)
        {
            var panel = FindByName(scene, panelName);
            if (panel == null) return 0;
            int changed = 0;
            foreach (var name in ResultActionButtons)
            {
                var t = FindInChildren(panel.transform, name);
                var tmp = t != null ? t.GetComponentInChildren<TMP_Text>(true) : null;
                if (tmp == null || tmp.enableAutoSizing) continue;
                tmp.enableWordWrapping = false;
                tmp.enableAutoSizing = true;
                tmp.fontSizeMax = tmp.fontSize;
                tmp.fontSizeMin = Mathf.Min(16f, tmp.fontSize);
                changed++;
            }
            return changed > 0 ? 1 : 0;
        }

        // Makes the pause "||" read as two solid bars: bold, 52pt (~54% of the
        // 96u button), slight character spacing between the pipes. Keeps the
        // "||" string (reliable LiberationSans coverage; block glyphs risk
        // missing-glyph boxes). Idempotent.
        private static int StrengthenPauseGlyph(UnityEngine.SceneManagement.Scene scene)
        {
            var btn = FindByName(scene, "PauseButton");
            var label = btn != null ? FindInChildren(btn.transform, "Label") : null;
            var tmp = label != null ? label.GetComponent<TMP_Text>() : null;
            if (tmp == null) return 0;
            bool changed = false;
            if (!Mathf.Approximately(tmp.fontSize, 52f)) { tmp.fontSize = 52f; changed = true; }
            if ((tmp.fontStyle & FontStyles.Bold) == 0)
            { tmp.fontStyle |= FontStyles.Bold; changed = true; }
            if (!Mathf.Approximately(tmp.characterSpacing, 6f))
            { tmp.characterSpacing = 6f; changed = true; }
            return changed ? 1 : 0;
        }

        // Pins anchoredPosition to exactly (x, y) (idempotent).
        private static int PinPos(UnityEngine.SceneManagement.Scene scene,
            string name, float x, float y)
        {
            var go = FindByName(scene, name);
            var rt = go != null ? go.transform as RectTransform : null;
            if (rt == null) return 0;
            var p = rt.anchoredPosition;
            if (Mathf.Approximately(p.x, x) && Mathf.Approximately(p.y, y)) return 0;
            rt.anchoredPosition = new Vector2(x, y);
            return 1;
        }

        private struct Item { public RectTransform RT; public float Y, H; }

        private static GameObject FindByName(UnityEngine.SceneManagement.Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name) return root;
                var f = FindInChildren(root.transform, name);
                if (f != null) return f.gameObject;
            }
            return null;
        }

        private static Transform FindInChildren(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var c = t.GetChild(i);
                if (c.name == name) return c;
                var r = FindInChildren(c, name);
                if (r != null) return r;
            }
            return null;
        }
    }
}
