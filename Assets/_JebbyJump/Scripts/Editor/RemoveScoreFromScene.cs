using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class RemoveScoreFromScene
{
    private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        int destroyed = 0;
        var allGOs = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in allGOs)
        {
            if (go == null) continue;
            if (!go.scene.IsValid()) continue; // skip prefab assets
            if (go.name != "ScoreText") continue;
            Debug.Log($"[RemoveScore] Destroying ScoreText under '{(go.transform.parent != null ? go.transform.parent.name : "<root>")}'");
            Object.DestroyImmediate(go);
            destroyed++;
        }
        Debug.Log($"[RemoveScore] Removed {destroyed} ScoreText GameObject(s).");

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[RemoveScore] Scene saved.");
    }
}
