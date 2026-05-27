using JebbyJump.Level;
using UnityEngine;

namespace JebbyJump.Progression
{
    // Menu-side mirror of the Game scene's LevelSessionController._levels
    // array. Authoritative ordering still lives in the Game scene; this
    // catalog exists so the Main Menu UI can list levels without loading
    // Game.unity. Keep the two in sync via the
    // "Jebby Jump/Progression/Create Or Sync Level Catalog" editor menu.
    [CreateAssetMenu(
        fileName = "LevelCatalog",
        menuName = "JebbyJump/LevelCatalog")]
    public class LevelCatalog : ScriptableObject
    {
        [SerializeField] private LevelConfig[] _levels;

        public int Count => _levels?.Length ?? 0;
        public LevelConfig[] Levels => _levels;

        public LevelConfig Get(int index)
        {
            if (_levels == null || _levels.Length == 0) return null;
            if (index < 0 || index >= _levels.Length) return null;
            return _levels[index];
        }

        // Stable persistent key for a level slot. Uses the LevelConfig
        // asset name (e.g. "Level1Config") so the key matches the
        // BestTimeStore convention (JebbyJump.BestTime.<assetName>).
        public string GetLevelKey(int index)
        {
            var cfg = Get(index);
            return cfg != null ? cfg.name : null;
        }
    }
}
