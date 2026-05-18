using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using JebbyJump.Level;

public class Phase15WireLevels
{
    public static void Execute()
    {
        var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (!activeScene.path.Contains("Game"))
        {
            Debug.LogError("[Phase15WireLevels] Game scene must be open. Currently open: " + activeScene.path);
            return;
        }

        // Load the three level config assets
        var level1 = AssetDatabase.LoadAssetAtPath<LevelConfig>("Assets/_JebbyJump/Settings/Level/Level1Config.asset");
        var level2 = AssetDatabase.LoadAssetAtPath<LevelConfig>("Assets/_JebbyJump/Settings/Level/Level2Config.asset");
        var level3 = AssetDatabase.LoadAssetAtPath<LevelConfig>("Assets/_JebbyJump/Settings/Level/Level3Config.asset");

        if (level1 == null) { Debug.LogError("[Phase15WireLevels] Level1Config.asset not found."); return; }
        if (level2 == null) { Debug.LogError("[Phase15WireLevels] Level2Config.asset not found."); return; }
        if (level3 == null) { Debug.LogError("[Phase15WireLevels] Level3Config.asset not found."); return; }

        // Find LevelSession in scene
        var sessionGO = GameObject.Find("LevelSession");
        if (sessionGO == null) { Debug.LogError("[Phase15WireLevels] LevelSession GameObject not found in scene."); return; }

        var session = sessionGO.GetComponent<LevelSessionController>();
        if (session == null) { Debug.LogError("[Phase15WireLevels] LevelSessionController component not found on LevelSession."); return; }

        var so = new SerializedObject(session);
        var levelsProp = so.FindProperty("_levels");
        if (levelsProp == null) { Debug.LogError("[Phase15WireLevels] _levels property not found on LevelSessionController."); return; }

        levelsProp.arraySize = 3;
        levelsProp.GetArrayElementAtIndex(0).objectReferenceValue = level1;
        levelsProp.GetArrayElementAtIndex(1).objectReferenceValue = level2;
        levelsProp.GetArrayElementAtIndex(2).objectReferenceValue = level3;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(activeScene);
        EditorSceneManager.SaveScene(activeScene);

        Debug.Log("[Phase15WireLevels] Done. LevelSession._levels = [Level1Config, Level2Config, Level3Config]");
    }
}
