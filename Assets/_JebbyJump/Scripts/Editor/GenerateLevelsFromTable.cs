using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using JebbyJump.Core;
using JebbyJump.Level;
using UnityEditor;
using UnityEngine;

// Deterministic LevelConfig generator (WorldExpansion100, phase P34E).
//
// Source of truth: 13_100_LEVEL_MASTER_TABLE.csv. The CSV carries every value
// a LevelConfig needs, so nothing is inferred here.
//
// Hard rules:
//  * World 1 (global ids 1-10) is READ-ONLY. Those are the shipped
//    Level1Config..Level10Config; their indices AND asset names must never
//    change or local save keys break (see doc 10). The generator verifies
//    they exist and reports them as preserved - it never writes them.
//  * Levels 11-100 are written as Level011Config..Level100Config.
//  * Idempotent: a re-run with an unchanged CSV writes nothing.
//  * Dry-run reports intended changes without touching disk.
//  * Balance is NOT validated here - every generated value stays a
//    PLAYTEST-HYPOTHESIS until P34T.
public static class GenerateLevelsFromTable
{
    private const string CsvPath =
        "Assets/_JebbyJump/Docs/Design/WorldExpansion100/13_100_LEVEL_MASTER_TABLE.csv";
    private const string LevelFolder = "Assets/_JebbyJump/Settings/Level";
    private const string RankFolder = "Assets/_JebbyJump/Settings/Level/TimeRanks";

    private const int WorldOneLastId = 10;
    private const float RowStartY = 3f;
    private const float MemoryJumpMultiplier = 0.35f;

    [MenuItem("Jebby Jump/Progression/Generate Levels From Table (Dry Run)")]
    public static void DryRun() => Run(apply: false);

    [MenuItem("Jebby Jump/Progression/Generate Levels From Table (Apply)")]
    public static void Apply() => Run(apply: true);

    private static void Run(bool apply)
    {
        var rows = ReadCsv(CsvPath, out string readError);
        if (readError != null) { Debug.LogError("[GenLevels] " + readError); return; }

        if (!Validate(rows, out string validationError))
        {
            Debug.LogError("[GenLevels] VALIDATION FAILED: " + validationError
                + "\nNothing was written.");
            return;
        }

        int created = 0, updated = 0, unchanged = 0, preserved = 0;
        var log = new StringBuilder();

        foreach (var row in rows)
        {
            int id = row.GlobalLevelId;
            if (id <= WorldOneLastId)
            {
                // World 1: verify, never write.
                string existing = LevelFolder + "/Level" + id + "Config.asset";
                if (AssetDatabase.LoadAssetAtPath<LevelConfig>(existing) == null)
                {
                    Debug.LogError("[GenLevels] Shipped World-1 asset missing: "
                        + existing + ". Aborting - refusing to recreate it.");
                    return;
                }
                preserved++;
                continue;
            }

            string levelPath = LevelFolder + "/Level" + id.ToString("000") + "Config.asset";
            string rankPath = RankFolder + "/Level" + id.ToString("000") + "_TimeRankConfig.asset";

            var state = WriteLevel(row, levelPath, rankPath, apply, log);
            if (state == WriteState.Created) created++;
            else if (state == WriteState.Updated) updated++;
            else unchanged++;
        }

        if (apply && (created > 0 || updated > 0))
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        string mode = apply ? "APPLY" : "DRY RUN";
        Debug.Log("[GenLevels] " + mode + " complete. "
            + "preserved(World 1)=" + preserved
            + " created=" + created + " updated=" + updated
            + " unchanged=" + unchanged + " total=" + rows.Count
            + "\nAll generated balance values remain PLAYTEST-HYPOTHESIS until P34T."
            + (log.Length > 0 ? "\n" + log : ""));
    }


    // Writes the ordered 100 LevelConfigs into LevelSessionController._levels in
    // Game.unity, then mirrors them into the LevelCatalog. Order is the save-key
    // contract: the shipped Level1Config..Level10Config MUST stay at indices 0-9
    // (doc 10), followed by Level011Config..Level100Config. Idempotent.
    [MenuItem("Jebby Jump/Progression/Sync Level Session Array (100)")]
    public static void SyncSessionArray()
    {
        // Open the scene FIRST. OpenScene(Single) unloads unused assets, which
        // would fake-null any LevelConfig held only by a local list - the 90
        // newly generated ones are referenced by nothing else yet, so loading
        // them before the open silently produced 90 null entries.
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
            "Assets/_JebbyJump/Scenes/Game.unity",
            UnityEditor.SceneManagement.OpenSceneMode.Single);

        var ordered = new List<LevelConfig>();
        for (int id = 1; id <= WorldMapping.TotalLevels; id++)
        {
            string path = id <= WorldOneLastId
                ? LevelFolder + "/Level" + id + "Config.asset"
                : LevelFolder + "/Level" + id.ToString("000") + "Config.asset";
            var cfg = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
            if (cfg == null)
            {
                Debug.LogError("[GenLevels] Missing level asset " + path
                    + " - run 'Generate Levels From Table (Apply)' first. Aborting.");
                return;
            }
            ordered.Add(cfg);
        }

        LevelSessionController session = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            session = root.GetComponentInChildren<LevelSessionController>(true);
            if (session != null) break;
        }
        if (session == null)
        { Debug.LogError("[GenLevels] No LevelSessionController in Game.unity."); return; }

        var so = new SerializedObject(session);
        var prop = so.FindProperty("_levels");
        bool dirty = prop.arraySize != ordered.Count;
        if (dirty) prop.arraySize = ordered.Count;
        for (int i = 0; i < ordered.Count; i++)
        {
            var el = prop.GetArrayElementAtIndex(i);
            if (el.objectReferenceValue != ordered[i])
            { el.objectReferenceValue = ordered[i]; dirty = true; }
        }

        if (dirty)
        {
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(session);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            Debug.Log("[GenLevels] Session array -> " + ordered.Count
                + " levels (World 1 preserved at indices 0-9).");
        }
        else Debug.Log("[GenLevels] Session array already in sync ("
            + ordered.Count + " levels).");

        // Mirror into the LevelCatalog INLINE. Delegating to
        // CreateOrSyncLevelCatalog here would re-open this same scene
        // additively and then close it with removeScene:true, which produced
        // a catalog full of nulls (and Unity's "unloading the last loaded
        // scene" warning). One open, one write.
        WriteLevelCatalog(ordered);
    }

    private const string LevelCatalogPath =
        "Assets/_JebbyJump/Settings/Level/LevelCatalog.asset";

    private static void WriteLevelCatalog(List<LevelConfig> ordered)
    {
        var catalog = AssetDatabase.LoadAssetAtPath<ScriptableObject>(LevelCatalogPath);
        if (catalog == null)
        {
            Debug.LogError("[GenLevels] LevelCatalog not found at " + LevelCatalogPath);
            return;
        }
        var so = new SerializedObject(catalog);
        var prop = so.FindProperty("_levels");
        bool dirty = prop.arraySize != ordered.Count;
        if (dirty) prop.arraySize = ordered.Count;
        int nulls = 0;
        for (int i = 0; i < ordered.Count; i++)
        {
            var el = prop.GetArrayElementAtIndex(i);
            if (el.objectReferenceValue != ordered[i])
            { el.objectReferenceValue = ordered[i]; dirty = true; }
            if (ordered[i] == null) nulls++;
        }
        if (dirty)
        {
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
        }
        Debug.Log("[GenLevels] LevelCatalog -> " + ordered.Count
            + " entries, " + nulls + " null (must be 0).");
    }

    private enum WriteState { Created, Updated, Unchanged }

    private static WriteState WriteLevel(Row row, string levelPath, string rankPath,
        bool apply, StringBuilder log)
    {
        var rank = AssetDatabase.LoadAssetAtPath<TimeRankConfig>(rankPath);
        var level = AssetDatabase.LoadAssetAtPath<LevelConfig>(levelPath);
        bool creating = level == null || rank == null;

        if (!apply)
        {
            if (creating) { log.AppendLine("  would create " + levelPath); return WriteState.Created; }
            bool wouldChange = LevelDiffers(level, row) || RankDiffers(rank, row);
            if (wouldChange) log.AppendLine("  would update " + levelPath);
            return wouldChange ? WriteState.Updated : WriteState.Unchanged;
        }

        EnsureFolders();

        if (rank == null)
        {
            rank = ScriptableObject.CreateInstance<TimeRankConfig>();
            AssetDatabase.CreateAsset(rank, rankPath);
        }
        if (level == null)
        {
            level = ScriptableObject.CreateInstance<LevelConfig>();
            AssetDatabase.CreateAsset(level, levelPath);
        }

        bool dirty = false;
        var rso = new SerializedObject(rank);
        dirty |= SetFloat(rso, "_sThreshold", row.SRank);
        dirty |= SetFloat(rso, "_aThreshold", row.ARank);
        dirty |= SetFloat(rso, "_bThreshold", row.BRank);
        if (rso.hasModifiedProperties) { rso.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(rank); }

        var lso = new SerializedObject(level);
        dirty |= SetInt(lso, "_sequenceLength", row.SequenceLength);
        dirty |= SetFloat(lso, "_memoryTimeSeconds", row.MemorySeconds);
        dirty |= SetFloat(lso, "_memoryPhaseJumpMultiplier", MemoryJumpMultiplier);
        dirty |= SetInt(lso, "_startingLives", row.Lives);
        dirty |= SetColors(lso, row.ColourCount);
        dirty |= SetFloat(lso, "_cactusSpawnChance", row.CactusChance);
        dirty |= SetInt(lso, "_platformsPerRow", row.ChoicesPerRow);
        dirty |= SetFloat(lso, "_rowVerticalSpacing", row.RowVerticalSpacing);
        dirty |= SetFloat(lso, "_rowStartY", RowStartY);
        dirty |= SetFloat(lso, "_platformWidth", row.PlatformWidth);
        dirty |= SetFloat(lso, "_platformHeight", 0.5f);
        dirty |= SetFloat(lso, "_rowHorizontalSpread", row.HorizontalSpread);
        dirty |= SetFloat(lso, "_rowVerticalJitter", row.VerticalJitter);
        dirty |= SetRef(lso, "_rankConfig", rank);
        if (lso.hasModifiedProperties) { lso.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(level); }

        if (creating) return WriteState.Created;
        return dirty ? WriteState.Updated : WriteState.Unchanged;
    }

    private static bool LevelDiffers(LevelConfig level, Row row)
    {
        var so = new SerializedObject(level);
        return so.FindProperty("_sequenceLength").intValue != row.SequenceLength
            || !Mathf.Approximately(so.FindProperty("_memoryTimeSeconds").floatValue, row.MemorySeconds)
            || so.FindProperty("_platformsPerRow").intValue != row.ChoicesPerRow
            || !Mathf.Approximately(so.FindProperty("_cactusSpawnChance").floatValue, row.CactusChance)
            || so.FindProperty("_availableColors").arraySize != row.ColourCount;
    }

    private static bool RankDiffers(TimeRankConfig rank, Row row)
    {
        var so = new SerializedObject(rank);
        return !Mathf.Approximately(so.FindProperty("_sThreshold").floatValue, row.SRank)
            || !Mathf.Approximately(so.FindProperty("_aThreshold").floatValue, row.ARank)
            || !Mathf.Approximately(so.FindProperty("_bThreshold").floatValue, row.BRank);
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder(RankFolder))
            AssetDatabase.CreateFolder(LevelFolder, "TimeRanks");
    }

    // First N of the six locked colours, in PlatformColor enum order.
    private static bool SetColors(SerializedObject so, int count)
    {
        var p = so.FindProperty("_availableColors");
        var wanted = new[]
        {
            PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green,
            PlatformColor.Yellow, PlatformColor.Purple, PlatformColor.Orange,
        };
        count = Mathf.Clamp(count, 1, wanted.Length);
        bool dirty = p.arraySize != count;
        if (dirty) p.arraySize = count;
        for (int i = 0; i < count; i++)
        {
            var el = p.GetArrayElementAtIndex(i);
            if (el.intValue != (int)wanted[i]) { el.intValue = (int)wanted[i]; dirty = true; }
        }
        return dirty;
    }

    // ---- validation -----------------------------------------------------

    private static bool Validate(List<Row> rows, out string error)
    {
        error = null;
        if (rows.Count != 100) { error = "expected 100 rows, got " + rows.Count; return false; }

        var seen = new HashSet<int>();
        var perWorld = new Dictionary<string, int>();
        int finales = 0;

        foreach (var r in rows)
        {
            if (!seen.Add(r.GlobalLevelId))
            { error = "duplicate global_level_id " + r.GlobalLevelId; return false; }
            if (r.SequenceLength > 10)
            { error = "level " + r.GlobalLevelId + " sequence_length > 10"; return false; }
            if (r.ChoicesPerRow > 6)
            { error = "level " + r.GlobalLevelId + " choices_per_row > 6"; return false; }
            if (r.SkillRequired)
            { error = "level " + r.GlobalLevelId + " is skill_required (forbidden)"; return false; }
            if (!(r.SRank < r.ARank && r.ARank < r.BRank))
            { error = "level " + r.GlobalLevelId + " rank thresholds not ordered S<A<B"; return false; }
            if (r.CactusChance < 0f || r.CactusChance > 1f)
            { error = "level " + r.GlobalLevelId + " cactus chance out of range"; return false; }

            int expectedWorld = WorldMapping.WorldNumberForLevelIndex(r.GlobalLevelId - 1);
            if (WorldMapping.WorldIdForNumber(expectedWorld) != r.WorldId)
            {
                error = "level " + r.GlobalLevelId + " world " + r.WorldId
                    + " disagrees with contiguous mapping (" + expectedWorld + ")";
                return false;
            }
            perWorld.TryGetValue(r.WorldId, out int c);
            perWorld[r.WorldId] = c + 1;
            if (r.Finale) finales++;
        }

        if (perWorld.Count != 10) { error = "expected 10 worlds, got " + perWorld.Count; return false; }
        foreach (var kv in perWorld)
            if (kv.Value != 10) { error = kv.Key + " has " + kv.Value + " levels, expected 10"; return false; }
        if (finales != 10) { error = "expected 10 finales, got " + finales; return false; }
        return true;
    }

    // ---- CSV ------------------------------------------------------------

    private class Row
    {
        public int GlobalLevelId; public string WorldId;
        public int SequenceLength, ColourCount, ChoicesPerRow, Lives;
        public float MemorySeconds, CactusChance, RowVerticalSpacing;
        public float PlatformWidth, HorizontalSpread, VerticalJitter;
        public float SRank, ARank, BRank;
        public bool SkillRequired, Finale;
    }

    private static List<Row> ReadCsv(string path, out string error)
    {
        error = null;
        string full = Path.GetFullPath(path);
        if (!File.Exists(full)) { error = "CSV not found: " + path; return null; }

        var lines = File.ReadAllLines(full);
        if (lines.Length < 2) { error = "CSV has no data rows"; return null; }

        var header = SplitCsvLine(lines[0]);
        var idx = new Dictionary<string, int>();
        for (int i = 0; i < header.Count; i++) idx[header[i].Trim()] = i;

        string[] required =
        {
            "global_level_id","world_id","sequence_length","unique_colours_used",
            "maximum_choices_per_row","cactus_spawn_chance","same_row_y_stagger",
            "platform_width_profile","horizontal_gap_profile","row_vertical_spacing",
            "memory_display_duration","lives","proposed_s_rank_time",
            "proposed_a_rank_time","proposed_b_rank_time","skill_required","finale_flag",
        };
        foreach (var c in required)
            if (!idx.ContainsKey(c)) { error = "CSV missing column: " + c; return null; }

        var rows = new List<Row>();
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var f = SplitCsvLine(lines[i]);
            var inv = CultureInfo.InvariantCulture;
            rows.Add(new Row
            {
                GlobalLevelId = int.Parse(f[idx["global_level_id"]], inv),
                WorldId = f[idx["world_id"]].Trim(),
                SequenceLength = int.Parse(f[idx["sequence_length"]], inv),
                ColourCount = int.Parse(f[idx["unique_colours_used"]], inv),
                ChoicesPerRow = int.Parse(f[idx["maximum_choices_per_row"]], inv),
                Lives = int.Parse(f[idx["lives"]], inv),
                MemorySeconds = float.Parse(f[idx["memory_display_duration"]], inv),
                CactusChance = float.Parse(f[idx["cactus_spawn_chance"]], inv),
                RowVerticalSpacing = float.Parse(f[idx["row_vertical_spacing"]], inv),
                PlatformWidth = float.Parse(f[idx["platform_width_profile"]], inv),
                HorizontalSpread = float.Parse(f[idx["horizontal_gap_profile"]], inv),
                VerticalJitter = float.Parse(f[idx["same_row_y_stagger"]], inv),
                SRank = float.Parse(f[idx["proposed_s_rank_time"]], inv),
                ARank = float.Parse(f[idx["proposed_a_rank_time"]], inv),
                BRank = float.Parse(f[idx["proposed_b_rank_time"]], inv),
                SkillRequired = f[idx["skill_required"]].Trim()
                    .Equals("true", StringComparison.OrdinalIgnoreCase),
                Finale = f[idx["finale_flag"]].Trim()
                    .Equals("true", StringComparison.OrdinalIgnoreCase),
            });
        }
        return rows;
    }

    // Quote-aware split (fields may contain commas inside double quotes).
    private static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;
        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                else inQuotes = !inQuotes;
            }
            else if (ch == ',' && !inQuotes) { result.Add(sb.ToString()); sb.Clear(); }
            else sb.Append(ch);
        }
        result.Add(sb.ToString());
        return result;
    }

    // ---- serialized helpers ---------------------------------------------

    private static bool SetInt(SerializedObject so, string field, int value)
    {
        var p = so.FindProperty(field);
        if (p == null || p.intValue == value) return false;
        p.intValue = value; return true;
    }

    private static bool SetFloat(SerializedObject so, string field, float value)
    {
        var p = so.FindProperty(field);
        if (p == null || Mathf.Approximately(p.floatValue, value)) return false;
        p.floatValue = value; return true;
    }

    private static bool SetRef(SerializedObject so, string field, UnityEngine.Object value)
    {
        var p = so.FindProperty(field);
        if (p == null || p.objectReferenceValue == value) return false;
        p.objectReferenceValue = value; return true;
    }
}
