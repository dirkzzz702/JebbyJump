using JebbyJump.Core;
using UnityEngine;

namespace JebbyJump.World
{
    // Ordered list of the ten WorldDefinitions (WorldExpansion100, P34B).
    // Mirrors the existing LevelCatalog pattern; kept in sync via the editor
    // menu "Jebby Jump/Progression/Create Or Sync World Catalog".
    [CreateAssetMenu(
        fileName = "WorldCatalog",
        menuName = "JebbyJump/WorldCatalog")]
    public class WorldCatalog : ScriptableObject
    {
        [SerializeField] private WorldDefinition[] _worlds;

        public int Count => _worlds?.Length ?? 0;
        public WorldDefinition[] Worlds => _worlds;

        public WorldDefinition Get(int index)
        {
            if (_worlds == null || index < 0 || index >= _worlds.Length)
                return null;
            return _worlds[index];
        }

        // 1-based world number (1..10).
        public WorldDefinition GetByNumber(int worldNumber)
            => Get(worldNumber - 1);

        // The world owning a 0-based level index, or null when out of range.
        public WorldDefinition ForLevelIndex(int levelIndex)
            => GetByNumber(WorldMapping.WorldNumberForLevelIndex(levelIndex));
    }
}
