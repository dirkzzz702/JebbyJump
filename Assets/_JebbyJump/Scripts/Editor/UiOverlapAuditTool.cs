using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace JebbyJump.EditorTools
{
    // READ-ONLY diagnostic (no gameplay/scene save). Opens Game.unity, measures the
    // gameplay HUD + each modal panel in canvas-reference space, and reports overlapping
    // text/interactive element pairs. Panels are activated + layout-rebuilt one at a time
    // (in isolation) so their real laid-out positions are measured; their active state is
    // restored afterward and the scene is NOT saved.
    public static class UiOverlapAuditTool
    {
        private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";

        // Groups measured in isolation. "GameplayHUD" = the always-active HUD + controls.
        private static readonly string[] ModalPanels =
            { "LevelCompletePanel", "GameOverPanel", "PausePanel", "SettingsPanel" };

        // Names whose subtree starts a DIFFERENT group (don't recurse across these).
        private static readonly HashSet<string> GroupBoundaries = new HashSet<string>
        {
            "LevelCompletePanel", "GameOverPanel", "PausePanel", "PauseMenu",
            "SettingsPanel", "FeedbackRoot", "TutorialHintRoot",
        };

        [MenuItem("Jebby Jump/QA/Audit UI Overlaps")]
        public static void RunMenu() => Debug.Log(Run());

        public static string Run()
        {
            string prev = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var scene = EditorSceneManager.GetActiveScene();

            var sb = new StringBuilder();
            sb.AppendLine("[UiOverlapAudit] Game.unity — overlapping text/interactive pairs (canvas-ref units).");
            int total = 0;

            // --- GameplayHUD: HUDCanvas text/controls + MobileControlsCanvas controls ---
            var hud = new List<Elem>();
            CollectByCanvas(scene, "HUDCanvas", hud);
            CollectByCanvas(scene, "MobileControlsCanvas", hud);
            total += ReportGroup(sb, "GameplayHUD", hud);

            // --- each modal panel, activated in isolation so layout is real ---
            foreach (var panelName in ModalPanels)
            {
                var root = FindByName(scene, panelName);
                if (root == null) continue;
                bool wasActive = root.activeSelf;
                root.SetActive(true);
                Canvas.ForceUpdateCanvases();
                var prt = root.transform as RectTransform;
                if (prt != null) LayoutRebuilder.ForceRebuildLayoutImmediate(prt);

                var list = new List<Elem>();
                CollectDescendants(root.transform, list, includeRootChildrenOnly: false, stopAtBoundaries: true);
                total += ReportGroup(sb, panelName + (wasActive ? "" : " (activated for audit)"), list);

                root.SetActive(wasActive);
            }

            sb.AppendLine($"\n[UiOverlapAudit] TOTAL overlapping pairs: {total}");

            try
            {
                string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Builds/UiAudit"));
                Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, "ui-overlap-report.txt"), sb.ToString());
            }
            catch { }

            if (!string.IsNullOrEmpty(prev) && prev != ScenePath)
                EditorSceneManager.OpenScene(prev, OpenSceneMode.Single);

            return sb.ToString();
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

        private static void CollectByCanvas(UnityEngine.SceneManagement.Scene scene, string canvasName, List<Elem> outList)
        {
            var canvasGo = FindByName(scene, canvasName);
            if (canvasGo != null) CollectDescendants(canvasGo.transform, outList, false, true);
        }

        private static void CollectDescendants(Transform t, List<Elem> outList, bool includeRootChildrenOnly, bool stopAtBoundaries)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);
                if (stopAtBoundaries && GroupBoundaries.Contains(child.name)) continue; // different group
                AddIfElement(child, outList);
                CollectDescendants(child, outList, includeRootChildrenOnly, stopAtBoundaries);
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
