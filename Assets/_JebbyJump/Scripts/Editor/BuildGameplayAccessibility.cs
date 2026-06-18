using System.Collections.Generic;
using JebbyJump.Player;
using JebbyJump.Sequence;
using JebbyJump.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// P22 idempotent scaffold for the gameplay layer (Game.unity):
//   - SequenceCanvas -> SWSS 1920x1080 (visual/layout-only; RenderMode + sorting
//     are NOT touched, so it stays below GameShellCanvas) and SequencePanel is
//     wrapped in a SafeArea root (applies the top inset).
//   - HUD content + mobile controls wrapped in SafeArea roots (reusing the P21
//     SafeAreaFitter pattern; control sizes/relative positions are preserved).
//   - MobileControlsCanvas gets a CanvasGroup; the always-active HUDCanvas gets
//     a GameplayModalInputGate wired to the InputReader + result / game-over /
//     settings panels (blocks AND clears gameplay input under shell modals).
// Re-runs only refresh wiring; nothing is duplicated (verify: no 2nd-run diff).
public static class BuildGameplayAccessibility
{
    private const string GameScenePath = "Assets/_JebbyJump/Scenes/Game.unity";

    [MenuItem("Jebby Jump/Scaffold/Build Gameplay Accessibility")]
    public static void Run()
    {
        if (!System.IO.File.Exists(GameScenePath))
        {
            Debug.LogError("[GameplayA11y] Game scene not found at " + GameScenePath);
            return;
        }
        OpenScene(GameScenePath);

        Canvas hud = FindCanvas("HUDCanvas");
        Canvas controls = FindCanvas("MobileControlsCanvas");
        Canvas sequence = FindCanvas("SequenceCanvas");
        if (hud == null || controls == null)
        {
            Debug.LogError("[GameplayA11y] HUDCanvas / MobileControlsCanvas not found. Abort.");
            return;
        }

        // 1. Memory phase: convert to the standard SWSS scaler (visual-only) and
        //    wrap the swatch panel in a safe-area root for the top inset.
        if (sequence != null)
        {
            ShellScaffold.EnsureStandardScaler(sequence);
            GameObject panel = FindSequencePanel();
            if (panel != null)
                ShellScaffold.EnsureSafeAreaForObjects(
                    sequence.transform, "SafeArea", new List<GameObject> { panel });
            EditorUtility.SetDirty(sequence.gameObject);
        }
        else
        {
            Debug.LogWarning("[GameplayA11y] SequenceCanvas not found; skipped memory-phase hardening.");
        }

        // 2. HUD content (lives / level / timer / feedback) under a safe-area root.
        ShellScaffold.EnsureSafeAreaMoveAll(hud.gameObject);

        // 3. Mobile controls under a safe-area root + a CanvasGroup for the gate.
        ShellScaffold.EnsureSafeAreaMoveAll(controls.gameObject);
        var controlsGroup = controls.GetComponent<CanvasGroup>()
            ?? controls.gameObject.AddComponent<CanvasGroup>();

        // 4. Modal input gate on the always-active HUDCanvas (NOT the controls
        //    canvas, which is deactivated on desktop by MobileControlsToggle).
        var gate = hud.GetComponent<GameplayModalInputGate>()
            ?? hud.gameObject.AddComponent<GameplayModalInputGate>();
        WireGate(gate, controlsGroup);

        EditorUtility.SetDirty(hud.gameObject);
        EditorUtility.SetDirty(controls.gameObject);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        Debug.Log("[GameplayA11y] Game scaffolded: SequenceCanvas SWSS + safe-area "
            + "roots (HUD/controls/sequence) + modal input gate wired.");
    }

    private static void WireGate(GameplayModalInputGate gate, CanvasGroup controlsGroup)
    {
        var so = new SerializedObject(gate);
        so.FindProperty("_controlsGroup").objectReferenceValue = controlsGroup;

        // Use the SAME InputReader asset the gameplay reads.
        var player = Object.FindFirstObjectByType<PlayerController>(
            FindObjectsInactive.Include);
        if (player != null)
        {
            var input = new SerializedObject(player)
                .FindProperty("_input").objectReferenceValue;
            if (input != null) so.FindProperty("_input").objectReferenceValue = input;
        }
        else
        {
            Debug.LogWarning("[GameplayA11y] No PlayerController; gate _input unwired.");
        }

        // Explicit modal panels (result + game over) from the HUDController refs.
        var hudCtrl = Object.FindFirstObjectByType<HUDController>(
            FindObjectsInactive.Include);
        if (hudCtrl != null)
        {
            var hso = new SerializedObject(hudCtrl);
            SetIfPresent(so, "_resultPanel",
                hso.FindProperty("_levelCompletePanel")?.objectReferenceValue);
            SetIfPresent(so, "_gameOverPanel",
                hso.FindProperty("_gameOverPanel")?.objectReferenceValue);
        }

        // Optional: pause->settings panel (PauseState already covers pause, but
        // the explicit panel ref keeps the gate defensive).
        var settings = Object.FindFirstObjectByType<SettingsPanelController>(
            FindObjectsInactive.Include);
        if (settings != null)
            so.FindProperty("_settingsPanel").objectReferenceValue = settings.gameObject;

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(gate);
    }

    private static GameObject FindSequencePanel()
    {
        var display = Object.FindFirstObjectByType<SequenceDisplayUI>(
            FindObjectsInactive.Include);
        if (display == null) return null;
        var container = new SerializedObject(display)
            .FindProperty("_container").objectReferenceValue as RectTransform;
        return container != null ? container.gameObject : null;
    }

    private static void SetIfPresent(SerializedObject so, string field, Object value)
    {
        if (value == null) return;
        var prop = so.FindProperty(field);
        if (prop != null) prop.objectReferenceValue = value;
    }

    private static Canvas FindCanvas(string goName)
    {
        var canvases = Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < canvases.Length; i++)
            if (canvases[i].name == goName) return canvases[i];
        return null;
    }

    private static void OpenScene(string path)
    {
        var active = SceneManager.GetActiveScene();
        if (active.path != path)
        {
            if (active.isDirty) EditorSceneManager.SaveScene(active);
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }
    }
}
