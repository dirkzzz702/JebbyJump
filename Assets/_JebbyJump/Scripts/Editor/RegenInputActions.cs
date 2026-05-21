using UnityEditor;
using UnityEngine;

public static class RegenInputActions
{
    public static void Execute()
    {
        const string path = "Assets/_JebbyJump/Settings/Input/JebbyInputActions.inputactions";
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();
        Debug.Log("[RegenInput] Reimported " + path);
    }
}
