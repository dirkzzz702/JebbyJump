using JebbyJump.Core;
using JebbyJump.Level;
using UnityEngine;

namespace JebbyJump.World
{
    // Applies a world's visuals inside the single Game scene
    // (WorldExpansion100, phase P34C). Ten themed worlds, one scene.
    //
    // Timing (matters - see doc 08):
    //   * Start()  -> applies before the first rendered frame, so entering the
    //                 Game scene on a World-2+ level never flashes World 1 art.
    //   * LevelChanged -> LevelSessionController.AdvanceToNextLevel() raises it
    //                 BEFORE MemoryPhaseController.RestartLevel() regenerates
    //                 the platforms, so crossing a world boundary re-themes
    //                 ahead of level generation. This is why no gameplay code
    //                 needed changing.
    //
    // Missing-art policy: a world with no background of its own falls back to
    // World 1's rather than rendering blank, and logs a warning. Worlds 3-10
    // legitimately have no art yet (arrives in P34I-P34R).
    [DefaultExecutionOrder(-50)]
    public class WorldThemeApplier : MonoBehaviour
    {
        [SerializeField] private WorldCatalog _catalog;
        [SerializeField] private LevelSessionController _levelSession;
        [SerializeField] private SpriteRenderer _background;
        [SerializeField] private SpriteRenderer _floorVisual;

        // 0 = nothing applied yet. Exposed for tests/diagnostics.
        public int AppliedWorldNumber { get; private set; }

        private bool _subscribed;

        private void OnEnable()
        {
            if (_levelSession != null && !_subscribed)
            {
                _levelSession.LevelChanged += OnLevelChanged;
                _subscribed = true;
            }
        }

        private void OnDisable()
        {
            if (_levelSession != null && _subscribed)
            {
                _levelSession.LevelChanged -= OnLevelChanged;
                _subscribed = false;
            }
        }

        private void Start() => ApplyForCurrentLevel();

        private void OnLevelChanged() => ApplyForCurrentLevel();

        public void ApplyForCurrentLevel()
        {
            if (_levelSession == null) return;
            ApplyForLevelIndex(_levelSession.CurrentLevelIndex);
        }

        // levelIndex is the 0-based index inside the session's level array.
        public void ApplyForLevelIndex(int levelIndex)
        {
            if (_catalog == null || _catalog.Count == 0)
            {
                Debug.LogWarning("[WorldTheme] No WorldCatalog assigned; "
                    + "leaving scene visuals untouched.", this);
                return;
            }

            int worldNumber = WorldMapping.WorldNumberForLevelIndex(levelIndex);
            if (worldNumber <= 0)
            {
                Debug.LogWarning("[WorldTheme] Level index " + levelIndex
                    + " maps to no world; leaving visuals untouched.", this);
                return;
            }

            var world = _catalog.GetByNumber(worldNumber);
            if (world == null)
            {
                Debug.LogWarning("[WorldTheme] World " + worldNumber
                    + " missing from catalog; leaving visuals untouched.", this);
                return;
            }

            Apply(world);
        }

        public void Apply(WorldDefinition world)
        {
            if (world == null) return;

            var visuals = world.Visuals;
            var fallback = _catalog != null ? _catalog.GetByNumber(1) : null;
            var fallbackVisuals = fallback != null ? fallback.Visuals : null;

            // Background: own art, else World 1's, else leave as-is (never blank).
            if (_background != null)
            {
                Sprite bg = visuals != null && visuals.HasBackground
                    ? visuals.Background
                    : null;
                if (bg == null && fallbackVisuals != null && fallbackVisuals.HasBackground)
                {
                    bg = fallbackVisuals.Background;
                    Debug.LogWarning("[WorldTheme] " + world.WorldId
                        + " has no background yet; falling back to World 1.", this);
                }
                if (bg != null) _background.sprite = bg;
            }

            // Floor: same policy.
            if (_floorVisual != null)
            {
                Sprite floor = visuals != null && visuals.HasFloor
                    ? visuals.Floor
                    : null;
                if (floor == null && fallbackVisuals != null && fallbackVisuals.HasFloor)
                    floor = fallbackVisuals.Floor;
                if (floor != null) _floorVisual.sprite = floor;
            }

            AppliedWorldNumber = world.WorldNumber;
            Debug.Log("[WorldTheme] Applied " + world.WorldId + " ("
                + world.DisplayName + ").");
        }
    }
}
