using UnityEngine;

namespace JebbyJump.World
{
    // Identity + level range for one themed world (WorldExpansion100, P34B).
    //
    // Visual/art fields (background, floor, six platform visuals, hazard
    // visual, thumbnail, story card, reward art) are intentionally NOT here
    // yet: P34B is the data foundation only ("no art wiring"). They are added
    // in P34C/P34H when something actually consumes them.
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

        public string WorldId => _worldId;
        public int WorldNumber => _worldNumber;
        public string DisplayName => _displayName;
        public int FirstGlobalLevelId => _firstGlobalLevelId;
        public int LastGlobalLevelId => _lastGlobalLevelId;

        // True when the given 1-based global level id belongs to this world.
        public bool ContainsGlobalLevelId(int globalLevelId)
            => globalLevelId >= _firstGlobalLevelId
               && globalLevelId <= _lastGlobalLevelId;
    }
}
