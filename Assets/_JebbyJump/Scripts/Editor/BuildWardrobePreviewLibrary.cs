using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using UnityEditor;
using UnityEngine;

// Idempotent scaffold: creates/populates the WardrobePreviewLibrary asset
// (UI-only outfit preview sprites) by loading each outfit's existing per-state
// sprites (idle/run/jump/fall/land/hurt/victory). One entry per WardrobeCatalog
// outfit, including the default. Replace-by-id keeps entries unique on rerun.
// Read-only on the source sprites; does NOT change import settings or any other
// asset.
public static class BuildWardrobePreviewLibrary
{
    private const string LibraryPath =
        "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/WardrobePreviewLibrary.asset";
    private const string DefaultSpriteFolder =
        "Assets/_JebbyJump/Art/Sprites/Characters/Jebby/";
    private const string OutfitsRoot =
        "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/";

    private static readonly string[] States =
        { "idle", "run", "jump", "fall", "land", "hurt", "victory" };

    [MenuItem("Jebby Jump/Scaffold/Build Wardrobe Preview Library")]
    public static void Run()
    {
        var library = AssetDatabase.LoadAssetAtPath<WardrobePreviewLibrary>(LibraryPath);
        if (library == null)
        {
            library = ScriptableObject.CreateInstance<WardrobePreviewLibrary>();
            AssetDatabase.CreateAsset(library, LibraryPath);
            Debug.Log("[WardrobePreview] Created " + LibraryPath);
        }

        int fullSets = 0, missingSprites = 0;
        foreach (var def in WardrobeCatalog.Outfits)
        {
            var poses = new Sprite[States.Length];
            int present = 0;
            for (int i = 0; i < States.Length; i++)
            {
                string path = SpritePath(def.Id, States[i]);
                poses[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (poses[i] != null) present++;
                else
                {
                    missingSprites++;
                    Debug.LogWarning("[WardrobePreview] " + def.Id
                        + ": sprite not found at " + path);
                }
            }
            // poses: idle, run, jump, fall, land, hurt, victory
            library.AddEntry(def.Id, poses[0], poses[1], poses[2], poses[3],
                poses[4], poses[5], poses[6]);
            if (present == States.Length) fullSets++;
        }

        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();
        Debug.Log("[WardrobePreview] Done: " + fullSets + "/"
            + WardrobeCatalog.Outfits.Count + " full pose sets, "
            + missingSprites + " missing sprites; library has "
            + library.Count + " entries.");
    }

    private static string SpritePath(string outfitId, string state)
    {
        if (outfitId == WardrobeCatalog.DefaultOutfitId)
            return DefaultSpriteFolder + "spr_jebby_" + state + "_01.png";
        return OutfitsRoot + ToPascal(outfitId) + "/Sprites/spr_jebby_"
            + outfitId + "_" + state + "_01.png";
    }

    private static string ToPascal(string snake)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var part in snake.Split('_'))
        {
            if (part.Length == 0) continue;
            sb.Append(char.ToUpperInvariant(part[0]));
            if (part.Length > 1) sb.Append(part.Substring(1));
        }
        return sb.ToString();
    }
}
