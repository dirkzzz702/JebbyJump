using System.Collections.Generic;
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
    // Full-screen panel: move ALL direct content children under a full-stretch
    // "SafeArea" (the panel's own backdrop Image stays edge-to-edge).
    // Idempotent: on re-run the only direct child is SafeArea, so nothing moves.
    public static RectTransform EnsureSafeAreaMoveAll(GameObject panel)
    {
        var safe = EnsureSafeAreaRoot(panel.transform, "SafeArea");
        var toMove = new List<Transform>();
        foreach (Transform c in panel.transform)
            if (c != safe) toMove.Add(c);
        foreach (var c in toMove) c.SetParent(safe, false);
        return safe;
    }

    // Move specific objects under a named SafeArea child of `parent` (e.g. the
    // Main Menu button stack, leaving sibling panels alone).
    public static RectTransform EnsureSafeAreaForObjects(
        Transform parent, string safeName, IList<GameObject> objects)
    {
        var safe = EnsureSafeAreaRoot(parent, safeName);
        foreach (var go in objects)
            if (go != null && go.transform.parent != safe)
                go.transform.SetParent(safe, false);
        return safe;
    }

    private static RectTransform EnsureSafeAreaRoot(Transform parent, string name)
    {
        var existing = parent.Find(name) as RectTransform;
        RectTransform safe;
        if (existing != null) safe = existing;
        else
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            Stretch(go.GetComponent<RectTransform>());
            safe = go.GetComponent<RectTransform>();
        }
        var fitter = safe.GetComponent<SafeAreaFitter>()
            ?? safe.gameObject.AddComponent<SafeAreaFitter>();
        var so = new SerializedObject(fitter);
        so.FindProperty("_target").objectReferenceValue = safe;
        so.ApplyModifiedPropertiesWithoutUndo();
        return safe;
    }

    // Raises a control's RectTransform height to at least the shell touch
    // minimum WITHOUT changing its width (hit area only - keeps small visuals).
    public static void EnsureMinHeight(GameObject control)
    {
        if (control == null) return;
        var rt = control.GetComponent<RectTransform>();
        if (rt == null) return;
        float min = ShellLayoutMetrics.MinTouchTargetCanvasUnits;
        if (rt.sizeDelta.y < min)
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, min);
    }

    public static void EnsureMinHeight(Transform root, string name)
    {
        var t = FindDeep(root, name);
        if (t != null) EnsureMinHeight(t.gameObject);
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
