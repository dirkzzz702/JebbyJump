using JebbyJump.Shell;
using JebbyJump.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Shared idempotent helpers for the P21 shell accessibility scaffolds: a
// safe-area content root (dim backdrop stays edge-to-edge), touch-target
// sizing, and the standard landscape CanvasScaler. Reused by the per-panel
// scaffolds + BuildGameShellCanvas + BuildMainMenuShell so the pattern + sizes
// live in one place.
internal static class ShellScaffold
{
    // Ensures `panel` has a full-stretch "SafeArea" child (with a SafeAreaFitter
    // targeting itself) and moves the named direct-content children into it.
    // The panel's own backdrop Image is left on the panel (edge-to-edge).
    // Idempotent: already-moved children + an existing fitter are reused.
    public static RectTransform EnsureSafeArea(
        GameObject panel, string[] contentNames)
    {
        var existing = panel.transform.Find("SafeArea") as RectTransform;
        RectTransform safe;
        if (existing != null) safe = existing;
        else
        {
            var go = new GameObject("SafeArea", typeof(RectTransform));
            go.transform.SetParent(panel.transform, false);
            Stretch(go.GetComponent<RectTransform>());
            safe = go.GetComponent<RectTransform>();
        }

        var fitter = safe.GetComponent<SafeAreaFitter>()
            ?? safe.gameObject.AddComponent<SafeAreaFitter>();
        var so = new SerializedObject(fitter);
        so.FindProperty("_target").objectReferenceValue = safe;
        so.ApplyModifiedPropertiesWithoutUndo();

        foreach (var name in contentNames)
        {
            var t = FindDeep(panel.transform, name);
            if (t != null && t.parent != safe) t.SetParent(safe, false);
        }
        return safe;
    }

    // Raises a control's RectTransform height to at least the shell touch
    // minimum WITHOUT changing its width (hit area only).
    public static void EnsureMinHeight(Transform root, string name)
    {
        var t = FindDeep(root, name);
        if (t == null) return;
        var rt = t.GetComponent<RectTransform>();
        if (rt == null) return;
        float min = ShellLayoutMetrics.MinTouchTargetCanvasUnits;
        if (rt.sizeDelta.y < min)
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, min);
    }

    // Standard landscape CanvasScaler (Scale With Screen Size, 1920x1080,
    // match 0.5) - the project-wide convention.
    public static void EnsureStandardScaler(Canvas canvas)
    {
        var scaler = canvas.GetComponent<CanvasScaler>()
            ?? canvas.gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode =
            CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        EditorUtility.SetDirty(scaler);
    }

    public static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
    }

    public static Transform FindDeep(Transform root, string name)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var c = root.GetChild(i);
            if (c.name == name) return c;
            var r = FindDeep(c, name);
            if (r != null) return r;
        }
        return null;
    }
}
