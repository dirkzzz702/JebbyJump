using System.Collections.Generic;
using JebbyJump.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// P21: wraps the Main Menu button stack in a safe-area root and enforces the
// >=90 touch-target minimum. The sibling panels (Level Select / Settings /
// Wardrobe) keep their own safe-area roots from their own scaffolds; the
// decorative background stays edge-to-edge. Idempotent. Run AFTER the other
// Main Menu scaffolds so all five buttons exist.
public static class BuildMainMenuShell
{
    private const string MainMenuScenePath =
        "Assets/_JebbyJump/Scenes/MainMenu.unity";

    [MenuItem("Jebby Jump/Scaffold/Build Main Menu Shell")]
    public static void Run()
    {
        if (!System.IO.File.Exists(MainMenuScenePath))
        {
            Debug.LogError("[Shell] MainMenu scene not found.");
            return;
        }
        var active = SceneManager.GetActiveScene();
        if (active.path != MainMenuScenePath)
        {
            if (active.isDirty) EditorSceneManager.SaveScene(active);
            EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
        }

        var menu = Object.FindFirstObjectByType<MainMenuController>(
            FindObjectsInactive.Include);
        if (menu == null) { Debug.LogError("[Shell] No MainMenuController."); return; }

        var so = new SerializedObject(menu);
        var buttons = new List<GameObject>();
        foreach (var field in new[]
        {
            "_continueButton", "_startButton", "_settingsButton",
            "_wardrobeButton", "_quitButton",
        })
        {
            var b = so.FindProperty(field).objectReferenceValue as Button;
            if (b != null) buttons.Add(b.gameObject);
        }
        if (buttons.Count == 0)
        {
            Debug.LogWarning("[Shell] No Main Menu buttons wired; abort.");
            return;
        }

        // Stable target parent across re-runs (skip an existing MenuSafeArea).
        Transform parent = buttons[0].transform.parent;
        if (parent != null && parent.name == "MenuSafeArea")
            parent = parent.parent;

        ShellScaffold.EnsureSafeAreaForObjects(parent, "MenuSafeArea", buttons);
        foreach (var b in buttons) ShellScaffold.EnsureMinHeight(b);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        Debug.Log("[Shell] Main Menu stack wrapped in MenuSafeArea; "
            + buttons.Count + " buttons sized >=90.");
    }
}
