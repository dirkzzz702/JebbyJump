using UnityEditor;
using UnityEditor.SceneManagement;

public static class SaveScene
{
    public static void Execute()
    {
        EditorSceneManager.SaveOpenScenes();
        UnityEngine.Debug.Log("[SaveScene] Scene saved.");
    }
}
