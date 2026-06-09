using JebbyJump.Wardrobe.Visual;
using UnityEditor;
using UnityEngine;

// Idempotent scaffold: attaches PlayerOutfitVisualController to the Jebby
// player prefab and wires its Animator + SpriteRenderer to the prefab's
// existing components. Re-runs reuse the existing component (no duplicate).
//
// Edits ONLY the prefab asset - the Game.unity instance inherits the
// component automatically (it is a prefab instance), so the scene is not
// touched. In P11 the controller's apply path is a safe no-op (no outfit
// art), so attaching it has no gameplay or visual effect.
public static class AttachPlayerOutfitVisual
{
    private const string JebbyPrefabPath =
        "Assets/_JebbyJump/Prefabs/Player/Jebby.prefab";

    [MenuItem("Jebby Jump/Scaffold/Attach Player Outfit Visual")]
    public static void Run()
    {
        if (!System.IO.File.Exists(JebbyPrefabPath))
        {
            Debug.LogError("[OutfitVisual] Jebby prefab not found at "
                + JebbyPrefabPath);
            return;
        }

        var root = PrefabUtility.LoadPrefabContents(JebbyPrefabPath);
        try
        {
            var ctrl = root.GetComponent<PlayerOutfitVisualController>();
            if (ctrl == null)
            {
                ctrl = root.AddComponent<PlayerOutfitVisualController>();
                Debug.Log("[OutfitVisual] Added PlayerOutfitVisualController "
                    + "to Jebby prefab.");
            }
            else
            {
                Debug.Log("[OutfitVisual] PlayerOutfitVisualController already "
                    + "present - reusing.");
            }

            var animator = root.GetComponent<Animator>();
            var sr = root.GetComponent<SpriteRenderer>();

            var so = new SerializedObject(ctrl);
            so.FindProperty("_animator").objectReferenceValue = animator;
            so.FindProperty("_spriteRenderer").objectReferenceValue = sr;
            so.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, JebbyPrefabPath);
            Debug.Log("[OutfitVisual] Wired _animator=" + (animator != null)
                + ", _spriteRenderer=" + (sr != null) + " on Jebby prefab.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }
}
