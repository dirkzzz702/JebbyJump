using JebbyJump.Core;
using UnityEngine;

namespace JebbyJump.Level
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "JebbyJump/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Sequence")]
        [SerializeField] private int _sequenceLength = 4;
        [SerializeField] private float _memoryTimeSeconds = 5f;
        [SerializeField] private float _memoryPhaseJumpMultiplier = 0.35f;
        [SerializeField] private int _startingLives = 3;
        [SerializeField] private PlatformColor[] _availableColors =
        {
            PlatformColor.Red, PlatformColor.Blue,
            PlatformColor.Yellow, PlatformColor.Green,
        };

        [Header("Obstacles")]
        [SerializeField] private float _cactusSpawnChance = 0f;

        [Header("Platform Layout")]
        [SerializeField] private int _platformsPerRow = 2;
        [SerializeField] private float _rowVerticalSpacing = 3.5f;
        [SerializeField] private float _rowStartY = 2f;
        [SerializeField] private float _platformWidth = 4f;
        [SerializeField] private float _platformHeight = 0.5f;
        [SerializeField] private float _rowHorizontalSpread = 8f;
        [SerializeField] private float _rowVerticalJitter = 0f;

        public int SequenceLength => _sequenceLength;
        public float MemoryTimeSeconds => _memoryTimeSeconds;
        public float MemoryPhaseJumpMultiplier => _memoryPhaseJumpMultiplier;
        public int StartingLives => _startingLives;
        public PlatformColor[] AvailableColors => _availableColors;

        public float CactusSpawnChance => _cactusSpawnChance;

        public int PlatformsPerRow => _platformsPerRow;
        public float RowVerticalSpacing => _rowVerticalSpacing;
        public float RowStartY => _rowStartY;
        public float PlatformWidth => _platformWidth;
        public float PlatformHeight => _platformHeight;
        public float RowHorizontalSpread => _rowHorizontalSpread;
        public float RowVerticalJitter => _rowVerticalJitter;

        private void OnValidate()
        {
            _sequenceLength = Mathf.Max(1, _sequenceLength);
            _startingLives = Mathf.Max(1, _startingLives);
            _memoryTimeSeconds = Mathf.Max(0.1f, _memoryTimeSeconds);
            _memoryPhaseJumpMultiplier = Mathf.Clamp01(_memoryPhaseJumpMultiplier);
            _cactusSpawnChance = Mathf.Clamp01(_cactusSpawnChance);
            _platformsPerRow = Mathf.Max(1, _platformsPerRow);
            _rowVerticalSpacing = Mathf.Max(0.1f, _rowVerticalSpacing);
            _platformWidth = Mathf.Max(0.1f, _platformWidth);
            _platformHeight = Mathf.Max(0.1f, _platformHeight);
            _rowVerticalJitter = Mathf.Clamp(_rowVerticalJitter, 0f, 1f);
        }
    }
}
