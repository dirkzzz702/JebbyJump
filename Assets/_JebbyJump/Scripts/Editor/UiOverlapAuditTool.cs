using System.IO;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.EditorTools
{
    // Menu wrapper around the shared UiOverlapMeasurement core: runs the read-only
    // Game.unity overlap audit, logs the report, and writes it to
    // Builds/UiAudit/ui-overlap-report.txt. The measurement itself lives in
    // JebbyJump.QA.Editor so the EditMode regression test measures the exact same way.
    public static class UiOverlapAuditTool
    {
        [MenuItem("Jebby Jump/QA/Audit UI Overlaps")]
        public static void RunMenu()
        {
            UiOverlapMeasurement.CountTextOverlaps(out string report);
            Debug.Log(report);
            WriteReport("ui-overlap-report.txt", report);
        }

        [MenuItem("Jebby Jump/QA/Audit Main Menu UI Overlaps")]
        public static void RunMainMenuMenu()
        {
            UiOverlapMeasurement.CountMainMenuTextOverlaps(out string report);
            bool orderOk = UiOverlapMeasurement.MenuStackRendersBelowPanels(out string detail);
            report += $"\n[UiOverlapAudit] Menu stack below panels: {orderOk} ({detail})\n";
            Debug.Log(report);
            WriteReport("ui-overlap-mainmenu-report.txt", report);
        }

        private static void WriteReport(string fileName, string report)
        {
            try
            {
                string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Builds/UiAudit"));
                Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, fileName), report);
            }
            catch { }
        }
    }
}
