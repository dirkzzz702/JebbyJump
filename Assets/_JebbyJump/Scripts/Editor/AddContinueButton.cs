using JebbyJump.Progression;
using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Idempotent scaffold: adds a Continue button to MainMenu.unity by
// cloning the existing Start button, relabels Start to "Level Select",
// stacks the three buttons Continue / Level Select / Quit, and wires
// MainMenuController._continueButton + _catalog. Re-running only refreshes
// wiring/labels/positions; it never duplicates the Continue button.
public static class AddContinueButton
{
    private const string MainMenuScenePath =
        "Assets/_JebbyJump/Scenes/MainMenu.unity";

    private const float ContinueY = 130f;
    private const float StartY    = 40f;
    private const float QuitY     = -50f;

    [MenuItem("Jebby Jump/Scaffold/Add Continue Button")]
    public static void Run()
    {
        if (!System.IO.File.Exists(MainMenuScenePath))
        {
            Debug.LogError(
                "[Continue] MainMenu scene not found at "
                + MainMenuScenePath);
            return;
        }

        EnsureSceneOpen();

        var menu = Object.FindFirstObjectByType<MainMenuController>(
            FindObjectsInactive.Include);
        if (menu == null)
        {
            Debug.LogError(
                "[Continue] No MainMenuController in the scene. Abort.");
            return;
        }

        var menuSo = new SerializedObject(menu);
        var startButton =
            menuSo.FindProperty("_startButton").objectReferenceValue
            as Button;
        var quitButton =
            menuSo.FindProperty("_quitButton").objectReferenceValue
            as Button;
        if (startButton == null)
        {
            Debug.LogError(
                "[Continue] MainMenuController._startButton is not wired; "
                + "need it as the clone template. Abort.");
            return;
        }

        Transform parent = startButton.transform.parent;
        float x = startButton.GetComponent<RectTransform>()
            .anchoredPosition.x;

        // Idempotent: reuse an existing ContinueButton if present.
        var existing = parent.Find("ContinueButton");
        Button continueButton;
        if (existing != null)
        {
            continueButton = existing.GetComponent<Button>();
            Debug.Log(
                "[Continue] ContinueButton already present - "
                + "refreshing label, position, and wiring only.");
        }
        else
        {
            var clone = Object.Instantiate(startButton.gameObject, parent);
            clone.name = "ContinueButton";
            continueButton = clone.GetComponent<Button>();
            Debug.Log("[Continue] Created ContinueButton (cloned Start).");
        }

        SetLabel(continueButton, "Continue");
        SetLabel(startButton, "Level Select");

        SetAnchoredY(continueButton, x, ContinueY);
        SetAnchoredY(startButton, x, StartY);
        if (quitButton != null) SetAnchoredY(quitButton, x, QuitY);

        // Put Continue at the top of the visual stack / sibling order.
        continueButton.transform.SetSiblingIndex(
            startButton.transform.GetSiblingIndex());

        // Wire references.
        menuSo.FindProperty("_continueButton").objectReferenceValue =
            continueButton;
        var catalogProp = menuSo.FindProperty("_catalog");
        if (catalogProp.objectReferenceValue == null)
        {
            var catalog = FindSingleCatalog();
            if (catalog != null)
                catalogProp.objectReferenceValue = catalog;
        }
        menuSo.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(menu);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        bool catalogWired =
            menuSo.FindProperty("_catalog").objectReferenceValue != null;
        Debug.Log(
            "[Continue] Wired _continueButton. _catalog wired: "
            + catalogWired
            + (catalogWired
                ? ""
                : " (no single LevelCatalog asset found - run "
                  + "'Jebby Jump/Progression/Create Or Sync Level Catalog' "
                  + "then re-run, or wire it by hand)."));
        Debug.Log(
            "[Continue] Buttons: Continue / Level Select / Quit.");
    }

    private static void EnsureSceneOpen()
    {
        var active = SceneManager.GetActiveScene();
        if (active.path != MainMenuScenePath)
        {
            if (active.isDirty)
                EditorSceneManager.SaveScene(active);
            EditorSceneManager.OpenScene(
                MainMenuScenePath, OpenSceneMode.Single);
        }
    }

    private static void SetLabel(Button button, string text)
    {
        if (button == null) return;
        var label = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            label.text = text;
            EditorUtility.SetDirty(label);
        }
    }

    private static void SetAnchoredY(Button button, float x, float y)
    {
        if (button == null) return;
        var rt = button.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        EditorUtility.SetDirty(rt);
    }

    private static LevelCatalog FindSingleCatalog()
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelCatalog");
        if (guids == null || guids.Length != 1) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<LevelCatalog>(path);
    }
}
