using System.Collections.Generic;
using JebbyJump.Core;
using JebbyJump.World;
using UnityEditor;
using UnityEngine;

// Editor menu item: create/refresh the ten WorldDefinition assets and the
// WorldCatalog that orders them (WorldExpansion100, phase P34B).
//
// The roster is the CONFIRMED one from the approval gate. Level ranges are
// derived from WorldMapping (contiguous, 10 levels per world), so the data
// cannot drift from the runtime arithmetic. Idempotent: re-running with no
// changes writes nothing.
public static class CreateOrSyncWorldCatalog
{
    private const string WorldFolder = "Assets/_JebbyJump/Settings/World";
    private const string CatalogAssetPath =
        "Assets/_JebbyJump/Settings/World/WorldCatalog.asset";

    // CONFIRMED roster (approval gate). Index 0 => world number 1.
    private static readonly string[] DisplayNames =
    {
        "Cloud Meadow",
        "Enchanted Forest",
        "Crystal Caves",
        "Sunshine Desert",
        "Ocean Sky",
        "Candy Cloud Kingdom",
        "Clockwork Heights",
        "Moonlit Dreamscape",
        "Stormfire Volcano",
        "Rainbow Tower Castle",
    };

    [MenuItem("Jebby Jump/Progression/Create Or Sync World Catalog")]
    public static void Run()
    {
        if (DisplayNames.Length != WorldMapping.WorldCount)
        {
            Debug.LogError("[WorldCatalog] Roster size " + DisplayNames.Length
                + " != WorldMapping.WorldCount " + WorldMapping.WorldCount);
            return;
        }

        if (!AssetDatabase.IsValidFolder(WorldFolder))
            AssetDatabase.CreateFolder("Assets/_JebbyJump/Settings", "World");

        var worlds = new List<WorldDefinition>(WorldMapping.WorldCount);
        int changed = 0;

        for (int i = 0; i < WorldMapping.WorldCount; i++)
        {
            int worldNumber = i + 1;
            string worldId = WorldMapping.WorldIdForNumber(worldNumber);
            string path = WorldFolder + "/" + worldId + "_WorldDefinition.asset";

            var def = AssetDatabase.LoadAssetAtPath<WorldDefinition>(path);
            if (def == null)
            {
                def = ScriptableObject.CreateInstance<WorldDefinition>();
                AssetDatabase.CreateAsset(def, path);
                changed++;
            }

            // Level ids are 1-based globally; derive from the mapping so the
            // asset can never disagree with WorldMapping.
            int first = WorldMapping.FirstLevelIndexOfWorld(worldNumber) + 1;
            int last = WorldMapping.LastLevelIndexOfWorld(worldNumber) + 1;

            var so = new SerializedObject(def);
            bool dirty = false;
            dirty |= SetString(so, "_worldId", worldId);
            dirty |= SetInt(so, "_worldNumber", worldNumber);
            dirty |= SetString(so, "_displayName", DisplayNames[i]);
            dirty |= SetInt(so, "_firstGlobalLevelId", first);
            dirty |= SetInt(so, "_lastGlobalLevelId", last);
            if (dirty)
            {
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(def);
                changed++;
            }

            worlds.Add(def);
        }

        changed += WriteCatalog(worlds);

        if (changed > 0) AssetDatabase.SaveAssets();
        Debug.Log("[WorldCatalog] " + WorldMapping.WorldCount
            + " world(s) synced; " + changed + " change(s). Levels "
            + "1-" + WorldMapping.TotalLevels + " mapped contiguously.");
    }

    private static int WriteCatalog(List<WorldDefinition> worlds)
    {
        int changed = 0;
        var catalog = AssetDatabase.LoadAssetAtPath<WorldCatalog>(CatalogAssetPath);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<WorldCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogAssetPath);
            changed++;
        }

        var so = new SerializedObject(catalog);
        var prop = so.FindProperty("_worlds");
        bool dirty = prop.arraySize != worlds.Count;
        if (dirty) prop.arraySize = worlds.Count;
        for (int i = 0; i < worlds.Count; i++)
        {
            var element = prop.GetArrayElementAtIndex(i);
            if (element.objectReferenceValue != worlds[i])
            {
                element.objectReferenceValue = worlds[i];
                dirty = true;
            }
        }
        if (dirty)
        {
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
            changed++;
        }
        return changed;
    }

    private static bool SetString(SerializedObject so, string field, string value)
    {
        var p = so.FindProperty(field);
        if (p == null || p.stringValue == value) return false;
        p.stringValue = value;
        return true;
    }

    private static bool SetInt(SerializedObject so, string field, int value)
    {
        var p = so.FindProperty(field);
        if (p == null || p.intValue == value) return false;
        p.intValue = value;
        return true;
    }
}
