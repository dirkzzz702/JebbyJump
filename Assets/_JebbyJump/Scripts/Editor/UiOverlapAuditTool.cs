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
            try
            {
                string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Builds/UiAudit"));
                Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, "ui-overlap-report.txt"), report);
            }
            catch { }
        }
    }
}
