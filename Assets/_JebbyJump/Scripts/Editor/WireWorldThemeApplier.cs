using JebbyJump.Level;
using JebbyJump.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Wires the WorldThemeApplier into Game.unity (WorldExpansion100, P34C).
// Creates a dedicated "WorldTheme" root object, adds the component, and binds
// the catalog + session + the Background / FloorVisual SpriteRenderers that
// already exist in the scene. Idempotent: re-running with nothing to change
// writes nothing.
public static class WireWorldThemeApplier
{
    private const string GameScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
    private const string CatalogPath =
        "Assets/_JebbyJump/Settings/World/WorldCatalog.asset";
    private const string RootName = "WorldTheme";

    [MenuItem("Jebby Jump/Release/Wire World Theme Applier")]
    public static void Run()
    {
        var scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);

        var catalog = AssetDatabase.LoadAssetAtPath<WorldCatalog>(CatalogPath);
        if (catalog == null)
        {
            Debug.LogError("[WireWorldTheme] WorldCatalog not found at "
                + CatalogPath + ". Run 'Create Or Sync World Catalog' first.");
            return;
        }

        LevelSessionController session = null;
        SpriteRenderer background = null, floorVisual = null;
        GameObject root = null;

        foreach (var go in scene.GetRootGameObjects())
        {
            if (go.name == RootName) root = go;
            if (session == null)
                session = go.GetComponentInChildren<LevelSessionController>(true);
            foreach (var sr in go.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (background == null && sr.gameObject.name == "Background")
                    background = sr;
                if (floorVisual == null && sr.gameObject.name == "FloorVisual")
                    floorVisual = sr;
            }
        }

        if (session == null)
        {
            Debug.LogError("[WireWorldTheme] No LevelSessionController in Game.unity.");
            return;
        }
        if (background == null)
            Debug.LogWarning("[WireWorldTheme] No 'Background' SpriteRenderer found.");
        if (floorVisual == null)
            Debug.LogWarning("[WireWorldTheme] No 'FloorVisual' SpriteRenderer found.");

        bool dirty = false;
        if (root == null)
        {
            root = new GameObject(RootName);
            dirty = true;
        }

        var applier = root.GetComponent<WorldThemeApplier>();
        if (applier == null)
        {
            applier = root.AddComponent<WorldThemeApplier>();
            dirty = true;
        }

        var so = new SerializedObject(applier);
        dirty |= SetRef(so, "_catalog", catalog);
        dirty |= SetRef(so, "_levelSession", session);
        dirty |= SetRef(so, "_background", background);
        dirty |= SetRef(so, "_floorVisual", floorVisual);
        if (so.hasModifiedProperties) so.ApplyModifiedPropertiesWithoutUndo();

        if (dirty)
        {
            EditorUtility.SetDirty(applier);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[WireWorldTheme] WorldThemeApplier wired into Game.unity ("
                + RootName + ").");
        }
        else Debug.Log("[WireWorldTheme] already up to date");
    }

    private static bool SetRef(SerializedObject so, string field, Object value)
    {
        var p = so.FindProperty(field);
        if (p == null || p.objectReferenceValue == value) return false;
        p.objectReferenceValue = value;
        return true;
    }
}
