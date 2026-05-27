using System.Collections.Generic;
using JebbyJump.Level;
using JebbyJump.Progression;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor menu item: read the LevelSessionController._levels array out
// of Game.unity and write the same ordered LevelConfig list into
// Assets/_JebbyJump/Settings/Level/LevelCatalog.asset. Creates the
// asset if missing. Single source of truth for level ordering remains
// the Game scene; the catalog is its menu-side mirror.
public static class CreateOrSyncLevelCatalog
{
    private const string GameScenePath =
        "Assets/_JebbyJump/Scenes/Game.unity";
    private const string CatalogAssetPath =
        "Assets/_JebbyJump/Settings/Level/LevelCatalog.asset";

    [MenuItem("Jebby Jump/Progression/Create Or Sync Level Catalog")]
    public static void Run()
    {
        if (!System.IO.File.Exists(GameScenePath))
        {
            Debug.LogError(
                "[LevelCatalog] Game scene not found at " + GameScenePath);
            return;
        }

        // Open Game.unity additively so we can inspect its components
        // without disturbing whatever scene the user has open.
        var openedScene = EditorSceneManager.OpenScene(
            GameScenePath, OpenSceneMode.Additive);

        try
        {
            var levels = ReadLevelsFromScene(openedScene);
            if (levels == null)
            {
                Debug.LogError(
                    "[LevelCatalog] Could not find a LevelSessionController "
                    + "in Game.unity. Aborting — catalog left untouched.");
                return;
            }
            if (levels.Count == 0)
            {
                Debug.LogError(
                    "[LevelCatalog] LevelSessionController._levels is empty. "
                    + "Aborting — catalog left untouched.");
                return;
            }

            WriteCatalog(levels);
        }
        finally
        {
            EditorSceneManager.CloseScene(openedScene, true);
        }
    }

    private static List<LevelConfig> ReadLevelsFromScene(Scene scene)
    {
        var roots = scene.GetRootGameObjects();
        LevelSessionController session = null;
        for (int i = 0; i < roots.Length; i++)
        {
            session =
                roots[i].GetComponentInChildren<LevelSessionController>(true);
            if (session != null) break;
        }
        if (session == null) return null;

        var so = new SerializedObject(session);
        var levelsProp = so.FindProperty("_levels");
        if (levelsProp == null || !levelsProp.isArray) return null;

        var result = new List<LevelConfig>(levelsProp.arraySize);
        for (int i = 0; i < levelsProp.arraySize; i++)
        {
            var element = levelsProp.GetArrayElementAtIndex(i);
            var cfg = element.objectReferenceValue as LevelConfig;
            result.Add(cfg);
        }
        return result;
    }

    private static void WriteCatalog(List<LevelConfig> levels)
    {
        bool created = false;
        var catalog =
            AssetDatabase.LoadAssetAtPath<LevelCatalog>(CatalogAssetPath);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<LevelCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogAssetPath);
            created = true;
        }

        var so = new SerializedObject(catalog);
        var levelsProp = so.FindProperty("_levels");
        levelsProp.arraySize = levels.Count;
        for (int i = 0; i < levels.Count; i++)
        {
            levelsProp.GetArrayElementAtIndex(i).objectReferenceValue =
                levels[i];
        }
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();

        Debug.Log(
            "[LevelCatalog] "
            + (created ? "Created " : "Updated ")
            + CatalogAssetPath + " with " + levels.Count + " level(s).");
        for (int i = 0; i < levels.Count; i++)
        {
            string name = levels[i] != null ? levels[i].name : "<null>";
            Debug.Log("  [" + i + "] " + name);
        }
        Debug.Log("[LevelCatalog] In sync with Game.unity.");
    }
}
