using UnityEngine;

namespace JebbyJump.World
{
    // Identity + level range + visuals for one themed world
    // (WorldExpansion100, P34B identity / P34C visuals).
    //
    // Remaining art refs (hazard visual, thumbnail, badge, story card,
    // finale, gem, cosmetic) arrive with their consuming phase - see
    // WorldVisualSet for why themed platform art waits for P34H/P34J.
    [CreateAssetMenu(
        fileName = "WorldDefinition",
        menuName = "JebbyJump/WorldDefinition")]
    public class WorldDefinition : ScriptableObject
    {
        [SerializeField] private string _worldId;            // "W01".."W10"
        [SerializeField] private int _worldNumber;           // 1..10
        [SerializeField] private string _displayName;        // "Cloud Meadow"
        [SerializeField] private int _firstGlobalLevelId;    // 1, 11, ... 91
        [SerializeField] private int _lastGlobalLevelId;     // 10, 20, ... 100
        [SerializeField] private WorldVisualSet _visuals = new WorldVisualSet();

        public string WorldId => _worldId;
        public int WorldNumber => _worldNumber;
        public string DisplayName => _displayName;
        public int FirstGlobalLevelId => _firstGlobalLevelId;
        public int LastGlobalLevelId => _lastGlobalLevelId;
        public WorldVisualSet Visuals => _visuals;

        // True when the given 1-based global level id belongs to this world.
        public bool ContainsGlobalLevelId(int globalLevelId)
            => globalLevelId >= _firstGlobalLevelId
               && globalLevelId <= _lastGlobalLevelId;
    }
}
