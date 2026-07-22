using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // READ-ONLY overlap measurement core (no scene save, no file I/O). Opens a scene,
    // measures each always-active group and each modal panel in canvas-reference space,
    // and returns the number of overlapping text/interactive element pairs plus a
    // human-readable report. Panels are activated + layout-rebuilt one at a time (in
    // isolation) so their real laid-out positions are measured; their active state is
    // restored afterward and the scene is NOT saved. Reused by the menu audit tools
    // (UiOverlapAuditTool) and the EditMode regression tests so the two never disagree.
    public static class UiOverlapMeasurement
    {
        public const string GameScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
        public const string MainMenuScenePath = "Assets/_JebbyJump/Scenes/MainMenu.unity";

        // Kept for the original Game-scene API.
        public const string ScenePath = GameScenePath;

        // ---- Game.unity model ----------------------------------------------------

        // Groups measured in isolation. "GameplayHUD" = the always-active HUD + controls.
        private static readonly string[] GameModalPanels =
            { "LevelCompletePanel", "GameOverPanel", "PausePanel", "SettingsPanel" };

        // Names whose subtree starts a DIFFERENT group (don't recurse across these).
        private static readonly HashSet<string> GameGroupBoundaries = new HashSet<string>
        {
            "LevelCompletePanel", "GameOverPanel", "PausePanel", "PauseMenu",
            "SettingsPanel", "FeedbackRoot", "TutorialHintRoot",
        };

        // Opens Game.unity, measures all groups, restores the previously active scene,
        // and returns the total overlapping-pair count (report text via out param).
        public static int CountTextOverlaps(out string report)
        {
            return MeasureScene(GameScenePath,
                new[] { new[] { "HUDCanvas", "MobileControlsCanvas" } },
                new[] { "GameplayHUD" },
                GameModalPanels, GameGroupBoundaries, out report);
        }

        // ---- MainMenu.unity model ------------------------------------------------

        private static readonly string[] MenuModalPanels =
            { "LevelSelectPanel", "SettingsPanel", "WardrobePanel", "StoryOverlay" };

        private static readonly HashSet<string> MenuGroupBoundaries = new HashSet<string>
        {
            "LevelSelectPanel", "SettingsPanel", "WardrobePanel", "UnlockCeremonyOverlay",
            "StoryOverlay",
        };

        // Opens MainMenu.unity and measures the always-active menu group (title + button
        // stack) and each panel in isolation. Menu-stack-vs-open-panel intersection is
        // intentionally NOT a violation: panels are modal covers that render above the
        // stack (see MenuStackRendersBelowPanels for that guarantee).
        public static int CountMainMenuTextOverlaps(out string report)
        {
            return MeasureScene(MainMenuScenePath,
                new[] { new[] { "MainMenuCanvas" } },
                new[] { "MenuShell" },
                MenuModalPanels, MenuGroupBoundaries, out report);
        }

        // True when the always-active MenuSafeArea button stack is an EARLIER sibling
        // than every modal panel under MainMenuCanvas, so an open panel (full-screen
        // raycast-blocking backdrop) renders above the stack and blocks its buttons.
        public static bool MenuStackRendersBelowPanels(out string detail)
        {
            string prev = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            var scene = EditorSceneManager.GetActiveScene();

            bool ok = false;
            var sb = new StringBuilder();
            var stack = FindByName(scene, "MenuSafeArea");
            if (stack == null)
            {
                sb.Append("MenuSafeArea not found");
            }
            else
            {
                int stackIdx = stack.transform.GetSiblingIndex();
                ok = true;
                foreach (var panelName in MenuModalPanels)
                {
                    var panel = FindByName(scene, panelName);
                    if (panel == null || panel.transform.parent != stack.transform.parent)
                        continue;
                    int idx = panel.transform.GetSiblingIndex();
                    sb.Append($"{panelName}={idx} ");
                    if (idx < stackIdx) ok = false;
                }
                sb.Append($"MenuSafeArea={stackIdx}");
            }
            detail = sb.ToString();

            if (!string.IsNullOrEmpty(prev) && prev != MainMenuScenePath)
                EditorSceneManager.OpenScene(prev, OpenSceneMode.Single);
            return ok;
        }

        // ---- shared measurement --------------------------------------------------

        private static int MeasureScene(string scenePath, string[][] alwaysGroupRoots,
            string[] alwaysGroupNames, string[] modalPanels,
            HashSet<string> groupBoundaries, out string report)
        {
            string prev = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var scene = EditorSceneManager.GetActiveScene();

            var sb = new StringBuilder();
            sb.AppendLine($"[UiOverlapAudit] {System.IO.Path.GetFileName(scenePath)} — overlapping text/interactive pairs (canvas-ref units).");
            int total = 0;

            for (int g = 0; g < alwaysGroupRoots.Length; g++)
            {
                var list = new List<Elem>();
                foreach (var rootName in alwaysGroupRoots[g])
                {
                    var rootGo = FindByName(scene, rootName);
                    if (rootGo != null)
                        CollectDescendants(rootGo.transform, list, groupBoundaries);
                }
                total += ReportGroup(sb, alwaysGroupNames[g], list);
            }

            // --- each modal panel, activated in isolation so layout is real ---
            foreach (var panelName in modalPanels)
            {
                var root = FindByName(scene, panelName);
                if (root == null) continue;
                bool wasActive = root.activeSelf;
                root.SetActive(true);
                Canvas.ForceUpdateCanvases();
                var prt = root.transform as RectTransform;
                if (prt != null) LayoutRebuilder.ForceRebuildLayoutImmediate(prt);

                var list = new List<Elem>();
                CollectDescendants(root.transform, list, groupBoundaries);
                total += ReportGroup(sb, panelName + (wasActive ? "" : " (activated for audit)"), list);

                root.SetActive(wasActive);
            }

            sb.AppendLine($"\n[UiOverlapAudit] TOTAL overlapping pairs: {total}");
            report = sb.ToString();

            if (!string.IsNullOrEmpty(prev) && prev != scenePath)
                EditorSceneManager.OpenScene(prev, OpenSceneMode.Single);

            return total;
        }

        private struct Elem
        {
            public string Name;
            public RectTransform RT;
            public Rect Rect;
            public bool IsText, Active;
        }

        private static int ReportGroup(StringBuilder sb, string group, List<Elem> list)
        {
            int count = 0;
            var lines = new List<string>();
            for (int i = 0; i < list.Count; i++)
                for (int j = i + 1; j < list.Count; j++)
                {
                    var a = list[i]; var b = list[j];
                    if (IsAncestor(a.RT, b.RT) || IsAncestor(b.RT, a.RT)) continue;
                    if (!(a.IsText || b.IsText)) continue;           // focus on text overlaps
                    float area = OverlapArea(a.Rect, b.Rect);
                    if (area <= 16f) continue;                        // ignore <4x4 grazes
                    lines.Add($"    {area:F0}u^2  '{a.Name}'{Fmt(a)}  <->  '{b.Name}'{Fmt(b)}");
                    count++;
                }
            if (count > 0)
            {
                sb.AppendLine($"\n== {group}  ({count} overlap(s)) ==");
                foreach (var l in lines) sb.AppendLine(l);
            }
            else sb.AppendLine($"\n== {group}: no text overlaps ==");
            return count;
        }

        private static void CollectDescendants(Transform t, List<Elem> outList, HashSet<string> boundaries)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);
                if (boundaries.Contains(child.name)) continue; // different group
                AddIfElement(child, outList);
                CollectDescendants(child, outList, boundaries);
            }
        }

        private static void AddIfElement(Transform t, List<Elem> outList)
        {
            var rt = t as RectTransform;
            if (rt == null) return;
            bool isText = t.GetComponent<TMP_Text>() != null;
            bool isSelectable = t.GetComponent<Selectable>() != null;
            if (!isText && !isSelectable) return;
            var canvas = t.GetComponentInParent<Canvas>();
            if (canvas == null) return;
            var root = canvas.rootCanvas.transform as RectTransform;
            outList.Add(new Elem
            {
                Name = t.name, RT = rt, Rect = RootLocalRect(rt, root),
                IsText = isText, Active = t.gameObject.activeInHierarchy,
            });
        }

        private static Rect RootLocalRect(RectTransform rt, RectTransform root)
        {
            var c = new Vector3[4];
            rt.GetWorldCorners(c);
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
            for (int i = 0; i < 4; i++)
            {
                Vector3 l = root.InverseTransformPoint(c[i]);
                if (l.x < minX) minX = l.x; if (l.x > maxX) maxX = l.x;
                if (l.y < minY) minY = l.y; if (l.y > maxY) maxY = l.y;
            }
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private static bool IsAncestor(RectTransform a, RectTransform node)
        {
            Transform t = node.parent;
            while (t != null) { if (t == a) return true; t = t.parent; }
            return false;
        }

        private static float OverlapArea(Rect a, Rect b)
        {
            float x = Mathf.Max(0, Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin));
            float y = Mathf.Max(0, Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin));
            return x * y;
        }

        private static string Fmt(Elem e)
            => $"[{(e.Active ? "on " : "off")} x{e.Rect.xMin:F0}..{e.Rect.xMax:F0} y{e.Rect.yMin:F0}..{e.Rect.yMax:F0}]";

        private static GameObject FindByName(UnityEngine.SceneManagement.Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == name) return root;
                var found = FindInChildren(root.transform, name);
                if (found != null) return found;
            }
            return null;
        }

        private static GameObject FindInChildren(Transform t, string name)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var c = t.GetChild(i);
                if (c.name == name) return c.gameObject;
                var r = FindInChildren(c, name);
                if (r != null) return r;
            }
            return null;
        }
    }
}
