using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // One-time, idempotent UI-only layout fix for the overlaps found by UiOverlapAuditTool.
    // Re-spaces the LevelComplete + Pause vertical groups and lifts the staggered Skill3
    // button clear of Skill1. Sets ONLY RectTransform anchoredPosition/sizeDelta — no
    // gameplay, script, prefab, or logic change. Re-running is a no-op once spacing holds.
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

            // Skill3 sits diagonally over Skill1 -> raise it clear (pure vertical move).
            groups += NudgeSkill3(scene);

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

        private static int NudgeSkill3(UnityEngine.SceneManagement.Scene scene)
        {
            var go = FindByName(scene, "Btn_Skill3");
            var rt = go != null ? go.transform as RectTransform : null;
            if (rt == null) return 0;
            var p = rt.anchoredPosition;
            // Skill1 top is ~290 (centre 240, h100). Raise Skill3 to centre 350 -> bottom 300.
            if (p.y < 340f) rt.anchoredPosition = new Vector2(p.x, 350f);
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
